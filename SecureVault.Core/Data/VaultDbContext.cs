using Microsoft.EntityFrameworkCore;
using SecureVault.Core.Entities;

namespace SecureVault.Core.Data;

/// <summary>
/// Contexto de Entity Framework Core para acceder a la base de datos del sistema de secretos.
/// </summary>
public class VaultDbContext : DbContext
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="VaultDbContext"/>.
    /// </summary>
    /// <param name="options">Opciones de configuración del contexto (incluye cadena de conexión).</param>
    public VaultDbContext(DbContextOptions<VaultDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Conjunto de entidades que representa la tabla de secretos cifrados.
    /// </summary>
    public DbSet<Secret> Secrets { get; set; }
}