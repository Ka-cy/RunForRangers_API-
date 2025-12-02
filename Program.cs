global using RunForRangersAPI.Services;
global using RunForRangersAPI.Models.EmailManagement_Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using RunForRangersAPI.Models;
using RunForRangersAPI.Extensions;




var builder = WebApplication.CreateBuilder(args);

// Configure services (equivalent to ConfigureServices method)
ConfigureServices(builder.Services, builder.Configuration);
builder.Services.AddScoped<IEmailService, IEmailServices>();
builder.Services.AddScoped<IEmailServices>(); // ? REGISTERED HERE
builder.Services.AddScoped<IStoredProcedureService, StoredProcedureService>();

builder.Services.AddScoped<IAuditLogService, AuditLogService>();
var app = builder.Build();

// Configure the HTTP request pipeline (equivalent to Configure method)
Configure(app, app.Environment);

app.Run();

// Method equivalent to Startup.ConfigureServices
static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{




    services.AddControllers();
    services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
    services.AddDbContext<RunForRangersDBContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));



    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();

    // Add CORS policy
    services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularApp", policy =>
        {
            policy.WithOrigins("http://localhost:4200", "https://localhost:7158") // Added Swagger URL
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });



}

// Method equivalent to Startup.Configure
static void Configure(WebApplication app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Ensure database and stored procedures exist
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<RunForRangersDBContext>();

        // Apply pending migrations
     //   context.Database.Migrate();

        // CRITICAL: Ensure stored procedures exist after migration
        context.EnsureAllStoredProcedures();

        // Alternative using the service
        // var spService = scope.ServiceProvider.GetRequiredService<IStoredProcedureService>();
        // await spService.EnsureStoredProceduresExistAsync();
    }



    app.UseHttpsRedirection();
    app.UseStaticFiles();
    // Use CORS
    app.UseCors("AllowAngularApp");

    app.UseAuthorization();

    app.MapControllers();
}
