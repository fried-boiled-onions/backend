namespace Backend.Models.Dtos;

public record MessageRequestDto(int ReceiverId, string Content);


public record MessageResponseDto
{
    public int SenderId { get; init; }
    public string SenderName { get; init; }
    public int ReceiverId { get; init; }
    public string Content { get; init; }
    public DateTime SentAt { get; init; }
}