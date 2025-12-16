namespace SecureVault.Core.Interfaces;

/// <summary>
/// Interfaz que define las operaciones de cifrado y descifrado de datos.
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// Cifra un texto plano y devuelve los datos cifrados junto con el vector de inicialización.
    /// </summary>
    /// <param name="plainText">Texto plano a cifrar.</param>
    /// <returns>Tupla con los datos cifrados y el vector de inicialización (IV).</returns>
    (byte[] EncryptedData, byte[] IV) Encrypt(string plainText);

    /// <summary>
    /// Descifra datos cifrados utilizando el vector de inicialización proporcionado.
    /// </summary>
    /// <param name="encryptedData">Datos cifrados a descifrar.</param>
    /// <param name="iv">Vector de inicialización utilizado durante el cifrado.</param>
    /// <returns>Texto plano original.</returns>
    string Decrypt(byte[] encryptedData, byte[] iv);
}