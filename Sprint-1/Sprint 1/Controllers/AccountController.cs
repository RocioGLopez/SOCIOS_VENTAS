using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using PrototipoCompras.Models;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace PrototipoCompras.Controllers;

public class AccountController : Controller
{
    private string GetUsersFilePath()
    {
        var dir = Path.Combine(AppContext.BaseDirectory, "App_Data");
        Directory.CreateDirectory(dir);
        return Path.Combine(dir, "users.json");
    }

    private List<UserAccount> ReadUsers()
    {
        var path = GetUsersFilePath();
        if (!System.IO.File.Exists(path)) return new List<UserAccount>();
        var json = System.IO.File.ReadAllText(path);
        try { return JsonSerializer.Deserialize<List<UserAccount>>(json) ?? new List<UserAccount>(); }
        catch { return new List<UserAccount>(); }
    }

    private void WriteUsers(List<UserAccount> users)
    {
        var path = GetUsersFilePath();
        var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
        System.IO.File.WriteAllText(path, json);
    }

    private string CreateSalt() => Guid.NewGuid().ToString("N");

    private string HashPassword(string password, string salt)
    {
        using var sha = SHA256.Create();
        var combined = salt + password;
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(combined));
        return Convert.ToHexString(bytes);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            ModelState.AddModelError(string.Empty, "El correo es requerido.");
            return View();
        }

        var users = ReadUsers();
        var user = users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "No existe una cuenta con ese correo.");
            ViewData["Email"] = email;
            return View();
        }

        var hash = HashPassword(password ?? string.Empty, user.Salt);
        if (!hash.Equals(user.PasswordHash, StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(string.Empty, "Contraseña incorrecta.");
            ViewData["Email"] = email;
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Nombres ?? user.Email),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        return RedirectToAction("Create", "Solicitudes");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(string nombres, string apellidos, string email, string telefono, string direccion, string nacimiento, string password, string confirm_password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirm_password))
        {
            ModelState.AddModelError(string.Empty, "Correo y contraseñas son requeridos.");
            return View();
        }
        if (password != confirm_password)
        {
            ModelState.AddModelError(string.Empty, "Las contraseñas no coinciden.");
            return View();
        }

        var users = ReadUsers();
        if (users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
        {
            ModelState.AddModelError(string.Empty, "Ya existe una cuenta con ese correo.");
            return View();
        }

        var salt = CreateSalt();
        var hash = HashPassword(password, salt);
        DateTime? fn = null;
        if (DateTime.TryParse(nacimiento, out var d)) fn = d;

        var user = new UserAccount
        {
            Email = email,
            Salt = salt,
            PasswordHash = hash,
            Nombres = nombres ?? string.Empty,
            Apellidos = apellidos ?? string.Empty,
            Telefono = telefono ?? string.Empty,
            Direccion = direccion ?? string.Empty,
            FechaNacimiento = fn
        };

        users.Add(user);
        WriteUsers(users);

        // sign-in after register
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Nombres ?? user.Email), new Claim(ClaimTypes.Email, user.Email) };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return RedirectToAction("Create", "Solicitudes");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }
}
