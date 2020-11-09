using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using static planning.Weekday;
using static System.Linq.Enumerable;
using static System.String;

namespace planning
{
    public enum Weekday
    {
        Mon, Tue, Wed, Thu, Fri, Sat, Sun
    }

    public class Repetition
    {
        public List<Weekday> Weekdays { get; set; }

        public int NumberOfWeeks { get; set; }

        public Repetition()
        {
            Weekdays = new List<Weekday>();
        }

        public bool this[int day] => Weekdays.Contains((Weekday)day);
    }

    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt)
        {
            int diff = (7 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public static Weekday Weekday(this DateTime dt)
        {
            return (Weekday)(((int)dt.DayOfWeek + 6) % 7);
        }

        public static int WeekOfYear(this DateTime date)
        {
            // ISO8601
            var day = (int)CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(date);
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date.AddDays(4 - (day == 0 ? 7 : day)), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }

    public class Activity
    {
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public int Duration { get; set; }
        public Repetition Repeat { get; set; }
        public bool AllDay { get; set; }
        public string Classes { get; set; }
        public int Layer { get; set; }

        DateTime FirstDay => Start;

        public DateTime LastDay
        {
            get
            {
                if (Repeat == null)
                {
                    return Start;
                }
                else
                {
                    return Start.AddDays((int)Repeat.Weekdays.Max() -
                        (int)Start.Weekday() + 7 * Repeat.NumberOfWeeks);
                }
            }
        }

        [JsonIgnore]
        public IEnumerable<DateTime> Occurences
        {
            get
            {
                yield return Start;
                if (Repeat != null)
                {
                    DateTime startOfWeek = Start.StartOfWeek().AddHours(Start.Hour);
                    for (int i = 0; i <= Repeat.NumberOfWeeks; ++i)
                        foreach (var repeatWd in Repeat.Weekdays)
                        {
                            var occurence = startOfWeek.AddDays((int)repeatWd + i * 7);
                            if (occurence > Start)
                            {
                                yield return occurence;
                            }
                        }
                }
            }
        }
    }

    public class DateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) =>
                DateTime.ParseExact(reader.GetString(),
                    "yyyy-MM-dd H", CultureInfo.InvariantCulture);

        public override void Write(
            Utf8JsonWriter writer,
            DateTime dateTimeValue,
            JsonSerializerOptions options) =>
                writer.WriteStringValue(dateTimeValue.ToString("yyyy-MM-dd H"));
    }

    class Schedule
    {
        public List<Activity> Activities { get; set; }

        public Schedule()
        {
            Activities = new List<Activity>();
        }

        IEnumerable<Activity> GetActivities(DateTime dt)
        {
            foreach (var activity in Activities)
            {
                foreach (var occurence in activity.Occurences)
                {
                    if (dt >= occurence && dt < occurence.AddHours(activity.Duration))
                    {
                        yield return activity;
                        break;
                    }
                }
            }
        }

        public (Activity activity, int duration) GetAcivity(DateTime dt)
        {
            var activities = GetActivities(dt);
            if (!activities.Any())
            {
                return (null, 0);
            }
            var activity = activities.Where(a => !a.AllDay).OrderByDescending(a => a.Layer).First();
            var stepDt = dt;
            while (true)
            {
                stepDt = stepDt.AddHours(1);
                activities = GetActivities(stepDt);
                if (!(activities.Any()
                    && activities.Where(a => !a.AllDay).OrderByDescending(a => a.Layer).First() == activity))
                {
                    return (activity, (stepDt - dt).Hours);
                }
            }
        }

    }

    class Program
    {
        static void Main(string[] args)
        {

            Activity lunch = new Activity()
            {
                Title = "Lunch",
                Start = new DateTime(2020, 11, 9, 12, 0, 0),
                Duration = 1,
                Repeat = new Repetition()
                {
                    Weekdays = new List<Weekday>() { Mon, Tue, Wed, Thu, Fri },
                    NumberOfWeeks = 5
                }
            };


            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                IgnoreNullValues = true,
                IgnoreReadOnlyProperties = true
            };

            options.Converters.Add(new DateTimeConverter());
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));


            Schedule schedule = new Schedule()
            {
                Activities = JsonSerializer.Deserialize<List<Activity>>(File.ReadAllText("test.json"), options)
            };

            DateTime dt = schedule.Activities.Max(a => a.Start).StartOfWeek().AddHours(8);

            var lastDay = schedule.Activities.OrderBy(a => a.LastDay).Last().LastDay;

            XmlWriterSettings settings = new XmlWriterSettings()
            {
                OmitXmlDeclaration = true,
                Indent = true
            };

            var weekDayName = new Dictionary<Weekday, string>()
            {
                { Mon, "Må" },
                { Tue, "Ti" },
                { Wed, "On" },
                { Thu, "To" },
                { Fri, "Fr" },
                { Sat, "Lö" },
                { Sun, "Sö" }
            };

            using (StreamWriter fileWriter = new StreamWriter("schedule.html"))
            using (XmlWriter xmlWriter = XmlWriter.Create(fileWriter, settings))
            {
                fileWriter.WriteLine("<html><head></head><body>");

                void startElem(string name) => xmlWriter.WriteStartElement(name);
                void attribute(string name, string value) => xmlWriter.WriteAttributeString(name, value);
                void innerText(string text) => xmlWriter.WriteString(text);
                void endElem() => xmlWriter.WriteEndElement();
                string styles(params (string name, string val)[] styles) =>
                    Join(";", styles.Select(s => s.name + ":" + s.val));

                startElem("div");

                while (dt.WeekOfYear() <= lastDay.WeekOfYear())
                {
                    startElem("h3");
                    innerText($"Vecka {dt.WeekOfYear()}");
                    endElem();

                    startElem("div");
                    attribute("class", "schedule");
                    attribute("style", styles(
                        ("display", "grid"),
                        ("grid-template-rows", Join(" ", Repeat("auto", 11))),
                        ("grid-template-columns", Join(" ", Repeat("1fr", 8)))
                    ));

                    for (int i = 8; i <= 16; ++i)
                    {
                        startElem("div");
                        attribute("style", styles(
                            ("grid-column", "1"),
                            ("grid-row", $"{i - 5}")
                        ));
                        innerText($"{i:D2}-{i + 1:D2}");
                        endElem();
                    }

                    for (int i = 0; i < 7; ++i)
                    {
                        startElem("div");
                        attribute("style", styles(
                            ("grid-row", "2"),
                            ("grid-column", $"{i + 2}")
                        ));
                        innerText($"{weekDayName[(Weekday)i]} {dt.AddDays(i):d/MM}");
                        endElem();
                    }

                    for (int wd = 0; wd < 7; ++wd)
                    {
                        var columnIndex = dt.Subtract(dt.StartOfWeek()).Days + 2;

                        var allDayActivite = schedule.Activities.Where(a => a.AllDay && a.Start.Date == dt.Date);

                        if (allDayActivite.Any())
                        {
                            startElem("div");
                            attribute("style", styles(
                                        ("grid-row", $"1"),
                                        ("grid-column", $"{columnIndex}")
                            ));
                            foreach (var activity in allDayActivite
                                )
                            {
                                startElem("div");
                                attribute("style", styles(
                                        ("width", $"100%")
                                ));
                                innerText(activity.Title);
                                endElem();

                            }
                            endElem();
                        }

                        while (dt.Hour < 17)
                        {
                            var occurence = schedule.GetAcivity(dt);
                            if (occurence.activity != null)
                            {
                                startElem("div");
                                attribute("style", styles(
                                    ("grid-row", $"{dt.Hour - 5} / span {occurence.duration}"),
                                    ("grid-column", $"{columnIndex}")
                                ));
                                innerText(occurence.activity.Title);
                                endElem();
                                dt = dt.AddHours(occurence.duration);
                            }
                            else
                            {
                                dt = dt.AddHours(1);
                            }
                        }
                        dt = new DateTime(dt.Year, dt.Month, dt.Day, 8, 0, 0).AddDays(1);
                    }
                    endElem();
                }
                endElem();
                xmlWriter.Flush();
                fileWriter.WriteLine("</body>");
            }
        }
    }
}

