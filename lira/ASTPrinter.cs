using System.Text;

namespace Lira;

public class ASTPrinter : IExpr.IVisitor<string>
{

    public string Print(IExpr expr) => expr.Accept(this);

    private String Parenthesize(String name, params IExpr[] exprs)
    {
        StringBuilder builder = new();
        builder.Append($"({name}");
        foreach (IExpr expr in exprs)
        {
            builder.Append(" ");
            builder.Append(expr.Accept(this));
        }

        builder.Append(")");

        return builder.ToString();
    }

    public string VisitBinary(IExpr.Binary binary) => Parenthesize(binary.Operator.Lexeme, binary.Left, binary.Right);

    public string VisitGrouping(IExpr.Grouping grouping) => Parenthesize("group", grouping.Expr);

    public string VisitLiteral(IExpr.Literal literal) => literal.Value is null ? "nil" : $"{literal.Value}";

    public string VisitVariable(IExpr.Variable variable) => $"var {variable.Identifier.Lexeme}";

    public string VisitUnary(IExpr.Unary unary) => Parenthesize(unary.Operator.Lexeme, unary.Right);

    public string VisitAssignment(IExpr.Assignment assignment) => $"{assignment.Identifier.Lexeme} = {assignment.Value}";

    public string VisitLogical(IExpr.Logical logical) => $"{logical.Left} {logical.Operator} {logical.Right}";
}