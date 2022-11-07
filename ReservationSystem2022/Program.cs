using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using ReservationSystem2022.Middleware;
using ReservationSystem2022.Models;
using ReservationSystem2022.Repositories;
using ReservationSystem2022.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// lis‰t‰‰n tietokantaan liittyen t‰m‰
// luodaan tietokantakONTeksi, k‰ytet‰‰n sqlserveri‰ ja k‰ytet‰‰n tuota reservationDB:t‰ joka oli appsettingsssiss‰
builder.Services.AddDbContext<ReservationContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("ReservationDB")));

builder.Services.AddControllers();

builder.Services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

// esitell‰‰n palvelut mit‰ k‰ytet‰‰n
// jotta n‰it‰ luokkia voi k‰ytt‰‰ hyˆdynt‰en dependency injektion tekniikkaa, on ne esitelt‰v‰ t‰‰ll‰ program.cs
// t‰ss‰ kerrotaan mik‰ luokka toteuttaa mink‰kin rajapinnan
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IReservationService, ReservationService>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Reservationsystem API",
            Description = "An ASP.NET Core Web API for managing items and their reservations",
            TermsOfService = new Uri("https://example.com/terms"),
            Contact = new OpenApiContact
            {
                Name = "Example Contact",
                Url = new Uri("https://example.com/contact")
            },
            License = new OpenApiLicense
            {
                Name = "Example License",
                Url = new Uri("https://example.com/license")
            }
        });
        // katsotaan ..portti/swagger tai ..portti/swagger.json

        // halutaan laittaa controllereihin xml kommentteja ett‰ mit‰ tietyt funktiot tekee (kommentit sitten lis‰t‰‰n controlleriin \\\)
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) // kehitysversiossa
{
    app.UseSwagger();
    app.UseSwaggerUI(); 
}

app.UseHttpsRedirection();

// katsotaan onko avainta, eli otetaan middleware kayttoon
app.UseMiddleware<ApiKeyMiddleware>();

// autentikointi kayttoon
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
