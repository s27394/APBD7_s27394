using APBD7.DTOs;
using APBD7.Models;

namespace APBD7.Interfaces;

public interface IDbService
{
    Task UpdateOrderFulfilledAt(int idOrder);
    Task<Product?> GetProduct(int id);
    Task<Warehouse?> GetWarehouse(int id);
    Task<Order?> GetOrder(int idProduct, int amount, DateTime date);
    Task<ProductWarehouse?> GetProductWarehouse(int orderId);
    Task<ProductWarehouse> AddProductToWarehouse(int requestIdWarehouse, int requestIdProduct, int orderIdOrder, int requestAmount, double productPrice, DateTime requestCreatedAt);
}