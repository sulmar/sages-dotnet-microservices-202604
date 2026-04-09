# .NET — ściąga `dotnet new` (szablony z projektu)

Poniżej skrócone nazwy szablonów (`-n` / short name) oraz typowe polecenia tworzenia projektów **zgodne ze strukturą tego repozytorium**. Pełna lista: `dotnet new list`.

Domyślnie projekt powstaje w bieżącym katalogu. Aby wskazać folder docelowy, użyj `-o NazwaKatalogu` (np. `dotnet new web -o ProductCatalog.Api`).

---

## Usługi WWW i API (HTTP)

| Szablon | Polecenie | SDK / zastosowanie w projekcie |
|---------|-----------|--------------------------------|
| **web** — ASP.NET Core (minimalny host) | `dotnet new web -n Nazwa` | `Microsoft.NET.Sdk.Web` — bramka YARP (`ApiGateway/YarpApiGateway`), mikrousługi REST (`*.Api`), Dashboard z workerem w tle |
| **webapi** — Web API (kontrolery) | `dotnet new webapi -n Nazwa` | Alternatywa dla REST z kontrolerami zamiast minimalnych endpointów |

---

## Worker (BackgroundService)

| Szablon | Polecenie | SDK / zastosowanie w projekcie |
|---------|-----------|--------------------------------|
| **worker** | `dotnet new worker -n Nazwa` | `Microsoft.NET.Sdk.Worker` — usługa **Payment** (konsument Redis Streams, brak klasycznego API HTTP) |

---

## Biblioteki (domena, infrastruktura)

| Szablon | Polecenie | SDK / zastosowanie w projekcie |
|---------|-----------|--------------------------------|
| **classlib** | `dotnet new classlib -n Nazwa` | `Microsoft.NET.Sdk` — projekty `*.Domain`, `*.Infrastructure` |

---

## gRPC

| Szablon | Polecenie | Uwagi |
|---------|-----------|--------|
| **grpc** | `dotnet new grpc -n Nazwa` | `Microsoft.NET.Sdk.Web` + `Grpc.AspNetCore` — serwer gRPC jak **Stock** (pliki `.proto` w `Protos/`) |

W repozytorium **Dashboard** to też `web`, ale z klientem gRPC (`Grpc.AspNetCore` + `Protobuf` z `GrpcServices="Client"`), a nie szablon `grpc`.

---

## Blazor (klient WebAssembly)

| Szablon | Polecenie | SDK / zastosowanie w projekcie |
|---------|-----------|--------------------------------|
| **blazorwasm** | `dotnet new blazorwasm -n Nazwa` | `Microsoft.NET.Sdk.BlazorWebAssembly` — **Clients/Blazor.Client** |

---

## Rozwiązanie i pliki pomocnicze

| Szablon | Polecenie | Uwagi |
|---------|-----------|--------|
| **sln** | `dotnet new sln -n Nazwa` | Plik rozwiązania (w tym repo często używany jest `dotnet build` z poziomu `src` bez jednego `.sln` w korzeniu) |
| **gitignore** | `dotnet new gitignore` | `.gitignore` dla .NET (wspomniane także w `.gitignore` repozytorium) |

---

## Przykłady jednym ciągiem

```bash
# Mikrousługa API (jak ProductCatalog.Api, Ordering.Api)
dotnet new web -o ProductCatalog.Api

# Worker jak Payment
dotnet new worker -o Payment.Api

# Warstwa domeny / infrastruktury
dotnet new classlib -o ProductCatalog.Domain
dotnet new classlib -o ProductCatalog.Infrastructure

# Serwer gRPC (punkt wyjścia pod Stock.Api)
dotnet new grpc -o Stock.Api

# Front Blazor WASM
dotnet new blazorwasm -o BlazorApp

# Rozwiązanie i dodanie projektów (opcjonalnie)
dotnet new sln -o MySystem
cd MySystem
dotnet sln add **/*.csproj
```

Opcje często przydatne przy szkoleniu: `--framework net10.0` (jeśli masz kilka TFMon w SDK), `-lang C#`.
