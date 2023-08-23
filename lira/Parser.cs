namespace Lira;

public class Parser
{
    public class ParserError : Exception {}

    private readonly List<Token> _tokens;
    private int _current = 0;

    public Parser(List<Token> tokens)
    {
        this._tokens = tokens;
    }

    public List<IStatement> Parse()
    {
        List<IStatement> statements = new();
        try
        {
            while (!IsAtEOF) statements.Add(Statement());
            return statements;
        }
        catch (ParserError e)
        {
            Lira.Error(-1, e.Message);
            return statements;
        }
    }

    private ParserError Error(Token token, string message)
    {
        Lira.Error(token.Line, message);
        return new();
    }

    private bool IsAtEOF => _current >= _tokens.Count;
    
    #region Helpers

    private Token Peek() => _tokens[_current];
    private Token Previous() => _tokens[_current - 1];
    private Token Advance() => !IsAtEOF ? _tokens[_current++] : _tokens[_current];

    private Token Consume(TokenKind kind, string message)
    {
        if (Check(kind)) return Advance();
        throw new ParserError();
    }

    private bool Check(TokenKind kind) => !IsAtEOF && Peek().Kind == kind;

    private bool Match(params TokenKind[] kinds)
    {
        foreach (TokenKind kind in kinds)
        {
            if (Check(kind))
            {
                Advance();
                return true;
            }
        }
        return false;
    }
    
    #endregion

    #region Statements
    
    private IStatement Statement()
    {
        if (Match(TokenKind.PRINT)) return PrintStatement();
        else return ExpressionStatement();
    }

    private IStatement.Print PrintStatement()
    {
        IExpr val = Expression();
        Consume(TokenKind.SEMICOLON, "Expected ';' after expression.");
        return new(val);
    }
    
    private IStatement.Expression ExpressionStatement()
    {
        IExpr val = Expression();
        Consume(TokenKind.SEMICOLON, "Expected ';' after expression.");
        return new(val);
    }
    
    #endregion

    #region Expressions
    
    private IExpr Expression() => Equality();

    private IExpr Equality()
    {
        IExpr expr = Comparison();
        while (Match(TokenKind.BANG_EQUAL, TokenKind.EQUAL_EQUAL))
        {
            Token @operator = Previous();
            IExpr right = Comparison();
            expr = new IExpr.Binary(expr, @operator, right);
        }
        return expr;
    }

    private IExpr Comparison()
    {
        IExpr expr = Term();
        while (Match(TokenKind.LESS, TokenKind.GREATER, TokenKind.LESS_EQUAL, TokenKind.GREATER_EQUAL))
        {
            Token @operator = Previous();
            IExpr right = Term();
            expr = new IExpr.Binary(expr, @operator, right);
        }
        return expr;
    }

    private IExpr Term()
    {
        IExpr expr = Factor();
        while (Match(TokenKind.MINUS, TokenKind.PLUS))
        {
            Token @operator = Previous();
            IExpr right = Factor();
            expr = new IExpr.Binary(expr, @operator, right);
        }
        return expr;
    }

    private IExpr Factor()
    {
        IExpr expr = Unary();
        while (Match(TokenKind.SLASH, TokenKind.STAR))
        {
            Token @operator = Previous();
            IExpr right = Unary();
            expr = new IExpr.Binary(expr, @operator, right);
        }
        return expr;
    }

    private IExpr Unary()
    {
        if (Match(TokenKind.BANG, TokenKind.MINUS))
        {
            Token @operator = Previous();
            IExpr right = Unary();
            return new IExpr.Unary(@operator, right);
        }
        return Primary();
    }

    private IExpr Primary()
    {
        if (Match(TokenKind.FALSE)) return new IExpr.Literal(false);
        if (Match(TokenKind.TRUE)) return new IExpr.Literal(true);
        if (Match(TokenKind.NIL)) return new IExpr.Literal(null);

        if (Match(TokenKind.NUMBER, TokenKind.STRING)) return new IExpr.Literal(Previous().Literal!);

        if (Match(TokenKind.LEFT_PAREN))
        {
            IExpr expr = Expression();
            Consume(TokenKind.RIGHT_PAREN, "Expect ')' after expression.");
            return new IExpr.Grouping(expr);
        }
        throw Error(Peek(), "Expected expression.");
    }
    
    #endregion

    /// <summary>
    /// Returns the parser to valid syntax upon encountering an error.
    /// </summary>
    private void Synchronize()
    {
        Advance();

        while (!IsAtEOF)
        {
            if (Previous().Kind == TokenKind.SEMICOLON) return;
            switch (Peek().Kind)
            {
                case TokenKind.CLASS:
                case TokenKind.FUNC:
                case TokenKind.VAR:
                case TokenKind.IF:
                case TokenKind.FOR:
                case TokenKind.WHILE:
                case TokenKind.PRINT:
                case TokenKind.RETURN:
                    return;
            }

            Advance();
        }
    }
}