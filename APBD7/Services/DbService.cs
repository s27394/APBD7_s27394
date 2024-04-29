using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Transactions;
using APBD7.DTOs;
using APBD7.Models;
using Dapper;

namespace APBD7.Services;

using APBD7.Interfaces;

public class DbService : IDbService
{
    private readonly IConfiguration _configuration;

    public DbService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Helper method for creating and opening connection
    private async Task<SqlConnection> GetConnection()
    {
        var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        return connection;
    }
    public async Task UpdateOrderFulfilledAt(int idOrder)
    {
        await using var connection = await GetConnection();
        await connection.ExecuteAsync("UPDATE [Order] SET FulfilledAt = @date WHERE IdOrder = @idOrder", new {date = DateTime.Now, idOrder});
    }
    public async Task<Product?> GetProduct(int id)
    {
        await using var connection = await GetConnection();
        return await connection.QueryFirstOrDefaultAsync<Product>("SELECT * FROM Product WHERE IdProduct = @id", new {id});
    }

    public async Task<Warehouse?> GetWarehouse(int id)
    {
        await using var connection = await GetConnection();
        return await connection.QueryFirstOrDefaultAsync<Warehouse>("SELECT * FROM Warehouse WHERE IdWarehouse = @id", new {id});
    }
    public async Task<Order?> GetOrder(int idProduct, int amount, DateTime date)
    {
        await using var connection = await GetConnection();
        var result = await connection.QueryFirstOrDefaultAsync<Order>("SELECT * FROM [Order] WHERE ((IdProduct = @idProduct) AND (Amount = @amount) AND (CreatedAt <= @date))", new {idProduct, amount, date});
        return result;
    }

    public async Task<ProductWarehouse?> GetProductWarehouse(int orderId)
    {
        await using var connection = await GetConnection();
        return await connection.QueryFirstOrDefaultAsync<ProductWarehouse>("SELECT * FROM [Product_Warehouse] where IdOrder = @orderId", new { orderId });
    }

    public async Task<ProductWarehouse> AddProductToWarehouse(int requestIdWarehouse, int requestIdProduct, int orderIdOrder, int requestAmount,
        double productPrice, DateTime requestCreatedAt)
    {
        // Generate a new product-warehouse and get his ID
        await using var connection = await GetConnection();
        var idProductWarehouse = await connection.ExecuteScalarAsync<int>(
            @"Insert into PRODUCT_WAREHOUSE values (@w_id, @p_id, @o_id, @amount, @price, @created_at); 
                        select cast(scope_identity() as int)",
            new
            {
                w_id = requestIdWarehouse,
                p_id = requestIdProduct,
                o_id = orderIdOrder,
                amount = requestAmount,
                price = productPrice,
                created_at = requestCreatedAt
            }
        );
        return await GetProductWarehouse(orderIdOrder);
    }
}