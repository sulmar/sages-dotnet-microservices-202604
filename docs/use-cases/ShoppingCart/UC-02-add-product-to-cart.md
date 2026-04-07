# UC-02 – Dodanie produktu do koszyka

## Opis

Użytkownik dodaje wybrany produkt do koszyka. System zapisuje produkt wraz z ceną w momencie dodania.

---

## Aktor

- Klient (użytkownik końcowy)

---

## Cel

Dodanie produktu do koszyka użytkownika.

---

## Scenariusz główny

1. Użytkownik wybiera produkt
2. Użytkownik klika „Add to cart”
3. Aplikacja wysyła żądanie `POST /api/cart` z danymi produktu
4. System zapisuje produkt w koszyku:
   - Id
   - Name
   - Price
   - DiscountedPrice
5. System zwraca odpowiedź
6. Aplikacja aktualizuje licznik koszyka
