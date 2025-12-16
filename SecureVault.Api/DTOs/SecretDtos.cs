namespace SecureVault.Api.DTOs;

/// <summary>
/// Representa la solicitud para crear un nuevo secreto.
/// </summary>
public class CreateSecretRequest
{
    /// <summary>
    /// Nombre identificador del secreto.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Valor del secreto en texto plano que será cifrado antes de almacenarse.
    /// </summary>
    public string Value { get; set; } = string.Empty;
}

/// <summary>
/// Representa la respuesta con un secreto descifrado.
/// </summary>
public class SecretResponse
{
    /// <summary>
    /// Identificador único del secreto.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nombre identificador del secreto.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Valor del secreto descifrado.
    /// </summary>
    public string DecryptedValue { get; set; } = string.Empty;

    /// <summary>
    /// Fecha y hora de creación del secreto.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}