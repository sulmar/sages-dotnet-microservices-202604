# UC-04 – Opłacenie zamówienia

## Opis

System przetwarza płatność dla złożonego zamówienia na podstawie zdarzenia.

---

## Aktor

- System (Payment)

---

## Cel

Przetworzenie płatności i aktualizacja statusu zamówienia.

---

## Scenariusz główny

1. System odbiera zdarzenie `OrderPlaced`
2. System przetwarza płatność
3. System publikuje zdarzenie `PaymentCompleted`
4. System aktualizuje status zamówienia

---

## Uwagi

- Brak bezpośredniego wywołania z Ordering (asynchronicznie)
- Payment działa niezależnie od Ordering
- Możliwe opóźnienia (eventual consistency)
