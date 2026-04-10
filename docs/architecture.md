# Architektura systemu

Ten dokument zawiera **diagram** architektury, **opis usług** oraz **komunikację w systemie**. Treść jest zgodna z kodem w katalogu `src`.

## Diagram architektury (Mermaid)

Diagram renderuje się m.in. w GitHubie, GitLabie, wielu edytorach Markdown oraz w [Mermaid Live Editor](https://mermaid.live).

```mermaid
flowchart TB
    User([Użytkownik / przeglądarka])

    subgraph client[Klient]
        Blazor[Blazor Client (WebAssembly)]
    end

    subgraph edge[Punkt wejścia HTTP]
        Gateway[ApiGateway (YARP)]
    end

    subgraph services[Mikrousługi i pomocnicze hosty]
        Identity[IdentityProvider]
        Catalog[ProductCatalog]
        Cart[ShoppingCart]
        Ordering[Ordering]
        Stock[Stock (gRPC)]
        Payment[Payment (worker)]
        Dashboard[Dashboard (SignalR + worker)]
    end

    Redis[(Redis)]

    User -->|HTTPS UI| Blazor
    Blazor -->|REST (sklep), SignalR| Gateway

    Gateway -->|reverse proxy, UI WASM catch-all| Blazor
    Gateway -->|/api/login| Identity
    Gateway -->|/api/products| Catalog
    Gateway -->|/api/cart| Cart
    Gateway -->|/api/orders| Ordering
    Gateway -->|/signalr| Dashboard

    Cart -->|HTTP checkout, Service Discovery| Ordering
    Cart -->|Redis Hash (koszyk)| Redis

    Ordering -->|gRPC CheckAvailability| Stock
    Ordering -->|Redis Streams XADD orders-stream| Redis

    Payment -->|Redis Streams, konsument grupy| Redis
    Dashboard -->|gRPC StreamStockUpdates| Stock
```

## Opis usług

Struktura katalogów w `src` odwzorowuje podział na komponenty systemu. Poniżej — usługi obecne w repozytorium i ich faktyczna rola w kodzie szkoleniowym.

| Usługa / komponent | Lokalizacja | Rola |
|--------------------|-------------|------|
| **ApiGateway (YARP)** | `ApiGateway/YarpApiGateway` | Punkt wejścia HTTP: reverse proxy z trasami do **Blazora** (`/{**catch-all}` — na końcu konfiguracji), **IdentityProvider** (`/api/login/...`), **Dashboard** (`/signalr/...`), **ProductCatalog** (`/api/products/...`), **ShoppingCart** (`/api/cart/...`), **Ordering** (`/api/orders/...`). **Service Discovery** dla adresów docelowych (`AddServiceDiscoveryDestinationResolver`). Przekazywanie ciasteczka `access_token` do nagłówka `Authorization: Bearer` (`AddTransforms`). Uwierzytelnianie **JWT Bearer** w bramie; polityki: m.in. `LoggedPolicy` (wymagany zalogowany użytkownik) na trasach katalogu, koszyka i zamówień; `CreatorPolicy` na trasie SignalR do Dashboardu. |
| **Blazor Client** | `Clients/Blazor.Client` | Front WebAssembly (UI sklepu). **Katalog i koszyk** — `HttpClient` z bazą `https://localhost:7000` (brama). **SignalR** — `https://localhost:7000/signalr/stock` (proxy YARP do Dashboardu). Domyślny zarejestrowany `HttpClient` ma `BaseAddress` = host WASM (np. `7108`) — użycie w `LoginComponent` (`POST api/login`) trafia wtedy na host Blazora, a nie na trasę bramy do IdentityProvider (pełny przepływ przez bramę wymaga spójnej bazy URL). |
| **IdentityProvider** | `IdentityProvider/IdentityProvider.Api` | Lekki host **logowania**: `POST /api/login` — walidacja (fałszywe repozytorium), ustawienie ciasteczka `access_token` (JWT). Nie jest klasycznym IdP produkcyjnym — demonstracja przepływu z bramą. |
| **ProductCatalog** | `MicroServices/ProductCatalog` | REST API katalogu (`GET /api/products` itd.); dane generowane w pamięci (Bogus / repozytorium in-memory). |
| **ShoppingCart** | `MicroServices/ShoppingCart` | Koszyk: persystencja pozycji w Redis (hash); `POST /api/cart/checkout` wywołuje logikę domenową z klientem HTTP `OrderingApi` (w `Program.cs`: baza `https://ordering` + Service Discovery). |
| **Ordering** | `MicroServices/Ordering` | Przyjmowanie zamówień (`POST /api/orders`): zapis do kanału w pamięci; **OrderWorker** → **OrderProcessor** — dla każdej pozycji **gRPC `CheckAvailability`** do **Stock**; po sukcesie nadanie `order.Id` (Nanoid), **XADD** na Redis Stream `orders-stream` (dla **Payment**). |
| **Stock** | `MicroServices/Stock` | Serwis **gRPC** (`CheckAvailability`, `ReserveStock`, `StreamStockUpdates`): weryfikacja dostępności dla **Ordering**; strumień aktualizacji dla **Dashboard**. Nie jest wystawiony przez YARP — tylko wywołania serwer-do-serwera po HTTPS (porty w `launchSettings` / adresy w kodzie klientów gRPC). |
| **Payment** | `MicroServices/Payment` | Proces roboczy (`BackgroundService`), nie klasyczne API HTTP: odczyt grupy konsumentów Redis z `orders-stream`, symulacja przetwarzania płatności i potwierdzenie wiadomości (`XACK`). |
| **Dashboard** | `MicroServices/Dashboard` | Host **SignalR** — hub `StockHub` pod ścieżką `/signalr/stock`; **StockWorker** subskrybuje **gRPC `StreamStockUpdates`** z **Stock** i rozsyła zdarzenia do klientów SignalR. Logowanie przez Serilog (konsola + plik rotowany). |

## Komunikacja w systemie

| Usługa | Komunikuje się z | Sposób |
|--------|------------------|--------|
| Blazor Client | ApiGateway | HTTP: produkty, koszyk, zamówienia (klienty wskazujące na port bramy) |
| Blazor Client | ApiGateway → Dashboard | WebSocket / SignalR (`/signalr/stock` — proxy YARP) |
| ApiGateway | Blazor, IdentityProvider, ProductCatalog, ShoppingCart, Ordering, Dashboard | HTTP (reverse proxy); do tras z polityką JWT oczekiwane ważne ciasteczko / token |
| IdentityProvider | — | Brak wywołań do innych usług w tym przykładzie (tylko odpowiedź na `POST /api/login`) |
| Ordering | Stock | gRPC (`CheckAvailability`) |
| ShoppingCart | Ordering | HTTP (`/api/cart/checkout` → klient `OrderingApi` + Service Discovery) |
| ShoppingCart | Redis | Redis (hash — pozycje koszyka) |
| Ordering | Payment (konsument) | Redis Streams (`orders-stream` — producent Ordering, konsument Payment) |
| Dashboard | Stock | gRPC (strumień `StreamStockUpdates`) |

System wykorzystuje różne sposoby komunikacji w zależności od problemu biznesowego.

- **HTTP** → najprostsza komunikacja
- **gRPC** → szybkie zapytania i kontrakty; strumienie serwerowe
- **Redis Hash** → przechowywanie stanu
- **Redis Streams** → komunikacja asynchroniczna
- **SignalR** → push aktualizacji stanów magazynu do przeglądarki (przez YARP do Dashboardu)

## Legenda — protokoły na łączach

| Protokół / mechanizm | Zastosowanie w systemie |
|----------------------|-------------------------|
| **HTTP / HTTPS** | Wejście z przeglądarki do gateway; reverse proxy YARP do Blazora, IdentityProvidera, mikrousług REST i Dashboardu (ścieżka SignalR); checkout z koszyka do Ordering (klient HTTP + Service Discovery). |
| **gRPC** | Ordering → Stock (`CheckAvailability`); Dashboard → Stock (`StreamStockUpdates`). |
| **Redis (Hash)** | Persystencja pozycji koszyka w ShoppingCart. |
| **Redis Streams** | Ordering publikuje zdarzenia zamówienia (`orders-stream`); Payment odczytuje je jako worker (grupa konsumentów). |
| **SignalR** | Dashboard publikuje aktualizacje stanów do Blazora; klient łączy się z hostem Dashboardu **przez** ApiGateway (trasa `/signalr/...`). |

## Uwagi

- **ApiGateway** jest docelowym punktem wejścia dla wywołań sklepu z Blazora (`https://localhost:7000` — katalog, koszyk, SignalR). Trasa `/api/login/...` w bramie kieruje do **IdentityProvider**; aby formularz logowania korzystał z tej trasy, `HttpClient` musiałby używać tej samej bazy URL co pozostałe wywołania (w kodzie szkoleniowym domyślny `HttpClient` wskazuje host aplikacji WASM).
- W `appsettings.json` bramy trasa **Blazora** (`/{**catch-all}`) jest **ostatnia** — w przeciwnym razie przejmowałaby ścieżki `/api/...` i `/signalr/...`.
- **Stock** i **Payment** nie mają tras w YARP: Stock tylko dla wywołań gRPC z innych hostów; Payment tylko Redis.
- **Payment** nie wystawia klasycznego API HTTP — jest procesem hostowanym (`BackgroundService`), powiązany z resztą systemu wyłącznie przez Redis Streams.
- Szczegóły tras proxy, klastrów i mapowania nazw usług Service Discovery: `ApiGateway/YarpApiGateway/appsettings*.json` oraz `Properties/launchSettings.json` poszczególnych projektów.
