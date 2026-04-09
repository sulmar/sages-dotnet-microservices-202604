
# Przykłady ze szkolenia

## Wprowadzenie

Witaj w repozytorium z materiałami do szkolenia **Architektura mikroserwisów z wykorzystaniem .NET**.

Do rozpoczęcia tego kursu potrzebujesz następujących rzeczy:

1. [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
2. [Docker](https://www.docker.com/products/docker-desktop/)

## Przygotowanie
1. Sklonuj repozytorium Git
```
git clone https://github.com/sulmar/sages-dotnet-microservices-...
```
2. Zbuduj
```
cd src
dotnet build
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

## 🧩 Komunikacja w systemie

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
