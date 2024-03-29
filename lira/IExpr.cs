namespace Lira;

public interface IExpr
{
    public interface IVisitor<T>
    {
        T VisitUnary(Unary unary);
        T VisitBinary(Binary binary);
        T VisitGrouping(Grouping grouping);
        T VisitLiteral(Literal literal);
        T VisitVariable(Variable variable);
        T VisitAssignment(Assignment assignment);
        T VisitLogical(Logical logical);
        T VisitFunctionCall(FunctionCall call);
    }

    public T Accept<T>(IVisitor<T> visitor);

    public struct Variable : IExpr
    {
        public Token Identifier { get; }

        public Variable(Token identifier)
        {
            this.Identifier = identifier;
        }

        public T Accept<T>(IVisitor<T> visitor) => visitor.VisitVariable(this);
    }

    public struct Assignment : IExpr
    {
        public Token Identifier { get; }
        public IExpr Value { get; }

        public Assignment(Token identifier, IExpr value)
        {
            this.Identifier = identifier;
            this.Value = value;
        }

        public T Accept<T>(IVisitor<T> visitor) => visitor.VisitAssignment(this);
    }

    public struct Logical : IExpr
    {
        public IExpr Left { get; }
        public Token @Operator { get; }
        public IExpr Right { get; }
        public Logical(IExpr Left, Token Operator, IExpr Right)
        {
            this.Left = Left;
            this.@Operator = @Operator;
            this.Right = Right;
        }

        public R Accept<R>(IVisitor<R> visitor) => visitor.VisitLogical(this);
    }

    public struct Unary : IExpr
    {
        public Token @Operator { get; }
        public IExpr Right { get; }

        public Unary(Token @Operator, IExpr Right)
        {
            this.@Operator = @Operator;
            this.Right = Right;
        }

        public R Accept<R>(IVisitor<R> visitor) => visitor.VisitUnary(this);
    }

    public struct Binary : IExpr
    {
        public IExpr Left { get; }
        public Token @Operator { get; }
        public IExpr Right { get; }

        public Binary(IExpr Left, Token @Operator, IExpr Right)
        {
            this.Left = Left;
            this.@Operator = @Operator;
            this.Right = Right;
        }

        public R Accept<R>(IVisitor<R> visitor) => visitor.VisitBinary(this);
    }

    public struct FunctionCall : IExpr
    {
        public IExpr Identifier { get; }
        /// <summary>
        /// Used for reporting runtime errors
        /// </summary>
        public Token ClosingParenthesis { get; }
        public List<IExpr> Arguments { get; }

        public FunctionCall(IExpr Identifier, Token ClosingParenthesis, List<IExpr> Arguments)
        {
            this.Identifier = Identifier;
            this.ClosingParenthesis = ClosingParenthesis;
            this.Arguments = Arguments;
        }

        public R Accept<R>(IVisitor<R> visitor) => visitor.VisitFunctionCall(this);
    }

    public struct Grouping : IExpr
    {
        public IExpr Expr { get; }

        public Grouping(IExpr Expr)
        {
            this.Expr = Expr;
        }

        public R Accept<R>(IVisitor<R> visitor) => visitor.VisitGrouping(this);
    }

    public struct Literal : IExpr
    {
        public Object? Value { get; }

        public Literal(Object? Value)
        {
            this.Value = Value;
        }

        public R Accept<R>(IVisitor<R> visitor) => visitor.VisitLiteral(this);
    }
}