﻿namespace ChatService.SignalR.Contracts;

public class MessageSignalRDto
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime Date { get; set; }
}