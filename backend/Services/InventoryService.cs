// File: Services/InventoryService.cs

using BloodBank.Backend.Data;
using BloodBank.Backend.Models;
using BloodBank.Backend.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloodBank.Backend.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(ApplicationDbContext context, ILogger<InventoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<InventoryRecord>> GetInventoryRecordsAsync()
        {
            _logger.LogInformation("Fetching inventory records from the database.");
            return await _context.InventoryRecords.ToListAsync();
        }

        public async Task<InventoryRecord> CreateInventoryRecordAsync(InventoryRecord record)
        {
            _context.InventoryRecords.Add(record);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created a new inventory record.");
            return record;
        }
    }
}
