using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using PrototipoCompras.Data;
using PrototipoCompras.Services;

// Hola me llamo John

var builder = WebApplication.CreateBuilder(args);

// ── Servicios existentes ───────────────────────────────────────
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IBitacoraService, BitacoraService>();

// ── Autenticación por cookie ───────────────────────────────────
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
    });

// ── Cadena de conexión ─────────────────────────────────────────
// ── Cadena de conexión ─────────────────────────────────────────
var dbConnection = builder.Configuration.GetConnectionString("AzureConnection")
    ?? throw new InvalidOperationException("No se encontró la cadena de conexión AzureConnection en appsettings.json");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(dbConnection, sqlOptions =>
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,                // número de intentos
            maxRetryDelay: TimeSpan.FromSeconds(10), // tiempo máximo entre intentos
            errorNumbersToAdd: null
        )
    ));
    // ── HttpClient para API externa de contactos ───────────────────
builder.Services.AddHttpClient<IContactosApiService, ContactosApiService>(client =>
{
    var baseUrl = builder.Configuration["ExternalApis:ContactosBaseUrl"]
                  ?? "https://web-service-contactos.onrender.com/";
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(20);
});


// ── NUEVO: Controllers para la API REST ───────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ── NUEVO: CORS para entidades externas ───────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("ExternalEntities", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ── Migraciones automáticas al arrancar ───────────────────────
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

// ── Pipeline ──────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// ── NUEVO: CORS antes de Auth ─────────────────────────────────
app.UseCors("ExternalEntities");

app.UseAuthentication();
app.UseAuthorization();

// ── NUEVO: Swagger ────────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI();

// ── Rutas ─────────────────────────────────────────────────────
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
