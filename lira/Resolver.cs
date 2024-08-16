using System.Linq.Expressions;

namespace Lira;

public class Resolver : IExpr.IVisitor<bool>, IStatement.IVisitor<bool>
{
    private Interpreter _interpreter { get; }
    private readonly Stack<Dictionary<String, bool>> scopes = new();

    public Resolver(Interpreter interpreter)
    {
        this._interpreter = interpreter;
    }

    public void BeginScope()
    {
        scopes.Push(new());
    }

    public void EndScope()
    {
        scopes.Pop();
    }

    public void Resolve(List<IStatement> statements)
    {
        foreach (IStatement statement in statements) Resolve(statement);
    }

    private void Resolve(IStatement statement)
    {
        statement.Accept(this);
    }

    private void Resolve(IExpr expr)
    {
        expr.Accept(this);
    }

    private void ResolveLocal(IExpr expr, Token identifier)
    {
        for (int i = scopes.Count - 1; i >= 0; i--)
        {
            if (scopes.ElementAt(i).ContainsKey(identifier.Lexeme))
            {
                _interpreter.Resolve(expr, scopes.Count - 1 - i);
                return;
            }
        }
    }

    private void ResolveFunction(IStatement.Function function)
    {
        BeginScope();
        foreach (Token param in function.Parameters)
        {
            Declare(param);
            Define(param);
        }
        Resolve(function.Body);
        EndScope();
    }

    private void Declare(Token identifier)
    {
        if (scopes.Count == 0) return;
        Dictionary<String, bool> scope = scopes.Peek();
        if (scope.ContainsKey(identifier.Lexeme))
        {
            Lira.Error(-1, "Resolving", $"Variable with name {identifier.Lexeme} already declared in this scope.");
        }
        scope.Add(identifier.Lexeme, false);
    }

    private void Define(Token identifier)
    {
        if (scopes.Count == 0) return;
        Dictionary<String, bool> scope = scopes.Peek();
        scope.Add(identifier.Lexeme, true);
    }

    public bool VisitUnary(IExpr.Unary unary)
    {
        Resolve(unary.Right);
        return true;
    }

    public bool VisitBinary(IExpr.Binary binary)
    {
        Resolve(binary.Left);
        Resolve(binary.Right);
        return true;
    }

    public bool VisitGrouping(IExpr.Grouping grouping)
    {
        Resolve(grouping.Expr);
        return true;
    }

    public bool VisitLiteral(IExpr.Literal literal)
    {
        return true;
    }

    public bool VisitVariable(IExpr.Variable variable)
    {
        if (scopes.Count != 0 && scopes.Peek()[variable.Identifier.Lexeme] == false)
        {
            Lira.Error(-1, "Resolving", "Can't read local variable in its own initializer.");
        }
        ResolveLocal(variable, variable.Identifier);
        return true;
    }

    public bool VisitAssignment(IExpr.Assignment assignment)
    {
        Resolve(assignment.Value);
        ResolveLocal(assignment, assignment.Identifier);
        return true;
    }

    public bool VisitLogical(IExpr.Logical logical)
    {
        Resolve(logical.Left);
        Resolve(logical.Right);
        return true;
    }

    public bool VisitFunctionCall(IExpr.FunctionCall call)
    {
        Resolve(call.Identifier);
        foreach (IExpr argument in call.Arguments)
        {
            Resolve(argument);
        }
        return true;
    }

    public bool VisitExpression(IStatement.Expression expr)
    {
        Resolve(expr.Expr);
        return true;
    }

    public bool VisitPrint(IStatement.Print print)
    {
        Resolve(print.Expr);
        return true;
    }

    public bool VisitVariable(IStatement.Variable variable)
    {
        Declare(variable.Identifier);
        Resolve(variable.Expr);
        Define(variable.Identifier);
        return true;
    }

    public bool VisitBlock(IStatement.Block block)
    {
        BeginScope();
        Resolve(block.Statements);
        EndScope();
        return true;
    }

    public bool VisitIf(IStatement.If ifStatement)
    {
        Resolve(ifStatement.Condition);
        Resolve(ifStatement.ThenBranch);
        if (ifStatement.ElseBranch is not null)
        {
            Resolve(ifStatement.ElseBranch);
        }
        return true;
    }

    public bool VisitWhile(IStatement.While whileStatement)
    {
        Resolve(whileStatement.Condition);
        Resolve(whileStatement.Body);
        return true;
    }

    public bool VisitFunction(IStatement.Function function)
    {
        Declare(function.Identifier);
        Define(function.Identifier);
        ResolveFunction(function);
        return true;
    }

    public bool VisitReturn(IStatement.Return returnStatement)
    {
        if (returnStatement.Value is not null)
        {
            Resolve(returnStatement.Value);
        }
        return true;
    }
}