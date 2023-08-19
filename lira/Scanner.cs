namespace Lira;

public partial class Scanner
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
            case '(': AddToken(TokenKind.LEFT_PAREN); break;
            case ')': AddToken(TokenKind.RIGHT_PAREN); break;
            case '{': AddToken(TokenKind.LEFT_BRACE); break;
            case '}': AddToken(TokenKind.RIGHT_BRACE); break;

            case ',': AddToken(TokenKind.COMMA); break;
            case '.': AddToken(TokenKind.DOT); break;
            case ';': AddToken(TokenKind.SEMICOLON); break;

            case '+': AddToken(TokenKind.PLUS); break;
            case '-': AddToken(TokenKind.MINUS); break;
            case '*': AddToken(TokenKind.STAR); break;
            case '/':
                // Skip double slash comments
                if (Match('/'))
                {
                    while (Peek() != '\n' && !IsAtEOF) Advance();
                }
                // Skip block comments
                else if (Match('*'))
                {
                    while (!(Peek() == '*' && PeekNext() == '/') && !IsAtEOF) Advance();
                    // Consume '*/'
                    Advance();
                    Advance();
                }
                else
                {
                    AddToken(TokenKind.SLASH);
                }
                break;

            case '!':
                AddToken(Match('=') ? TokenKind.BANG_EQUAL : TokenKind.BANG);
                break;
            case '=':
                AddToken(Match('=') ? TokenKind.EQUAL_EQUAL : TokenKind.EQUAL);
                break;
            case '<':
                AddToken(Match('=') ? TokenKind.LESS_EQUAL : TokenKind.LESS);
                break;
            case '>':
                AddToken(Match('=') ? TokenKind.GREATER_EQUAL : TokenKind.GREATER);
                break;

            case ' ': case '\r': case '\t': break;
            case '\n': _line++; break;

            case '"': ReadString(); break;

            case 'o':
                if (Match('r')) AddToken(TokenKind.OR);
                break;

            default:
                if (IsDigit(c)) ReadNumber();
                else if (IsAlpha(c)) ReadIdentifier();
                else
                {
                    // TODO: Throw error
                }
                break;

        }
    }

    private void AddToken(TokenKind kind, object? literal = null)
    {
        string text = _source.Substring(_start, _current - _start);
        Token newToken = new(kind, text, _line, literal);
        _tokens.Add(newToken);
    }

    // Identifiers
    private void ReadIdentifier ()
    {
        while (IsAlphaNumeric(Peek())) Advance();
        String text = _source.Substring(_start, _current - _start);
        TokenKind kind = _keywords.TryGetValue(text, out TokenKind value) ? _keywords[text] : TokenKind.IDENTIFIER;
        AddToken((TokenKind)kind);
    }

    private bool IsAlpha (char c)
    {
        // Acceptable initial character for a variable name
        return  (c >= 'a' && c <= 'z') ||
                (c >= 'A' && c <= 'Z') ||
                (c == '_');
    }
    
    private bool IsAlphaNumeric (char c) => IsAlpha(c) || IsDigit(c);

    // Strings 
    private void ReadString ()
    {
        while (Peek() != '"' && !IsAtEOF)
        {
            if (Peek() == '\n') _line++;
            Advance();
        }
        if (IsAtEOF)
        {
            // TODO: Handle unterminated string
            return;
        }
        // Consume closing quote
        Advance();
        // Trim the quotes
        String value = _source.Substring(_start + 1, _current - _start - 2);
        AddToken(TokenKind.STRING, value);
    }

    // Numbers (Float)
    private void ReadNumber ()
    {
        while (IsDigit(Peek())) Advance();
        // Decimals
        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            Advance();
            while (IsDigit(Peek())) Advance();
        }
        double value = double.Parse(_source.Substring(_start, _current - _start)); // Note: This feels like cheating right?
        AddToken(TokenKind.NUMBER, value);
    }

    private bool IsDigit (char c) => c >= '0' && c <= '9';

    #region Character Navigation

    private char Advance() => _source[_current++];
    private char Peek() => (IsAtEOF) ? '\0' : _source[_current];
    private char PeekNext() => (_current + 1 >= _source.Length) ? '\0' : _source[_current + 1];
    private bool Match (char exp)
    {
        if (IsAtEOF) return false;
        if (_source[_current] != exp) return false;
        _current++;
        return true;
    }

    #endregion

}
