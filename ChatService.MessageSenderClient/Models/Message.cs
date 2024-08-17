namespace ChatService.MessageSenderClient.Models;

public class Message
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime Date { get; set; }
}