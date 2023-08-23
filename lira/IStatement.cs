namespace Lira;

public interface IStatement
{
    
    public interface IVisitor<T>
    {
        T VisitExpression(Expression expr);
        T VisitPrint(Print print);
    }
    
    public IExpr Expr { get; }
    public T Accept<T>(IVisitor<T> visitor);
    
    public class Expression : IStatement
    {
        public IExpr Expr { get; }
        public Expression(IExpr expr)
        {
            this.Expr = expr;
        }

        public T Accept<T>(IVisitor<T> visitor) => visitor.VisitExpression(this);
    }
    
    public class Print : IStatement
    {
        public IExpr Expr { get; }
        public Print(IExpr expr)
        {
            this.Expr = expr;
        }

        public T Accept<T>(IVisitor<T> visitor) => visitor.VisitPrint(this);
    }
}