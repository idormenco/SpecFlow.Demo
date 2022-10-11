using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpecFlow.Demo.Api.Entities;
using SpecFlow.Demo.Api.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SpecFlow.Demo.Api.Controllers;

[ApiController]
[Route("group")]
[Produces("application/json")]
[Consumes("application/json")]
[Authorize]
public class GroupsController : ControllerBase
{
    private readonly DataContext _dataContext;

    public GroupsController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    /// <summary>
    /// Gets groups for current user.
    /// </summary>
    /// <returns></returns>
    [HttpGet("/groups")]
    [SwaggerResponse(StatusCodes.Status200OK, "User groups", typeof(IImmutableList<GroupModel>))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something bad happened", typeof(ProblemDetails))]
    public async Task<ActionResult<IImmutableList<GroupModel>>> GetGroupsAsync()
    {
        var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value!);
        var userGroups = await _dataContext
            .GroupsMembers
            .Where(gm => gm.UserId == userId)
            .Include(gm => gm.Group)
            .Select(x => new GroupModel
            {
                Id = x.GroupId,
                Name = x.Group.Name
            })
            .ToListAsync();

        return Ok(userGroups.ToImmutableArray());
    }

    /// <summary>
    /// Adds a new group
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [SwaggerResponse(StatusCodes.Status200OK, "New group", typeof(GroupModel))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "A business rule was violated", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something bad happened", typeof(ProblemDetails))]
    public async Task<ActionResult<GroupModel>> CreateNewGroupAsync([FromBody] GroupModelRequest group)
    {
        var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value!);
        var user = await _dataContext.Users.FirstAsync(x => x.Id == userId);

        var newGroup = new Group
        {
            Name = group.Name,
            AdminId = userId
        };

        var groupMember = new GroupMember
        {
            Group = newGroup,
            User = user
        };
        await _dataContext.Groups.AddAsync(newGroup);
        await _dataContext.GroupsMembers.AddAsync(groupMember);
        await _dataContext.SaveChangesAsync();

        return Ok(new GroupModel
        {
            Id = newGroup.Id,
            Name = newGroup.Name
        });
    }

    /// <summary>
    /// Updates a group
    /// </summary>
    [HttpPut]
    [Route("{groupId:guid}")]
    [SwaggerResponse(StatusCodes.Status200OK, "Updated group", typeof(GroupModel))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "A business rule was violated", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something bad happened", typeof(ProblemDetails))]
    public async Task<ActionResult<GroupModel>> UpdateGroupAsync([FromRoute] Guid groupId, [FromBody] GroupModelRequest group)
    {
        var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value!);
        var existingGroup = await _dataContext.Groups.FirstOrDefaultAsync(g => g.Id == groupId && g.AdminId == userId);
        if (existingGroup == null)
        {
            return BadRequest();
        }

        existingGroup.Name = group.Name;
        await _dataContext.SaveChangesAsync();

        return Ok(new GroupModel
        {
            Id = existingGroup.Id,
            Name = existingGroup.Name
        });
    }

    /// <summary>
    /// Delete a group
    /// </summary>
    /// <returns></returns>
    [HttpDelete]
    [Route("{groupId:guid}")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Group was deleted")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "A business rule was violated", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something bad happened", typeof(ProblemDetails))]
    public async Task<IActionResult> DeleteGroupAsync([FromRoute] Guid groupId)
    {
        var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value!);

        var existingGroup = await _dataContext.Groups.FirstOrDefaultAsync(g => g.Id == groupId && g.AdminId == userId);
        if (existingGroup == null)
        {
            return BadRequest();
        }

        _dataContext.Groups.Remove(existingGroup);
        await _dataContext.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Adds user to a group
    /// </summary>
    /// <returns></returns>
    [HttpPut]
    [Route("{groupId:guid}/invite/{userId:guid}")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "User was added as member")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "A business rule was violated", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something bad happened", typeof(ProblemDetails))]
    public async Task<IActionResult> InviteUserToGroupAsync([FromRoute] Guid groupId, [FromRoute] Guid userId)
    {
        var adminId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value!);
        var existingGroup = await _dataContext.Groups.FirstOrDefaultAsync(g => g.Id == groupId && g.AdminId == adminId);
        if (existingGroup == null)
        {
            return BadRequest();
        }

        var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null)
        {
            return BadRequest();
        }

        var groupMember = new GroupMember
        {
            Group = existingGroup,
            User = user
        };

        await _dataContext.GroupsMembers.AddAsync(groupMember);
        await _dataContext.SaveChangesAsync();

        return NoContent();
    }


    /// <summary>
    /// Removes user from a group
    /// </summary>
    /// <returns></returns>
    [HttpPut]
    [Route("{groupId:guid}/remove/{userId:guid}")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "User was removed from members")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "A business rule was violated", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something bad happened", typeof(ProblemDetails))]
    public async Task<IActionResult> RemoveUserFromGroupAsync([FromRoute] Guid groupId, [FromRoute] Guid userId)
    {
        var currentUserId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value!);
        var requestedUserMembership = await _dataContext.GroupsMembers.FirstOrDefaultAsync(gm => gm.UserId == userId && gm.GroupId == groupId);
        var isGroupAdmin = await _dataContext.Groups.AnyAsync(g => g.Id == groupId && g.AdminId == currentUserId);
        var isRequestedUserMemberOfGroup = requestedUserMembership != null;

        if (currentUserId == userId && isRequestedUserMemberOfGroup)
        {
            if (isGroupAdmin)
            {
                return Forbid("Can't leave while you are admin!");
            }
        }

        if (!isGroupAdmin || !isRequestedUserMemberOfGroup)
        {
            return Forbid();
        }

        _dataContext.GroupsMembers.Remove(requestedUserMembership);
        await _dataContext.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Gets groups for current user.
    /// </summary>
    /// <returns></returns>
    [HttpGet("{groupId:guid}/members")]
    [SwaggerResponse(StatusCodes.Status200OK, "User groups", typeof(IImmutableList<GroupMemberModel>))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Something bad happened", typeof(ProblemDetails))]
    public async Task<ActionResult<IImmutableList<GroupModel>>> GetGroupMembers(Guid groupId)
    {
        var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value!);
        var isMemberOfGroup = await _dataContext.GroupsMembers.AnyAsync(x => x.UserId == userId && x.GroupId == groupId);

        if (!isMemberOfGroup)
        {
            return Forbid();
        }

        var groupMembers = await _dataContext
            .GroupsMembers
            .Where(gm => gm.GroupId == groupId)
            .Include(gm => gm.User)
            .Select(x => new GroupMemberModel(x.UserId, x.User.Name))
            .ToListAsync();

        return Ok(groupMembers.ToImmutableArray());
    }
}
