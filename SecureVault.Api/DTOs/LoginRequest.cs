namespace SecureVault.Api.DTOs;

/// <summary>
/// Representa las credenciales de autenticación de un usuario.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Nombre de usuario.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña del usuario.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}