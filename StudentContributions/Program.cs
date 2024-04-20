using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using StudentContributions.DataAccess.Data;
using StudentContributions.DataAccess.Repository;
using StudentContributions.DataAccess.Repository.IRepository;
using StudentContributions.Models.Models;
using StudentContributions.Utility.Interfaces;
using StudentContributions.Utility.Services;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(c =>
{
    c.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(30);
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlServerOptionsAction: sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    }));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDBContext>().AddDefaultTokenProviders();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<IUnitOfWork,UnitOfWorks>();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddRazorPages();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
});

builder.Services.ConfigureApplicationCookie(option =>
{
    option.LoginPath = $"/Identity/Account/Login";
    option.LogoutPath = $"/Identity/Account/Logout";
    option.AccessDeniedPath = @"/Identity/Account/AccessDenied";
});
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
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(
name: "default",
    pattern: "{area=Student}/{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "Student", "Coordinator", "Manager" };
    foreach (var role in roles)
    {
        if (!roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
        {
            roleManager.CreateAsync(new IdentityRole(role)).GetAwaiter().GetResult();
        }
    }
}

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    string AD_email = "admin@email.com";
    string AD_pass = "!Admin123";
    string ST1_email = "student1@email.com";
    string St1_pass = "!Student123";
    string ST2_email = "student2@email.com";
    string St2_pass = "!Student123";
    string MN_email = "manager@email.com";
    string MN_pass = "!Manager123";
    if (userManager.FindByEmailAsync(AD_email).GetAwaiter().GetResult() == null)
    {
        var user = new ApplicationUser();
        user.Email = AD_email;
        user.EmailConfirmed = true;
        user.UserName = AD_email;
        userManager.CreateAsync(user, AD_pass).GetAwaiter().GetResult();
        userManager.AddToRoleAsync(user, "Admin").GetAwaiter().GetResult();
    }
    if (userManager.FindByEmailAsync(ST1_email).GetAwaiter().GetResult() == null)
    {
        var user = new ApplicationUser();
        user.Email = ST1_email;
        user.EmailConfirmed = true;
        user.UserName = ST1_email;
        userManager.CreateAsync(user, St1_pass).GetAwaiter().GetResult();
        userManager.AddToRoleAsync(user, "Student").GetAwaiter().GetResult();
    }
    if (userManager.FindByEmailAsync(ST2_email).GetAwaiter().GetResult() == null)
    {
        var user = new ApplicationUser();
        user.Email = ST2_email;
        user.EmailConfirmed = true;
        user.UserName = ST2_email;
        userManager.CreateAsync(user, St2_pass).GetAwaiter().GetResult();
        userManager.AddToRoleAsync(user, "Student").GetAwaiter().GetResult();
    }
    if (userManager.FindByEmailAsync(MN_email).GetAwaiter().GetResult() == null)
    {
        var user = new ApplicationUser();
        user.Email = MN_email;
        user.EmailConfirmed = true;
        user.UserName = MN_email;
        userManager.CreateAsync(user, MN_pass).GetAwaiter().GetResult();
        userManager.AddToRoleAsync(user, "Manager").GetAwaiter().GetResult();
    }
}

app.Run();
