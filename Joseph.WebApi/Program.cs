using System.Text;
using Joseph.Context;
using Joseph.Data.Entities;
using Joseph.WebApi.Services;
using Joseph.WebApi.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;

string connectionString = configuration
    .GetConnectionString(builder.Environment.IsProduction() ? "ConnectionString" : "ConnectionStringLocal");

// Add services to the container.
builder.Services.AddDbContext<ApplicationContext>(options => 
    options.UseSqlServer(configuration.GetConnectionString("ConnectionStringLocal"),
        b=> b.MigrationsAssembly("Joseph.WebApi"))
    );

// For Identity
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Default Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
});

// Adding Authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })

// Adding Jwt Bearer
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            //ValidateIssuer = true,
            //ValidateAudience = true,
            ValidAudience = configuration["JWT:ValidAudience"],
            ValidIssuer = configuration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
        };
    });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

builder.Services.AddScoped<IJobTypeService, JobTypeService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IAnswerService, AnswerService>();
builder.Services.AddScoped<IFilterService, FilterService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationContext>();
        UserManager<User> userManager = services.GetRequiredService<UserManager<User>>();
        RoleManager<IdentityRole> roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        DbInitializer.Initialize(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(c =>
{
    c.AllowAnyMethod();
    c.AllowAnyHeader();
    c.AllowAnyOrigin();
});
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();