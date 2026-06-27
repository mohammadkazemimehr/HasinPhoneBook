using System.Linq.Expressions;

namespace Hasin.Model.Specifications.Bases;

internal sealed class ParameterReplacer : ExpressionVisitor
{
    private readonly ParameterExpression _oldParameter;
    private readonly ParameterExpression _newParameter;

    private ParameterReplacer(
        ParameterExpression oldParameter,
        ParameterExpression newParameter)
    {
        _oldParameter = oldParameter;
        _newParameter = newParameter;
    }

    public static Expression Replace(
        Expression expression,
        ParameterExpression oldParameter,
        ParameterExpression newParameter)
    {
        return new ParameterReplacer(oldParameter, newParameter)
            .Visit(expression)!;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        return node == _oldParameter
            ? _newParameter
            : base.VisitParameter(node);
    }
}