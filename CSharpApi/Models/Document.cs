public class Document 
{
    public Guid Id { get; set; }
    public string FileName { get; set; } 
    public string FilePath { get; set; } 
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; } 
    public string ContentType { get; set; } 
    public long FileSize { get; set; }
    public List<UserTag> UserTags { get; set; } = new List<User>();
    public List<AITag> AITags {get; set; } = new List<AITags>();
}

public class 