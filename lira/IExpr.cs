namespace Lira;

public interface IExpr
{
	public interface IVisitor<T>
    {
	    T VisitUnary(Unary unary);
		T VisitBinary(Binary binary);
		T VisitGrouping(Grouping grouping);
		T VisitLiteral(Literal literal);
	}

	public R Accept<R>(IVisitor<R> visitor);

	public struct Unary : IExpr
	{
		public Token @Operator { get; }
		public IExpr Right { get; }
		public Unary(Token @Operator,IExpr Right)
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
		public Binary(IExpr Left,Token @Operator,IExpr Right)
		{
			this.Left = Left;
			this.@Operator = @Operator;
			this.Right = Right;
		}

		public R Accept<R>(IVisitor<R> visitor) => visitor.VisitBinary(this);
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