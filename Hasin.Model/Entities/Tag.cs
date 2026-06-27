namespace Hasin.Model.Entities;

public sealed class Tag : BaseEntity
{
    public Tag(string key)
    {
        Key = key;
    }
    public string Key { get; set; }
}