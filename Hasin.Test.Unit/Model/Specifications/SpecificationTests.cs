using System.Linq.Expressions;
using Hasin.Model.Entities;
using Hasin.Model.Specifications.Bases;
using Hasin.Model.Specifications.Contacts;
using Hasin.Test.Unit.TestHelpers;

namespace Hasin.Test.Unit.Model.Specifications;

[TestFixture]
public class ContactsByTagSpecificationTests
{
    [Test]
    public void IsSatisfiedBy_ReturnsTrue_WhenContactHasMatchingTag()
    {
        var contact = ContactFactory.ValidContact(tag: new Tag("vip"));
        var spec = new ContactsByTagSpecification("vip");

        Assert.That(spec.IsSatisfiedBy(contact), Is.True);
    }

    [Test]
    public void IsSatisfiedBy_ReturnsFalse_WhenContactHasDifferentTag()
    {
        var contact = ContactFactory.ValidContact(tag: new Tag("family"));
        var spec = new ContactsByTagSpecification("vip");

        Assert.That(spec.IsSatisfiedBy(contact), Is.False);
    }

    [Test]
    public void IsSatisfiedBy_ReturnsFalse_WhenContactHasNoTag()
    {
        var contact = ContactFactory.ValidContact(tag: null);
        var spec = new ContactsByTagSpecification("vip");

        Assert.That(spec.IsSatisfiedBy(contact), Is.False);
    }
}

[TestFixture]
public class CompositeSpecificationTests
{
    private sealed class FirstNameSpecification : Specification<Contact>
    {
        private readonly string _firstName;

        public FirstNameSpecification(string firstName) => _firstName = firstName;

        public override Expression<Func<Contact, bool>> ToExpression()
            => contact => contact.FirstName == _firstName;
    }

    [Test]
    public void And_ReturnsTrue_OnlyWhenBothSpecificationsAreSatisfied()
    {
        var contact = ContactFactory.ValidContact(firstName: "Mohammad", tag: new Tag("vip"));
        var spec = new FirstNameSpecification("Mohammad").And(new ContactsByTagSpecification("vip"));

        Assert.That(spec.IsSatisfiedBy(contact), Is.True);
    }

    [Test]
    public void And_ReturnsFalse_WhenOnlyOneSpecificationIsSatisfied()
    {
        var contact = ContactFactory.ValidContact(firstName: "Mohammad", tag: new Tag("family"));
        var spec = new FirstNameSpecification("Mohammad").And(new ContactsByTagSpecification("vip"));

        Assert.That(spec.IsSatisfiedBy(contact), Is.False);
    }

    [Test]
    public void Or_ReturnsTrue_WhenAtLeastOneSpecificationIsSatisfied()
    {
        var contact = ContactFactory.ValidContact(firstName: "Hasin", tag: new Tag("vip"));
        var spec = new FirstNameSpecification("").Or(new ContactsByTagSpecification("vip"));

        Assert.That(spec.IsSatisfiedBy(contact), Is.True);
    }

    [Test]
    public void Or_ReturnsFalse_WhenNeitherSpecificationIsSatisfied()
    {
        var contact = ContactFactory.ValidContact(firstName: "Hasin", tag: new Tag("family"));
        var spec = new FirstNameSpecification("Mohammad").Or(new ContactsByTagSpecification("vip"));

        Assert.That(spec.IsSatisfiedBy(contact), Is.False);
    }
}
