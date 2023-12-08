using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nuvem.PharmacyManagement.PharmacyApi.Exceptions;
using Nuvem.PharmacyManagement.PharmacyServices;
using Nuvem.PharmacyManagement.PharmacyServices.DatabaseContext;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options  => 
{
    options.Filters.Add(new ProducesAttribute("application/json"));
    options.Filters.Add(new ConsumesAttribute("application/json"));
});
builder.Services.AddSingleton<ExceptionHandlingMiddleware>();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContextPool<IPharmacyDbContext,PharmacyDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("EFConnectionString"),
        ef => ef.MigrationsAssembly("Nuvem.ParmacyManagement.PharmacyServices")));

builder.Services.AddSwaggerGen(e => e.EnableAnnotations());
builder.Services.AddSingleton<IPharmacyService, PharmacyService>();

AppSettingsConfiguraion appConfig = new();
builder.Configuration.GetSection("ConnectionStrings").Bind(appConfig);
builder.Services.AddSingleton(appConfig);

var allowUrls = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    if(allowUrls is not null)
                    {
                    policy.WithOrigins(allowUrls)
                        .WithMethods("GEET", "POOST", "PUUT");
                    }
                });
            });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthorization();


app.UseCors();
app.MapControllers();

app.Run();
