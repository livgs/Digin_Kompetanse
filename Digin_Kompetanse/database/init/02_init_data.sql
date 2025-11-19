--
-- PostgreSQL database dump
--

\restrict xw1BNx2WAT7y3IRaJWNWwYb5bdj0mIAA6xtDqxKLepgtAw8dyNKWZ3v2YCbA1dw

-- Dumped from database version 17.6 (Debian 17.6-2.pgdg13+1)
-- Dumped by pg_dump version 17.6 (Debian 17.6-2.pgdg13+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Data for Name: fagomrade; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.fagomrade (fagomrade_id, fagomrade_navn, "BedriftId") VALUES (1, 'Programvareutvikling', NULL);
INSERT INTO public.fagomrade (fagomrade_id, fagomrade_navn, "BedriftId") VALUES (2, 'Infrastruktur og Cloud', NULL);
INSERT INTO public.fagomrade (fagomrade_id, fagomrade_navn, "BedriftId") VALUES (3, 'IT-sikkerhet', NULL);
INSERT INTO public.fagomrade (fagomrade_id, fagomrade_navn, "BedriftId") VALUES (4, 'Databaser', NULL);
INSERT INTO public.fagomrade (fagomrade_id, fagomrade_navn, "BedriftId") VALUES (5, 'Dataanalyse', NULL);
INSERT INTO public.fagomrade (fagomrade_id, fagomrade_navn, "BedriftId") VALUES (6, 'Systemarkitektur', NULL);
INSERT INTO public.fagomrade (fagomrade_id, fagomrade_navn, "BedriftId") VALUES (7, 'Design', NULL);
INSERT INTO public.fagomrade (fagomrade_id, fagomrade_navn, "BedriftId") VALUES (8, 'Forretningssystemer', NULL);
INSERT INTO public.fagomrade (fagomrade_id, fagomrade_navn, "BedriftId") VALUES (9, 'Prosjektledelse', NULL);
INSERT INTO public.fagomrade (fagomrade_id, fagomrade_navn, "BedriftId") VALUES (10, 'Drift og support', NULL);
INSERT INTO public.fagomrade (fagomrade_id, fagomrade_navn, "BedriftId") VALUES (11, 'Kunstig intelligens', NULL);


--
-- Data for Name: kompetanse; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (3, 'Fullstack');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (4, 'Mobilutvikling');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (5, 'Spillutvikling');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (7, 'Cloud-native verktøy');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (8, 'CI/CD');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (9, 'DevOps');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (12, 'Applikasjonssikkerhet');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (13, 'Kryptografi');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (15, 'NoSQL');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (17, 'Datamodellering');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (20, 'Big Data');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (22, 'Enterprisearkitektur');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (23, 'Integrasjonsarkitektur');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (24, 'Mikrotjenester');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (25, 'Service-Oriented Architecture');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (26, 'Designmønstre');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (27, 'ERP');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (28, 'CRM');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (29, 'Saks- og dokumenthåndtering');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (30, 'Business Process Management');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (33, 'Verktøy (prosjektledelse)');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (34, 'Teamledelse');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (35, 'IT-drift');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (39, 'Generativ AI');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (41, 'AI i praksis');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (42, 'Etikk og ansvarlig AI');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (1, 'Frontend');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (36, 'Nettverk og kommunikasjon');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (21, 'Tjenestearkitektur');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (6, 'Skytjenester');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (32, 'Tradisjonell prosjektledelse');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (38, 'Grunnleggende AI-konsepter');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (14, 'Relasjonelle databaser');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (10, 'Informasjonssikkerhet');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (37, 'IT-support');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (2, 'Backend');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (40, 'MLOps og AI-drift');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (16, 'Datavarehus');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (11, 'Operativ sikkerhet');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (31, 'Smidige metoder');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (19, 'Maskinlæring');
INSERT INTO public.kompetanse (kompetanse_id, kompetanse_kategori) VALUES (18, 'Dataanalyse');


--
-- Data for Name: fagomrade_has_kompetanse; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (1, 1);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (1, 2);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (1, 3);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (1, 4);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (1, 5);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (2, 6);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (2, 7);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (2, 8);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (2, 9);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (3, 10);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (3, 11);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (3, 12);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (3, 13);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (4, 14);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (4, 15);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (4, 16);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (4, 17);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (5, 18);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (5, 19);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (5, 20);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (6, 21);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (6, 22);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (6, 23);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (6, 24);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (6, 25);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (7, 26);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (8, 27);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (8, 28);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (8, 29);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (8, 30);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (9, 31);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (9, 32);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (9, 33);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (9, 34);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (10, 35);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (10, 36);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (10, 37);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (11, 38);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (11, 39);
INSERT INTO public.fagomrade_has_kompetanse (fagomrade_id, kompetanse_id) VALUES (11, 40);


--
-- Data for Name: under_kompetanse; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (1, 'HTML/CSS', 1);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (2, 'JavaScript', 1);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (3, 'TypeScript', 1);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (4, 'UI/UX design', 1);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (5, 'Java', 2);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (6, 'Kotlin', 2);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (7, 'C#', 2);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (8, 'Python', 2);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (9, 'Django', 2);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (10, 'Flask', 2);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (11, 'Node.js', 2);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (12, 'PHP', 2);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (13, 'Lavarel', 2);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (14, 'Ruby', 2);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (15, 'Ruby on Rails', 2);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (16, 'iOS', 4);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (17, 'Android', 4);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (18, 'Flutter', 4);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (19, 'React Native', 4);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (20, 'Unity', 5);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (21, 'Unreal Engine', 5);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (22, 'C++', 5);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (23, 'AWS', 6);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (24, 'Microsoft Azure', 6);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (25, 'Google Cloud Platform', 6);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (26, 'Kubernetes', 7);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (27, 'Docker', 7);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (28, 'Serverless', 7);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (29, 'Jenkins', 8);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (30, 'GitHub Actions', 8);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (31, 'GitLab CI', 8);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (32, 'Ansible', 9);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (33, 'Terraform', 9);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (34, 'Pulumi', 9);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (35, 'Monitoring', 9);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (36, 'ISO 27001', 10);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (37, 'NIST', 10);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (38, 'GRC', 10);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (39, 'Etisk hacking', 11);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (40, 'SIEM', 11);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (41, 'Brannmurer', 11);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (42, 'IPS', 11);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (43, 'Secure Coding', 12);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (44, 'OWASP Top 10', 12);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (45, 'TLS/SSL', 13);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (46, 'Hashing & PKI', 13);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (47, 'PostgresSQL', 14);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (48, 'MySQL', 14);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (49, 'MariaDB', 14);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (50, 'Microsoft SQL Server', 14);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (51, 'Oracle DB', 14);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (52, 'MongoDB', 15);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (53, 'Couchbase', 15);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (54, 'Cassandra', 15);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (55, 'DynamoDB', 15);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (56, 'Snowflake', 16);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (57, 'BigQuery', 16);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (58, 'Amazon Redshift', 16);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (59, 'ER-diagram', 17);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (60, 'Transaksjonsstyring', 17);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (61, 'Databaseindeksering', 17);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (62, 'Excel', 18);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (68, 'TensorFlow', 19);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (71, 'Apache Spark', 20);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (72, 'Hadoop', 20);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (73, 'Hive', 20);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (74, 'Kafka', 20);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (75, 'TOGAF', 22);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (76, 'ArchiMate', 22);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (77, 'REST', 23);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (78, 'SOAP', 23);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (79, 'GraphQL', 23);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (80, 'Message Queues', 23);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (81, 'SAP', 27);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (82, 'Microsoft Dynamics', 27);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (83, 'Oracle ERP', 27);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (84, 'Salesforce', 28);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (85, 'HubSpot', 28);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (86, 'Sharepoint', 29);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (87, 'ePhorte', 29);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (88, 'Public 360', 29);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (89, 'Prosessautomatisering', 30);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (90, 'RPA', 30);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (92, 'Kanban', 31);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (94, 'Prince2', 32);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (95, 'PMP', 32);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (96, 'Jira', 33);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (97, 'Confluence', 33);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (98, 'Trello', 33);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (99, 'Tverrfaglig teamledelse', 34);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (100, 'DevOps-samarbeid', 34);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (101, 'Windows/Linux serverdrift', 35);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (102, 'Backup & Disaster Recovery', 35);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (103, 'TCP/IP', 36);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (104, 'DNS', 36);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (105, 'VPN', 36);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (106, 'Cisco', 36);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (107, 'Fortinet', 36);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (108, 'Palo Alto', 36);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (109, 'ITIL-rammeverk', 37);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (110, 'ServiceNow', 37);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (111, 'Freshdesk', 37);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (112, 'Nevrale nettverk', 38);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (113, 'NLP', 38);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (114, 'Dyp læring', 38);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (115, 'Språkmodeller', 39);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (116, 'Generativ bilde-/videoteknologi', 39);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (117, 'Tekst-til-tale', 39);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (118, 'Talegjenkjenning', 39);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (119, 'Modelltrening og tuning', 40);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (120, 'Modell-deployment', 40);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (121, 'Chatbots', 41);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (122, 'Anbefalingssystemer', 41);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (123, 'Prediktiv Analyse', 41);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (124, 'Reguleringer og standarder', 42);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (91, 'Scrum', NULL);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (93, 'SAFe', NULL);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (67, 'Scikit-learn', NULL);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (69, 'PyTorch', NULL);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (70, 'Modelltrening', NULL);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (63, 'Power BI', NULL);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (64, 'Tableu', NULL);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (65, 'Pandas', NULL);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (66, 'NumPy', NULL);
INSERT INTO public.under_kompetanse (underkompetanse_id, underkompetanse_navn, kompetanse_id) VALUES (125, 'Singleton', 26);


--
-- Name: fagomrade_fagomrade_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.fagomrade_fagomrade_id_seq', 1, false);


--
-- Name: kompetanse_kompetanse_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.kompetanse_kompetanse_id_seq', 42, true);


--
-- Name: under_kompetanse_underkompetanse_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.under_kompetanse_underkompetanse_id_seq', 1, false);


--
-- PostgreSQL database dump complete
--

\unrestrict xw1BNx2WAT7y3IRaJWNWwYb5bdj0mIAA6xtDqxKLepgtAw8dyNKWZ3v2YCbA1dw

