namespace AuthAPI.Domain.Entities;

public class AuditLog : BaseEntity<Guid>
{
    /// <summary> Id of user who created change. </summary>
    public string? UserId { get; set; }

    /// <summary> State of change </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary> Name of Table modified. </summary>
    public string EntityName { get; set; } = string.Empty;

    /// <summary> Id of the changed record. </summary>
    public string? EntityId { get; set; } 

    public DateTime Timestamp { get; set; } 
}
