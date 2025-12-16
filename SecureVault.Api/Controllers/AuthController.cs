using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SecureVault.Api.DTOs;

namespace SecureVault.Api.Controllers;

/// <summary>
/// Controlador que maneja la autenticación de usuarios mediante JWT.
/// NOTA: Esta implementación utiliza credenciales de demostración configuradas en appsettings.json.
/// En producción, debe implementarse autenticación real con base de datos y hash de contraseñas.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController(IConfiguration config) : ControllerBase
{
    private readonly IConfiguration _config = config;

    /// <summary>
    /// Autentica un usuario y genera un token JWT válido por 10 minutos.
    /// Valida las credenciales contra configuración (solo para demo) y genera un token firmado con la clave JWT configurada.
    /// </summary>
    /// <param name="request">Credenciales de acceso (usuario y contraseña).</param>
    /// <returns>Token JWT y fecha de expiración si las credenciales son válidas, o Unauthorized si son incorrectas.</returns>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var demoUsername = _config["Auth:DemoUsername"]
            ?? throw new InvalidOperationException("La configuración 'Auth:DemoUsername' no está configurada. Debe configurarse en appsettings.json.");

        var demoPassword = _config["Auth:DemoPassword"]
            ?? throw new InvalidOperationException("La configuración 'Auth:DemoPassword' no está configurada. Debe configurarse en appsettings.json.");

        if (request.Username != demoUsername || request.Password != demoPassword)
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