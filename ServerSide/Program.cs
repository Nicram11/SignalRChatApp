using ChatApp.Entities;
using ChatApp.Models;
using ChatApp.Models.ModelValidators;
using ChatApp.Services;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using AutoMapper;
using FluentValidation.AspNetCore;
using ChatApp;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ChatApp.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSignalR();
var authenticationSettings = new AuthenticationSettings();
builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);
// Add services to the container.
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = "Bearer";
    option.DefaultScheme = "Bearer";
    option.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;
    cfg.Events = new JwtBearerEvents();
    cfg.Events.OnMessageReceived = context => {

        if (context.Request.Cookies.ContainsKey("JwtToken"))
        {
            context.Token = context.Request.Cookies["JwtToken"];
        }

        return Task.CompletedTask;
    };
    cfg.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = authenticationSettings.JwtIssuer,
        ValidAudience = authenticationSettings.JwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
    };
   
});
builder.Services.AddSingleton(authenticationSettings);

builder.Services.AddControllers();
builder.Services.AddFluentValidation();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddDbContext<ChatAppDbContext>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IValidator<RegisterUserDTO>, RegisterValidator>();
builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseAuthentication();

app.UseRouting();
app.UseHttpsRedirection();

app.UseCors( policy => policy.WithOrigins("http://localhost:57314", "https://localhost:7044")
                                                  .AllowAnyHeader()
                                                  .AllowAnyMethod().AllowCredentials());

app.UseAuthorization();

app.MapHub<ChatHub>("/Home/Index");
/*app.UseEndpoints(endpoints => {
    endpoints.MapHub<ChatHub>("https://localhost:7044/Home/Index");
});*/


app.MapControllers();

app.Run();
