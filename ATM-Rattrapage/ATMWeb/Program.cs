using ATMWeb.Data;
using ATMWeb.Repositories;
using ATMWeb.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite("Data Source=atm.db"));

builder.Services.AddScoped<ICarteRepository, CarteRepository>();
builder.Services.AddScoped<ICompteRepository, CompteRepository>();
builder.Services.AddScoped<IAtmService, AtmService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    context.Database.Migrate();
    SeedData.Initialize(context);
}

app.MapControllers();

app.Run();