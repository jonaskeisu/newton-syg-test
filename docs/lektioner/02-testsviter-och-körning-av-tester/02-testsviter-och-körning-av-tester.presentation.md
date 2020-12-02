---
presentation:
  width: 1200
  height: 600
  theme: 'serif.css'
  center: false
  slideNumber: true
---
<style type="text/css">
  .reveal h1 {
    display: inline;
    text-align: center;
    display: flex;
    flex-direction: column;
    align-items: center;
  }
  .reveal p {
    text-align: left;
  }
  .reveal ul {
    display: block;
  }
  .reveal ol {
    display: block;
  }
  .reveal section {
    resize: false;
    width: 100%;
    height: 100;
    text-align: left;
   
  }
  .reveal pre {
    zoom: 110%;
  }
  div.slides{
    # border: 1px solid black;
  }
  .reveal code {
    zoom: 90%;
  }
</style>

<!-- slide -->

# Testsviter och körning av tester

<!-- slide -->

## Kategoriering av tester

Det går att kategorisera tester med hjälp av attribut: 

```cs
using NUnit.Framework;
namespace NUnitNamespace{
    public class UnitTest1 {
        [Test, Property("Priority", 1), Category("CategoryA")]
        public void TestMethod1() {
        }
        [Test, Property("Priority", 2)]
        public void TestMethod2() {
        }
    }
}
```

<!-- slide -->

## Selektiv körning av tester

- Vid körning av tester kan argumentet ``--filter`` användas för att välja ut vilka tester som skall ingå i körningen. 

Exempel: 

```console
$ dotnet test --filter "FullyQualifiedName~UnitTest1|TestCategory=CategoryA"
```

<!-- slide -->

## Urval av tester

<div style="zoom: 0.6">

| Uttryck | Resultat | 
| --- | --- | 
| ``dotnet test --filter Metod`` | Kör enbart tester med det exakta metodnamnet  | 
| ``dotnet test --filter Name~TestMethod1`` | Kör alla tester som har ett metodnamn som innehåller säksträngen. | 
| ``dotnet test --filter FullyQualifiedName~NUnitNamespace.UnitTest1`` | Kör alla tester som har ett fullständigt kvalificerat namn som innehåller söksträngen. |
| ``dotnet test --filter TestCategory=CategoryA`` | Kör bara tester i med kategori ``CategoryA``. | 
| ``dotnet test --filter Priority=2`` | Kör bara tester där egenskapen ``Property`` har värdet ``2`` | 

</div>

<!-- slide -->

## Testsviter

- Urval av tester används för att skapa testsviter med olika syfte. 
- T.ex. särskiljning av: 
  - Experiment från kritiska tester
  - Långsamma tester som skall köras ibland från snabba tester som bör köras innan varje commit. 

<!-- slide -->

## Interaktiv körning av tester 

- Tester kan köras interaktivt från Visual Studio Code via länkar ovanför respektive testmetod i kodredigeringsfönstret.
  - Även med debuggning. 
- Praktiskt för att kunna fokusera på ett fallerande test i taget. 


<!-- slide -->

## Test Explorer

- Många utvecklingsmiljöer har något form av grafisk Test Explorer. 
- I Visual Studio Code finns t.ex. extensions *.NET Core Test Explorer*
  - Fungerar 

<!-- slide -->

## Parametrisk

- *Parametrisk* (eng. *parameterized*) betyder att något har en parameter, precis som en funktion kan ha parametrar. 
- Ett objekt som är parametriskt måste få sina parametrar tilldelade ett värde för att bli något konkret. 
- En generisk metod är t.ex. en mall för en metod med en typparameter. Givet ett värde på parameter (i det här fallet en typ) så blir mallen en konkret metod som kan anropas. 

<!-- slide -->

## Parametrisk testning

- Parametrisk testning är mallar för att skapa tester genom att tilldela parametrar olika värden. 
- Vanligaste formen av parametrisk test tar parametervärden som argument till testmetoden.

<!-- slide -->

## Sequential 

Med extraattributet ``Sequential`` så skapas ett parametriserat test t.ex. på följande vis: 

```cs
[Test, Sequential]
public void MyTest(
    [Values(1, 2, 3)] int x,
    [Values("A", "B", "C")] string s) {
    /* ... */
}
```

<!-- slide -->

För första värdet på samtliga parameterarna skapas ett test, sedan ett för andra värdet på samtliga parametrar, osv. 

```cs
MyTest(1, "A")
MyTest(2, "B")
MyTest(3, "C")
```

<!-- slide -->

## Combinatorial 

Med extraattributet ``Combinatorial`` så skapas ett parametriserat test på liknande vis som för ``Sequential``: 

```cs
[Test, Sequential]
public void MyTest(
    [Values(1, 2, 3)] int x,
    [Values("A", "B")] string s) {
    /* ... */
}
```

<!-- slide -->

.. men de resulterande testen blir alla möjliga kombinationer av värden tilldelade parametrarna:

```cs
MyTest(1, "A")
MyTest(1, "B")
MyTest(2, "A")
MyTest(2, "B")
MyTest(3, "A")
MyTest(3, "B")
```