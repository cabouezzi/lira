namespace Lira;

public sealed class Token
{
	public TokenKind Kind { get; }
	public string Lexeme { get; }
	public object? Literal { get; }
	public int Line { get; }

	public Token(TokenKind type, string lexeme, int line, object? literal = null)
	{
		this.Kind = type;
		this.Lexeme = lexeme;
		this.Literal = literal;
		this.Line = line;
	}

	override public string ToString() => $"[{Kind} {Lexeme} {Literal}]";
}
