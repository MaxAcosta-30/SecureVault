# SecureVault API - Sistema de Gestión de Secretos Seguro

Una API RESTful construida con .NET 10 que implementa cifrado AES-256 para proteger datos sensibles y autenticación JWT para garantizar acceso seguro a los recursos. Este sistema permite almacenar y recuperar secretos de forma segura mediante técnicas avanzadas de criptografía.

## 🚀 Descripción

SecureVault es una aplicación de gestión de secretos que proporciona un almacenamiento seguro para información sensible. Cada secreto se cifra utilizando el algoritmo AES-256 antes de ser almacenado en la base de datos, garantizando que incluso si alguien accede a los datos en bruto, no podrá leerlos sin la clave de cifrado correspondiente.

La API implementa autenticación basada en tokens JWT (JSON Web Tokens), asegurando que solo los usuarios autenticados puedan crear y acceder a los secretos almacenados.

## 🛠️ Tecnologías

- **.NET 10** - Framework y runtime
- **Entity Framework Core** - ORM para acceso a datos
- **SQL Server** - Base de datos relacional
- **Swagger/OpenAPI** - Documentación interactiva de la API
- **AES-256** - Algoritmo de cifrado simétrico
- **JWT (JSON Web Tokens)** - Sistema de autenticación y autorización
- **ASP.NET Core** - Framework web para construir APIs REST

## 📦 Instalación y Configuración

### Requisitos Previos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) o SQL Server LocalDB
- [Visual Studio 2022](https://visualstudio.microsoft.com/) o [Visual Studio Code](https://code.visualstudio.com/)

### Pasos de Instalación

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/[TU-USUARIO]/SecureVault.git
   cd SecureVault
   ```
   
   > ⚠️ **Nota:** Reemplaza `[TU-USUARIO]` con tu nombre de usuario de GitHub.

2. **Restaurar dependencias NuGet**
   ```bash
   dotnet restore
   ```

3. **Configurar la base de datos**
   
   Asegúrate de tener SQL Server ejecutándose. Luego, ejecuta las migraciones:
   
   ```bash
   cd SecureVault.Api
   dotnet ef database update
   ```
   
   Esto creará la base de datos `SecureVaultDb` con todas las tablas necesarias.

4. **Configurar appsettings.json**
   
   **IMPORTANTE**: El archivo `appsettings.json` no se incluye en el repositorio por razones de seguridad. Debes crearlo manualmente en la carpeta `SecureVault.Api/` con el siguiente contenido:

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
       "Key": "TuClaveDe32CaracteresParaAES256!"
     },
     "Jwt": {
       "Key": "TuClaveSecretaSuperLargaParaFirmarTokensJWT_Minimo32Caracteres!",
       "Issuer": "SecureVaultApi",
       "Audience": "SecureVaultUsers"
     },
     "Auth": {
       "DemoUsername": "admin",
       "DemoPassword": "12345"
     }
   }
   ```

   **Notas importantes**:
   - `Encryption:Key`: Debe tener **exactamente 32 caracteres** para AES-256. Esta clave se usa para cifrar los secretos.
   - `Jwt:Key`: Clave para firmar los tokens JWT. Debe ser segura y única.
   - `Auth:DemoUsername` y `Auth:DemoPassword`: Credenciales de demostración (solo para desarrollo/demo).
   - Ajusta la cadena de conexión según tu configuración de SQL Server.

5. **Ejecutar la aplicación**
   ```bash
   dotnet run --project SecureVault.Api
   ```

   La API estará disponible en:
   - HTTP: `http://localhost:5000`
   - HTTPS: `https://localhost:5001`
   - Swagger UI: `https://localhost:5001/swagger`

## 🔐 Nota de Seguridad

Por razones de seguridad, los archivos `appsettings.json` y `appsettings.Development.json` **NO** están incluidos en el repositorio, ya que contienen información sensible como:

- Claves de cifrado JWT
- Cadenas de conexión a la base de datos
- Configuraciones específicas del entorno

**Es fundamental** que crees estos archivos localmente siguiendo la estructura mostrada en la sección de instalación. En un entorno de producción, se recomienda utilizar:

- Variables de entorno
- Azure Key Vault
- AWS Secrets Manager
- Otros servicios de gestión de secretos

## 📡 Endpoints

### Autenticación

#### `POST /api/auth/login`
Autentica un usuario y devuelve un token JWT.

**Body:**
```json
{
  "username": "admin",
  "password": "12345"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expires": "2024-12-15T12:30:00Z"
}
```

### Secretos

**Todos los endpoints de secretos requieren autenticación JWT.**

Agrega el token en el header de la petición:
```
Authorization: Bearer {tu_token_jwt}
```

#### `POST /api/secrets`
Crea un nuevo secreto cifrado.

**Body:**
```json
{
  "name": "API Key de GitHub",
  "value": "ghp_xxxxxxxxxxxxxxxxxxxx"
}
```

**Response:**
```json
{
  "message": "Secreto guardado exitosamente (¡y cifrado!)",
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

#### `GET /api/secrets/{id}`
Obtiene un secreto por su ID y lo devuelve descifrado.

**Response:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "API Key de GitHub",
  "decryptedValue": "ghp_xxxxxxxxxxxxxxxxxxxx",
  "createdAt": "2024-12-15T10:00:00Z"
}
```

## 📚 Documentación Adicional

Puedes explorar y probar todos los endpoints utilizando la interfaz Swagger UI disponible en `/swagger` cuando la aplicación está en modo desarrollo.

## 🤝 Contribuciones

Las contribuciones son bienvenidas. Por favor:

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📄 Licencia

Este proyecto es de código abierto y está disponible bajo la licencia MIT.

## 👤 Autor

**Max Acosta** - [@MaxAcosta-30](https://github.com/MaxAcosta-30)

---

⭐ Si este proyecto te resultó útil, ¡dale una estrella!

