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

    subgraph services[Mikrousługi]
        Catalog[ProductCatalog]
        Cart[ShoppingCart]
        Ordering[Ordering]
        Stock[Stock (gRPC)]
        Payment[Payment (worker)]
        Dashboard[Dashboard]
    end

    Redis[(Redis)]

    User -->|HTTPS UI| Blazor
    Blazor -->|HTTP/HTTPS REST| Gateway

    Gateway -->|HTTP reverse proxy, UI/WASM| Blazor
    Gateway -->|HTTP /api/products/...| Catalog
    Gateway -->|HTTP /api/cart/...| Cart
    Gateway -->|HTTP /api/orders/...| Ordering

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
| **ApiGateway (YARP)** | `ApiGateway/YarpApiGateway` | Punkt wejścia HTTP: reverse proxy z konfiguracją tras do Blazora (`/{**catch-all}`), katalogu (`/api/products/...`), koszyka (`/api/cart/...`) i zamówień (`/api/orders/...`); integracja z Service Discovery; szkielet uwierzytelniania Bearer i polityk autoryzacji. |
| **Blazor Client** | `Clients/Blazor.Client` | Front WebAssembly (UI sklepu); żądania do API kierowane przez gateway (np. produkty i koszyk). |
| **ProductCatalog** | `MicroServices/ProductCatalog` | REST API katalogu (`GET /api/products`); dane generowane w pamięci (Bogus / repozytorium in-memory). |
| **ShoppingCart** | `MicroServices/ShoppingCart` | Koszyk: persystencja pozycji w Redis (hash); `checkout` wywołuje logikę domenową z klientem HTTP do usługi Ordering (Service Discovery). |
| **Ordering** | `MicroServices/Ordering` | Przyjmowanie zamówień (`POST /api/orders`): przed utworzeniem zamówienia weryfikacja dostępności przez klienta gRPC do **Stock**; po utworzeniu — publikacja zdarzenia na Redis Stream `orders-stream` (m.in. dla płatności). |
| **Stock** | `MicroServices/Stock` | Serwis **gRPC**: m.in. sprawdzanie dostępności i strumieniowe aktualizacje stanów magazynu (wykorzystywane przez Ordering i worker w Dashboard). |
| **Payment** | `MicroServices/Payment` | Proces roboczy (`BackgroundService`), nie klasyczne API HTTP: odczyt grupy konsumentów Redis z `orders-stream`, symulacja przetwarzania płatności i potwierdzenie wiadomości (`XACK`). |
| **Dashboard** | `MicroServices/Dashboard` | Minimalna aplikacja hostująca worker nasłuchujący strumienia gRPC Stock (`StreamStockUpdates`); logowanie przez Serilog (konsola + plik rotowany). |

## Komunikacja w systemie

| Usługa | Komunikuje się z | Sposób |
|--------|------------------|--------|
| Blazor Client | ApiGateway | HTTP (produkty, koszyk itd.) |
| ApiGateway | Blazor, ProductCatalog, ShoppingCart, Ordering | HTTP (reverse proxy) |
| Ordering | Stock | gRPC |
| ShoppingCart | Ordering | HTTP (checkout; Service Discovery) |
| ShoppingCart | Redis | Redis (hash — pozycje koszyka) |
| Ordering | Payment (konsument) | Redis Streams (`orders-stream`) |
| Dashboard | Stock | gRPC (strumień aktualizacji) |

System wykorzystuje różne sposoby komunikacji w zależności od problemu biznesowego.

- **HTTP** → najprostsza komunikacja
- **gRPC** → szybkie zapytania i kontrakty
- **Redis Hash** → przechowywanie stanu
- **Redis Streams** → komunikacja asynchroniczna

## Legenda — protokoły na łączach

| Protokół / mechanizm | Zastosowanie w systemie |
|----------------------|-------------------------|
| **HTTP / HTTPS** | Wejście z przeglądarki do gateway; reverse proxy YARP do mikrousług i hostowania Blazora; checkout z koszyka do Ordering (klient HTTP + Service Discovery). |
| **gRPC** | Ordering → Stock (synchroniczna weryfikacja dostępności); Dashboard → Stock (strumień aktualizacji stanów). |
| **Redis (Hash)** | Persystencja pozycji koszyka w ShoppingCart. |
| **Redis Streams** | Ordering publikuje zdarzenia zamówienia (`orders-stream`); Payment odczytuje je jako worker (grupa konsumentów). |

## Uwagi

- **ApiGateway** jest jedynym punktem wejścia HTTP z perspektywy klienta Blazor (adres bazowy w konfiguracji klienta).
- **Payment** nie wystawia klasycznego API HTTP — jest procesem hostowanym (`BackgroundService`), powiązany z resztą systemu wyłącznie przez Redis Streams.
- Szczegóły tras proxy i portów znajdują się w `ApiGateway/YarpApiGateway/appsettings*.json` oraz w `Properties/launchSettings.json` poszczególnych projektów.
