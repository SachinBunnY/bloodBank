// Services/InventoryService.cs
using BloodBank.Backend.Data;
using BloodBank.Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BloodBank.Backend.Services
{
    public class InventoryService
    {
        private readonly ApplicationDbContext _context;

        public InventoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InventoryRecord>> GetInventoryAsync()
        {
            return await _context.InventoryRecords.ToListAsync();
        }

        // Additional methods for handling inventory can be added here
    }
}
