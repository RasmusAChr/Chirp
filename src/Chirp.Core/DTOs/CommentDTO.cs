namespace Chirp.Core.DTOs;

public class CommentDTO
{
    public required CommentDTO DTO { get; set; } // AuthorDTO
    public required int CheepId { get; set; } //CheepId of commented cheep
    public required string Text { get; set; } // Message text
    public required string FormattedTimeStamp { get; set; } // Time stamp as a formatted string
}