# Digin Kompetanse – Produksjonsoppsett (Docker Hub + Google Cloud)

Dette dokumentet viser **hvordan man setter opp Digin Kompetanse i produksjon**, basert på:

- Ferdig bygget Docker-image på Docker Hub
- PostgreSQL i **Google Cloud SQL**
- Import av SQL-skjema + init-data
- Webhost med **Google Cloud Run**
- Miljøvariabler (DB + SMTP)
- Admin- og bedriftinnlogging

**Ferdig bygget image på Docker Hub** 

```
docker.io/camillaur/digin_kompetanse:latest

```

**Alternativ: Hvis du trenger å bygge nytt image selv**

```docker
docker buildx build \
  --platform linux/amd64,linux/arm64 \
  -t camillaur/digin_kompetanse:latest \
  --push .
```

# Sett opp Google Cloud
<br>

## 1. Opprett Google Cloud-prosjekt

1. Gå til: https://console.cloud.google.com/
2. Velg **New Project**
3. Gi det navnet: `digin-kompetanse-prod` e.l
<br><br>

# Cloud SQL (PostgreSQL) – Databaseoppsett
<br>

## 2. Opprett Cloud SQL-instans

1. Gå til:
    
    **SQL → Create Instance → PostgreSQL**
    
2. Velg:
    - PostgreSQL 15–17 (17 anbefalt)
    - Edition Preset: Vi brukte Sandbox
    - Instance ID: digin-kompetanse-db e.l
    - Region: `europe-north2` (Stockholm)
3. Sett passord for brukeren `postgres`
4. Vent til instansen er ferdig
<br>

## 3. Opprett database

1. Gå inn i instansen
2. Velg **Databases**
3. Klikk **Create database**
4. Navngi databasen: digin_kompetanse e.l
<br>

## 4. Importer SQL-filer

<img width="1104" height="1356" alt="image" src="https://github.com/user-attachments/assets/c1f32ea5-9a75-459c-83f7-07ee7338ea02" />

<br><br>
Du skal importere begge:

```
01_schema.sql
02_init_data.sql
```

### Slik gjør du det:

1. Gå til Cloud SQL-instansen
2. Velg **Import**
3. Velg fil (må lastes opp til Google Cloud Storage først)
4. Trykk på **"Create a bucket"** (kan hete f.eks "digin-kompetanse" e.l)
   - Velg region: `europe-north2` (Stockholm)
   - På **Set a default class**, velg "Standard"
   - På **Prevent Public Access**, kryss av på "Enforce public access prevention on this bucket"
   - På **Access Control**, velg "Uniform"
   - Innstillingene skal se slik ut når du er ferdig:
     <br><br>
![Bilde2](https://github.com/user-attachments/assets/bb3195c3-a320-4008-9bbd-8e13c2c52752)

![Bilde4](https://github.com/user-attachments/assets/11758c3a-71ba-44cb-a034-f42d7e7fb214)

  
  - Trykk "Create" og deretter trykk på knappen "Select"
<br>

6. Kjør begge SQL-filene i denne rekkefølgen:

⚠️ **NB: Se instruksjonsvideo ved å trykke på bildet:**

[![Se video på YouTube](https://img.youtube.com/vi/q_rOq6QV5vs/maxresdefault.jpg)](https://youtu.be/q_rOq6QV5vs)


✔ Først `01_schema.sql`


✔ Deretter `02_init_data.sql`
<br><br>

## 5. Legg inn admin og bedrift manuelt i Cloud SQL Studio:

![Bilde5](https://github.com/user-attachments/assets/15bc46ab-987c-45cc-bf07-d2af1902f508)

<br>		
		
### Bedrift

```sql
INSERT INTO bedrift (bedrift_navn, bedrift_epost)
VALUES ('Bedriftnavn', 'bedriftepost@gmail.com');
```

### Admin

Passordet må være **bcrypt-hash**  

```sql
INSERT INTO admin (admin_epost, admin_passord_hash, navn)
VALUES ('admin@epost.no', '<bcrypt-hash>', 'Administrator');

```

- Trykk “Run” når du har lagt det inn
<br><br>
# Deploy til Google Cloud Run
<br>

## 6. Opprett Cloud Run service

1. Gå til:
    
    **Cloud Run → Create Service**
    
2. Velg:
    
    **Deploy from an existing container image**
    
3. I feltet “Container image URL”, skriv:

```sql
docker.io/camillaur/digin_kompetanse:latest
```
<br>

## 7. Koble Cloud Run til Cloud SQL

1. Under **Connections**
2. Velg:
    - `Add connection`
    - Velg Cloud SQL-instansen:
        
        `digin-kompetanse-db`
        
3. Cloud Run lager automatisk mount:

```sql
/cloudsql/<INSTANCE_CONNECTION_NAME>
```
<br>

## 8. Legg inn miljøvariabler i Cloud Run

Klikk på **"Edit & Deploy new revision":** 
<br><br>
![Bilde3](https://github.com/user-attachments/assets/07ba1d8c-8741-435c-8b58-0b0911515864)


Klikk deretter på knappen **"Variables & Secrets":** 
<br><br>
![Bilde1](https://github.com/user-attachments/assets/979a146c-bbb2-4332-b1b4-622c8fb2e567)


I Cloud Run → Environment Variables:

Database (OBLIGATORISK)

```sql
DB_HOST=/cloudsql/<INSTANCE_CONNECTION_NAME>
DB_PORT=5432
DB_USER=postgres
DB_PASSWORD=<det du satte da du opprettet databasen>
DB_NAME=digin_kompetanse
ASPNETCORE_ENVIRONMENT=Production
```

E-post / SMTP (OTP system)

```sql
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USER=<gmailkonto>
SMTP_PASS=<gmail app passord>
SMTP_FROM=<gmailkonto>
SMTP_ENABLE_STARTTLS=true
```

✔ Ingen `ASPNETCORE_URLS`

✔ Ingen connection string manuelt – Program.cs bygger det

<br>

## 9. Deploy

Når du klikker **Deploy**, får du en URL:

```sql
https://digin-kompetanse-xxxxxxx-uc.a.run.app
```
<br>

## Oppsummering

1. Lag et nytt Google Cloud-prosjekt
2. Opprett Cloud SQL (PostgreSQL)
3. Lag database: digin_kompetanse
4. Importer 01_schema.sql og 02_init_data.sql
5. Legg inn admin og bedrift manuelt i databasen
6. Lag en Cloud Run service med:
[docker.io/camillaur/digin_kompetanse:latest](http://docker.io/camillaur/digin_kompetanse:latest)
7. Koble Cloud SQL-instansen til Cloud Run
8. Legg inn DB_ og SMTP_ miljøvariabler
9. Deploy → åpne nettadressen
