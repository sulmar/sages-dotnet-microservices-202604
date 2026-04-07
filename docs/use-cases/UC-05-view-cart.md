# UC-05 – Wyświetlenie koszyka

## Opis

Użytkownik przegląda zawartość swojego koszyka.

---

## Aktor

- Klient (użytkownik końcowy)

---

## Cel

Wyświetlenie produktów dodanych do koszyka.

---

## Scenariusz główny

1. Użytkownik przechodzi do koszyka
2. Aplikacja wysyła żądanie `GET /api/cart`
3. System pobiera dane z Redis
4. System zwraca listę produktów
5. Produkty są wyświetlane użytkownikowi

---

## Uwagi

- Koszyk zawiera snapshot danych produktu
- Brak komunikacji z ProductCatalog
