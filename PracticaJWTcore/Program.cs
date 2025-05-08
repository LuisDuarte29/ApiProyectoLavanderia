using Microsoft.EntityFrameworkCore;
using PracticaJWTcore.Repositorios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Azure.Core.Pipeline;
using Microsoft.OpenApi.Models;
using PracticaJWTcore.Services;
using PracticaJWTcore.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Configuration.AddJsonFile("appsettings.json");
var secretkey = builder.Configuration.GetSection("JWT").GetSection("Key").ToString();
var keyBytes = Encoding.ASCII.GetBytes(secretkey);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        //El IssuerSigningKey se usa para validar la firma del token
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mi API", Version = "v1" });

    // Definir autenticación con JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingrese el token JWT en el formato: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    // Aplicar autenticación a todas las solicitudes protegidas
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IServicesRepository, ServicesRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IAutenticacionRepository, AutenticacionRespository>();
builder.Services.AddScoped<IPedidosRepository, PedidosRepository>();
builder.Services.AddScoped<ICustomerServices, CustomerServices>();
builder.Services.AddScoped<IServiceServices, ServiceServices>();
builder.Services.AddScoped<IAppointmentServices, AppoitmentServices>();
builder.Services.AddScoped<IAutenticacionServices, AutenticacionServices>();
builder.Services.AddScoped<IPedidosServices, PedidosServices>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine("Cadena de conexión: " + connectionString);

builder.Services.AddDbContext<PracticaJWTcoreContext>(sqlBuilder =>
{

    sqlBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddDbContext<CustomerDataBaseCustomer>(sqlBuilder =>
{

    sqlBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();

}
app.UseRouting();
app.UseCors("PermitirTodo"); // ?? Debe estar ANTES de UseAuthorization()
app.UseAuthentication(); // ?? Si usas JWT, agrega esto antes de Authorization
app.UseAuthorization();

app.MapControllers();

app.Run();
