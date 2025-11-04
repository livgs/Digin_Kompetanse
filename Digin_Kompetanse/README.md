# DIGIN KOMPETANSEPLATTFORM
## Om prosjektet
Digin Kompetanse er en webapplikasjon utviklet for å la medlemsbedrifter i Digin registrere sin kompetanse innen ulike fagområder.  
Løsningen gjør det mulig for bedrifter å sende inn sin kompetanseprofil, og for Digin-administratorer å administrere og filtrere innsendte data via et eget administrasjonspanel.

Formålet er å gi Digin en helhetlig oversikt over kompetansen i medlemsnettverket for å:
- Finne relevante samarbeidspartnere
- Synliggjøre felles kompetanseområder
- Identifisere kompetansegap i regionen

## Instruksjoner:

## Du må ha følgende installert: 
- Docker Desktop 
- Git 
- (Anbefalt) Rider eller Visual Studio Code

1. Klon prosjektet 
```bash
git clone https://github.com/livgs/Digin_Kompetanse.git
cd Digin_Kompetanse
```

2. Opprett .env fil 
- Lag en fil kalt .env i rotmappen med følgende innhold:

**Database**
```bash
DB_USER=privat
DB_PASSWORD=privat
DB_NAME=digin_kompetanse
DB_HOST=db
DB_PORT=5432
ConnectionStrings__DefaultConnection="Host=${DB_HOST};Port=${DB_PORT};Database=${DB_NAME};Username=${DB_USER};Password=${DB_PASSWORD}"
```

**SMTP (for engangskode-utsending)**
```bash
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USER=din.epost@gmail.com
SMTP_PASS=ditt_app_passord
SMTP_FROM=din.epost@gmail.com
```

3. Bygg og start containerne
```bash
docker compose up --build
docker compose up -d
```

4. Kjør migrasjoner (for å oppdatere databasen)
```bash
docker compose exec app dotnet ef database update
```

5. Åpne applikasjonen 
- Når alt kjører, åpne nettleseren på: http://localhost:5068

6. (Valgfritt) Legg til test-bedrift i databasen
Kjør følgende i terminalen:
```bash
docker compose exec db psql -U postgres -d digin_kompetanse
```
Deretter i PostgreSQL-prompten:
```sql
  INSERT INTO bedrift (bedrift_navn, bedrift_epost)
  VALUES ('Testbedrift', 'test@eksempel.no');
 ```

7. Logg inn som administrator 
- Logg inn med hardkodet epost og passord

## 🧱 Teknologistack

| Komponent | Teknologi |
|------------|------------|
| **Backend** | ASP.NET Core (.NET 9/10) |
| **Frontend** | Razor Pages (HTML5, CSS, Bootstrap 5, JavaScript) |
| **Database** | PostgreSQL 17 |
| **ORM** | Entity Framework Core |
| **Containerisering** | Docker + Docker Compose |
| **Autentisering** | Session-basert login med engangskode via SMTP (MailKit) |
| **Sikkerhet** | BCrypt for passordhashing, sanitiserte SQL-relasjoner |
| **CI/CD (valgfritt)** | GitHub Actions |

---

## ⚙️ Funksjonalitet

### 👥 Bedrift
- Logger inn med **e-post og engangskode** (sendes via SMTP – f.eks. Gmail eller annen SMTP-tjener)
- E-post må være **whitelistet** i `bedrift`-tabellen før innlogging
- Kan legge til **flere fagområder, kompetanser og underkompetanser** i samme innsending
- Kan se og **slette tidligere registrerte kompetanser**
- Alle registreringer lagres i `bedrift_kompetanse` med tidsstempel og kobling til riktig bedrift

### 🧑‍💼 Administrator
- Logger inn med (per nå hardkodet epost og passord)
- Kan se alle innsendte kompetanser fra bedrifter
- Kan **filtrere** på fagområde, kompetanse og underkompetanse
- Kan **slette bedrifter**
- Har tilgang til **CSV-eksport** av registrerte data
- Adminpanelet bruker Digin-design med moderne blå/hvit fargepalett og responsivt oppsett

---

## Database-struktur

### Viktige tabeller
| Tabell | Formål |
|---------|--------|
| **bedrift** | Inneholder whitelistede bedrifter og e-poster som kan logge inn |
| **fagomrade** | Overordnede fagområder (eks. IT-sikkerhet, Infrastruktur, Cloud) |
| **kompetanse** | Kompetansekategorier tilknyttet fagområder |
| **under_kompetanse** | Spesifikke underkompetanser under hver kompetanse |
| **bedrift_kompetanse** | Koblingstabell mellom bedrift og registrerte kompetanser |
| **login_token** | Midlertidige engangskoder for e-postinnlogging |
| **admin** | Administratorbrukere med passord (BCrypt) |

### Relasjonsnøkler
- `bedrift_kompetanse` kobles til:
    - `bedrift` → `bedrift_id`
    - `fagomrade` → `fagomrade_id`
    - `kompetanse` → `kompetanse_id`
    - `under_kompetanse` → `underkompetanse_id`

---

