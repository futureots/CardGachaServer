using CardGachaServer.Database;
using CardGachaServer.Service;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsHistoryTable("__EFMigrationsHistory_App")));

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsHistoryTable("__EFMigrationsHistory_Auth")));

builder.Services.AddControllers();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var database =scope.ServiceProvider.GetService<ApplicationDbContext>();
    var authDb = scope.ServiceProvider.GetService<AuthDbContext>();

    // db 구조가 변경될때만 실행
    /*database?.Database.EnsureDeleted();
    database?.Database.EnsureCreated();*/
    // 나중에 1차적으로 완료되면 migration 만들어서 사용하기
    database?.Database.Migrate();
    authDb?.Database.Migrate();
    
    //authDb?.Database.EnsureCreated();
}

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseRouting();

app.UseHttpsRedirection();

app.Run();
