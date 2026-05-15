using Microsoft.EntityFrameworkCore;
using crudApp.model;

namespace crudapp.Config;

public class DatabaseConnection : DbContext
{
    public DbSet<ProductModel> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            $"Server={Environment.GetEnvironmentVariable("DB_SERVER")};" +
            $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
            $"User Id={Environment.GetEnvironmentVariable("DB_USER")};" +
            $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};" +
            "TrustServerCertificate=true;"
            );
    }
}