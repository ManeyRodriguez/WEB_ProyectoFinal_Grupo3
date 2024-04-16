using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using WEB_ProyectoFinal_Grupo3.Areas.Identity.Data;
using WEB_ProyectoFinal_Grupo3.Data;

using WEB_ProyectoFinal_Grupo3.Models;

var builder = WebApplication.CreateBuilder(args);

// Agregar la configuración desde appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Obtener la cadena de conexión de la configuración
var connectionString = builder.Configuration.GetConnectionString("DBContext")
                        ?? throw new InvalidOperationException("Connection string 'DBContext' not found.");

// Configurar el servicio de base de datos
builder.Services.AddDbContext<DBContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Configurar Identity
builder.Services.AddDefaultIdentity<Usuario>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<DBContext>();


// Configurar cookies de la aplicación
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

// Configurar servicios de MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
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

// Mapear rutas para controladores
app.MapControllerRoute(
    name: "categoria",
    pattern: "{controller=Categoria}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "paquete",
    pattern: "{controller=Paquete}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "venta",
    pattern: "{controller=Venta}/{action=Index}/{id?}");

// Ruta predeterminada para el controlador Home
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



// Mapear rutas para páginas Razor (Login y Logout)
app.MapRazorPages();

// Crear la base de datos y las tablas si aún no existen
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var dbContext = serviceProvider.GetRequiredService<DBContext>();
    dbContext.Database.EnsureCreated();
}

// Ejecutar la aplicación


    app.Run();


