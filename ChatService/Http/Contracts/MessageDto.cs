namespace ChatService.Http.Contracts;

public class MessageDto
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime Date { get; set; }
}