
# Przykłady ze szkolenia

## Wprowadzenie

Witaj w repozytorium z materiałami do szkolenia **Architektura mikroserwisów z wykorzystaniem .NET**.

Do rozpoczęcia tego kursu potrzebujesz następujących rzeczy:

1. [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
2. [Docker](https://www.docker.com/products/docker-desktop/)

## Stos technologiczny

- .NET 10
- ASP.NET Core Minimal API
- Docker
- Redis

## Przygotowanie

Sklonuj repozytorium Git i przejdź do katalogu projektu:

```
git clone https://github.com/sulmar/sages-dotnet-microservices-202604
cd sages-dotnet-microservices-202604
```

## Budowanie i uruchamianie

Kod rozwiązania leży w katalogu [`src`](src). Nie ma jednego pliku `.sln` — kompilacja z poziomu `src` obejmuje wszystkie projekty w podkatalogach.

### Budowanie

```
cd src
dotnet build
```

### Redis

Usługi **ShoppingCart**, **Ordering** i **Payment** łączą się z Redis pod adresem `localhost:6379`. Uruchom serwer Redis przed startem aplikacji, np. w Dockerze:

```
docker run -d --name redis-course -p 6379:6379 redis
```

Szczegóły użycia Redis w kodzie: [docs/redis.md](docs/redis.md).

### Uruchamianie (środowisko Development)

System to kilka osobnych procesów (mikrousługi, klient Blazor, brama YARP). W profilu **Development** adresy docelowe bramy są zdefiniowane w plikach `appsettings.Development.json`; usługi nasłuchują na **HTTPS** na portach z `Properties/launchSettings.json`. Aby powiązanie Service Discovery działało zgodnie z konfiguracją, uruchamiaj projekty z profilem **`https`** (w przeciwnym razie porty mogą się nie zgadzać).

W osobnych terminalach, z katalogu głównego repozytorium (ścieżki względem niego):

1. **Stock** (gRPC — wymagany m.in. przez Ordering):

   `dotnet run --project src/MicroServices/Stock/Stock.Api/Stock.Api.csproj --launch-profile https`

2. **ProductCatalog**, **ShoppingCart**, **Ordering**, **Payment**:

   `dotnet run --project src/MicroServices/ProductCatalog/ProductCatalog.Api/ProductCatalog.Api.csproj --launch-profile https`

   `dotnet run --project src/MicroServices/ShoppingCart/ShoppingCart.Api/ShoppingCart.Api.csproj --launch-profile https`

   `dotnet run --project src/MicroServices/Ordering/Ordering.Api/Ordering.Api.csproj --launch-profile https`

   `dotnet run --project src/MicroServices/Payment/Payment.Api/Payment.Api.csproj`

3. **Blazor Client** (profil `https` — brama kieruje na `https://localhost:7108`):

   `dotnet run --project src/Clients/Blazor.Client/BlazorApp.csproj --launch-profile https`

4. **ApiGateway (YARP)** — punkt wejścia dla przeglądarki:

   `dotnet run --project src/ApiGateway/YarpApiGateway/YarpApiGateway.csproj --launch-profile https`

Opcjonalnie możesz uruchomić **Dashboard** (worker nasłuchujący strumienia gRPC Stock):

`dotnet run --project src/MicroServices/Dashboard/Dashboard.Api/Dashboard.Api.csproj --launch-profile https`

Po starcie otwórz w przeglądarce **bramę**: [https://localhost:7000](https://localhost:7000) (alternatywnie HTTP: [http://localhost:5256](http://localhost:5256)). Przy pierwszym wejściu na HTTPS .NET może poprosić o zaufanie certyfikatu deweloperskiego (`dotnet dev-certs https --trust`).

## Przypadki użycia

Lista przypadków użycia systemu: [docs/use-cases/README.md](docs/use-cases/README.md).

## Architektura

Opis usług, komunikacja w systemie oraz diagram architektury: [docs/architecture.md](docs/architecture.md).
