namespace Lira;

public interface IStatement
{

    public interface IVisitor<T>
    {
        T VisitExpression(Expression expr);
        T VisitPrint(Print print);
        T VisitVariable(Variable variable);
        T VisitBlock(Block block);
    }

    public T Accept<T>(IVisitor<T> visitor);

    public struct Variable : IStatement
    {
        public Token Identifier { get; }
        public IExpr Expr { get; }

        public Variable(Token identifier, IExpr expr)
        {
            this.Identifier = identifier;
            this.Expr = expr;
        }

        public T Accept<T>(IVisitor<T> visitor) => visitor.VisitVariable(this);
    }

    public struct Expression : IStatement
    {
        public IExpr Expr { get; }
        public Expression(IExpr expr)
        {
            this.Expr = expr;
        }

        public T Accept<T>(IVisitor<T> visitor) => visitor.VisitExpression(this);
    }

    public struct Print : IStatement
    {
        public IExpr Expr { get; }
        public Print(IExpr expr)
        {
            this.Expr = expr;
        }

        public T Accept<T>(IVisitor<T> visitor) => visitor.VisitPrint(this);
    }

    public struct Block : IStatement
    {
        public List<IStatement> Statements { get; }
        public Block(List<IStatement> statements)
        {
            this.Statements = statements;
        }

        public T Accept<T>(IVisitor<T> visitor) => visitor.VisitBlock(this);
    }
}