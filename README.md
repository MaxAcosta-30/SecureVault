# SecureVault API

Sistema de gestión de secretos empresarial con cifrado AES-256 y autenticación basada en tokens JWT. Implementado sobre .NET 10 con arquitectura RESTful para almacenamiento seguro de información sensible.

## General Overview

SecureVault proporciona un servicio de almacenamiento cifrado para datos sensibles mediante criptografía simétrica AES-256. Cada secreto se cifra antes de persistir en la base de datos, garantizando confidencialidad incluso ante acceso no autorizado a los datos almacenados.

La autenticación se implementa mediante JSON Web Tokens (JWT), restringiendo el acceso a recursos protegidos únicamente a usuarios autenticados. El sistema valida la identidad del usuario y emite tokens con tiempo de expiración configurable.

## Tech Stack

| Componente | Tecnología | Versión |
|------------|------------|---------|
| Runtime | .NET | 10.0 |
| Framework Web | ASP.NET Core | 10.0 |
| ORM | Entity Framework Core | 10.0 |
| Base de Datos | SQL Server | - |
| Documentación API | Swagger/OpenAPI | 3.0 |
| Cifrado | AES-256 | - |
| Autenticación | JWT | - |

## Security Specs

### Cifrado de Datos

- **Algoritmo**: AES-256 (Advanced Encryption Standard)
- **Modo**: CBC (Cipher Block Chaining)
- **Longitud de clave**: 256 bits (32 bytes)
- **Vector de inicialización**: Generado aleatoriamente por operación

Los secretos se cifran en el momento de almacenamiento y se descifran únicamente durante la recuperación autorizada. La clave de cifrado debe mantenerse fuera del control de versiones y gestionarse mediante servicios de gestión de secretos en entornos de producción.

### Autenticación y Autorización

- **Protocolo**: JWT (JSON Web Tokens)
- **Algoritmo de firma**: HMAC-SHA256
- **Validación**: Issuer, Audience y expiración temporal
- **Transporte**: Header Authorization Bearer Token

Los tokens JWT incluyen claims de identidad y expiración. La validación se realiza en cada solicitud a endpoints protegidos mediante middleware de autenticación ASP.NET Core.

### Gestión de Secretos

La configuración de aplicación contiene información sensible que requiere manejo estricto:

- Claves de cifrado AES-256
- Claves de firma JWT
- Cadenas de conexión a base de datos
- Credenciales de autenticación

**Requisitos de seguridad**:

- El archivo `appsettings.json` está excluido del control de versiones mediante `.gitignore`
- En producción, utilizar servicios de gestión de secretos (Azure Key Vault, AWS Secrets Manager, HashiCorp Vault)
- Rotación periódica de claves de cifrado y firma JWT
- Acceso restringido a archivos de configuración mediante permisos del sistema operativo

## Local Setup

### Requisitos Previos

- .NET 10 SDK instalado y configurado
- SQL Server o SQL Server LocalDB en ejecución
- Editor de código o IDE compatible con .NET

### Procedimiento de Instalación

1. Clonar el repositorio:
   ```bash
   git clone <repository-url>
   cd SecureVault
   ```

2. Restaurar dependencias NuGet:
   ```bash
   dotnet restore
   ```

3. Aplicar migraciones de base de datos:
   ```bash
   cd SecureVault.Api
   dotnet ef database update
   ```

4. Configurar archivo de configuración de aplicación.

### Configuración de Secretos

**ADVERTENCIA**: La configuración de secretos es un procedimiento crítico de seguridad. El archivo `appsettings.json` debe crearse manualmente en el directorio `SecureVault.Api/` con la siguiente estructura:

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
    "Key": "BASE64_ENCODED_32_BYTE_KEY_HERE"
  },
  "Jwt": {
    "Key": "EstaEsUnaClaveSuperSecretaParaFirmarTokensJWT_TieneQueSerLarga!",
    "Issuer": "SecureVaultApi",
    "Audience": "SecureVaultUsers",
    "ExpirationMinutes": 60
  },
  "Auth": {
    "DemoUsername": "admin",
    "DemoPasswordHash": "BCRYPT_HASH_HERE"
  }
}
```

**Especificaciones de configuración**:

| Parámetro | Requisito | Descripción |
|-----------|-----------|-------------|
| `Encryption:Key` | Cadena Base64 de exactamente 32 bytes (44 caracteres con padding) | Clave de cifrado AES-256 en formato Base64. **IMPORTANTE**: Debe ser Base64, no texto plano, para evitar problemas de encoding con caracteres especiales. |
| `Jwt:Key` | Mínimo 32 caracteres | Clave de firma para tokens JWT. Debe ser única y de alta entropía. |
| `Jwt:Issuer` | String no vacío | Identificador del emisor de tokens. |
| `Jwt:Audience` | String no vacío | Identificador de la audiencia esperada. |
| `Auth:DemoPasswordHash` | Hash BCrypt válido | Hash BCrypt de la contraseña del usuario demo. **IMPORTANTE**: Ya no se usa `DemoPassword` en texto plano por seguridad. |
| `ConnectionStrings:DefaultConnection` | Cadena de conexión válida | Cadena de conexión a SQL Server. Ajustar según entorno. |

**Procedimiento de generación de claves**:

1. **Generar clave de encriptación (Base64)**:
   ```csharp
   // Ejecutar en C# Interactive o crear un script temporal
   using System.Security.Cryptography;
   var key = new byte[32];
   RandomNumberGenerator.Fill(key);
   Console.WriteLine(Convert.ToBase64String(key));
   ```

2. **Generar hash BCrypt de contraseña**:
   ```csharp
   // Ejecutar en C# Interactive (requiere BCrypt.Net-Next)
   using BCrypt.Net;
   var hash = BCrypt.HashPassword("tu_contraseña_aqui", workFactor: 12);
   Console.WriteLine(hash);
   ```

   O usar una herramienta en línea como: https://bcrypt-generator.com/

3. **Seguridad**:
   - No reutilizar claves entre entornos (desarrollo, staging, producción)
   - Documentar el proceso de rotación de claves en procedimientos operativos
   - Mantener las claves fuera del control de versiones

5. Ejecutar la aplicación:
   ```bash
   dotnet run --project SecureVault.Api
   ```

La API estará disponible en los siguientes endpoints:

- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

## API Reference

### Autenticación

#### POST /api/auth/login

Autentica un usuario y devuelve un token JWT válido.

**Request Body**:
```json
{
  "username": "string",
  "password": "string"
}
```

**Response 200 OK**:
```json
{
  "token": "string",
  "expires": "2024-12-15T12:30:00Z"
}
```

**Response 401 Unauthorized**: Credenciales inválidas.

### Secretos

Todos los endpoints de secretos requieren autenticación mediante token JWT en el header `Authorization`:

```
Authorization: Bearer {token}
```

#### POST /api/secrets

Crea un nuevo secreto cifrado en el almacenamiento.

**Request Body**:
```json
{
  "name": "string",
  "value": "string"
}
```

**Response 201 Created**:
```json
{
  "message": "Secreto guardado exitosamente",
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**Response 400 Bad Request**: Datos de entrada inválidos.

**Response 401 Unauthorized**: Token JWT ausente o inválido.

#### GET /api/secrets/{id}

Recupera un secreto por identificador y lo devuelve descifrado.

**Path Parameters**:
- `id` (UUID): Identificador único del secreto.

**Response 200 OK**:
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "string",
  "decryptedValue": "string",
  "createdAt": "2024-12-15T10:00:00Z"
}
```

**Response 404 Not Found**: Secreto no encontrado.

**Response 401 Unauthorized**: Token JWT ausente o inválido.

### Documentación Interactiva

La documentación completa de la API está disponible mediante Swagger UI en el endpoint `/swagger` cuando la aplicación se ejecuta en modo desarrollo.

## Licencia

Este proyecto está disponible bajo la licencia MIT.
