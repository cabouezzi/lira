namespace Lira;

public partial class Scanner
{
    static private Dictionary<String, TokenKind> _keywords = new Dictionary<string, TokenKind>();
    static Scanner()
    {
        _keywords.Add("and", TokenKind.AND);
        _keywords.Add("class", TokenKind.CLASS);
        _keywords.Add("else", TokenKind.ELSE);
        _keywords.Add("false", TokenKind.FALSE);
        _keywords.Add("func", TokenKind.FUNC);
        _keywords.Add("for", TokenKind.FOR);
        _keywords.Add("if", TokenKind.IF);
        _keywords.Add("nil", TokenKind.NIL);
        _keywords.Add("or", TokenKind.OR);
        _keywords.Add("print", TokenKind.PRINT);
        _keywords.Add("return", TokenKind.RETURN);
        _keywords.Add("super", TokenKind.SUPER);
        _keywords.Add("this", TokenKind.THIS);
        _keywords.Add("true", TokenKind.TRUE);
        _keywords.Add("var", TokenKind.VAR);
        _keywords.Add("while", TokenKind.WHILE);
    }
}