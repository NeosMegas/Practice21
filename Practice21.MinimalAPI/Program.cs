using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Practice21.MinimalAPI.Data;
using Practice21.MinimalAPI;
using Practice21.MinimalAPI.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.Net.Mime;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<PhoneBookContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("PhoneBookContext") ?? throw new InvalidOperationException("Connection string 'PhoneBookContext' not found.")));
builder.Services.AddDbContext<UserContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("UserContext") ?? throw new InvalidOperationException("Connection string 'UserContext' not found.")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/accessdenied";
    }
    );
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// Add services to the container.

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/accessdenied", async (HttpContext context) =>
{
    context.Response.StatusCode = StatusCodes.Status403Forbidden;
    await context.Response.WriteAsync("Access Denied");
});

var dbScope = app.Services.CreateScope();
var services = dbScope.ServiceProvider;
SeedData.InitializeUsers(services);
SeedData.InitializePhoneBook(services);

app.MapPost("/login", async (string? returnUrl, HttpContext context) =>
{
    if (context.Request.ContentType is null) return Results.BadRequest();
    var form = context.Request.Form;
    if (!form.ContainsKey("login") || !form.ContainsKey("password"))
        return Results.BadRequest("No login or password provided");
    string? login = form["login"];
    string? password = form["password"];
    UserContext userContext = new UserContext(services.GetRequiredService<DbContextOptions<UserContext>>());
    User? user = userContext.Users.FirstOrDefault(u => u.Login == login && u.Password == password);
    if (user is null || user.Login is null) return Results.Unauthorized();
    var claims = new List<Claim>
    {
        new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
        new Claim(ClaimsIdentity.DefaultRoleClaimType, userContext.Roles.First(r => r.Id == user.RoleId).Name)
    };
    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
    await context.SignInAsync(claimsPrincipal);
    app.Logger.LogInformation("User {0} logged in", user.Login);
    return Results.Ok(new User() { Id = user.Id, Login = user.Login, Password = "", RoleId = user.RoleId });
});

app.MapGet("/logout", async (HttpContext context) =>
    {
        await context.SignOutAsync();
        return Results.Ok("Logged out");
    });

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
};

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapPhoneBookEntryEndpoints();
app.MapUserEndpoints();

app.Run();