using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using WebAPI.Models;
using WebApp.ApiClients;
using WebAPI.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("ERROR: Connection string is missing!");
    // Za development možete hardcodirati za testiranje:
    connectionString = "Server=localhost;Port=5432;Database=fretfully-serene-crane.data-1.use1.tembo.io;User Id=postgres;Password=1ssKI1zsVZ965lZH;";
    // U produkciji bacite iznimku umjesto hardcodiranja
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("Server=fretfully-serene-crane.data-1.use1.tembo.io;Port=5432;Database=postgres;User Id=postgres;Password=1ssKI1zsVZ965lZH;"));

builder.Services.AddHttpClient("ProductsAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:5207/"); 
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpClient<ProductApiClient>(client =>
{
    client.BaseAddress = new Uri("https://oicar-team-11-ajugostitelj-11.onrender.com/api/");
    client.DefaultRequestHeaders
          .Accept
          .Add(new MediaTypeWithQualityHeaderValue("application/json"));
    // you can also set timeouts, add Polly policies, etc., here
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
});

//builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}"
);

app.UseSession();


app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
