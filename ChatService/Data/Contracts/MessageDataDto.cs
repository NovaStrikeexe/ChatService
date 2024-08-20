namespace ChatService.Data.Contracts;

public class MessageDataDto
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime Date { get; set; }
}