namespace nlox;

public interface IScanner
{
    public List<Token> ScanTokens();
}

public class Scanner : IScanner
{
    private readonly string _source;
    private readonly List<Token> _tokens = [];
    private int _start = 0;
    private int _current = 0;
    private int _line = 1;

    public Scanner(string source)
    {
        _source = source;
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            _start = _current;
            ScanToken();
        }

        _tokens.Add(new Token(TokenKind.Eof, "", null, _line));
        return _tokens;
    }

    private void ScanToken()
    {
        var c = Advance();

        switch (c)
        {
            case '(': AddToken(TokenKind.LeftParen); break;
            case ')': AddToken(TokenKind.RightParen); break;
            case '{': AddToken(TokenKind.LeftBrace); break;
            case '}': AddToken(TokenKind.RightBrace); break;
            case ',': AddToken(TokenKind.Comma); break;
            case '.': AddToken(TokenKind.Dot); break;
            case '-': AddToken(TokenKind.Minus); break;
            case '+': AddToken(TokenKind.Plus); break;
            case ';': AddToken(TokenKind.Semicolon); break;
            case '*': AddToken(TokenKind.Star); break;
            
            default:
                Lox.Error(_line, $"Unexpected character: '{c}'.");
                break;
        }
    }

    private void AddToken(TokenKind kind, object? literal = null)
    {
        var text = _source.Substring(_start, _current);
        var token = new Token(kind, text, literal, _line);
        _tokens.Add(token);
    }

    private char Advance() => _source[_current++];

    private bool IsAtEnd() => _current >= _source.Length;
}