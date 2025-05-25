using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class MessagesController : ControllerBase
{
    private readonly AppDbContext _context;

    public MessagesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetMessages(int userId)
    {
        //var currentUserIdStr = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var currentUserIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Console.WriteLine($"Parsed userId: {currentUserIdStr}");
        //Console.WriteLine($"Test parse userId: {test}");
        if (!int.TryParse(currentUserIdStr, out var currentUserId))
            return Unauthorized();

        var messages = await _context.Messages
            .Where(m => (m.SenderId == currentUserId && m.ReceiverId == userId)
                     || (m.SenderId == userId && m.ReceiverId == currentUserId))
            .OrderBy(m => m.SentAt)
            .Select(m => new
            {
                m.Id,
                SenderId = m.SenderId,
                SenderName = m.Sender.Username,
                ReceiverId = m.ReceiverId,
                ReceiverName = m.Receiver.Username,
                m.Content,
                m.SentAt,
                m.IsRead
            })
            .ToListAsync();

        return Ok(messages);
    }
}