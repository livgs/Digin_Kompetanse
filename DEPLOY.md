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

## 1. Opprett Google Cloud-prosjekt

1. Gå til: https://console.cloud.google.com/
2. Velg **New Project**
3. Gi det navnet: `digin-kompetanse-prod` (eller noe lignende)

# Cloud SQL (PostgreSQL) – Databaseoppsett

## 2. Opprett Cloud SQL-instans

1. Gå til:
    
    **SQL → Create Instance → PostgreSQL**
    
2. Velg:
    - PostgreSQL 15–17 (17 anbefalt)
    - Edition Preset: Sandbox (eller en som er ønskelig)
    - Instance ID: digin-kompetanse-db (eller noe lignende)
    - Region: `europe-north2` (Stockholm)
3. Sett passord for brukeren `postgres`
4. Vent til instansen er ferdig

## 3. Opprett database

1. Gå inn i instansen
2. Velg **Databases**
3. Klikk **Create database**
4. Navngi databasen (digin_kompetanse eller lignende)

## 4. Importer SQL-filer

<img width="1104" height="1356" alt="image" src="https://github.com/user-attachments/assets/c1f32ea5-9a75-459c-83f7-07ee7338ea02" />


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
<img width="555" height="473" alt="Skjermbilde 2025-11-30 kl  19 50 51" src="https://github.com/user-attachments/assets/f2351573-209b-4ccd-b176-13604ea18a3f" />

<img width="575" height="737" alt="Skjermbilde 2025-11-30 kl  19 51 27" src="https://github.com/user-attachments/assets/785be5ae-bf58-40e2-82db-d59b2797d300" />
  
  - Trykk "Create" og deretter trykk på knappen "Select"

6. Kjør begge SQL-filene i denne rekkefølgen:
<img width="533" height="419" alt="Skjermbilde 2025-11-30 kl  19 55 11" src="https://github.com/user-attachments/assets/83593ec0-735d-42a8-b652-7e21caa76625" />

✔ Først `01_schema.sql`


✔ Deretter `02_init_data.sql`


## 5. Legg inn admin og bedrift manuelt i Cloud SQL Studio:

<img width="2938" height="1578" alt="image" src="https://github.com/user-attachments/assets/507741b8-f369-4e7c-8bad-4545e26202dc" />  <br><br>		
		

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

# Deploy til Google Cloud Run

## 6. Opprett Cloud Run service

1. Gå til:
    
    **Cloud Run → Create Service**
    
2. Velg:
    
    **Deploy from an existing container image**
    
3. I feltet “Container image URL”, skriv:

```sql
docker.io/camillaur/digin_kompetanse:latest
```

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

## 8. Legg inn miljøvariabler i Cloud Run

Klikk på **"Edit & Deploy new revision":** <br>

<img width="2930" height="224" alt="image" src="https://github.com/user-attachments/assets/e21215d8-3ab0-4676-ad9e-0efe405c346e" /> <br>

Klikk deretter på knappen **"Variables & Secrets":**


<img width="1610" height="790" alt="image" src="https://github.com/user-attachments/assets/5f9e1119-b424-4cba-b54d-17942265df1b" />


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

## 9. Deploy

Når du klikker **Deploy**, får du en URL:

```sql
https://digin-kompetanse-xxxxxxx-uc.a.run.app
```

# Oppsummering

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
