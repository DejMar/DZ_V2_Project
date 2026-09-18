# Dom Zdravlja — Evidencija lijekova i flote

C# Blazor Server aplikacija za evidenciju lijekova i vozila u domu zdravlja. Podaci se čuvaju u **MySQL** bazi preko **Entity Framework Core**.

Student: **Dejan Marjanovic — FIT 16/25**  
Repozitorij: [github.com/DejMar/DZ_V2_Project](https://github.com/DejMar/DZ_V2_Project)

## Uloge

| Uloga | Opis |
|-------|------|
| **Administrator** | Upravlja lijekovima, korisnicima i vozilima; pregleda izvještaje i statistiku flote |
| **Moderator** | Odobrava, odbija i izdaje lijekove; evidentira prijem zaliha |
| **Korisnik** | Šalje zahtjeve za lijekove za svoju ambulantu i prati njihov status |
| **Vozač** | Vodi vožnje (početna/završna km), evidentira točenje goriva i vidi dodijeljena vozila |

## Funkcionalnosti

### Lijekovi
- Prijava po ulogama (session autentifikacija)
- CRUD nad lijekovima i korisnicima
- Zahtjevi za lijekove (korisnik → moderator → izdavanje)
- Prijem zaliha i historija prijema
- Rok trajanja lijekova (isteklo / ističe uskoro / niska zaliha)
- Dashboard po ulozi i izvoz izvještaja (CSV / HTML za PDF)

### Flota
- Admin kreira vozila (oznaka, marka, model, godina, gorivo, rezervoar, km) i dodjeljuje ih vozaču
- Vozač evidentira rutu: početna i završna kilometraža
- Vozač evidentira točenje goriva (litri, cijena, stanica, pun rezervoar)
- Statistika flote ukupno, po godinama i po mjesecima (`/admin/flota-statistika`)

## Preduvjeti

- **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)**
- **MySQL** na `localhost:3306`

Provjera:

```bash
dotnet --version
```

Treba da prikaže verziju `8.0.x`.

## Pokretanje

1. Pokreni MySQL.
2. Ako `root` ima lozinku, unesi je u `DomZdravlja/appsettings.json`:

```json
"DefaultConnection": "Server=localhost;Port=3306;Database=dom_zdravlja;User=root;Password=tvoja_lozinka;"
```

3. Pokreni aplikaciju iz foldera projekta:

```bash
cd DomZdravlja
dotnet restore
dotnet run
```

4. Otvori:

```
http://localhost:5141/prijava
```

Pri prvom pokretanju aplikacija primjenjuje EF Core migracije i ubacuje demo podatke ako su tabele prazne.

Zaustavljanje: `Ctrl + C` u terminalu.

### Visual Studio (Windows)

1. Otvori `DomZdravlja.sln`
2. Pritisni **F5**
3. Browser se otvara automatski

### Česti problemi

| Problem | Rješenje |
|---------|----------|
| `dotnet: command not found` | Instaliraj .NET 8 SDK i restartuj terminal |
| Greška konekcije na MySQL | Pokreni MySQL i provjeri connection string |
| Port 5141 je zauzet | `dotnet run --urls "http://localhost:5200"` |
| Korisnik se ne može prijaviti | Provjeri da nalog nije deaktiviran |

## Demo nalozi

| Korisničko ime | Lozinka | Uloga |
|----------------|---------|-------|
| admin | admin123 | Administrator |
| moderator | mod123 | Moderator |
| korisnik1 | user123 | Korisnik (Opća ambulanta) |
| korisnik2 | user123 | Korisnik (Pedijatrijska ambulanta) |
| korisnik3 | user123 | Korisnik (Stomatološka ambulanta) |
| vozac1 | vozac123 | Vozač (demo vozilo A12-B-345) |

## Stranice aplikacije

| URL | Uloga | Opis |
|-----|-------|------|
| `/prijava` | Svi | Prijava, dokumentacija i footnote autora |
| `/pregled` | Svi | Dashboard po ulozi |
| `/prijem-zaliha` | Admin, Moderator | Prijem zaliha |
| `/admin/lijekovi` | Administrator | Lijekovi |
| `/admin/korisnici` | Administrator | Korisnici i uloge (uključujući Vozač) |
| `/admin/vozila` | Administrator | Vozila i dodjela vozaču |
| `/admin/flota-statistika` | Administrator | Statistika flote (ukupno / godina / mjesec) |
| `/admin/izvjestaji` | Administrator | Izvještaji i izvoz CSV/PDF |
| `/moderator/zahtjevi` | Moderator | Obrada zahtjeva |
| `/korisnik/novi-zahtjev` | Korisnik | Novi zahtjev |
| `/korisnik/moji-zahtjevi` | Korisnik | Moji zahtjevi |
| `/vozac/vozila` | Vozač | Dodijeljena vozila |
| `/vozac/voznje` | Vozač | Početna i završna kilometraža |
| `/vozac/gorivo` | Vozač | Točenje goriva |

## Dokumentacija

| Fajl | Opis |
|------|------|
| [DOKUMENTACIJA.html](DOKUMENTACIJA.html) | Uputstvo, test scenariji, tok rada |
| [STRUKTURA_KODA.html](STRUKTURA_KODA.html) | Objašnjenje strukture koda |

U aplikaciji:
- `http://localhost:5141/DOKUMENTACIJA.html`
- `http://localhost:5141/STRUKTURA_KODA.html`

## Struktura projekta

```
DZ_V2_Project/
├── README.md
├── DOKUMENTACIJA.html
├── STRUKTURA_KODA.html
├── DomZdravlja.sln
└── DomZdravlja/
    ├── Program.cs
    ├── AuthEndpoints.cs
    ├── ExportEndpoints.cs
    ├── Models/
    ├── Services/
    ├── Components/Pages/
    │   ├── Admin/          # Lijekovi, Korisnici, Vozila, Statistika flote, Izvještaji
    │   ├── Moderator/      # Zahtjevi
    │   ├── User/           # Novi zahtjev, Moji zahtjevi
    │   └── Vozac/          # Moja vozila, Vožnje, Gorivo
    ├── Data/               # AppDbContext, DbSeeder, schema.sql
    ├── Migrations/         # EF Core migracije
    └── wwwroot/
```

## Tok rada

**Lijekovi:** prijem zaliha → zahtjev korisnika → odobrenje/odbijanje → izdavanje (zaliha se smanjuje).

**Flota:** admin kreira vozača i vozilo → dodijeli vozilo → vozač vodi vožnje i točenja → admin pregleda statistiku po periodima.

## Tehnologije

- .NET 8
- Blazor Server
- ASP.NET Session autentifikacija
- MySQL + Entity Framework Core (Pomelo)
