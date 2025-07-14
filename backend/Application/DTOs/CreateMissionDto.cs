public class CreateMissionDto
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Location { get; set; } = default!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public Guid? OrganizationId { get; set; }
}
