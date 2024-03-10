namespace Lira;

public interface IStatement
{

    public interface IVisitor<T>
    {
        T VisitExpression(Expression expr);
        T VisitPrint(Print print);
        T VisitVariable(Variable variable);
        T VisitBlock(Block block);
        T VisitIf(If ifStatement);
        T VisitWhile(While whileStatement);
        T VisitFunction(Function function);
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

    public struct Function : IStatement
    {
        public Token Identifier { get; }
        public List<Token> Parameters { get; }
        public List<IStatement> Body { get; }
        public Function(Token Identifier, List<Token> Parameters, List<IStatement> Body)
        {
            this.Identifier = Identifier;
            this.Parameters = Parameters;
            this.Body = Body;
        }

        public T Accept<T>(IVisitor<T> visitor) => visitor.VisitFunction(this);
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

    public struct If : IStatement
    {
        public IExpr Condition { get; }
        public IStatement ThenBranch { get; }
        public IStatement? ElseBranch { get; }
        public If(IExpr Condition, IStatement ThenBranch, IStatement? ElseBranch)
        {
            this.Condition = Condition;
            this.ThenBranch = ThenBranch;
            this.ElseBranch = ElseBranch;
        }

        public T Accept<T>(IVisitor<T> visitor) => visitor.VisitIf(this);
    }


    public struct While : IStatement
    {
        public IExpr Condition { get; }
        public IStatement Body { get; }
        public While(IExpr Condition, IStatement Body)
        {
            this.Condition = Condition;
            this.Body = Body;
        }

        public T Accept<T>(IVisitor<T> visitor) => visitor.VisitWhile(this);
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