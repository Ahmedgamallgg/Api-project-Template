namespace Domain.Models;


public class BaseEntity<TKey> : IAuditedEntity
{
    public TKey Id { get; set; }


}

public class IAuditedEntity
{
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public string? DeletedBy { get; set; }
}
