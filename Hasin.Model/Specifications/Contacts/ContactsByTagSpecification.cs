using System.Linq.Expressions;
using Hasin.Model.Entities;
using Hasin.Model.Specifications.Bases;

namespace Hasin.Model.Specifications.Contacts;

public sealed class ContactsByTagSpecification : Specification<Contact>
{
    private readonly string _tag;

    public ContactsByTagSpecification(string tag)
    {
        _tag = tag;
    }

    public override Expression<Func<Contact, bool>> ToExpression()
    {
        return contact =>
            contact.Tag != null &&
            contact.Tag.Key == _tag;
    }
}