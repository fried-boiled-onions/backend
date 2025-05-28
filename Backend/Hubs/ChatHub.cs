using Microsoft.AspNetCore.SignalR;
using Backend.Models.Dtos;
using Backend.Data;
using Backend.Models;

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
        var senderId = int.Parse(Context.UserIdentifier!);

        var msg = new Message
        {
            SenderId = senderId,
            ReceiverId = message.ReceiverId,
            Content = message.Content,
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        _dbContext.Messages.Add(msg);
        await _dbContext.SaveChangesAsync();

        var response = new MessageResponseDto(senderId, message.ReceiverId, message.Content, msg.SentAt);

        await Clients.User(message.ReceiverId.ToString()).SendAsync("ReceiveMessage", response);
        await Clients.Caller.SendAsync("ReceiveMessage", response);
    }

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"User connected: {Context.UserIdentifier}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"User disconnected: {Context.UserIdentifier}");
        await base.OnDisconnectedAsync(exception);
    }
}