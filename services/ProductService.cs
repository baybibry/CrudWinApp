using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using crudApp.model;
using Microsoft.Data.SqlClient;
using DotNetEnv;

namespace crudapp.services;

public class ProductService
{
    private readonly string _connectionString;

    public ProductService()
    {
        Env.Load();

        _connectionString =
            $"Server={Environment.GetEnvironmentVariable("DB_SERVER")};" +
            $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
            $"User Id={Environment.GetEnvironmentVariable("DB_USER")};" +
            $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};" +
            "TrustServerCertificate=true;";
    }

    private SqlConnection GetConnection() => new SqlConnection(_connectionString);

    // ── Add ──────────────────────────────────────────────────────────────────
    public async Task AddProduct(ProductModel product)
    {
        product.CreatedAt  = DateTime.Now;
        product.UpdatedAt  = DateTime.Now;
        product.IsAvailable = true;

        using var conn = GetConnection();
        using var cmd  = new SqlCommand("sp_AddProduct", conn) { CommandType = CommandType.StoredProcedure };

        cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
        cmd.Parameters.AddWithValue("@Description", product.Description);
        cmd.Parameters.AddWithValue("@Price",       product.Price);
        cmd.Parameters.AddWithValue("@ConstPrice",  product.ConstPrice);
        cmd.Parameters.AddWithValue("@Discount",    product.Discount);
        cmd.Parameters.AddWithValue("@Quantity",    product.Quantity);
        cmd.Parameters.AddWithValue("@IsAvailable", product.IsAvailable);
        cmd.Parameters.AddWithValue("@CreatedAt",   product.CreatedAt);
        cmd.Parameters.AddWithValue("@UpdatedAt",   product.UpdatedAt);

        await conn.OpenAsync();
        var newId = await cmd.ExecuteScalarAsync();
        product.Id = Convert.ToInt32(newId);
    }

    // ── Get All ───────────────────────────────────────────────────────────────
    public async Task<List<ProductModel>> GetAllProducts()
    {
        var products = new List<ProductModel>();

        using var conn = GetConnection();
        using var cmd  = new SqlCommand("sp_GetAllProducts", conn) { CommandType = CommandType.StoredProcedure };

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
            products.Add(MapProduct(reader));

        return products;
    }

    // ── Get By Id ─────────────────────────────────────────────────────────────
    public async Task<ProductModel> GetProductById(int id)
    {
        using var conn = GetConnection();
        using var cmd  = new SqlCommand("sp_GetProductById", conn) { CommandType = CommandType.StoredProcedure };

        cmd.Parameters.AddWithValue("@Id", id);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
            return MapProduct(reader);

        throw new Exception("Product not found.");
    }

    // ── Update ────────────────────────────────────────────────────────────────
    public async Task UpdateProduct(int id, ProductModel updatedProduct)
    {
        using var conn = GetConnection();
        using var cmd  = new SqlCommand("sp_UpdateProduct", conn) { CommandType = CommandType.StoredProcedure };

        cmd.Parameters.AddWithValue("@Id",          id);
        cmd.Parameters.AddWithValue("@ProductName", updatedProduct.ProductName);
        cmd.Parameters.AddWithValue("@Description", updatedProduct.Description);
        cmd.Parameters.AddWithValue("@Price",       updatedProduct.Price);
        cmd.Parameters.AddWithValue("@ConstPrice",  updatedProduct.ConstPrice);
        cmd.Parameters.AddWithValue("@Discount",    updatedProduct.Discount);
        cmd.Parameters.AddWithValue("@Quantity",    updatedProduct.Quantity);
        cmd.Parameters.AddWithValue("@IsAvailable", updatedProduct.IsAvailable);
        cmd.Parameters.AddWithValue("@UpdatedAt",   DateTime.Now);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    // ── Delete ────────────────────────────────────────────────────────────────
    public async Task DeleteProduct(int id)
    {
        using var conn = GetConnection();
        using var cmd  = new SqlCommand("sp_DeleteProduct", conn) { CommandType = CommandType.StoredProcedure };

        cmd.Parameters.AddWithValue("@Id", id);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    // ── Helper ────────────────────────────────────────────────────────────────
    private static ProductModel MapProduct(SqlDataReader reader) => new ProductModel
    {
        Id          = reader.GetInt32("Id"),
        ProductName = reader.GetString("ProductName"),
        Description = reader.GetString("Description"),
        Price       = reader.GetDecimal("Price"),
        ConstPrice  = reader.GetDecimal("ConstPrice"),
        Discount    = reader.GetDecimal("Discount"),
        Quantity    = reader.GetInt64("Quantity"),
        IsAvailable = reader.GetBoolean("IsAvailable"),
        CreatedAt   = reader.GetDateTime("CreatedAt"),
        UpdatedAt   = reader.GetDateTime("UpdatedAt"),
    };
}