using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SCinema.Data;
using SCinema.Models;
using SCinema.Services;

var builder = WebApplication.CreateBuilder(args);

// ===== DB =====
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ===== Identity (UNE SEULE CONFIG) =====
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false; // mets true en prod si tu veux confirmation email
        // options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

// ===== MVC / Razor =====
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// ===== Services applicatifs =====
builder.Services.AddHttpClient();           // pour ExternalSeeder (appel API)
builder.Services.AddScoped<ExternalSeeder>();



// 🔒 En DEV: force la déconnexion à chaque run en changeant le nom du cookie
if (builder.Environment.IsDevelopment())
{
    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.Cookie.Name = "SCinema.Auth." + Guid.NewGuid().ToString("N"); // cookie différent à chaque run
        options.SlidingExpiration = false; // pas de prolongation silencieuse
    });
}



var app = builder.Build();

// ===== Seeding (migrations + rôles + films) =====
using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;

    // 0) Migrations
    var db = sp.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();

    // 1) Rôles
    var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = new[] { "Client", "Fournisseur" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // 2) Films via API (ne fait rien si déjà présents)
    var seeder = sp.GetRequiredService<ExternalSeeder>();
    await seeder.SeedFilmsAsync();
}

// ===== Pipeline =====
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // avant Authorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
