﻿namespace Chirp.Core.DTOs;

public class AuthorDTO
{
    public int AuthorId { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public ICollection<string> AuthorsFollowed { get; set; } // List of authors followed
}