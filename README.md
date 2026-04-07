
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

Struktura katalogów w `src` odwzorowuje podział na komponenty systemu.

| Usługa / komponent | Lokalizacja | Rola |
|--------------------|-------------|------|
| **ApiGateway** | `ApiGateway` | Punkt wejścia dla ruchu z zewnątrz: routing i ewentualna agregacja żądań do mikrousług. |
| **Blazor Client** | `Clients/Blazor.Client` | Aplikacja frontowa (WebAssembly) — interfejs użytkownika sklepu. |
| **ProductCatalog** | `MicroServices/ProductCatalog` | Katalog produktów: opisy, ceny, widoczność oferty. |
| **ShoppingCart** | `MicroServices/ShoppingCart` | Koszyk: pozycje wybrane przez użytkownika przed złożeniem zamówienia. |
| **Ordering** | `MicroServices/Ordering` | Zamówienia: przyjmowanie, statusy i obsługa procesu zakupowego. |
| **Stock** | `MicroServices/Stock` | Magazyn: dostępność, rezerwacje i zmiany stanów. |
| **Payment** | `MicroServices/Payment` | Płatności: obsługa płatności powiązana z zamówieniami. |
| **IdentityProvider** | `IdentityProvider` | Dostawca tożsamości — logowanie, tokeny, integracja z protokołami typu OpenID Connect / OAuth. |
| **Dashboard** | `MicroServices/Dashboard` | Widoki zbiorcze / raporty dla operatorów lub analityki. |
| **Monitoring** | `MicroServices/Monitoring` | Obserwowalność: metryki, logi, śledzenie żądań między usługami. |

## 🧩 Komunikacja w systemie

| Usługa | Komunikuje się z | Sposób |
|--------|------------------|--------|
| Ordering | ProductCatalog | HTTP |
| Ordering | Stock | gRPC |
| ShoppingCart | Redis | Redis Hash |
| Ordering | Payment | Redis Streams |

System wykorzystuje różne sposoby komunikacji w zależności od problemu biznesowego.

- **HTTP** → najprostsza komunikacja
- **gRPC** → szybkie zapytania i kontrakty
- **Redis Hash** → przechowywanie stanu
- **Redis Streams** → komunikacja asynchroniczna
