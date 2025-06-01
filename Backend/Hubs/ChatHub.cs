using Microsoft.AspNetCore.SignalR;
using Backend.Models.Dtos;
using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Backend.Hubs;

public class ChatHub : Hub
{
    private readonly AppDbContext _dbContext;

    public ChatHub(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task SendMessage(MessageRequestDto message)
    {
        try
        {
            var nameIdentifier = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(nameIdentifier))
            {
                Console.WriteLine("UserIdentifier is null or empty. Claims: " +
                    string.Join(", ", Context.User?.Claims.Select(c => $"{c.Type}: {c.Value}") ?? ["No claims"]));
                throw new HubException("User is not authenticated");
            }
            if (!int.TryParse(nameIdentifier, out var senderId))
            {
                Console.WriteLine($"Error: Invalid UserIdentifier format: {nameIdentifier}");
                throw new HubException("Invalid user identifier format");
            }
            Console.WriteLine($"User {senderId} is sending to {message.ReceiverId}, Content: {message.Content}");

            var users = await _dbContext.Users
                .Where(u => u.Id == senderId || u.Id == message.ReceiverId)
                .ToListAsync();

            var sender = users.FirstOrDefault(u => u.Id == senderId);
            var receiverExists = users.Any(u => u.Id == message.ReceiverId);

            if (sender == null)
            {
                Console.WriteLine($"Error: Sender {senderId} does not exist");
                throw new HubException($"Sender {senderId} does not exist");
            }
            if (!receiverExists)
            {
                Console.WriteLine($"Error: Receiver {message.ReceiverId} does not exist");
                throw new HubException($"Receiver {message.ReceiverId} does not exist");
            }

            var msg = new Message
            {
                SenderId = senderId,
                ReceiverId = message.ReceiverId,
                Content = message.Content,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            Console.WriteLine($"Adding message to DbContext: SenderId={senderId}, ReceiverId={message.ReceiverId}, Content={message.Content}");
            _dbContext.Messages.Add(msg);
            await _dbContext.SaveChangesAsync();
            Console.WriteLine($"Message Saved: Id={msg.Id}, Content={msg.Content}");

            var response = new MessageResponseDto
            {
                SenderId = senderId,
                SenderName = sender.Username,
                ReceiverId = message.ReceiverId,
                Content = message.Content,
                SentAt = msg.SentAt
            };

            await Clients.User(message.ReceiverId.ToString()).SendAsync("ReceiveMessage", response);
            await Clients.User(senderId.ToString()).SendAsync("ReceiveMessage", response);
            Console.WriteLine($"Message sent to receiver {message.ReceiverId} and sender {senderId}");
        }
        catch (Exception ex)
        {
            var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown sender";
            var receiverId = message?.ReceiverId.ToString() ?? "unknown receiver";
            Console.WriteLine($"Error: Sender: {senderId}, Receiver: {receiverId}, ErrorText: {ex.Message}, StackTrace: {ex.StackTrace}");
            throw new HubException($"Failed to send message: {ex.Message}");
        }
    }

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"User connected: {Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown"}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"User disconnected: {Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown"}");
        await base.OnDisconnectedAsync(exception);
    }
}