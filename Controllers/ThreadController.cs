using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChatBackend.Context;
using ChatBackend.Models;
using ChatBackend.Utils;
using Azure.Communication;
using Azure.Communication.Chat;
using Azure.Communication.Identity;
using Azure.Core;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace ChatBackend.Controllers;

public class ThreadOption
{
    public string? topicName { get; set; }
    public User createdBy { get; set; }
}

[ApiController]
[Route("[controller]")]
public class ThreadController : ControllerBase
{
    private readonly ChatBackendContext _context;
    private readonly ILogger<ThreadController> _logger;
    private readonly ACSIdentity identity;

    private readonly string acsConnectionString;
    private readonly Uri acsEndpoint;

    public ThreadController(
        ILogger<ThreadController> logger,
        ChatBackendContext context,
        IConfiguration configuration
    )
    {
        _logger = logger;
        _context = context;
        // _config = configuration;
        acsConnectionString = configuration.GetValue<string>("ACS_CONNECTION_STRING");
        //endpoint=https://xxxxx.communication.azure.com/;
        Regex reg = new Regex("endpoint=(.+?);");
        Match match = reg.Match(acsConnectionString);
        if(!match.Success)
        {
            throw new Exception("invalid ACS_CONNECTION_STRING");
        }
        acsEndpoint = new Uri(match.Groups[1].Value.Trim());
        identity = new ACSIdentity(acsConnectionString);
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        var threads = await _context.ACSThread.ToListAsync();
        return StatusCode(200, threads);
    }

    [HttpGet("{threadId}")]
    public async Task<IActionResult> Get(string threadId)
    {
        var thread = await _context.ACSThread.FindAsync(threadId);
        if (thread == null)
        {
            return StatusCode(404);
        }
        return StatusCode(200, thread);
    }

    [HttpPost("")]
    public async Task<IActionResult> Post(ThreadOption option)
    {
        AccessToken tkn = identity.Identity == null
            ? await identity.CreateTokenAsync()
            : await identity.GetRefleshedTokenAsync();
        ACSChat chat = new ACSChat(acsEndpoint, tkn);
        var mod = identity.GenerateChatParticipant("moderator");
        ACSChatThread chatThread = new ACSChatThread(chat.Client, mod);

        string threadId = chatThread.CreateThread(
            option.topicName != null
                ? option.topicName
                : "Empty Topic"
        );

        var creatorIdentity = new CommunicationUserIdentifier(
            option.createdBy.UserIdentityId
        );
        var creator = new ChatParticipant(creatorIdentity) {
            DisplayName = option.createdBy.UserName
        };
        chatThread.AddParticipants(
            new ChatParticipant[] { creator }
        );

        ACSThread model = new ACSThread() {
            ModeratorIdentityId = mod.User.RawId,
            ThreadId = threadId
        };

        try
        {
            _context.Add(model);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _context.Remove(model);
            _logger.LogError("エンティティが追加できない！", model, ex);
            return StatusCode(500, model);
        }
        return StatusCode(200, model);
        //}
        //else
        //{
        //    return StatusCode(400, user);
        //}
    }

    /*
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
    */

    [HttpDelete("{threadId}")]
    public async Task<IActionResult> Delete(string threadId)
    {
        ACSThread? model = null;
        try
        {
            model = await _context.ACSThread.FindAsync(threadId);
            if (model == null)
            {
                return StatusCode(400);
            }
            _context.Remove(model);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError("エンティティが削除できない！", model, ex);
            return StatusCode(500, model);
        }
        return StatusCode(200);
    }

    private bool UserExists(string userName)
    {
        return _context.User.Any(e => e.UserName == userName);
    }
}

