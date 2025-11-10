# Digin Kompetanse
## Om prosjektet
Digin Kompetanse er en webapplikasjon utviklet for å la medlemsbedrifter i Digin registrere sin kompetanse innen ulike fagområder.  
Løsningen gjør det mulig for bedrifter å sende inn kompetanse, og for Digin-administratorer å administrere og filtrere innsendte data via et eget administrasjonspanel.


## Instruksjoner:

## Du må ha dette installert: 
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

#### *(Valgfritt – ikke i bruk i denne versjonen)*
```bash
# SENDGRID_API_KEY=
# SENDGRID_FROM_EMAIL=
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

Legg inn admin-bruker i databasen
```bash
docker compose exec db psql -U postgres -d digin_kompetanse
```

Deretter i PostgreSQL-prompten (bytt ut verdiene med dine egne): 
```sql
INSERT INTO admin (admin_epost, admin_passord_hash, navn)
VALUES (
  'AdminEpost',
  'HashetPassord',
  'Navn'
);
```

## Teknologistack

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
- E-post må være **whitelistet** i `bedrift`-tabellen før innlogging
- Logger inn med **e-post og engangskode** (sendes via SMTP – f.eks. Gmail eller annen SMTP-tjener)
- Kan legge til **flere fagområder, kompetanser og underkompetanser** i samme innsending
- Kan se og **slette tidligere registrerte kompetanser**
- Alle registreringer lagres i `bedrift_kompetanse` med tidsstempel og kobling til riktig bedrift

### 🧑‍💼 Administrator
- Logger inn med epost (brukernavn) og passord som blir hentet fra databasen
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

## 📧 E-post (SMTP / MailKit)

Denne versjonen av prosjektet bruker **MailKit** til å sende engangskoder via SMTP.  
SMTP-konfigurasjonen hentes fra miljøvariablene i `.env`:

```bash
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USER=din.epost@gmail.com
SMTP_PASS=ditt_app_passord
SMTP_FROM=din.epost@gmail.com
SMTP_ENABLE_STARTTLS=true
```

Applikasjonen validerer e-postadresser mot databasen og sender engangskoder via denne SMTP-tilkoblingen.

OTP-flyten håndteres av:

- OtpService (generering, lagring og validering av koder)

- InMemoryOtpRateLimiter (IOtpRateLimiter) (begrensning på antall forespørsler)

- OtpOptions (konfigurasjon for lengde, TTL og grense per e-post)

- AuthController (sender e-posten via MailKit)

> For Gmail-brukere må du opprette et **App Password** under Google-kontoens sikkerhetsinnstillinger.

---

### Mulig videreutvikling: SendGrid (ikke implementert)
Prosjektet er i dag bygget rundt SMTP via MailKit i AuthController for utsending av engangskoder (OTP).
Selve OTP-flyten håndteres av:

- OtpService (genererer, lagrer og verifiserer koder)

- InMemoryOtpRateLimiter (IOtpRateLimiter) (begrensning på antall forespørsler per e-post)

- OtpOptions (lengde, levetid og grenser)

Disse kan gjenbrukes uendret dersom man ønsker å bytte fra SMTP til SendGrid som transportlag for e-post.
**Sendgrid** er **ikke implementert** i denne versjonen, men kan legges til senere.

Fremgangsmåte:
1. Opprett en konto på [SendGrid](https://sendgrid.com/).
2. Opprett en API-nøkkel og legg den i `.env`:
   ```bash
   SENDGRID_API_KEY=din_sendgrid_nokkel
   SENDGRID_FROM_EMAIL=kontakt@dinbedrift.no
   ```
3. Lag en egen klasse, for eksempel SendGridOtpEmailSender, som:
   - bygger en e-post med OTP-koden
   - sender e-posten ved hjelp av SendGrid sitt .NET-SDK og SENDGRID_API_KEY

4. Bruk denne klassen i stedet for SMTP-delen i AuthController:
   - enten ved å kalle SendGridOtpEmailSender direkte i RequestOtp-metoden,
   - eller ved å injisere den som avhengighet og bruke den i OtpService.

---

## 👤 Utviklet av
Liv Gudrun Staaland, Camilla Uglem Remøy, Adrian Mallinckrodt Øien 



