using Games.Application.Authentication.Services;
using Games.Application.Infrastructure;
using Games.Application.Persistence;
using Games.Application.TicTacToe.Hubs;
using Games.Application.TicTacToe.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using TicTacToe;

var AllowCors = "_allowCors";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<TicTacToeService>();
builder.Services.AddScoped<LobbyService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<Encryptor>();

builder.Services.AddDbContext<AuthDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});


builder.Services.AddDbContext<GamesDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowCors,
    policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
     });
});

builder.Services.AddSignalR(opts => opts.EnableDetailedErrors = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors(AllowCors);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseRouting();
app.MapAreaControllerRoute("Auth", "Auth", "{controller}");
app.MapDefaultControllerRoute();

app.MapHub<TicTacToeHub>("/tictactoehub");
app.MapHub<LobbyHub>("/lobbyhub");

app.Run();
