// File: Interfaces/IInventoryService.cs


using BloodBank.Backend.Models;

public interface IInventoryService
{
    // Define your methods here
    Task<User> Inventory(string email, string BloodGroup);
}

// File: Services/InventoryService.cs



public class InventoryService : IInventoryService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(ApplicationDbContext context, ILogger<InventoryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Implement your methods here
}
