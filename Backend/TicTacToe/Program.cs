using Games.Application.Authentication.Services;
using Games.Application.Infrastructure;
using Games.Application.Persistence;
using Games.Application.TicTacToe.Hubs;
using Games.Application.TicTacToe.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
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
builder.Services.AddScoped<JwtService>();

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

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

var app = builder.Build();

app.UseAuthentication();

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

app.MapDefaultControllerRoute();

app.MapHub<TicTacToeHub>("/tictactoehub");
app.MapHub<LobbyHub>("/lobbyhub");

app.Run();
