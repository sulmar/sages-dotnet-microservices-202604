# UC-07 – Aktualizacja ilości produktu w koszyku

## Opis

Użytkownik zmienia ilość wybranego produktu w koszyku. System aktualizuje ilość produktu.

---

## Aktor

- Klient (użytkownik końcowy)

---

## Cel

Zmiana ilości produktu w koszyku użytkownika.

---

## Scenariusz główny

1. Użytkownik przechodzi do koszyka
2. Użytkownik zmienia ilość produktu
3. Aplikacja wysyła żądanie `PUT /api/cart/items/{productId}`
4. System aktualizuje ilość produktu w koszyku
5. System zwraca odpowiedź
6. Aplikacja aktualizuje widok koszyka

---

## Uwagi

- Brak komunikacji z innymi usługami
