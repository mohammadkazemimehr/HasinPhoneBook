namespace Hasin.Model.Entities;

public class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreationDateTime { get; set; }

    public BaseEntity()
    {
        Id = Guid.NewGuid();
        CreationDateTime = DateTime.Now;
    }
}