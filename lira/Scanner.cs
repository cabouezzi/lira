using System;

namespace Lira;

public class Scanner
{

    private readonly string _source;
    private readonly List<Token> _tokens = new();

    private int _start = 0;
    private int _current = 0;
    private int _line = 1;

    public Scanner(string source)
    {
        this._source = source;
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEOF)
        {
            _start = _current;
            ScanToken();
        }

        return _tokens;
    }

    private bool IsAtEOF => _current >= _source.Length;

    private void ScanToken()
    {
        char c = Advance();
        switch (c)
        {
            case '+': AddToken(TokenKind.PLUS); break;
            case '-': AddToken(TokenKind.MINUS); break;
            case '*': AddToken(TokenKind.STAR); break;
            case '/': AddToken(TokenKind.SLASH); break;
            // TODO: Throw error
            default: break;
        }
    }

    private void AddToken(TokenKind kind, object? literal = null)
    {
        string text = _source.Substring(_start, _current-_start);
        Token newToken = new Token(kind, text, _line, literal);
        _tokens.Add(newToken);
    }

    #region Character Navigation

    private char Advance() => _source[_current++];
    private char Peek() => _source[_current];
    private char PeekNext() => _source[_current++];

    #endregion

}
