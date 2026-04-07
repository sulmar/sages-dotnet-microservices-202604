# UC-06 – Usunięcie produktu z koszyka

## Opis

Użytkownik usuwa wybrany produkt z koszyka. System aktualizuje zawartość koszyka.

---

## Aktor

- Klient (użytkownik końcowy)

---

## Cel

Usunięcie produktu z koszyka użytkownika.

---

## Scenariusz główny

1. Użytkownik przechodzi do koszyka
2. Użytkownik wybiera produkt do usunięcia
3. Aplikacja wysyła żądanie `DELETE /api/cart/items/{productId}`
4. System usuwa produkt z koszyka
5. System zwraca odpowiedź
6. Aplikacja aktualizuje widok koszyka

---

## Uwagi

- Brak komunikacji z innymi usługami
