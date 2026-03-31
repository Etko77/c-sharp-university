# WebScraperApp – C# Console

## Структура на файловете

```
WebScraperApp/
├── Program.cs           ← Главен файл: меню + извиква задачите
├── WhoisTask.cs         ← Задача 1: IP → Държава
├── CurrentTimeTask.cs   ← Задача 2: Текущ час в София
├── NewsScraperTask.cs   ← Задача 3: Scrape новини от Mediapool
├── NewsArticle.cs       ← Модел за новинарска статия
└── WebScraperApp.csproj
```

## Стартиране

```bash
dotnet restore
dotnet run
```

## Зависимости

- `HtmlAgilityPack` – HTML парсер (сваля се автоматично)
