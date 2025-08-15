using AspNetCoreLiveMonitoring.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using EM.BL;
using EM.DAL;
using EM.DAL.EF;
using EM.UI.MVC;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLiveMonitoring();

//config DB
builder.Services.AddDbContext<EmDbContext>(optionsBuilder =>
    optionsBuilder.UseSqlite("Data Source=../EMDatabase.db"));

// Dependency Injection configuratie
// Hierdoor kan ik de Manager en Repository in mijn controllers gebruiken, en zullen de HTTP req goed verwerkt worden
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<IManager, Manager>();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        //er was een problemm met mijn enum te lezen vanuit de payload met JSON. deze convert zorgt dat het wel lukt
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Voeg MVC toe
builder.Services.AddControllersWithViews();

// ASP.NET Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false; // Geen e-mailverificatie nodig
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<EmDbContext>();

builder.Services.AddScoped<SignInManager<IdentityUser>>();

builder.Services.ConfigureApplicationCookie(cfg =>
{
    cfg.Events.OnRedirectToLogin += ctx =>
    {
        if (ctx.Request.Path.StartsWithSegments("/api"))
        {
            ctx.Response.StatusCode = 401;
        }

        return Task.CompletedTask;
    };

    cfg.Events.OnRedirectToAccessDenied += ctx =>
    {
        if (ctx.Request.Path.StartsWithSegments("/api"))
        {
            ctx.Response.StatusCode = 403;
        }

        return Task.CompletedTask;
    };
});

// Auhtorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrator", policy => policy.RequireRole("Admin"));
});

// Register IdentitySeeder and DataSeeder for dependency injection
builder.Services.AddScoped<IdentitySeeder>();
builder.Services.AddScoped<DataSeeder>();


var app = builder.Build();


    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<EmDbContext>();
        bool isDatabaseCreated = EmDbContext.CreateDatabase(context);
        if (isDatabaseCreated)
        {
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            
            var identitySeeder = new IdentitySeeder(userManager, roleManager);
            await identitySeeder.SeedAsync(); 

            var dataSeeder = new DataSeeder(context, userManager);
            await dataSeeder.SeedAsync();

           
        }
        
    }


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAndMapLiveMonitoring();
//middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public partial class Program { }