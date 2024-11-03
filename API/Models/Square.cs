namespace API.Models;

public struct Square(int id, bool isActive)
{
    public int Id { get; } = id;
    public bool IsActive { get; } = isActive;
}