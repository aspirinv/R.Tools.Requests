using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using R.Tools.Requests;
using R.Tools.Requests.Contracts;
using R.Tools.Requests.Sample;
using R.Tools.Requests.Sample.Controllers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthController.Key)),
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidateAudience = false,
        ValidateIssuer = false
    };
});
builder.Services.AddAuthorization();

builder.Services.AddTransient<IRequestStorage, ConsoleStorage>();
builder.RegisterRequestsCollector(o => {
    o.DefineUserId = c => c.FindFirst("Id")?.Value;
    o.ShouldCollect = p => !p.StartsWithSegments("/api/ping");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.CollectRequests();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();
