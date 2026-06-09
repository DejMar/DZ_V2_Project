# Dom Zdravlja — Evidencija lijekova

Jednostavna C# Blazor aplikacija za evidenciju lijekova u domu zdravlja. Podaci se čuvaju lokalno u JSON fajlovima — bez baze podataka, Dockera ili vanjskih servisa.

## Uloge

| Uloga | Opis |
|-------|------|
| **Administrator** | Unosi i uređuje lijekove, pregleda izvještaje o zalihama i zahtjevima |
| **Moderator** | Odobrava, odbija i izdaje lijekove ambulantama na osnovu zahtjeva |
| **Korisnik** | Šalje zahtjeve za lijekove za svoju ambulantu |

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

## Struktura projekta

```
DomZdravlja/
├── Models/          # Entiteti (Lijek, Zahtjev, Korisnik...)
├── Services/        # Poslovna logika i JSON repozitorij
├── Components/      # Blazor UI komponente
└── Data/            # JSON fajlovi (automatski kreirani pri pokretanju)
```

## Tok rada

1. **Korisnik** pošalje zahtjev za lijek
2. **Moderator** odobri ili odbije zahtjev
3. **Moderator** izda lijek ambulanti (smanjuje se zaliha)
4. **Administrator** prati stanje zaliha i izvještaje

## Tehnologije

- .NET 8
- Blazor Server (interaktivni UI)
- JSON file storage
