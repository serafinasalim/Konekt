using Cuba_Staterkit.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<AppDbContext>();
builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation();

// Tambahkan konfigurasi otentikasi di sini
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        // Konfigurasi lainnya
    });

// Add session configuration directly in Program.cs
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Set your preferred timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//MySQL DB Conn
string _GetConnStringName = builder.Configuration.GetConnectionString("DefaultConnectionMySQL");
builder.Services.AddDbContextPool <AppDbContext>(options => options.UseMySql(_GetConnStringName, ServerVersion.AutoDetect(_GetConnStringName)));

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",                  // Nama route (bisa diberi nama sesuai kebutuhan)
        pattern: "{controller=Account}/{action=Login}/{id?}");
});

//app.Use(async (context, next) =>
//{
//    var isAuthenticated = context.Session.GetString("IsAuthenticated");

//    // Check if the request is not for the login or logout page
//    if (context.Request.Path != "/Account/Login" && context.Request.Path != "/Account/Logout")
//    {
//        // Redirect to the login page if not authenticated
//        if (string.IsNullOrEmpty(isAuthenticated) || isAuthenticated != "true")
//        {
//            context.Response.Redirect("/Account/Login");
//            return;
//        }
//    }

//    await next();
//});

app.Run();
