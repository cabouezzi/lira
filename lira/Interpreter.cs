namespace Lira;

public class Interpreter : IExpr.IVisitor<object?>, IStatement.IVisitor<bool>
{
    public void Interpret(List<IStatement> statements)
    {
        try
        {
            foreach (IStatement statement in statements) Execute(statement);
        }
        catch (RuntimeError e)
        {
            Lira.Error(-1, e.Message);
        }
    }

    private void Execute(IStatement statement) => statement.Accept(this);

    private object? Evaluate(IExpr expr) => expr.Accept(this);

    private bool IsTruthy(object? obj)
    {
        if (obj is null) return false;
        if (obj is bool b) return b;
        return true;
    }

    private bool IsEqual(object? lhs, object? rhs)
    {
        if (lhs is null && rhs is null) return true;
        if (lhs is null || rhs is null) return false;

        // *Custom equality here

        return lhs.Equals(rhs);
    }

    private void CheckNumbers(Token token, params object?[] objs)
    {
        if (objs.Any(obj => obj is not double))
            throw new RuntimeError(token, "Operand must be a number");
    }

    #region Visitor Interface

    public object? VisitUnary(IExpr.Unary unary)
    {
        var right = Evaluate(unary.Right)!;

        switch (unary.Operator.Kind)
        {
            case TokenKind.BANG: return !IsTruthy(right);
            case TokenKind.MINUS:
                CheckNumbers(unary.Operator, right);
                return -(double)right;
            // unreachable
            default: return null;
        }
    }

    public object? VisitBinary(IExpr.Binary binary)
    {
        var left = Evaluate(binary.Left)!;
        var right = Evaluate(binary.Right)!;

        switch (binary.Operator.Kind)
        {
            case TokenKind.MINUS:
                CheckNumbers(binary.Operator, left, right);
                return (double)left - (double)right;
            case TokenKind.SLASH:
                CheckNumbers(binary.Operator, left, right);
                return (double)left / (double)right;
            case TokenKind.STAR:
                CheckNumbers(binary.Operator, left, right);
                return (double)left * (double)right;

            case TokenKind.PLUS when left is double ld && right is double rd: return ld + rd;
            case TokenKind.PLUS when left is string ls && right is string rs: return ls + rs;
            case TokenKind.PLUS:
                throw new RuntimeError(binary.Operator, "Operator can only be used on numbers or strings.");

            case TokenKind.GREATER:
                CheckNumbers(binary.Operator, left, right);
                return (double)left > (double)right;
            case TokenKind.GREATER_EQUAL:
                CheckNumbers(binary.Operator, left, right);
                return (double)left >= (double)right;
            case TokenKind.LESS:
                CheckNumbers(binary.Operator, left, right);
                return (double)left < (double)right;
            case TokenKind.LESS_EQUAL:
                CheckNumbers(binary.Operator, left, right);
                return (double)left <= (double)right;

            case TokenKind.EQUAL_EQUAL: return IsEqual(left, right);
            case TokenKind.BANG_EQUAL: return !IsEqual(left, right);

            default: return null;
        }
    }

    public object? VisitGrouping(IExpr.Grouping grouping) => Evaluate(grouping.Expr);

    public object? VisitLiteral(IExpr.Literal literal) => literal.Value;

    #endregion

    public bool VisitExpression(IStatement.Expression expr)
    {
        try
        {
            Evaluate(expr.Expr);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool VisitPrint(IStatement.Print print)
    {
        try
        {
            var val = Evaluate(print.Expr);
            Console.WriteLine(val);
            return true;
        }
        catch
        {
            return false;
        }
    }
}