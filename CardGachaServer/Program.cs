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

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsHistoryTable("__EFMigrationsHistory_User")));

builder.Services.AddControllers();

builder.Services.AddScoped<IGachaService, GachaService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));

// 인증 구현
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IConfiguration>((options,config) =>
    {
        var firebaseProjectId = config["Firebase:ProjectId"];
        
        options.Authority = $"https://securetoken.google.com/{firebaseProjectId}";
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://securetoken.google.com/{firebaseProjectId}", 
            
            ValidateAudience = true,
            ValidAudience = firebaseProjectId,
            
            ValidateIssuerSigningKey = true,
            
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),

            ValidAlgorithms = new[] {"RS256"},
        };
        
        // 추가: 실패 원인 로깅
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"[JWT 인증 실패] {context.Exception}");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine($"[JWT Challenge] Error: {context.Error}, Description: {context.ErrorDescription}");
                return Task.CompletedTask;
            }
        };

    });
builder.Services.AddAuthorization();

var app = builder.Build();



app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var masterDb =scope.ServiceProvider.GetService<MasterDbContext>();
    var userDb = scope.ServiceProvider.GetService<UserDbContext>();

    
    masterDb?.Database.Migrate();
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
