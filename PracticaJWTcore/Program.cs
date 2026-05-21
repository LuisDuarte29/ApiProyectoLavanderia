using Microsoft.EntityFrameworkCore;
using PracticaJWTcore.Repositorios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Azure.Core.Pipeline;
using Microsoft.OpenApi.Models;
using PracticaJWTcore.Services;
using PracticaJWTcore.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// CORS queda abierto para mantener compatibilidad con los frontends de desarrollo.
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

// Configura validacion JWT para que los endpoints protegidos puedan aceptar Bearer tokens.
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

// Swagger documenta la API y permite probar endpoints enviando Authorization: Bearer {token}.
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

// Repositories: concentran el acceso a EF Core, ADO.NET y stored procedures.
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IServicesRepository, ServicesRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IAutenticacionRepository, AutenticacionRespository>();
builder.Services.AddScoped<IUsuariosRepository, UsuarioRepository>();
builder.Services.AddScoped<IPedidosRepository, PedidosRepository>();
builder.Services.AddScoped<IVentasRepository, VentasRepository>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IArticulosRepository, ArticulosRepository>();

// Services: concentran reglas de negocio y coordinan las operaciones de cada modulo.
builder.Services.AddScoped<ICustomerServices, CustomerServices>();
builder.Services.AddScoped<IServiceServices, ServiceServices>();
builder.Services.AddScoped<IAppointmentServices, AppoitmentServices>();
builder.Services.AddScoped<IAutenticacionServices, AutenticacionServices>();
builder.Services.AddScoped<IPedidosServices, PedidosServices>();
builder.Services.AddScoped<IVentasService, VentasService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IArticulosService, ArticulosService>();
builder.Services.AddScoped<IVehicleModal, VehicleModalServices>();
builder.Services.AddScoped<ICustomerModal, CustomerModalServices>();
builder.Services.AddScoped<IServicioModal, ServiciosModalServices>();
builder.Services.AddScoped<IUsuarioServices, UsuarioServices>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Logging.AddConsole();

// DbContext principal de EF Core; mantiene el contrato con SQL Server configurado en appsettings.json.
builder.Services.AddDbContext<PracticaJWTcoreContext>(sqlBuilder =>
{

    sqlBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddEndpointsApiExplorer();

// Controllers expone los endpoints HTTP. CamelCase conserva el formato JSON que consume el frontend.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; // Para mantener los nombres de las propiedades como están en el modelo

});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();

}
app.UseRouting();

// Pipeline HTTP: CORS debe ejecutarse antes de autenticacion/autorizacion para aceptar llamadas del frontend.
app.UseCors("PermitirTodo"); // ?? Debe estar ANTES de UseAuthorization()
app.UseAuthentication(); // ?? Si usas JWT, agrega esto antes de Authorization
app.UseAuthorization();

// Mapea los atributos [Route] y [Http...] definidos en los controllers.
app.MapControllers();

app.Run();
