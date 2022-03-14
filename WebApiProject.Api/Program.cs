using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApiProject.Authentication;
using WebApiProject.DataService.Data;
using WebApiProject.DataService.IConfiguration;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Adding controller support
builder.Services.AddControllers();

//Update the JWT Config from the settings
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

// Add DbContext for the API
builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add dependency injection for UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Enable Api Version
builder.Services.AddApiVersioning(opt =>
{
    // provides to the client the different Api versions that we have
    opt.ReportApiVersions = true;
    //this will allow the api to automatically provide a default version
    opt.AssumeDefaultVersionWhenUnspecified = true;

    opt.DefaultApiVersion = ApiVersion.Default;
});

// Gettings the secret from the config
var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtConfig:Secret"]);

var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = false, // ToDo Update
    ValidateAudience = false, //  ToDo Update
    RequireExpirationTime = false, // ToDo Update
    ValidateLifetime = true,
};

// Injection into our DI container
builder.Services.AddSingleton(tokenValidationParameters);

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(jwt =>
    {
        jwt.SaveToken = true;
        jwt.TokenValidationParameters = tokenValidationParameters;
    });

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Builds the web application
var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    //app.UseSwaggerUI();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiProject.Api v1"));
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();