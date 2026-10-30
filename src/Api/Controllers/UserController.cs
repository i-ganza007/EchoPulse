using Microsoft.AspNetCore.Mvc;
using Echo.src.Application.DTOs.Responses;
using Echo.src.Application.Services;
using Echo.src.Domain.Entities;
[ApiController]
[Route("api/[controller]/[action]")]
public class UserController:ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsers();
        return Ok(users);
    }

    [AcceptVerbs("QUERY","DELETE")]
    
    public async Task<IActionResult> GetBatchUsers([FromBody] ICollection<Guid> userIds)
    {
        var users = await _userService.GetUserBatchUsers(userIds);
        return Ok(users);
    }



    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetSingleUser(Guid userId)
    {
        if(_userService.GetSingleUser(userId) is null)
        {
            return NotFound($"User with ID {nameof(userId)} not found.");
        }
        return Ok(_userService.GetSingleUser(userId));
    }

    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> DeleteUser(Guid userId )
    {
        if(await _userService.DeleteUser(userId))
        {
            return Ok($"User with ID {nameof(userId)} deleted successfully.");
        }
        return NotFound($"User with ID {nameof(userId)} not found.");
    }

    [AcceptVerbs("QUERY","DELETE")]
    public async Task<IActionResult> DeleteBatchUsers([FromBody] ICollection<Guid> userIds)
    {
        var users = await _userService.DeleteBatchUsers(userIds);
        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        if(await _userService.CreateUser(user))
        {
            return Ok($"User with ID {nameof(user.userId)} created successfully.");
        }
        return BadRequest($"Failed to create user with ID {nameof(user.userId)}.");
    }
   
}