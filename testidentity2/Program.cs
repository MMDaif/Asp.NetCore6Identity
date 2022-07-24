using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using testidentity2.Data;
using testidentity2.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using testidentity2.Extensions;
using testidentity2.Services;

var builder = WebApplication.CreateBuilder(args);
// Get Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// Set DataBase
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
// Set Identity user and store
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options => options.Password.RequiredLength = 8).AddEntityFrameworkStores<ApplicationDbContext>();
// set Identity authorization
//builder.Services.AddIdentityServer().AddApiAuthorization<ApplicationUser, ApplicationDbContext>();
// Set Identity authentication

builder.Services.AddSingleton<testidentity2.Services.ITokenService, testidentity2.Services.TokenService>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["JwtOptions:Issuer"],
        ValidAudience = builder.Configuration["JwtOptions:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtOptions:Key"]))
    };
});

builder.Services.AddOpenAPI();

builder.Services.AddControllersWithViews();

var app = builder.Build();
app.ConfigureOpenAPI();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});


app.MapGet("/", () => "Hello World!");

app.Run();
