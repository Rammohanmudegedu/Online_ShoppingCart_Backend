using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Online_ShoppingCart_API.Auth;
using Online_ShoppingCart_API.Extensions;
using Online_ShoppingCart_API.Handlers;
using Online_ShoppingCart_API.Service;
using shopping.application.Iservices;
using shopping.DataAccess.IRepositories;
using shopping.DataAccess.Repositories;
using Shopping.DataAccess.Models;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<StoreContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("ShoppingCartDB")));

builder.Services.AddDbContext<AuthContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("ShoppingCartAuthDB")));

// Register Global Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Dependency Injection for Services and Repositories
builder.Services.AddAllServices();
builder.Services.AddAllRepositories();


builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.IsEssential = true; 
});

builder.Services.AddHttpContextAccessor();

var Configuration = builder.Configuration;


builder.Services.AddIdentityCore<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AuthContext>();

builder.Services.Configure<IdentityOptions>(options =>
{

    options.Password.RequiredLength = 8;
});



builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(o =>
{
    var Key = Encoding.UTF8.GetBytes(Configuration["JWT:Key"]);
    o.SaveToken = true;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Configuration["JWT:Issuer"],
        ValidAudience = Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Key)
    };
});



builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseCors();

app.UseSession();

app.Run();
