using System;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpecFlow.Demo.Api.Entities;
using SpecFlow.Demo.Api.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace SpecFlow.Demo.Api.Controllers
{
    [ApiController]
    [Route("backpack")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Authorize]
    public class BackpackController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public BackpackController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        /// <summary>
        /// Gets backpacks for current user.
        /// </summary>
        /// <returns></returns>
        [HttpGet("/backpacks")]
        [SwaggerResponse(StatusCodes.Status200OK, "User backpacks", typeof(IImmutableList<BackpackModel>))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something bad happened", typeof(ProblemDetails))]
        public async Task<ActionResult<IImmutableList<BackpackModel>>> GetBackpacksAsync()
        {
            var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value!);
            var userBackpacks = await _dataContext.Backpacks.Where(b => b.OwnerId == userId).ToListAsync();

            return Ok(userBackpacks.ToImmutableArray());
        }

        /// <summary>
        /// Adds a new backpack
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, "New backpack", typeof(BackpackModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "A business rule was violated", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something bad happened", typeof(ProblemDetails))]
        public async Task<ActionResult<BackpackModel>> CreateNewBackpackAsync([FromBody] BackpackModelRequest backpack)
        {
            var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value!);

            var newBackpack = new Backpack
            {
                Name = backpack.Name,
                OwnerId = userId
            };
            await _dataContext.Backpacks.AddAsync(newBackpack);
            await _dataContext.SaveChangesAsync();

            return Ok(new BackpackModel
            {
                Id = newBackpack.Id,
                Name = newBackpack.Name
            });
        }

        /// <summary>
        /// Updates a backpack
        /// </summary>
        [HttpPut]
        [Route("{backpackId:guid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Updated backpack", typeof(BackpackModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "A business rule was violated", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something bad happened", typeof(ProblemDetails))]
        public async Task<ActionResult<BackpackModel>> UpdateBackpackAsync([FromRoute] Guid backpackId, [FromBody] BackpackModelRequest backpack)
        {
            var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value!);
            var existingBackpack = await _dataContext.Backpacks.FirstOrDefaultAsync(b => b.Id == backpackId && b.OwnerId == userId);
            if (existingBackpack == null)
            {
                return NotFound();
            }

            existingBackpack.Name = backpack.Name;
            await _dataContext.SaveChangesAsync();
            return Ok(new BackpackModel
            {
                Id = existingBackpack.Id,
                Name = existingBackpack.Name
            });
        }

        /// <summary>
        /// Delete a backpack
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("{backpackId:guid}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Backpack was deleted")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "A business rule was violated", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something bad happened", typeof(ProblemDetails))]
        public async Task<IActionResult> DeleteBackpackAsync([FromRoute] Guid backpackId)
        {
            var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value!);

            var existingBackpack = await _dataContext.Backpacks.FirstOrDefaultAsync(b => b.Id == backpackId && b.OwnerId == userId);
            if (existingBackpack == null)
            {
                return NotFound();
            }

            _dataContext.Backpacks.Remove(existingBackpack);
            await _dataContext.SaveChangesAsync();

            return NoContent();
        }
    }
}