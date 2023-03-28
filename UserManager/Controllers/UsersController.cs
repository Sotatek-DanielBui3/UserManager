using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net;
using UserManager.Data;
using UserManager.Data.Entities;
using UserManager.Models;

namespace UserManager.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserCreateResult), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> CreateUserAsync([FromBody] UserCreateModel model)
    {
        var newUser = new User
        {
            FullName = model.FullName,
            Email = model.Email,
            Phone = model.Phone,
            Age = model.Age
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return StatusCode((int)HttpStatusCode.Created, new UserCreateResult { Id = newUser.Id });
    }

    [HttpPatch("{id}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> UpdateUserAsync([FromRoute] int id, [FromBody] UserUpdateModel model)
    {
        var userToUpdate = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (userToUpdate == null)
        {
            return NotFound();
        }

        userToUpdate.FullName = model.FullName;
        userToUpdate.Email = model.Email;
        userToUpdate.Phone = model.Phone;
        userToUpdate.Age = model.Age;
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetUserByIdAsync([FromRoute] int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<User>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetUsersAsync([FromQuery] string? email, [FromQuery] string? phone, [FromQuery] SortedByField? sortedBy)
    {
        var query = _context.Users.AsQueryable();
        if (email != null)
        {
            query = query.Where(u => u.Email.Contains(email));
        }

        if (phone != null)
        {
            query = query.Where(u => u.Phone != null && u.Phone.Contains(phone));
        }

        if (sortedBy != null)
        {
            if (sortedBy == SortedByField.Email)
            {
                query = query.OrderBy(u => u.Email);
            } else if (sortedBy == SortedByField.Phone)
            {
                query = query.OrderBy(u => u.Phone);
            }
        }

        var users = await query.ToListAsync();
        return Ok(users);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> DeleteUserAsync([FromRoute] int id)
    {
        var userToDelete = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (userToDelete == null)
        {
            return NotFound();
        }

        _context.Users.Remove(userToDelete);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    public enum SortedByField
    {
        Email,
        Phone
    }
}
