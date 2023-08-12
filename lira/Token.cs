using System;

namespace Lira;
	
public sealed class Token
{
	readonly TokenKind Kind;
	readonly string Lexeme;
	readonly object? Literal;
	readonly int Line;

	public Token(TokenKind type, string lexeme, int line, object? literal = null)
	{
		this.Kind = type;
		this.Lexeme = lexeme;
		this.Literal = literal;
		this.Line = line;
	}

	override public string ToString() => $"{Kind} {Lexeme} {Literal}";
}
