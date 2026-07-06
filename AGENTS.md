# CoverageBSL — руководство для агентов

Внешняя компонента (native add-in) для сбора **покрытия кода** платформы **1С:Предприятие** через HTTP-отладчик. Используется из **OneScript** (пакет `coveragebsl`) и может подключаться как внешняя компонента в 1С.

## Назначение

Библиотека подключается к серверу отладки 1С (`http://localhost:1550` по умолчанию), регистрирует UI-отладчик, настраивает авто-подключение к целям отладки, включает режим замера (`measure mode`) и собирает данные о выполненных строках модулей. Результат сериализуется в JSON.

Типичный сценарий — CI/CD: запуск тестов через `vrunner` с параметрами `/Debug -http -attach /DEBUGGERURL`, сбор покрытия во время прогона, сохранение `platformCoverage.json`.

## Архитектура

```
OneScript / 1С
    │
    ▼
CoverageBSL.AddIn          ← обёртки [ContextClass] для BSL (МенеджерПокрытия, СессияПокрытия, …)
    │
    ▼
CoverageBSL                ← высокоуровневый API (ICoverageManager, ICoverageSession, ICoverageData)
    │
    ▼
DebugClientBSL             ← HTTP-клиент протокола RDBG (attach, targets, measure)
    │
    ▼
DebugBSL                   ← модели XML-запросов/ответов отладчика 1С (сериализация, DTO)
    │
    ▼
Сервер отладки 1С          ← POST application/xml → /e1crdbg/rdbg?cmd=…
```

### Слои проектов

| Проект | Путь | Роль |
|--------|------|------|
| **DebugBSL** | `src/DebugBSL/` | DTO и XML-сериализация протокола отладчика 1С (`debugger/debugRDBGRequestResponse`, `debugger/debugMeasure`, `debugger/debugBaseData`, …). Без сетевого кода. |
| **DebugClientBSL** | `src/DebugClientBSL/` | HTTP-клиент: `DebuggerClientExecutor` (POST XML), `DebuggerClientSession` (attach/detach/ping), менеджеры целей и замера. |
| **CoverageBSL** | `src/CoverageBSL/` | Доменный API покрытия: `CoverageManager`, `CoverageSession`, `CoverageData`. |
| **CoverageBSL.AddIn** | `src/CoverageBSL.AddIn/` | Мост в OneScript/1С: классы с `[ContextClass]`, двуязычные имена (en/ru). |
| **DebugBSL.Tests** | `src/DebugBSL.Tests/` | Unit-тесты сериализации и моделей (NUnit, эталонные XML). |
| **DebugClientBSL.Tests** | `src/DebugClientBSL.Tests/` | Unit-тесты клиента (mock HTTP). |

Общие настройки сборки: `src/Common/project.props` (версия `0.5.0`, `TargetFramework`: `net8.0`; версия C# — по умолчанию для SDK).

### Зависимости

| Пакет | Проект | Версия |
|-------|--------|--------|
| OneScript.StandardLibrary | CoverageBSL.AddIn | 2.1.0 |
| NUnit | *.Tests | 4.6.1 |
| NUnit3TestAdapter | *.Tests | 6.2.0 |
| Microsoft.NET.Test.Sdk | *.Tests | 18.7.0 |

Пакет opm требует OneScript `2.1.0` (см. `build/packagedef.os`).

## Поток работы (runtime)

1. **CoverageManager(debuggerURI)** — создаёт `IDebuggerClient` через `DebuggerClientFactory`.
2. **NewCoverageSession(infobaseAlias)** — сессия с alias информационной базы (например `DefAlias`).
3. **Attach(password)** — `attachDebugUI`; при успехе настраивает auto-attach для типов целей (Client, ManagedClient, WEBClient, Server, ServerEmulation) и подключает уже существующие цели.
4. **StartCoverageCapture()** — `setMeasureMode` с новым GUID; сервер начинает присылать `PerformanceInfoMain` через ping-события.
5. Запуск тестируемого кода (например `vrunner run` с флагами отладки).
6. **StopCoverageCapture()** — останавливает measure mode, агрегирует `PerformanceInfoMain` в `ICoverageData`.
7. **Detach()** — отключение от отладчика.
8. **SerializeJSON** — вывод в JSON (модули, строки, счётчики выполнения).

Ключевые файлы реализации:

- `src/CoverageBSL/Impl/CoverageManager.cs` — точка входа API.
- `src/CoverageBSL/Impl/CoverageSession.cs` — attach + measure lifecycle.
- `src/DebugClientBSL/Impl/DebuggerClientMeasureManager.cs` — start/stop measure, накопление `PerformanceInfo`.
- `src/DebugClientBSL/Internal/DebuggerClientExecutor.cs` — HTTP POST к `{debuggerURI}/e1crdbg/rdbg?cmd={command}&dbgui={sessionId}`.

## Протокол отладчика

- Транспорт: **HTTP POST**, тело и ответ — **XML** (`application/xml`).
- URL: `{root}/e1crdbg/rdbg?cmd=<команда>&dbgui=<guid>` (см. `RequestParameters`).
- Основные команды: `attachDebugUI`, `detachDebugUI`, `setMeasureMode`, `setAutoAttachSettings`, `attachDetachDebugTargets`, `pingDebugUI`, `getAPIVer`, `test`.
- Данные покрытия: `PerformanceInfoMain` → `PerformanceInfoModule` → `PerformanceInfoLine` (namespace `http://v8.1c.ru/8.3/debugger/debugMeasure`).

При изменении протокола сначала обновляйте модели в **DebugBSL**, затем логику в **DebugClientBSL**, затем при необходимости обёртки в **CoverageBSL.AddIn**.

## API для скриптов (OneScript)

Подключение пакета: `#Использовать coveragebsl` (см. `build/package-loader.os`).

| Класс (en / ru) | Методы |
|-----------------|--------|
| `CoverageManager` / `МенеджерПокрытия` | `TestConnection`, `APIVersion`, `NewCoverageSession`, `GetCoverableLines` |
| `CoverageSession` / `СессияПокрытия` | `Attach`, `Detach`, `StartCoverageCapture`, `StopCoverageCapture` |
| `CoverageData` / `ДанныеПокрытия` | `TotalDurability`, `Data`, `SerializeJSON`, `PrepareModuleCoverableLines`, `PrepareModuleCoverableLinesFromSource` |

Пример: `examples/collectCoverage.ru.os`, `examples/collectCoverage.en.os`.

Загрузка DLL: `build/package-loader.os` подключает `bin/net8.0/CoverageBSL.AddIn.dll`.

## Сборка

### .NET (разработка)

```bash
dotnet build src/CoverageBSL.sln -c Release
```

### Пакет OneScript (opm)

```bash
opm build build/packagedef.os
```

Скрипт `build/packagedef.os` перед упаковкой:
1. Собирает solution через `dotnet msbuild`.
2. Копирует 4 DLL (`CoverageBSL.AddIn.dll`, `CoverageBSL.dll`, `DebugBSL.dll`, `DebugClientBSL.dll`) в `bin/net8.0`.

Версия пакета берётся из `<ReleaseNumber>` в `src/Common/project.props`.

## Тесты

```bash
dotnet build src/CoverageBSL.sln -c Release
dotnet test src/CoverageBSL.sln -c Release
```

- **DebugBSL.Tests** — round-trip XML (эталонные файлы в `debugger/**/*.xml`), 16 тестов.
- **DebugClientBSL.Tests** — `MockHttpMessageHandler` для HTTP-слоя, 20 тестов.
- **CoverageBSL.AddIn.Tests** — извлечение строк к покрытию из AST BSL по правилам Coverage41C (`Resources/*.bsl`), 1 тест.
- Итого **37 тестов** (`net8.0`).

При добавлении новых DTO добавляйте XML-фикстуры и тесты сериализации в DebugBSL.Tests.

**NUnit 4.6 и async:** для проверки async-кода предпочитайте `await Assert.ThatAsync(...)` в тестах `async Task`. При неоднозначности перегрузок `AsyncTestDelegate` и `Func<Task>` явно приводите лямбду к `Func<Task>`:

```csharp
await Assert.ThatAsync(
    (Func<Task>)(async () => await client.TestAsync()),
    Throws.Nothing);

await Assert.ThatAsync(
    (Func<Task>)(async () => await executor.ExecuteAsync<T>(request, parameters)),
    Throws.Exception.With.Message.EqualTo("Test"));
```

## Соглашения кода

- **Namespace**: `com.github.yukon39.<ProjectName>`.
- **Интерфейсы** в корне проекта (`ICoverageManager`), реализации в `Impl/`.
- **Async**: публичные sync-методы — обёртки над `*Async` с `.ConfigureAwait(false)` (см. `CoverageManager`).
- **AddIn**: все публичные методы оборачиваются в try/catch с `RuntimeExceptionFactory` и локализованными сообщениями (`Locale.NStr` en/ru). Используются API OneScript 2.x (`OneScript.*`, `AutoCollectionContext`, `EnumValue`, …).
- **Не дублировать** протокольные модели — они живут только в DebugBSL.
- Минимальный scope изменений: не трогать несвязанные слои.
- При обновлении NuGet-пакетов: синхронно обновляйте оба тестовых `.csproj` и проверяйте сборку и тесты.
- **AGENTS.md**: при существенных изменениях в проекте (TFM, зависимости, API, сборка, тесты) всегда обновляйте этот файл.

## Структура репозитория

```
CoverageBSL/
├── AGENTS.md              ← этот файл
├── README.md
├── build/
│   ├── packagedef.os      ← сборка opm-пакета
│   └── package-loader.os  ← загрузка native add-in
├── examples/              ← примеры OneScript
└── src/
    ├── Common/project.props
    ├── CoverageBSL.sln
    ├── DebugBSL/
    ├── DebugClientBSL/
    ├── CoverageBSL/
    ├── CoverageBSL.AddIn/
    ├── CoverageBSL.AddIn.Tests/
    ├── DebugBSL.Tests/
    └── DebugClientBSL.Tests/
```

Строки к покрытию (`GetCoverableLines`, `PrepareModuleCoverableLines*`) реализованы целиком в **CoverageBSL.AddIn** (`Internal/`).

## Частые задачи для агентов

| Задача | Где искать |
|--------|------------|
| Новое поле в данных покрытия | `PerformanceInfo*.cs` → AddIn wrappers в `CoverageBSL.AddIn/Debugger/` |
| Новая команда отладчика | `debugRDBGRequestResponse/` + `DebuggerClient*Manager.cs` |
| Ошибка attach / auto-attach | `CoverageSession.AttachAsync`, `DebuggerClientTargetsManager` |
| Проблема загрузки в OneScript | `package-loader.os`, версии `OneScript.StandardLibrary` в `.csproj` |
| Сериализация JSON из BSL | `CoverageDataContextClass.SerializeJson`, `CoverageModuleInfoList` |
| Строки к покрытию (исполняемые строки модуля) | `AstCoverableLinesCollector`, `CoverableLinesProvider`, `CoverableLinesPreparer` (AddIn/Internal), `CoverageManager.GetCoverableLines` |

## Ограничения и контекст

- Требуется запущенный **сервер отладки 1С** и процессы, запущенные с `/Debug -http -attach`.
- Пароль отладчика передаётся в `Attach`, но передача credentials в HTTP-запросе в коде закомментирована (`DebuggerClientSession`).
- Проект не содержит CI workflow в `.github/` — сборка и тесты локальные.
- Лицензия: см. `LICENSE`.
