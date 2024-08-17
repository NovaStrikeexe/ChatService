﻿using System.ComponentModel.DataAnnotations;

namespace ChatService.Models;

public class MsgDto
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(128, ErrorMessage = "Content length can't be more than 128 characters.")]
    public string Content { get; set; }
    
    public DateTime Date { get; set; }
}