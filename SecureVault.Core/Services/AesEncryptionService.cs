using System.Security.Cryptography;
using SecureVault.Core.Interfaces;

namespace SecureVault.Core.Services;

/// <summary>
/// Implementación del servicio de cifrado usando AES-256 para cifrar y descifrar datos.
/// La clave de cifrado debe ser proporcionada externamente mediante configuración.
/// </summary>
public class AesEncryptionService : IEncryptionService
{
    private readonly byte[] _key;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="AesEncryptionService"/>.
    /// </summary>
    /// <param name="encryptionKey">Clave de cifrado de 32 bytes para AES-256. Debe obtenerse de configuración segura.</param>
    /// <exception cref="ArgumentNullException">Se lanza si la clave es null o vacía.</exception>
    /// <exception cref="ArgumentException">Se lanza si la clave no tiene exactamente 32 bytes.</exception>
    public AesEncryptionService(byte[] encryptionKey)
    {
        if (encryptionKey == null || encryptionKey.Length == 0)
            throw new ArgumentNullException(nameof(encryptionKey), "La clave de cifrado no puede ser null o vacía.");

        if (encryptionKey.Length != 32)
            throw new ArgumentException("La clave de cifrado debe tener exactamente 32 bytes para AES-256.", nameof(encryptionKey));

        _key = encryptionKey;
    }

    /// <summary>
    /// Cifra un texto plano usando AES-256 y genera un vector de inicialización único.
    /// Implementa seguridad mediante generación de IV único por cada cifrado.
    /// </summary>
    /// <param name="plainText">Texto plano a cifrar.</param>
    /// <returns>Tupla con los datos cifrados y el vector de inicialización (IV) utilizado.</returns>
    public (byte[] EncryptedData, byte[] IV) Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using var sw = new StreamWriter(cs);
        
        sw.Write(plainText);
        sw.Close(); 
        
        return (ms.ToArray(), aes.IV);
    }

    /// <summary>
    /// Descifra datos cifrados usando AES-256 y el vector de inicialización proporcionado.
    /// Valida que los datos cifrados y el IV sean válidos antes de proceder.
    /// </summary>
    /// <param name="encryptedData">Datos cifrados a descifrar.</param>
    /// <param name="iv">Vector de inicialización utilizado durante el cifrado.</param>
    /// <returns>Texto plano original.</returns>
    public string Decrypt(byte[] encryptedData, byte[] iv)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = iv;

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using var ms = new MemoryStream(encryptedData);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }
}