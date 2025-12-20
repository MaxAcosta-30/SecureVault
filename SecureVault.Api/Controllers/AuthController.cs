using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SecureVault.Api.DTOs;
using BCrypt.Net;

namespace SecureVault.Api.Controllers;

/// <summary>
/// Controlador que maneja la autenticación de usuarios mediante JWT.
/// Utiliza hash BCrypt para almacenar contraseñas de forma segura.
/// NOTA: Esta implementación utiliza credenciales de demostración configuradas en appsettings.json.
/// En producción, debe implementarse autenticación real con base de datos.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController(IConfiguration config) : ControllerBase
{
    private readonly IConfiguration _config = config;

    /// <summary>
    /// Autentica un usuario y genera un token JWT válido por 10 minutos.
    /// Valida las credenciales contra configuración usando hash BCrypt para comparar contraseñas de forma segura.
    /// </summary>
    /// <param name="request">Credenciales de acceso (usuario y contraseña).</param>
    /// <returns>Token JWT y fecha de expiración si las credenciales son válidas, o Unauthorized si son incorrectas.</returns>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var demoUsername = _config["Auth:DemoUsername"]
            ?? throw new InvalidOperationException("La configuración 'Auth:DemoUsername' no está configurada. Debe configurarse en appsettings.json.");

        // La contraseña en appsettings.json debe estar hasheada con BCrypt
        var demoPasswordHash = _config["Auth:DemoPasswordHash"]
            ?? throw new InvalidOperationException("La configuración 'Auth:DemoPasswordHash' no está configurada. Debe configurarse en appsettings.json como un hash BCrypt de la contraseña.");

        // Validar usuario
        if (request.Username != demoUsername)
        {
            return Unauthorized("Credenciales incorrectas.");
        }

        // Validar contraseña usando BCrypt (comparación segura contra timing attacks)
        if (!BCrypt.Net.BCrypt.Verify(request.Password, demoPasswordHash))
        {
            return Unauthorized("Credenciales incorrectas.");
        }

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, request.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: creds
        );

        return Ok(new { 
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expires = token.ValidTo
        });
    }
}