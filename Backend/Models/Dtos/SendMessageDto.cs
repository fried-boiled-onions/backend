namespace Backend.Models.Dtos;

public record MessageRequestDto(int ReceiverId, string Content);

public record MessageResponseDto(int SenderId, int ReceiverId, string Content, DateTime SentAt);