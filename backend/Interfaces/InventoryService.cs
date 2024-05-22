using BloodBank.Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloodBank.Backend.Interfaces
{
    public interface IInventoryService
    {
        Task<List<InventoryRecord>> GetInventoryRecordsAsync();
        Task<InventoryRecord> CreateInventoryRecordAsync(InventoryRecord record);
    }
}
