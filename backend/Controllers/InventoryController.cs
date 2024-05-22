// Controllers/InventoryController.cs
using BloodBank.Backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BloodBank.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryService _inventoryService;

        public InventoryController(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet("get-inventory")]
        public async Task<IActionResult> GetInventory()
        {
            var inventory = await _inventoryService.GetInventoryAsync();
            return Ok(new { success = true, inventory });
        }
    }
}
