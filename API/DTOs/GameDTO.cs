namespace API.DTOs;

public record GameDTO
{
    public required int GameID { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Icon { get; set; }
    public required string AltText { get; set; }
    public required string Route { get; set; }
}