using System.Runtime.InteropServices;

namespace Lira;

public class Interpreter : IExpr.IVisitor<object?>, IStatement.IVisitor<bool>
{
    /// <summary>
    /// Use this environment to define native functions and variables.
    /// </summary>
    public readonly Environment Globals = new();
    private Environment Environment;

    public Interpreter()
    {
        this.Environment = Globals;
    }

    public void Interpret(List<IStatement> statements)
    {
        try
        {
            foreach (IStatement statement in statements) Execute(statement);
        }
        catch (Exception e)
        {
            Lira.Error(-1, "Interpreting", e.Message);
        }
    }

    private void Execute(IStatement statement) => statement.Accept(this);

    public void ExecuteBlock(List<IStatement> statements, Environment environment)
    {
        Environment prev = this.Environment;
        try
        {
            this.Environment = environment;
            foreach (IStatement statement in statements) Execute(statement);
        }
        finally
        {
            this.Environment = prev;
        }
    }

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

    #region Expression Visitor Interface

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
                throw new RuntimeError(binary.Operator, "Operator can only be used on defined numbers or strings.");

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

    public object? VisitVariable(IExpr.Variable variable) => Environment.Get(variable.Identifier);

    public object? VisitAssignment(IExpr.Assignment assignment)
    {
        var val = Evaluate(assignment.Value);
        Environment.Assign(assignment.Identifier, val);
        return val;
    }

    public object? VisitLogical(IExpr.Logical logical)
    {
        object? left = Evaluate(logical.Left);
        if (logical.Operator.Kind == TokenKind.OR)
        {
            if (IsTruthy(left)) return left;
        }
        else
        {
            if (!IsTruthy(left)) return left;
        }
        return Evaluate(logical.Right);
    }

    public object? VisitFunctionCall(IExpr.FunctionCall call)
    {
        object? identifier = Evaluate(call.Identifier);

        List<object?> arguments = new();
        foreach (IExpr expr in call.Arguments)
        {
            arguments.Add(Evaluate(expr));
        }

        // NOTE: The book also throws runtime errors when arguments length doesn't match function's arity.
        // Instead, the Parser will throw an error.
        if (identifier is not ILiraCallable function)
        {
            throw new RuntimeError(call.ClosingParenthesis, "Can only call functions and classes.");
        }

        return function.Call(this, arguments);
    }

    #endregion

    #region Statement Visitor Interface

    public bool VisitExpression(IStatement.Expression expr)
    {
        Evaluate(expr.Expr);
        return true;
    }

    public bool VisitPrint(IStatement.Print print)
    {
        var val = Evaluate(print.Expr);
        if (val is null) val = "nil";
        Console.WriteLine(val);
        return true;
    }

    public bool VisitVariable(IStatement.Variable variable)
    {
        var val = Evaluate(variable.Expr);
        Environment.Define(variable.Identifier.Lexeme, val);
        return true;
    }

    public bool VisitBlock(IStatement.Block block)
    {
        ExecuteBlock(block.Statements, new Environment(this.Environment));
        return true;
    }

    public bool VisitIf(IStatement.If ifStatement)
    {
        if (IsTruthy(Evaluate(ifStatement.Condition)))
        {
            Execute(ifStatement.ThenBranch);
        }
        else if (ifStatement.ElseBranch is not null)
        {
            Execute(ifStatement.ElseBranch);
        }
        return true;
    }

    public bool VisitWhile(IStatement.While whileStatement)
    {
        while (IsTruthy(Evaluate(whileStatement.Condition)))
        {
            Execute(whileStatement.Body);
        }
        return true;
    }

    public bool VisitFunction(IStatement.Function function)
    {
        ILiraCallable.LiraFunction fn = new(function, Environment);
        Environment.Define(function.Identifier.Lexeme, fn);
        return true;
    }

    public bool VisitReturn(IStatement.Return returnStatement)
    {
        object? returned = null;
        if (returnStatement.Value is not null)
        {
            returned = Evaluate(returnStatement.Value);
        }
        throw new Return(returned);
    }

    #endregion
}