namespace SecureVault.Core.Entities;

/// <summary>
/// Representa un secreto almacenado de forma cifrada en la base de datos.
/// </summary>
public class Secret
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
    /// Valor del secreto cifrado usando AES-256.
    /// </summary>
    public byte[] EncryptedValue { get; set; } = [];

    /// <summary>
    /// Vector de inicialización (IV) utilizado durante el cifrado.
    /// </summary>
    public byte[] InitializationVector { get; set; } = [];

    /// <summary>
    /// Fecha y hora de creación del secreto.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}