using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureVault.Api.DTOs;
using SecureVault.Core.Data;
using SecureVault.Core.Entities;
using SecureVault.Core.Interfaces;

namespace SecureVault.Api.Controllers;

/// <summary>
/// Controlador que gestiona los secretos almacenados de forma segura mediante cifrado AES-256.
/// Todos los endpoints requieren autenticación JWT válida mediante el atributo [Authorize].
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SecretsController(VaultDbContext context, IEncryptionService encryptionService) : ControllerBase
{
    private readonly VaultDbContext _context = context;
    private readonly IEncryptionService _encryptionService = encryptionService;

    /// <summary>
    /// Crea un nuevo secreto cifrado y lo almacena en la base de datos.
    /// Requiere un token JWT válido. El valor del secreto se cifra usando AES-256 antes de almacenarse.
    /// </summary>
    /// <param name="request">Datos del secreto a crear (nombre y valor en texto plano).</param>
    /// <returns>Confirmación de éxito con el ID del secreto creado.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateSecret([FromBody] CreateSecretRequest request)
    {
        var (encryptedData, iv) = _encryptionService.Encrypt(request.Value);

        var secret = new Secret
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            EncryptedValue = encryptedData,
            InitializationVector = iv,
            CreatedAt = DateTime.UtcNow
        };

        _context.Secrets.Add(secret);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Secreto guardado exitosamente (¡y cifrado!)", Id = secret.Id });
    }

    /// <summary>
    /// Obtiene un secreto por su ID y lo devuelve descifrado.
    /// Requiere un token JWT válido. El secreto se descifra usando AES-256 antes de ser devuelto.
    /// </summary>
    /// <param name="id">Identificador único del secreto.</param>
    /// <returns>El secreto descifrado si existe, o NotFound si no se encuentra.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSecret(Guid id)
    {
        var secret = await _context.Secrets.FindAsync(id);

        if (secret == null)
            return NotFound("No se encontró el secreto.");

        var decryptedValue = _encryptionService.Decrypt(secret.EncryptedValue, secret.InitializationVector);

        var response = new SecretResponse
        {
            Id = secret.Id,
            Name = secret.Name,
            DecryptedValue = decryptedValue,
            CreatedAt = secret.CreatedAt
        };

        return Ok(response);
    }
}