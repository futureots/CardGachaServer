using CardGachaServer.Database;
using CardGachaServer.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

builder.Services.AddDbContext<MasterDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsHistoryTable("__EFMigrationsHistory_Master")));

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsHistoryTable("__EFMigrationsHistory_Auth")));

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsHistoryTable("__EFMigrationsHistory_User")));

builder.Services.AddControllers();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IGachaService, GachaService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<RsaKeyProvider>();
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));

// 인증 구현
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<RsaKeyProvider,IConfiguration>((options, keyProvider,config) =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = config["Jwt:Issuer"],
            
            ValidateAudience = true,
            ValidAudience = config["Jwt:Audience"],
            
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(keyProvider.PublicKey),
            
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),

        };

    });


var app = builder.Build();



app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var masterDb =scope.ServiceProvider.GetService<MasterDbContext>();
    var authDb = scope.ServiceProvider.GetService<AuthDbContext>();
    var userDb = scope.ServiceProvider.GetService<UserDbContext>();

    
    masterDb?.Database.Migrate();
    authDb?.Database.Migrate();
    userDb?.Database.Migrate();
}

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
