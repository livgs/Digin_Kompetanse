// DIGIN KOMPETANSEPLATTFORM

## // Om prosjektet
Digin Kompetanse er en webapplikasjon utviklet for å la medlemsbedrifter i Digin registrere sin kompetanse innen ulike fagområder.  
Løsningen gjør det mulig for bedrifter å sende inn sin kompetanseprofil, og for Digin-administratorer å administrere og filtrere innsendte data via et eget administrasjonspanel.

Formålet er å gi Digin en helhetlig oversikt over kompetansen i medlemsnettverket for å:
- Finne relevante samarbeidspartnere
- Synliggjøre felles kompetanseområder
- Identifisere kompetansegap i regionen

---

## // Teknologistack
| Komponent | Teknologi |
|------------|------------|
| Backend | ASP.NET Core (.NET 9/10) |
| Frontend | Razor Pages (Bootstrap 5, HTML5, CSS) |
| Database | PostgreSQL 17 |
| ORM | Entity Framework Core |
| Containerisering | Docker & Docker Compose |
| Autentisering | Sessions + BCrypt |
| CI/CD (valgfritt) | GitHub Actions |

---

## // Funksjonalitet

### Bedriftsbruker
- Kan logge inn med e-post og engangskode
- Kan registrere kompetanse innen fagområder, kompetansekategorier og underkompetanser
- Data lagres i PostgreSQL via relasjonsbaserte tabeller:
    - bedrift
    - fagomrade
    - kompetanse
    - under_kompetanse
    - bedrift_kompetanse

### Administrator
- Logger inn via et eget adminpanel (`/Admin/AdminLogin`)
- Kan se alle innsendte kompetanser
- Kan filtrere på fagområde og kompetanse
- AdminDashboard har moderne Digin-design med blå/hvit fargepalett

---

## // Database
Databasen inneholder relasjoner mellom følgende tabeller:
- bedrift
- fagomrade
- kompetanse
- under_kompetanse
- bedrift_kompetanse
- admin
- login_token

