# Generación de Secretos para SecureVault

Este documento explica cómo generar los secretos necesarios para configurar SecureVault.

## 1. Generar Clave de Encriptación (Base64)

La clave de encriptación debe ser exactamente 32 bytes y estar codificada en Base64.

### Opción A: Usando PowerShell

```powershell
# Generar 32 bytes aleatorios y convertir a Base64
$key = New-Object byte[] 32
[System.Security.Cryptography.RandomNumberGenerator]::Fill($key)
[Convert]::ToBase64String($key)
```

### Opción B: Usando C# Interactive (dotnet-script)

```csharp
using System.Security.Cryptography;
var key = new byte[32];
RandomNumberGenerator.Fill(key);
Console.WriteLine(Convert.ToBase64String(key));
```

### Opción C: Usando OpenSSL (si está instalado)

```bash
openssl rand -base64 32
```

## 2. Generar Hash BCrypt de Contraseña

La contraseña debe almacenarse como hash BCrypt, no en texto plano.

### Opción A: Usando C# Interactive (requiere BCrypt.Net-Next)

Primero instala el paquete:
```bash
dotnet add SecureVault.Api package BCrypt.Net-Next
```

Luego ejecuta:
```csharp
using BCrypt.Net;
var hash = BCrypt.HashPassword("tu_contraseña_aqui", workFactor: 12);
Console.WriteLine(hash);
```

### Opción B: Usando herramienta en línea

Visita: https://bcrypt-generator.com/

Ingresa tu contraseña y copia el hash generado.

### Opción C: Script rápido en C#

Crea un archivo temporal `GenerateHash.cs`:

```csharp
using BCrypt.Net;
Console.WriteLine("Ingresa la contraseña:");
var password = Console.ReadLine() ?? "";
var hash = BCrypt.HashPassword(password, workFactor: 12);
Console.WriteLine($"Hash BCrypt: {hash}");
```

Ejecuta:
```bash
dotnet script GenerateHash.cs
```

## 3. Ejemplo de appsettings.json

Una vez generados los valores, tu `appsettings.json` debería verse así:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=SecureVaultDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Encryption": {
    "Key": "TU_CLAVE_BASE64_AQUI_DEBE_SER_44_CARACTERES"
  },
  "Jwt": {
    "Key": "EstaEsUnaClaveSuperSecretaParaFirmarTokensJWT_TieneQueSerLarga!",
    "Issuer": "SecureVaultApi",
    "Audience": "SecureVaultUsers",
    "ExpirationMinutes": 60
  },
  "Auth": {
    "DemoUsername": "admin",
    "DemoPasswordHash": "$2a$12$TU_HASH_BCRYPT_AQUI_DEBE_SER_LARGO"
  }
}
```

## Notas Importantes

- **NUNCA** commits el archivo `appsettings.json` con valores reales al repositorio
- Genera claves diferentes para cada entorno (desarrollo, staging, producción)
- La clave de encriptación en Base64 debe decodificar a exactamente 32 bytes
- El hash BCrypt incluye el salt automáticamente, no necesitas almacenarlo por separado
- Usa un workFactor de 12 o superior para BCrypt en producción

