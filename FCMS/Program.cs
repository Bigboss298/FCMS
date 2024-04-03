using FCMS.Auth;
using FCMS.FileManager;
using FCMS.Gateway;
using FCMS.Implementations.Repository;
using FCMS.Implementations.Service;
using FCMS.Interfaces.Repository;
using FCMS.Interfaces.Service;
using FCMS.Persistence;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(c => c
                .AddPolicy("FCMS", builder => builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin()));

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FCMS", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
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
                }
            },
            new string[]{}
        }
    });
});

#region | Services and Repositories
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped<IBaseRepository, BaseRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IFarmerService, FarmerService>();
builder.Services.AddScoped<IFarmerRepository, FarmerRepository>();


builder.Services.AddScoped<IFileManager, FileManager>();


builder.Services.AddScoped<IFileManager, FileManager>();
builder.Services.AddScoped<IMapper, Mapper>();

builder.Services.AddScoped<IAddressRepository, AddressRepository>();

var configuration = builder.Configuration;
var paystackApiKey = configuration["Paystack:ApiKey"];
builder.Services.AddScoped<IPaystackService, PaystackService>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddScoped<IProductOrderRepository, ProductOrderRepository>();

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

builder.Services.AddScoped<IPaymentDetails, PaymentDetails>();

#endregion

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySQL(builder.Configuration.GetConnectionString("MyConnection")));




#region | Authentication
builder.Services.AddScoped<IJWTManager, JWTManager>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").LastOrDefault();

            var isTokenValid = JWTManager.IsTokenValid(builder.Configuration["Jwt:Key"].ToString(), builder.Configuration["Jwt:Issuer"].ToString(), token);
            context.Response.StatusCode = isTokenValid ? (int)HttpStatusCode.OK : (int)HttpStatusCode.Unauthorized;

            return Task.CompletedTask;
        }
    };
});
#endregion




//builder.Services.AddSwaggerGen(option =>
//{
//    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//{
//    In = ParameterLocation.Header,
//    Description = "Please enter a valid token",
//    Name = "Authorization",
//    Type = SecuritySchemeType.Http,
//    BearerFormat = "JWT",
//    Scheme = "Bearer"
//});
//option.AddSecurityRequirement(new OpenApiSecurityRequirement
//                 {
//                     {
//                         new OpenApiSecurityScheme
//                         {
//                             Reference = new OpenApiReference
//                             {
//                                 Type=ReferenceType.SecurityScheme,
//                                 Id="Bearer"
//                             }
//                         },
//                         new string[]{}
//                     }
//                 });
//});

//builder.Services.AddAuthentication(auth =>
//{
//    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = builder.Configuration["Jwt:Issuer"],
//        ValidAudience = builder.Configuration["Jwt:Issuer"],
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//    };
//});



var app = builder.Build();


//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FCMS v1"));
//}

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("FCMS");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
