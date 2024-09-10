using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlaninarskeAvantureBackend.Migrations
{
    /// <inheritdoc />
    public partial class prva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Drzava",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drzava", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Korisnici",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prezime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatumRodjenja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Drzava = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lozinka = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SlikaKorisnika = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    jelPotvrdjen = table.Column<bool>(type: "bit", nullable: false),
                    TWOFA = table.Column<bool>(type: "bit", nullable: false),
                    isAktivan = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Korisnici", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VerifikacijskiKodovi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kod = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerifikacijskiKodovi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AutentifikacijaToken",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vrijednost = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KorisnikID = table.Column<int>(type: "int", nullable: false),
                    vrijemeEvidentiranja = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutentifikacijaToken", x => x.id);
                    table.ForeignKey(
                        name: "FK_AutentifikacijaToken_Korisnici_KorisnikID",
                        column: x => x.KorisnikID,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Camping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kontakt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CijenaPoNoci = table.Column<int>(type: "int", nullable: false),
                    X = table.Column<float>(type: "real", nullable: false),
                    Y = table.Column<float>(type: "real", nullable: false),
                    KorisnikId = table.Column<int>(type: "int", nullable: false),
                    CampSlika = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lokacija = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Camping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Camping_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Dogadjaj",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VrijemePocetka = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VrijemeZavrsetka = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Lokacija = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    xKordinata = table.Column<float>(type: "real", nullable: false),
                    yKordinata = table.Column<float>(type: "real", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrojPlaninara = table.Column<int>(type: "int", nullable: false),
                    KorisnikId = table.Column<int>(type: "int", nullable: false),
                    VrijemeKreiranja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SlikaDogadjaja = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dogadjaj", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dogadjaj_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "KorisnikFA",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikId = table.Column<int>(type: "int", nullable: false),
                    Kod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatumIsteka = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isAktivan = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KorisnikFA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KorisnikFA_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Oprema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Proizvodjac = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kategorija = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Recenzija = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KorisnikId = table.Column<int>(type: "int", nullable: false),
                    SlikaOpreme = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oprema", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Oprema_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Planina",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NajvisiVrh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Visina = table.Column<int>(type: "int", nullable: false),
                    OpisPlanine = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KorisnikId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Planina", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Planina_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "PlaninarskoDrustvo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adresa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kontakt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KorisnikId = table.Column<int>(type: "int", nullable: false),
                    DrustvoSlika = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaninarskoDrustvo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaninarskoDrustvo_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tekst = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KorisnikId = table.Column<int>(type: "int", nullable: false),
                    Tezina = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Vrijeme = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrojLajkova = table.Column<int>(type: "int", nullable: false),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Pracenja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikUserId = table.Column<int>(type: "int", nullable: false),
                    KorsnikZapracenId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pracenja", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pracenja_Korisnici_KorisnikUserId",
                        column: x => x.KorisnikUserId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Pracenja_Korisnici_KorsnikZapracenId",
                        column: x => x.KorsnikZapracenId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Putopis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KorisnikId = table.Column<int>(type: "int", nullable: false),
                    DatumObjavljivanja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Putopis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Putopis_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Vodic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prezime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Iskustvo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Specijalizacija = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lokacija = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kontakt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KorisnikId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vodic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vodic_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "DogadjajKorisnici",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikId = table.Column<int>(type: "int", nullable: false),
                    DogadjajID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DogadjajKorisnici", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DogadjajKorisnici_Dogadjaj_DogadjajID",
                        column: x => x.DogadjajID,
                        principalTable: "Dogadjaj",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_DogadjajKorisnici_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "SigurnosniIzvjestaj",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatumIzvjestaja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    x = table.Column<float>(type: "real", nullable: false),
                    y = table.Column<float>(type: "real", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ostecenja = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Povreda = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KorisnikId = table.Column<int>(type: "int", nullable: false),
                    DogadjajID = table.Column<int>(type: "int", nullable: false),
                    SlikaIzvjestaj = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SigurnosniIzvjestaj", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SigurnosniIzvjestaj_Dogadjaj_DogadjajID",
                        column: x => x.DogadjajID,
                        principalTable: "Dogadjaj",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_SigurnosniIzvjestaj_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Sponzor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kontakt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DogadjajId = table.Column<int>(type: "int", nullable: false),
                    KorisnikId = table.Column<int>(type: "int", nullable: false),
                    SponzorSlika = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sponzor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sponzor_Dogadjaj_DogadjajId",
                        column: x => x.DogadjajId,
                        principalTable: "Dogadjaj",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Sponzor_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",  
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Staza",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tezina = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duzina = table.Column<int>(type: "int", nullable: false),
                    NadmorskaVisina = table.Column<int>(type: "int", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lokacija = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pocetakX = table.Column<float>(type: "real", nullable: false),
                    pocetakY = table.Column<float>(type: "real", nullable: false),
                    KorisnikId = table.Column<int>(type: "int", nullable: false),
                    VrijemeKreiranja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ocjena = table.Column<double>(type: "float", nullable: false),
                    PlaninaId = table.Column<int>(type: "int", nullable: false),
                    krajX = table.Column<float>(type: "real", nullable: false),
                    krajY = table.Column<float>(type: "real", nullable: false),
                    SlikaStaze = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staza", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Staza_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Staza_Planina_PlaninaId",
                        column: x => x.PlaninaId,
                        principalTable: "Planina",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "DrustvoKorisnici",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikId = table.Column<int>(type: "int", nullable: false),
                    PlaninarskoDrustvoID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrustvoKorisnici", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DrustvoKorisnici_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_DrustvoKorisnici_PlaninarskoDrustvo_PlaninarskoDrustvoID",
                        column: x => x.PlaninarskoDrustvoID,
                        principalTable: "PlaninarskoDrustvo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Komentar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikId = table.Column<int>(type: "int", nullable: false),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    TekstKomentara = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LikeCounter = table.Column<int>(type: "int", nullable: false),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Komentar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Komentar_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Komentar_Post_PostId",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "PostLajkovi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikId = table.Column<int>(type: "int", nullable: false),
                    PostId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostLajkovi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostLajkovi_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_PostLajkovi_Post_PostId",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "OcjenaStaze",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Komentar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ocjena = table.Column<int>(type: "int", nullable: false),
                    Kvalitet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StazaId = table.Column<int>(type: "int", nullable: false),
                    KorisnikId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OcjenaStaze", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OcjenaStaze_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_OcjenaStaze_Staza_StazaId",
                        column: x => x.StazaId,
                        principalTable: "Staza",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "PutopisStaze",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PutopisId = table.Column<int>(type: "int", nullable: false),
                    StazaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PutopisStaze", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PutopisStaze_Putopis_PutopisId",
                        column: x => x.PutopisId,
                        principalTable: "Putopis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_PutopisStaze_Staza_StazaId",
                        column: x => x.StazaId,
                        principalTable: "Staza",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "SigurnosniRizik",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tipRizika = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    opisRizika = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    lokacijaRizika = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StazaId = table.Column<int>(type: "int", nullable: false),
                    RizikSlika = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SigurnosniRizik", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SigurnosniRizik_Staza_StazaId",
                        column: x => x.StazaId,
                        principalTable: "Staza",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "KomentarLajkovi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikId = table.Column<int>(type: "int", nullable: false),
                    KomentarID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KomentarLajkovi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KomentarLajkovi_Komentar_KomentarID",
                        column: x => x.KomentarID,
                        principalTable: "Komentar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_KomentarLajkovi_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AutentifikacijaToken_KorisnikID",
                table: "AutentifikacijaToken",
                column: "KorisnikID");

            migrationBuilder.CreateIndex(
                name: "IX_Camping_KorisnikId",
                table: "Camping",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Dogadjaj_KorisnikId",
                table: "Dogadjaj",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_DogadjajKorisnici_DogadjajID",
                table: "DogadjajKorisnici",
                column: "DogadjajID");

            migrationBuilder.CreateIndex(
                name: "IX_DogadjajKorisnici_KorisnikId",
                table: "DogadjajKorisnici",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_DrustvoKorisnici_KorisnikId",
                table: "DrustvoKorisnici",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_DrustvoKorisnici_PlaninarskoDrustvoID",
                table: "DrustvoKorisnici",
                column: "PlaninarskoDrustvoID");

            migrationBuilder.CreateIndex(
                name: "IX_Komentar_KorisnikId",
                table: "Komentar",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Komentar_PostId",
                table: "Komentar",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_KomentarLajkovi_KomentarID",
                table: "KomentarLajkovi",
                column: "KomentarID");

            migrationBuilder.CreateIndex(
                name: "IX_KomentarLajkovi_KorisnikId",
                table: "KomentarLajkovi",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_KorisnikFA_KorisnikId",
                table: "KorisnikFA",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_OcjenaStaze_KorisnikId",
                table: "OcjenaStaze",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_OcjenaStaze_StazaId",
                table: "OcjenaStaze",
                column: "StazaId");

            migrationBuilder.CreateIndex(
                name: "IX_Oprema_KorisnikId",
                table: "Oprema",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Planina_KorisnikId",
                table: "Planina",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaninarskoDrustvo_KorisnikId",
                table: "PlaninarskoDrustvo",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_KorisnikId",
                table: "Post",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_PostLajkovi_KorisnikId",
                table: "PostLajkovi",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_PostLajkovi_PostId",
                table: "PostLajkovi",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Pracenja_KorisnikUserId",
                table: "Pracenja",
                column: "KorisnikUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Pracenja_KorsnikZapracenId",
                table: "Pracenja",
                column: "KorsnikZapracenId");

            migrationBuilder.CreateIndex(
                name: "IX_Putopis_KorisnikId",
                table: "Putopis",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_PutopisStaze_PutopisId",
                table: "PutopisStaze",
                column: "PutopisId");

            migrationBuilder.CreateIndex(
                name: "IX_PutopisStaze_StazaId",
                table: "PutopisStaze",
                column: "StazaId");

            migrationBuilder.CreateIndex(
                name: "IX_SigurnosniIzvjestaj_DogadjajID",
                table: "SigurnosniIzvjestaj",
                column: "DogadjajID");

            migrationBuilder.CreateIndex(
                name: "IX_SigurnosniIzvjestaj_KorisnikId",
                table: "SigurnosniIzvjestaj",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_SigurnosniRizik_StazaId",
                table: "SigurnosniRizik",
                column: "StazaId");

            migrationBuilder.CreateIndex(
                name: "IX_Sponzor_DogadjajId",
                table: "Sponzor",
                column: "DogadjajId");

            migrationBuilder.CreateIndex(
                name: "IX_Sponzor_KorisnikId",
                table: "Sponzor",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Staza_KorisnikId",
                table: "Staza",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Staza_PlaninaId",
                table: "Staza",
                column: "PlaninaId");

            migrationBuilder.CreateIndex(
                name: "IX_Vodic_KorisnikId",
                table: "Vodic",
                column: "KorisnikId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AutentifikacijaToken");

            migrationBuilder.DropTable(
                name: "Camping");

            migrationBuilder.DropTable(
                name: "DogadjajKorisnici");

            migrationBuilder.DropTable(
                name: "DrustvoKorisnici");

            migrationBuilder.DropTable(
                name: "Drzava");

            migrationBuilder.DropTable(
                name: "KomentarLajkovi");

            migrationBuilder.DropTable(
                name: "KorisnikFA");

            migrationBuilder.DropTable(
                name: "OcjenaStaze");

            migrationBuilder.DropTable(
                name: "Oprema");

            migrationBuilder.DropTable(
                name: "PostLajkovi");

            migrationBuilder.DropTable(
                name: "Pracenja");

            migrationBuilder.DropTable(
                name: "PutopisStaze");

            migrationBuilder.DropTable(
                name: "SigurnosniIzvjestaj");

            migrationBuilder.DropTable(
                name: "SigurnosniRizik");

            migrationBuilder.DropTable(
                name: "Sponzor");

            migrationBuilder.DropTable(
                name: "VerifikacijskiKodovi");

            migrationBuilder.DropTable(
                name: "Vodic");

            migrationBuilder.DropTable(
                name: "PlaninarskoDrustvo");

            migrationBuilder.DropTable(
                name: "Komentar");

            migrationBuilder.DropTable(
                name: "Putopis");

            migrationBuilder.DropTable(
                name: "Staza");

            migrationBuilder.DropTable(
                name: "Dogadjaj");

            migrationBuilder.DropTable(
                name: "Post");

            migrationBuilder.DropTable(
                name: "Planina");

            migrationBuilder.DropTable(
                name: "Korisnici");
        }
    }
}
