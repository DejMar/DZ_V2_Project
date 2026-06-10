# Dom Zdravlja — Evidencija lijekova

Jednostavna C# Blazor aplikacija za evidenciju lijekova u domu zdravlja. Podaci se čuvaju lokalno u JSON fajlovima — bez baze podataka, Dockera ili vanjskih servisa.

## Uloge

| Uloga | Opis |
|-------|------|
| **Administrator** | Upravlja lijekovima i korisnicima, pregleda izvještaje i izvozi podatke |
| **Moderator** | Odobrava, odbija i izdaje lijekove ambulantama (ne može izdati istekle lijekove) |
| **Korisnik** | Šalje zahtjeve za lijekove za svoju ambulantu i prati njihov status |

## Funkcionalnosti

### Osnovne
- Prijava po ulogama (session autentifikacija)
- CRUD nad lijekovima
- Zahtjevi za lijekove (korisnik → moderator → izdavanje)
- Izvještaji o zalihama i zahtjevima po ambulantama
- Upozorenje za nisku zalihu

### Nove funkcionalnosti
- **Upravljanje korisnicima** (`/admin/korisnici`) — dodavanje, uređivanje, aktivacija/deaktivacija naloga
- **Rok trajanja lijekova** — praćenje isteka, upozorenje „Ističe uskoro" (30 dana), blokada izdavanja isteklih lijekova
- **Izvoz izvještaja** — preuzimanje CSV fajla i HTML izvještaja za PDF štampu
- **Dokumentacija u aplikaciji** — linkovi na stranici za prijavu otvaraju HTML uputstva

## Preuzimanje sa GitHuba

### Preduvjeti

Instaliraj **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** (ne samo runtime).

Provjeri instalaciju:

```bash
dotnet --version
```

Treba da prikaže verziju `8.0.x`. Aplikacija radi na Windows, macOS i Linux — nije potrebna baza podataka ni Docker.

### Koraci

1. **Kloniraj repozitorij** (ili preuzmi ZIP sa GitHuba i raspakuj):

```bash
git clone https://github.com/KORISNIK/DZ_V2_Project.git
cd DZ_V2_Project
```

2. **Pokreni aplikaciju:**

```bash
cd DomZdravlja
dotnet run
```

3. **Otvori u browseru:**

```
http://localhost:5141/prijava
```

4. **Prijavi se** jednim od demo naloga (vidi tabelu ispod).

Pri prvom pokretanju automatski se kreira folder `DomZdravlja/Data/` sa početnim podacima (korisnici, lijekovi, ambulante).

### Visual Studio (Windows)

1. Otvori `DomZdravlja.sln`
2. Pritisni **F5** ili klikni **Run**
3. Browser se otvara automatski

### Česti problemi

| Problem | Rješenje |
|---------|----------|
| `dotnet: command not found` | Instaliraj .NET 8 SDK i restartuj terminal |
| Port 5141 je zauzet | Pokreni sa: `dotnet run --urls "http://localhost:5200"` i otvori `http://localhost:5200/prijava` |
| Stranica se ne učitava | Sačekaj nekoliko sekundi nakon `dotnet run`, provjeri tačan port u terminalu |
| Korisnik ne može da se prijavi | Provjeri da nije deaktiviran; za nove demo korisnike obriši `Data/users.json` i restartuj app |

## Pokretanje (lokalni razvoj)

```bash
cd DomZdravlja
dotnet run
```

Aplikacija je dostupna na **http://localhost:5141** — stranica za prijavu: **http://localhost:5141/prijava**

## Demo nalozi

| Korisničko ime | Lozinka | Uloga |
|----------------|---------|-------|
| admin | admin123 | Administrator |
| moderator | mod123 | Moderator |
| korisnik1 | user123 | Korisnik (Opća ambulanta) |
| korisnik2 | user123 | Korisnik (Pedijatrijska ambulanta) |
| korisnik3 | user123 | Korisnik (Stomatološka ambulanta) |

## Stranice aplikacije

| URL | Uloga | Opis |
|-----|-------|------|
| `/prijava` | Svi | Prijava + linkovi na dokumentaciju |
| `/admin/lijekovi` | Administrator | Upravljanje lijekovima i rokom trajanja |
| `/admin/korisnici` | Administrator | Upravljanje korisnicima |
| `/admin/izvjestaji` | Administrator | Izvještaji i izvoz CSV/PDF |
| `/moderator/zahtjevi` | Moderator | Obrada zahtjeva |
| `/korisnik/novi-zahtjev` | Korisnik | Slanje zahtjeva |
| `/korisnik/moji-zahtjevi` | Korisnik | Pregled vlastitih zahtjeva |

## Dokumentacija

| Fajl | Opis |
|------|------|
| [DOKUMENTACIJA.html](DOKUMENTACIJA.html) | Uputstvo, test scenariji, tok rada |
| [STRUKTURA_KODA.html](STRUKTURA_KODA.html) | Objašnjenje strukture koda i fajlova |

Dokumentacija je dostupna i iz aplikacije (dugmad na stranici za prijavu) ili direktno na:
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
    ├── Program.cs              # Ulazna tačka, registracija servisa
    ├── AuthEndpoints.cs        # Login/logout HTTP endpointi
    ├── ExportEndpoints.cs      # CSV i HTML izvoz izvještaja
    ├── Models/                 # Entiteti (Lijek, Zahtjev, Korisnik...)
    ├── Services/               # Poslovna logika i JSON repozitorij
    ├── Components/             # Blazor UI komponente
    │   └── Pages/
    │       ├── Admin/          # Lijekovi, Korisnici, Izvještaji
    │       ├── Moderator/      # Zahtjevi
    │       └── User/           # Novi zahtjev, Moji zahtjevi
    ├── wwwroot/                # CSS, HTML dokumentacija
    └── Data/                   # JSON fajlovi (runtime)
        ├── users.json
        ├── medicines.json
        ├── ambulances.json
        └── requests.json
```

## Tok rada

1. **Korisnik** pošalje zahtjev za lijek
2. **Moderator** odobri ili odbije zahtjev
3. **Moderator** izda lijek ambulanti (smanjuje se zaliha; istekli lijekovi se ne mogu izdati)
4. **Administrator** prati stanje zaliha, upravlja korisnicima i izvozi izvještaje

## Tehnologije

- .NET 8
- Blazor Server (interaktivni UI)
- ASP.NET Session autentifikacija
- JSON file storage
