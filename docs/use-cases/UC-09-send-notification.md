# UC-09 – Wysłanie powiadomienia

## Opis

System wysyła powiadomienie o zakończeniu płatności do zewnętrznego systemu.

---

## Aktor

- System (zewnętrzny odbiorca / webhook)

---

## Cel

Poinformowanie zewnętrznego systemu o statusie płatności.

---

## Scenariusz główny

1. System odbiera zdarzenie `PaymentCompleted`
2. System przygotowuje dane powiadomienia
3. System wysyła żądanie HTTP (webhook)
4. System odbiera odpowiedź
5. System zapisuje wynik wysłania

---

## Uwagi

- Komunikacja oparta o HTTP (webhook)
- Wywołanie do zewnętrznego systemu
- Możliwe retry w przypadku błędów
- System działa asynchronicznie (po zdarzeniu)
