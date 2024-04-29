using APBD7.Models;

namespace APBD7.DTOs;

public record ProductWarehouseDTO
{
    public int IdProduct { get; set; }
    public int IdWarehouse { get; set; }
    public int Amount { get; set; }
    public DateTime CreatedAt { get; set; }

    public ProductWarehouseDTO() { }

    public ProductWarehouseDTO(ProductWarehouse productWarehouse)
    {
        IdProduct = productWarehouse.IdProduct;
        IdWarehouse = productWarehouse.IdWarehouse;
        Amount = productWarehouse.Amount;
        CreatedAt = productWarehouse.CreatedAt;
    }
    public ProductWarehouseDTO(int idProduct, int idWarehouse, int amount, DateTime createdAt)
    {
        IdProduct = idProduct;
        IdWarehouse = idWarehouse;
        Amount = amount;
        CreatedAt = createdAt;
    }
}

public record AddProductToWarehouseRequestDto(int IdProduct, int IdWarehouse, int Amount, DateTime CreatedAt);
