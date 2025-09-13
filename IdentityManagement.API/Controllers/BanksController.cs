using IdentityManagement.API.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace IdentityManagement.API.Controllers
{
    [ApiController]
    public class BanksController : ControllerBase
    {
        private readonly BankService _bankSvc;

        public BanksController(BankService bankSvc)
        {
            _bankSvc = bankSvc;
        }

        [HttpGet]
        [Route("/api/banks")]
        public async Task<IActionResult> GetBanks()
        {
            var banks = await _bankSvc.GetAllBanksAsync();
            return Ok(banks);
        }
    }
}
