using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChatBackend.Context;
using ChatBackend.Models;
using ChatBackend.Utils;

namespace ChatBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ChatBackendContext _context;
    private readonly ILogger<UserController> _logger;
    // private readonly IConfiguration _config;

    public UserController(
        ILogger<UserController> logger,
        ChatBackendContext context
    )
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet("{userName}")]
    public async Task<IActionResult> Get(string userName)
    {
        var user = await _context.User.FindAsync(userName);
        if (user == null)
        {
            return StatusCode(404);
        }
        return StatusCode(200, user);
    }

    [HttpPost("")]
    public async Task<IActionResult> Post([Bind("UserName, UserIdentityId")] User user)
    {
        /*
        var user = TryValidateModel(userFromBody)
            ? userFromBody
            : userFromForm;
        */
        //if(ModelState.IsValid)
        //{
            try
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _context.Remove(user);
                _logger.LogError("エンティティが追加できない！", user, ex);
                return StatusCode(500, user);
            }
            return StatusCode(200, user);
        //}
        //else
        //{
        //    return StatusCode(400, user);
        //}
    }

    [HttpPut("{userName}")]
    public async Task<IActionResult> Put(string userName, [Bind("UserName, UserIdentityId")] User user)
    {
        if (ModelState.IsValid)
        {
            if (!UserExists(user.UserName))
            {
                return StatusCode(404);
            }
            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _context.Remove(user);
                _logger.LogError("エンティティが編集できない！", user, ex);
                return StatusCode(500, user);
            }
            return StatusCode(200, user);
        }
        else
        {
            return StatusCode(400, user);
        }
    }

    [HttpDelete("{userName}")]
    public async Task<IActionResult> Delete(string userName)
    {
        User? user = null;
        try
        {
            user = await _context.User.FindAsync(userName);
            if (user == null)
            {
                return StatusCode(400);
            }
            _context.Remove(user);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError("エンティティが削除できない！", user, ex);
            return StatusCode(500, user);
        }
        return StatusCode(200);
    }

    private bool UserExists(string userName)
    {
        return _context.User.Any(e => e.UserName == userName);
    }
}

