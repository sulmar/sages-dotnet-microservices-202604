# UC-03 – Złożenie zamówienia

## Opis

Użytkownik składa zamówienie na podstawie zawartości koszyka. System weryfikuje dane i tworzy zamówienie.

---

## Aktor

- Klient (użytkownik końcowy)

---

## Cel

Utworzenie zamówienia na podstawie koszyka użytkownika.

---

## Scenariusz główny

1. Użytkownik przechodzi do checkout
2. Aplikacja wysyła żądanie `POST /api/orders`
3. System pobiera zawartość koszyka
4. System pobiera dane produktów
5. System sprawdza dostępność (Stock)
6. System tworzy zamówienie
7. System publikuje zdarzenie `OrderPlaced`
8. System zwraca odpowiedź

---

## Uwagi

- Zamówienie zapisuje snapshot danych (cena, nazwa)
