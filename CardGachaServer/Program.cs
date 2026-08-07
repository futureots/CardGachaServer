using CardGachaServer.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var database =scope.ServiceProvider.GetService<ApplicationDbContext>();

    // db 구조가 변경될때만 실행
    database?.Database.EnsureDeleted();
    database?.Database.EnsureCreated();
    // 나중에 1차적으로 완료되면 migration 만들어서 사용하기
    //database?.Database.Migrate();
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
