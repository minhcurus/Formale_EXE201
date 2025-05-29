using System.Text;
using Application.Interface;
using Application.Service;
using Application.Settings;
using Infrastructure;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VaccinceCenter.Repositories.Base;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//jwt
builder.Services.Configure<JwtSetting>(
    builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSetting>();
var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));


//DI Service
builder.Services.AddScoped<IUserAccountService, UserAccountService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

//DI Repository
builder.Services.AddScoped(typeof(GenericRepository<>));
builder.Services.AddScoped<UserAccountRepository>();
builder.Services.AddScoped<ProductRepository>();
builder.Services.AddScoped<ProductBrandRepository>();
builder.Services.AddScoped<ProductCategoryRepository>();
builder.Services.AddScoped<ProductColorRepository>();
builder.Services.AddScoped<ProductMaterialRepository>();
builder.Services.AddScoped<ProductSizeRepository>();
builder.Services.AddScoped<ProductStyleRepository>();
builder.Services.AddScoped<ProductTypeRepository>();



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

//Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; 
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero 
    };
});

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,

            },
            new List<string>()
        }
    });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.Run();
