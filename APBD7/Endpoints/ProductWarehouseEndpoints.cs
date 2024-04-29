using APBD7.DTOs;
using APBD7.Interfaces;
using APBD7.Models;

namespace APBD7.Endpoints;

using System.Data.SqlClient;
using Dapper;
using FluentValidation;
using APBD7.Services;


public static class ProductWarehouseEndpoints
{
    public static void RegisterProductWarehouseEndpoints(this WebApplication app)
    {
        var warehouse = app.MapGroup("warehouse");
        warehouse.MapPost("", AddProductToWarehouse);
    }

    private static async Task<IResult> AddProductToWarehouse(
        ProductWarehouseDTO request,
        IDbService db,
        IValidator<ProductWarehouseDTO> validator
    )
    {
        var validate = await validator.ValidateAsync(request);
        if (!validate.IsValid)
        {
            return Results.ValidationProblem(validate.ToDictionary());
        }

        // 1. checking if product and warehouse exists
        var product = await db.GetProduct(request.IdProduct);
        if (product is null)
        {
            return Results.NotFound("This product does not exist in Db.");
        }
        var warehouse = await db.GetWarehouse(request.IdWarehouse);
        if (warehouse is null)
        {
            return Results.NotFound("This warehouse does not exist in Db.");
        }
        var order = await db.GetOrder(request.IdProduct, request.Amount, request.CreatedAt);
        if (order is null)
        {
            return Results.NotFound("This order does not exist in Db.");

        }
        if (request.Amount < 1)
        {
            return Results.BadRequest("Amount can't be less than 1.");
        }
        await db.UpdateOrderFulfilledAt(order.IdOrder);

        // 3. checking if order is not already fulfilled
        var productWarehouseFromDb = await db.GetProductWarehouse(order.IdOrder);
        if (productWarehouseFromDb is not null)
        {
            return Results.Conflict("This order is already fulfilled.");
        }

        await db.UpdateOrderFulfilledAt(order.IdOrder);


        var result = await db.AddProductToWarehouse(request.IdWarehouse, request.IdProduct, order.IdOrder, request.Amount, product.Price*request.Amount, request.CreatedAt);

        return Results.Created($"productWarehouse/{result.IdProductWarehouse}", new ProductWarehouseDTO(result));
    }
}