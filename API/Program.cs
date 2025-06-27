using System.Text;
using API.Mapper;
using Application.Interface;
using Application.Service;
using Application.Service.Application.Service;
using Application.Settings;
using Infrastructure;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using VaccinceCenter.Repositories.Base;

var builder = WebApplication.CreateBuilder(args);

//Database Context
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//jwt
builder.Services.Configure<JwtSetting>(
    builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSetting>();
var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

//settings
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.Configure<PayOsSetting>(builder.Configuration.GetSection("PayOsConfig"));

builder.Services.Configure<GoogleSetting>(
    builder.Configuration.GetSection("GoogleSetting"));

//Google + JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
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
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["GoogleSetting:ClientId"];
    options.ClientSecret = builder.Configuration["GoogleSetting:ClientSecret"];
    options.CallbackPath = "/signin-google"; 

    options.SaveTokens = true;
    options.Scope.Add("profile");
    options.Scope.Add("email");

    options.Events.OnCreatingTicket = ctx =>
    {
        return Task.CompletedTask;
    };
});

builder.Services.AddHttpContextAccessor();

//OpenRouter
builder.Services.AddHttpClient("OpenRouter", c =>
{
    c.BaseAddress = new Uri("https://openrouter.ai/");
    c.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", builder.Configuration["OpenRouter:ApiKey"]);
    c.DefaultRequestHeaders.Accept.Add(new("application/json"));
});

//DI Service
builder.Services.AddScoped<IUserAccountService, UserAccountService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<CurrentUserService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPremiumService, PremiumService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddHttpClient<PayOsService>();
builder.Services.AddHttpClient<OpenRouterService>();
builder.Services.AddScoped<IOpenRouterService, OpenRouterService>();
builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();
builder.Services.AddScoped<IProductBrandService, ProductBrandService>();
builder.Services.AddScoped<IProductStyleService, ProductStyleService>();
builder.Services.AddScoped<IProductTypeService, ProductTypeService>();
builder.Services.AddScoped<IProductMaterialService, ProductMaterialService>();
builder.Services.AddScoped<IOutfitService, OutfitService>();
builder.Services.AddScoped<IOutfitComboItemService, OutfitComboItemService>();
builder.Services.AddScoped<IUserClosetService, UserClosetService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IProductColorService, ProductColorService>();


//DI Repository
builder.Services.AddScoped(typeof(GenericRepository<>));
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddScoped<UserAccountRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<OrderRepository>();
builder.Services.AddScoped<PremiumRepository>();
builder.Services.AddScoped<ProductRepository>();
builder.Services.AddScoped<ProductBrandRepository>();
builder.Services.AddScoped<ProductCategoryRepository>();
builder.Services.AddScoped<ProductColorRepository>();
builder.Services.AddScoped<ProductMaterialRepository>();
builder.Services.AddScoped<ProductSizeRepository>();
builder.Services.AddScoped<ProductStyleRepository>();
builder.Services.AddScoped<ProductTypeRepository>();
builder.Services.AddScoped<UserClosetRepository>();
builder.Services.AddScoped<CartRepository>();
builder.Services.AddScoped<FeedbackRepository>();


builder.Services.AddScoped<PaymentRepository>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.WithOrigins("")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

    // Cấu hình JWT Bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập token dạng: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http, 
        Scheme = "bearer",               
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "bearer",
                Name = "Authorization",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
