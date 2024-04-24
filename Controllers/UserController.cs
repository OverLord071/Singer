using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Singer.Application;
using Singer.Domain;
using Singer.Interfaces;

namespace Singer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserApplication _userApplication;

    public UserController(IUserApplication userApplication)
    {
        _userApplication = userApplication;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
    {
        var users = await _userApplication.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<User>> GetUserById(Guid id)
    {
        var user = await _userApplication.GetUserByIdAsync(id);
        if (user == null) 
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(CreateUserDto user)
    {
        try
        {
            var createdUser = await _userApplication.CreateUserAsync(user.Name, user.PasswordHash, user.Email);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id}, createdUser);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<User>> UpdateUser(Guid id, User user)
    {
        try
        {
            var updatedUser = await _userApplication.UpdateUserAsync(id, user.Name, user.Email);
            return Ok(updatedUser);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteUser(Guid id)
    {
        try
        {
            await _userApplication.DeleteUserAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("approve/{userId}")]
    //[Authorize(Roles = "Admin")]
    public async Task<ActionResult<User>> ApproveUser(Guid userId)
    {
        try
        {
            var approvedUser = await _userApplication.ApproveUserAsync(userId);
            return Ok(approvedUser);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("authenticate")]
    public async Task<ActionResult<(string key, string iv, string token)>> AuthenticateUser(LoginRequest loginRequest)
    {
        try
        {
            var (key, iv, token) = await _userApplication.AuthenticateUserAsync(loginRequest.Username, loginRequest.Password);
            return Ok(new { key, iv, token });
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
    }
}
