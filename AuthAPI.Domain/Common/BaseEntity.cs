namespace AuthAPI.Domain.Common;

/// <summary>
/// Base class for all entities in the domain layer.
/// </summary>
public abstract class BaseEntity<T>
{
    // Remove 'required' so new entities can be created without explicitly setting Id
    // when the database will generate the value.
    public T Id { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

