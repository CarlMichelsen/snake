namespace Domain;

public class User
{
    public required Guid Id { get; init; }
    
    public required string Name { get; set; }
    
    public required DateTimeOffset CreatedAt { get; init; }
}