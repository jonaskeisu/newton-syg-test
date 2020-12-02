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

# Test Coverage

<!-- slide -->

## Vad är test coverage? 

- *Test coverage* betyder att under körning alla unittester samlar information om exakt vilka kodrader i produktionskoden som körs
- Informationen belyser vilken del av produktionskod som saknar unittester
- Företag använder ofta procenten av rader i produktionskoden som körs via unittester som en del av sitt kvalitetsmått för mjukvaran

<!-- slide -->

## Hur lägger man till test coverage?

- I katalogen för UnitTest-projektet lägg till följande paket: 

```console
$ dotnet add package coverlet.msbuild
```

<!-- slide -->

## Köra tester med testcoverage

- Med det nya paketet på plats kan unittesterna köras med kommandot: 

```console
$ dotnet test /p:CollectCoverage=true
```

- Resulterar i en rapport på skärmen och en json-fil innehållande test coverage-information. 

<!-- slide -->

- Kör tester inne från VS Code genom att lägga till följande task i ``tasks.json``:

```json
{
    "label": "NUnit tests",
    "command": "dotnet",
    "type": "process",
    "args": [
        "test",
        "${workspaceFolder}/PokerLib.UnitTest/PokerLib.UnitTest.csproj",
        "/p:CollectCoverage=true",
        "/p:CoverletOutputFormat=\"opencover,lcov\"", 
        "/p:CoverletOutput=../lcov"
    ],
    "problemMatcher": "$msCompile"
}
```

OBS: Ändrasökväg och projektnamn ovan för att exakt matcha ditt projekt ovan. 

<!-- slide -->

## Visualisering av test coverage

- Installera extension Coverage Gutters i VS Code
- Lägg till följande rad i VS Code JSON Settings:

```json
"coverage-gutters.lcovname": "lcov.opencover.xml"
```

