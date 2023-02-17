using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pustok.Areas.Admin.Services;
using Pustok.Data;
using Pustok.Models;
using Pustok.Services;
using static Pustok.Services.EmailSender;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();

builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromSeconds(20);
});

builder.Services.AddDbContext<DataContext>(opt => {
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddScoped<IEmailSender, EmaiSender>();

builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{

    opt.Password.RequiredUniqueChars = 0;
    opt.Password.RequireUppercase = true;
    opt.Password.RequireLowercase = true;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequiredLength = 8;
    opt.Password.RequireDigit = true;
    opt.User.RequireUniqueEmail = false;

}).AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();
builder.Services.AddScoped<LayoutService>();




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

app.UseRouting();


app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}"
    );
//app.MapGet("/", () => "Hello World!");

app.Run();
