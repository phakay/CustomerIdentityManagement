using IdentityManagement.API.Core.Repositories;
using IdentityManagement.API.Dtos;
using IdentityManagement.API.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace IdentityManagement.API.Controllers
{
    [ApiController]
    public class StatesController : ControllerBase
    {
        private IStateRepository _stateRepo;

        public StatesController(IStateRepository stateRepo)
        {
            _stateRepo = stateRepo;
        }

        [Route("/api/states")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StateDto>>> Get()
        {
            var states = await _stateRepo.GetAllAsync();

            var retStates = states.Select(s => new StateDto
            {
                Id = s.Id,
                Name = s.Name
            });

            return retStates.ToActionResult();
        }


        [Route("/api/states/{id:int}")]
        [HttpGet]
        public async Task<ActionResult<StateDto>> Get(int id)
        {
            var state = await _stateRepo.GetByIdAsync(id);

            if (state == null) return NotFound();

            var retState = new StateDto
            {
                Id = state.Id,
                Name = state.Name,
                Lgas = state.Lgas.Select(l => new LgaDto
                {
                    Id = l.Id,
                    Name = l.Name
                }).ToArray()
            };

            return retState;
        }
    }
}
