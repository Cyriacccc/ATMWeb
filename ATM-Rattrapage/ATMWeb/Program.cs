// Import des espaces de noms nécessaires
using ATMWeb.Data;              // Accès au DataContext et SeedData
using ATMWeb.Repositories;     // Accès aux repositories
using ATMWeb.Services;         // Accès au service métier
using Microsoft.EntityFrameworkCore; // Entity Framework (SQLite)
using Scalar.AspNetCore;       // Interface de documentation (Scalar)

// Création du builder : permet de configurer l’application
var builder = WebApplication.CreateBuilder(args);

// Configuration de la base de données SQLite
// On indique que le DataContext utilisera un fichier atm.db
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite("Data Source=atm.db"));

// Injection de dépendances (principe SOLID)
// On associe chaque interface à son implémentation concrète
builder.Services.AddScoped<ICarteRepository, CarteRepository>();
builder.Services.AddScoped<ICompteRepository, CompteRepository>();
builder.Services.AddScoped<IAtmService, AtmService>();

// Activation des contrôleurs (API REST)
builder.Services.AddControllers();

// Activation d’OpenAPI (utile pour Scalar)
builder.Services.AddOpenApi();

// Construction de l’application (fin de la phase de configuration)
var app = builder.Build();

// Création d’un scope pour utiliser le DataContext au démarrage
using (var scope = app.Services.CreateScope())
{
    // Récupération du DataContext via l’injection de dépendances
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();

    // Applique automatiquement les migrations (création/mise à jour des tables)
    context.Database.Migrate();

    // Initialise la base avec des données par défaut (carte, PIN, solde)
    SeedData.Initialize(context);
}

// Activation de la documentation OpenAPI
app.MapOpenApi();

// Activation de l’interface Scalar sur /scalar
app.MapScalarApiReference("/scalar");

// Activation des routes des contrôleurs (API)
app.MapControllers();

// Route simple pour tester que l’API fonctionne
// Accessible via http://localhost:7046/api
app.MapGet(
    "/api",
    () =>
        Results.Ok(
            new
            {
                message = "API ATM disponible",
                endpoints = new[]
                {
                    "POST /api/atm/reset",
                    "GET /api/compte/solde",
                    "POST /api/compte/versement",
                    "POST /api/compte/retrait",
                },
            }
        )
);

// Démarrage de l’application (mise en écoute du serveur)
app.Run();