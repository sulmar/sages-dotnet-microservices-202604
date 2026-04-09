# Redis — ściąga (struktury i polecenia z projektu)

Poniżej: typy danych Redis oraz polecenia ** faktycznie wykorzystane w mikroserwisach** (`ShoppingCart`, `Ordering`, `Payment`) i odpowiadające im wywołania w **StackExchange.Redis** (C#).

Połączenie w kodzie: `ConnectionMultiplexer.Connect("localhost:6379")`, baza: `GetDatabase()`.

---

## String

Typ **STRING** przechowuje wartość tekstową lub binarną pod jednym kluczem.

W **tym repozytorium** nie ma osobnych operacji `SET` / `GET` na kluczach typu string — wartości tekstowe pojawiają się jako **pola i wartości** w hashach oraz wpisach streamu (Redis i tak trzyma je jako stringi).

| Polecenie Redis | Opis |
|-----------------|------|
| `SET key value` | Ustawia wartość klucza |
| `GET key` | Odczyt wartości |
| `INCR` / `DECR` | Licznik na stringu (jeśli wartość jest liczbą) |

---

## Hash

Mapa pól → wartości pod jednym kluczem. W projekcie: **koszyk zakupów** — klucz sesji, pola to pozycje produktów z ilością.

**Serwis:** `ShoppingCart`  
**Plik:** `ShoppingCart.Infrastructure/Repositories/RedisCartItemRepository.cs`

| Konwencja | Przykład |
|-----------|----------|
| Klucz koszyka | `cart:{SessionId}` |
| Pole (produkt) | `product:{product.Id}` |
| Wartość pola | ilość sztuk (liczba całkowita) |

| Polecenie Redis | StackExchange.Redis | Zastosowanie w projekcie |
|-----------------|---------------------|-------------------------|
| `HINCRBY key field increment` | `HashIncrementAsync` | Zwiększenie ilości produktu w koszyku (+1) |
| `HGETALL key` | `HashGetAllAsync` | Lista wszystkich pozycji koszyka |
| `EXPIRE key seconds` | `KeyExpireAsync` | TTL koszyka — **2 minuty** (`TimeSpan.FromMinutes(2)`) |

### EXPIRE i TTL

Dotyczą **całego klucza** (np. hash koszyka `cart:{SessionId}`), nie pojedynczych pól.

| Polecenie Redis | StackExchange.Redis | Opis |
|-----------------|---------------------|------|
| `EXPIRE key seconds` | `KeyExpireAsync(key, TimeSpan)` lub `KeyExpireAsync(key, expiry)` | Ustawia lub przedłuża czas życia klucza (sekundy / `DateTime`). |
| `TTL key` | `KeyTimeToLiveAsync(key)` | **CLI:** pozostałe sekundy do wygaśnięcia; `-1` = klucz bez TTL; `-2` = brak klucza. **SE.Redis:** `TimeSpan?` z pozostałym czasem (gdy klucz nie istnieje — `null`). |

W projekcie wywoływane jest tylko **`EXPIRE`** (przez `KeyExpireAsync`) przy każdym dodaniu pozycji do koszyka. **`TTL`** służy do odczytu pozostałego czasu (np. w debugowaniu lub UI); w kodzie mikroserwisów nie występuje.

---

## Stream

Log wiadomości z ID typu czasowego. W projekcie: **zdarzenia zamówień** — producent zapisuje złożenie zamówienia, **Payment** czyta w grupie konsumentów i potwierdza przetworzenie.

**Stream:** `orders-stream`  
**Grupa konsumentów:** `payment-group`  
**Producent:** `Ordering.Api` (`Program.cs`)  
**Konsument:** `Payment.Api` (`PaymentWorker.cs`)

Pola wpisu (jak w `StreamAddAsync`):

| Pole | Przykładowa treść |
|------|-------------------|
| `type` | `order-placed` |
| `orderId` | identyfikator zamówienia (Nanoid) |
| `amount` | suma zamówienia (string z liczby) |
| `status` | np. `pending` |

| Polecenie Redis | StackExchange.Redis | Kto / co |
|-----------------|---------------------|----------|
| `XADD key * field value ...` | `StreamAddAsync` | Ordering — nowy wpis po utworzeniu zamówienia |
| `XGROUP CREATE key group id MKSTREAM` | `StreamCreateConsumerGroupAsync(..., createStream: true)` | Payment — utworzenie grupy, jeśli nie istnieje (`StreamGroupInfoAsync` + warunek) |
| `XREADGROUP GROUP group consumer COUNT n STREAMS key >` | `StreamReadGroupAsync` | Payment — odczyt nowych wiadomości dla konsumenta (`">"`, `count: 1`) |
| `XACK key group id` | `StreamAcknowledgeAsync` | Payment — potwierdzenie po „przetworzeniu” płatności |

Komentarze w kodzie mapują te same polecenia na składnię CLI (np. `XREADGROUP ... STREAMS orders-stream >`).

---

## Szybkie odniesienie do plików

| Obszar | Plik |
|--------|------|
| Hash / koszyk | `src/MicroServices/ShoppingCart/ShoppingCart.Infrastructure/Repositories/RedisCartItemRepository.cs` |
| Stream / producent | `src/MicroServices/Ordering/Ordering.Api/Program.cs` |
| Stream / konsument | `src/MicroServices/Payment/Payment.Api/PaymentWorker.cs` |
| Rejestracja Redis | `ShoppingCart.Api/Program.cs`, `Ordering.Api/Program.cs`, `Payment.Api/Program.cs` |
