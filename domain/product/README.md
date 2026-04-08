# Model domenowy

Ten dokument warto uzupełnić po opisaniu pierwszych przypadków użycia. Ma pomagać uchwycić obiekty domenowe i reguły biznesowe, a nie być tylko listą klas.

## Opis domeny

Kontekst `ProductCatalog` odpowiada za udostepnianie katalogu produktow do przegladania i filtrowania. System przechowuje informacje o produkcie (nazwa, cena, kategoria) oraz opcjonalnej cenie promocyjnej.

---

## Elementy modelu

### ProductCatalog

Kontekst domenowy odpowiedzialny za operacje odczytu na katalogu produktow (lista produktow, filtrowanie po kategorii, pobranie produktu).

Glowne atrybuty:

- `Products` (kolekcja `Product`)

Najwazniejsze reguly biznesowe:

- katalog zwraca liste wszystkich dostepnych produktow,
- katalog pozwala filtrowac produkty po nazwie kategorii,
- katalog udostepnia pobranie pojedynczego produktu po identyfikatorze.

---

### Product

Encja domenowa reprezentujaca produkt oferowany w katalogu.

Glowne atrybuty:

- `Name`
- `Price`
- `DiscountedPrice`
- `Category`

Najwazniejsze reguly biznesowe:

- produkt ma cene bazowa (`Price`),
- cena promocyjna (`DiscountedPrice`) jest opcjonalna i moze byc wykorzystana przy promocjach,
- produkt nalezy do jednej kategorii (`Category`).

---

### Category

Element domenowy opisujacy kategorie produktu.

Glowne atrybuty:

- `Name`

---

## Relacje

Opisz zaleznosci pomiedzy encjami, value objects i enumami.

- `ProductCatalog` zarzadza kolekcja `Product`.
- `Product` posiada jedna `Category`.

## Uwagi

Model jest wstepny i skupia sie na obiektach widocznych w aktualnym kodzie domenowym `ProductCatalog`. Brakuje jeszcze jawnego identyfikatora produktu w encji `Product` oraz bardziej szczegolowych inwariantow (np. walidacji cen).
