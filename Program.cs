// Importing necessary namespaces and libraries
using AmazonCloneMVC.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

// Create a new web application builder
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


// Configure and add the database context service using SQLite
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("AppContextDB") ?? throw new InvalidOperationException("Connection string 'AppContextDB' not found.")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<MyDbContext>();

// Configure and add session state services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    // Set session cookie options
    options.Cookie.Name = ".Sook.ma.Session";
    options.IdleTimeout = TimeSpan.FromDays(7);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.MaxAge = TimeSpan.FromDays(7);
});

// Register CartService as a scoped service
// builder.Services.AddScoped<CartService>();
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Build the web application
var app = builder.Build();

// Configure the HTTP request pipeline.

// Check if the environment is not development
if (!app.Environment.IsDevelopment())
{
    // Use exception handling middleware and set the default HSTS value
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Get the logger factory
var loggerFactory = app.Services.GetService<ILoggerFactory>();

// Add file logging to the logger factory
loggerFactory.AddFile($@"{Directory.GetCurrentDirectory()}\Logs\log.txt");

// Enable HTTPS redirection and serve static files
app.UseHttpsRedirection();
app.UseStaticFiles();

// Configure routing and authorization
app.UseRouting();
app.UseAuthentication();;
app.UseAuthorization();

// Use session
app.UseSession();

app.MapRazorPages();
// Map the default controller route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Run the application
app.Run();
