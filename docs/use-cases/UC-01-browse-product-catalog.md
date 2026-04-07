# UC-01 – Przeglądanie katalogu produktów

## Opis

Użytkownik przegląda listę dostępnych produktów w sklepie. System prezentuje podstawowe informacje o produkcie, takie jak nazwa i cena.

---

## Aktor

- Klient (użytkownik końcowy)

---

## Cel

Wyświetlenie listy produktów dostępnych w ofercie.

---

## Scenariusz główny

1. Użytkownik otwiera stronę główną (`/`)
2. System pobiera listę produktów z API: `GET /api/products`
3. System wyświetla produkty w formie listy (grid)
4. Każdy produkt zawiera:
   - nazwę
   - cenę
   - opcjonalnie cenę promocyjną

---

## Uwagi

- Aktualnie dane są mockowane w kodzie

