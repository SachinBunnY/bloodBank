using BloodBank.Backend.Interfaces;
using BloodBank.Backend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BloodBank.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet("get-inventory")]
        public async Task<IActionResult> GetInventory()
        {
            var inventoryRecords = await _inventoryService.GetInventoryRecordsAsync();
            return Ok(new { success = true, inventory = inventoryRecords });
        }

        [HttpPost("create-inventory")]
        public async Task<IActionResult> CreateInventory([FromBody] InventoryRecord record)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newRecord = await _inventoryService.CreateInventoryRecordAsync(record);
            return Ok(new { success = true, inventory = newRecord });
        }

    }
}
