using AspNetCoreLiveMonitoring.Extensions;
using Microsoft.EntityFrameworkCore;
using EM.BL;
using EM.DAL;
using EM.DAL.EF;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLiveMonitoring();
builder.Services.AddDbContext<EmDbContext>(optionsBuilder =>
    optionsBuilder.UseSqlite("Data Source=../EMDatabase.db"));

// Dependency Injection configuratie
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

var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{
    var dbCtx = serviceScope.ServiceProvider.GetRequiredService<EmDbContext>();
    if (dbCtx.CreateDatabase(true))
    {
            DataSeeder.Seed(dbCtx);
    }
    
}

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();



    