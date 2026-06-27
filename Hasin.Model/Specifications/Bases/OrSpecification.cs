using System.Linq.Expressions;

namespace Hasin.Model.Specifications.Bases;

public sealed class OrSpecification<T> : Specification<T>
{
    private readonly Specification<T> _left;
    private readonly Specification<T> _right;

    public OrSpecification(
        Specification<T> left,
        Specification<T> right)
    {
        _left = left;
        _right = right;
    }

    public override Expression<Func<T, bool>> ToExpression()
    {
        var leftExpression = _left.ToExpression();
        var rightExpression = _right.ToExpression();

        var parameter = Expression.Parameter(typeof(T));

        var leftBody = ParameterReplacer.Replace(
            leftExpression.Body,
            leftExpression.Parameters[0],
            parameter);

        var rightBody = ParameterReplacer.Replace(
            rightExpression.Body,
            rightExpression.Parameters[0],
            parameter);

        return Expression.Lambda<Func<T, bool>>(
            Expression.OrElse(leftBody, rightBody),
            parameter);
    }
}