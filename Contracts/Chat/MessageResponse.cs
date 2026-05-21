namespace Career_Path.Contracts.Chat;

public record MessageResponse(
 int Id,
 string SenderId,
 string SenderName,
 string Content,
 DateTime SentAt,
 bool IsRead
);
