# UC-08 – Obliczenie ceny dla użytkownika

## Opis

System oblicza cenę produktu dla konkretnego użytkownika, uwzględniając dostępne promocje.

---

## Aktor

- Klient (użytkownik końcowy)

---

## Cel

Wyznaczenie końcowej ceny produktu dla użytkownika.

---

## Scenariusz główny

1. Użytkownik przegląda produkty lub koszyk
2. System pobiera cenę bazową produktu
3. System sprawdza dostępne promocje
4. System oblicza cenę końcową
5. System zwraca cenę do aplikacji

---

## Uwagi

- Cena bazowa pochodzi z katalogu produktów
- Promocje mogą zależeć od:
  - użytkownika
  - czasu
  - reguł biznesowych
- Wynik może różnić się od `DiscountedPrice` w produkcie
