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
        try
        {
            var senderId = int.Parse(Context.UserIdentifier!);
            Console.WriteLine($"User {senderId} is sending to {message.ReceiverId}");

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
            Console.WriteLine($"Message Saved");

            var response = new MessageResponseDto(senderId, message.ReceiverId, message.Content, msg.SentAt);

            await Clients.User(message.ReceiverId.ToString()).SendAsync("ReceiveMessage", response);
            Console.WriteLine($"Message sent");
            await Clients.Caller.SendAsync("ReceiveMessage", response);
            Console.WriteLine($"Message sended back");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: Sender: {SenderId}, Receiver: {ReceiverId}, ErrorText: {ex.Message}")
        }
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