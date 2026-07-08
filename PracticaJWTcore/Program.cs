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
        policy =>
        {
            var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
            if (origins.Length > 0)
            {
                policy.WithOrigins(origins)
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }
            else
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }
        });
});


var secretkey = builder.Configuration["JWT:Key"]
    ?? throw new InvalidOperationException("Falta configurar JWT:Key.");
var issuer = builder.Configuration["JWT:Issuer"]
    ?? throw new InvalidOperationException("Falta configurar JWT:Issuer.");
var audience = builder.Configuration["JWT:Audience"]
    ?? throw new InvalidOperationException("Falta configurar JWT:Audience.");
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
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience
    };
});

// Swagger documenta la API y permite probar endpoints enviando Authorization: Bearer {token}.
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mi API", Version = "v1" });

    // Definir autenticaci�n con JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingrese el token JWT en el formato: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    // Aplicar autenticaci�n a todas las solicitudes protegidas
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
builder.Services.AddScoped<IAutenticacionRepository, AutenticacionRespository>();
builder.Services.AddScoped<IUsuariosRepository, UsuarioRepository>();
builder.Services.AddScoped<IVentasRepository, VentasRepository>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IArticulosRepository, ArticulosRepository>();
builder.Services.AddScoped<ICustomerServices, CustomerServices>();
builder.Services.AddScoped<IAutenticacionServices, AutenticacionServices>();
builder.Services.AddScoped<IVentasService, VentasService>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IArticulosService, ArticulosService>();
builder.Services.AddScoped<ICustomerModal, CustomerModalServices>();
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
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; // Para mantener los nombres de las propiedades como est�n en el modelo

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
