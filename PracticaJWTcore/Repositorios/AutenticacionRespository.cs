
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PracticaJWTcore.Dtos;
using PracticaJWTcore.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PracticaJWTcore.Repositorios
{
    // Repository de autenticacion: valida hashes de clave con PasswordHasher y consulta roles para generar JWT.
    public class AutenticacionRespository : IAutenticacionRepository
    {
        private readonly PracticaJWTcoreContext _context;
        // PasswordHasher<T>: utilitario de ASP.NET Core para generar y verificar hashes de contraseña
        // Implementa hashing seguro (PBKDF2) y comparación resistente a tiempo.
        private readonly PasswordHasher<Usuarios> _passwordHasher = new();
        private readonly string secretKey;
        private readonly string issuer;
        private readonly string audience;
        public AutenticacionRespository(PracticaJWTcoreContext context, IConfiguration configuration)
        {
            _context = context;
            this.secretKey = configuration["JWT:Key"]
                ?? throw new InvalidOperationException("Falta configurar JWT:Key.");
            this.issuer = configuration["JWT:Issuer"]
                ?? throw new InvalidOperationException("Falta configurar JWT:Issuer.");
            this.audience = configuration["JWT:Audience"]
                ?? throw new InvalidOperationException("Falta configurar JWT:Audience.");
        }

        private sealed class UsuarioPasswordInfo
        {
            public int? IdUsuario { get; set; }
            public string? Correo { get; set; }
            public string? Clave { get; set; }
            public int? RoleId { get; set; }
        }

        private async Task<UsuarioPasswordInfo?> GetUsuarioPasswordInfo(string? correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
                return null;

            var connection = _context.Database.GetDbConnection();
            // GetDbConnection: obtiene la conexión ADO.NET (DbConnection) que usa el DbContext.
            // Permite crear comandos de bajo nivel sin depender directamente de SqlClient.
            var shouldCloseConnection = connection.State != ConnectionState.Open;

            if (shouldCloseConnection)
                await connection.OpenAsync();

            try
            {
                // CreateCommand: crea un DbCommand agnóstico al proveedor (ej. SqlCommand para SQL Server)
                // Usar DbCommand permite trabajar con parámetros y ejecutar SQL de forma eficiente.
                await using var command = connection.CreateCommand();
                command.CommandText = @"
SELECT TOP (1) IdUsuario, correo, clave, RoleId
FROM dbo.Usuarios
WHERE correo = @correo";

                var correoParameter = command.CreateParameter();
                // CreateParameter: crea un DbParameter apropiado para el proveedor (evita string concatenation)
                correoParameter.ParameterName = "@correo";
                correoParameter.Value = correo;
                command.Parameters.Add(correoParameter);

                // ExecuteReaderAsync: ejecuta el comando y devuelve un DbDataReader de forma asíncrona
                await using var reader = await command.ExecuteReaderAsync();
                // ReadAsync avanza el lector al primer registro (si existe)
                if (!await reader.ReadAsync())
                    return null;

                return new UsuarioPasswordInfo
                {
                    IdUsuario = reader.IsDBNull(0) ? null : reader.GetInt32(0),
                    Correo = reader.IsDBNull(1) ? null : reader.GetString(1),
                    Clave = reader.IsDBNull(2) ? null : reader.GetString(2),
                    RoleId = reader.IsDBNull(3) ? null : reader.GetInt32(3)
                };
            }
            finally
            {
                if (shouldCloseConnection)
                    await connection.CloseAsync();
            }
        }

        public async Task<TokenRolDTO> GenerateToken(UsuarioLogin usuario, int result,int rolId,string rolName)
        {

            // El JWT incluye rol y correo para que Authorization pueda evaluar identidad/rol.
            byte[] token;
            string TokenCreado = "";
            if (result == 1)
            {
                token = Encoding.ASCII.GetBytes(secretKey);
                // ClaimsIdentity: representa una identidad con claims que se incluirán en el JWT
                var claims = new ClaimsIdentity();
                claims.AddClaim(new Claim(ClaimTypes.Role, rolName));
                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.correo));
                claims.AddClaim(new Claim("roleId", rolId.ToString()));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddHours(1),
                    Issuer = issuer,
                    Audience = audience,
                    // SigningCredentials + SymmetricSecurityKey: configura la clave y algoritmo para firmar el JWT
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(token), SecurityAlgorithms.HmacSha256Signature)
                };
                // JwtSecurityTokenHandler: clase que crea y serializa tokens JWT
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);
                var tokenCread = tokenHandler.WriteToken(tokenConfig);
                TokenRolDTO tokenRol = new TokenRolDTO
                {
                    Token = tokenCread,
                    RolId = rolId
                };
                return tokenRol;
            }
            else
            {
                 TokenRolDTO tokenVacio= new TokenRolDTO
                {
                    Token = "",
                    RolId = 0
                };
                return tokenVacio;
             }
        }



        public async Task<TokenRolDTO> Login(UsuarioLogin usuario)
        {

            string rolName = string.Empty;
            int roleId = 0;
            int result = 0;

            var usuarioEntity = await GetUsuarioPasswordInfo(usuario.correo);

            if (usuarioEntity?.Clave != null && !string.IsNullOrWhiteSpace(usuario.clave))
            {
                // PasswordHasher verifica el hash almacenado en SQL Server; la clave en texto nunca se guarda.
                var passwordUser = new Usuarios
                {
                    IdUsuario = usuarioEntity.IdUsuario,
                    correo = usuarioEntity.Correo,
                    RoleId = usuarioEntity.RoleId ?? 0
                };
                // VerifyHashedPassword: compara el hash almacenado con la contraseña en texto
                // Retorna PasswordVerificationResult (Success, Failed, etc.)
                var verification = _passwordHasher.VerifyHashedPassword(passwordUser, usuarioEntity.Clave, usuario.clave);
                if (verification != PasswordVerificationResult.Failed)
                {
                    result = 1;
                    roleId = usuarioEntity.RoleId ?? 0;
                    rolName = await _context.Roles.Where(r => r.RoleId == roleId)
                        .Select(r => r.RoleName)
                        .FirstOrDefaultAsync() ?? string.Empty;
                }
            }

            return await GenerateToken(usuario, result, roleId, rolName);
            
        }
        public async Task<int> CambioClave(CambioClave cambios)
        {
            if (string.IsNullOrWhiteSpace(cambios.claveActual) || string.IsNullOrWhiteSpace(cambios.nuevaClave))
                return 0;

            var usuarioInfo = await GetUsuarioPasswordInfo(cambios.correo);
            if (usuarioInfo?.Clave == null || usuarioInfo.IdUsuario == null)
                return 0;

            // Primero valida la clave actual antes de reemplazar el hash almacenado.
            var passwordUser = new Usuarios
            {
                IdUsuario = usuarioInfo.IdUsuario,
                correo = usuarioInfo.Correo,
                RoleId = usuarioInfo.RoleId ?? 0
            };
            var verification = _passwordHasher.VerifyHashedPassword(passwordUser, usuarioInfo.Clave, cambios.claveActual);
            if (verification == PasswordVerificationResult.Failed)
                return 0;

            var nuevaClaveHash = _passwordHasher.HashPassword(passwordUser, cambios.nuevaClave);
            // ExecuteSqlInterpolatedAsync: ejecuta SQL interpolado parametrizado desde EF Core
            // Protege contra SQL injection al parametrizar automáticamente los valores interpolados.
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE dbo.Usuarios SET clave = {nuevaClaveHash} WHERE IdUsuario = {usuarioInfo.IdUsuario.Value}");
            return 1;

        }

     
    }
}
