using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using PrototipoCompras.Data;
using PrototipoCompras.Services;

// Hola me llamo John

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IBitacoraService, BitacoraService>();

// Add cookie authentication so we can sign-in users from AccountController
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
    });

// Usa DefaultConnection si existe; si no, usa Admin
var dbConnection =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? builder.Configuration.GetConnectionString("Admin")
    ?? throw new InvalidOperationException("No se encontró una cadena de conexión válida en appsettings.json");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(dbConnection));

// HttpClient para consumir la API externa de contactos
builder.Services.AddHttpClient<IContactosApiService, ContactosApiService>(client =>
{
    var baseUrl = builder.Configuration["ExternalApis:ContactosBaseUrl"]
                  ?? "https://web-service-contactos.onrender.com/";

    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(20);
});

var app = builder.Build();

// Apply pending EF Core migrations at startup so seeded data is created.
// If the DB server is not reachable, log the error and allow the app to continue.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error applying EF Core migrations at startup. Database may be unreachable.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();