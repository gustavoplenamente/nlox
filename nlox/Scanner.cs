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

            case '!':
                AddToken(Match('=') ? TokenKind.BangEqual : TokenKind.Bang);
                break;
            case '=':
                AddToken(Match('=') ? TokenKind.EqualEqual : TokenKind.Equal);
                break;
            case '>':
                AddToken(Match('=') ? TokenKind.GreaterEqual : TokenKind.Greater);
                break;
            case '<':
                AddToken(Match('=') ? TokenKind.LessEqual : TokenKind.Less);
                break;
            
            case '/':
                if (Match('/')) // this is a comment
                {
                    while(Peek() != '\n' && !IsAtEnd())
                        Advance();
                }
                else
                {
                    AddToken(TokenKind.Slash);
                }
                break;

            case ' ':
            case '\r':
            case '\t':
                break;

            case '\n':
                _line++;
                break;

            case '"':
                ScanString();
                break;

            default:
                if (IsDigit(c))
                {
                    ScanNumber();
                    break;
                }

                Lox.Error(_line, $"Unexpected character: '{c}'.");
                break;
        }
    }

    private void ScanString()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n') _line++;
            Advance();
        }

        if (IsAtEnd())
        {
            Lox.Error(_line, $"Unterminated string.");
            return;
        }

        // Capture the closing quotes
        Advance();

        // Get text inside quotes
        var str = _source.Substring(_start + 1, _current - _start - 1);
        AddToken(TokenKind.String, str);
    }

    private void ScanNumber()
    {
        while (IsDigit(Peek()))
            Advance();

        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            // Consume the dot
            Advance();

            while (IsDigit(Peek()))
                Advance();
        }

        var numStr = _source.Substring(_start, _current - _start);
        var num = double.Parse(numStr);
        AddToken(TokenKind.Number, num);
    }

    private void AddToken(TokenKind kind, object? literal = null)
    {
        var text = _source.Substring(_start, _current - _start);
        var token = new Token(kind, text, literal, _line);
        _tokens.Add(token);
    }

    private bool Match(char expected)
    {
        if (IsAtEnd()) return false;
        if (Peek() != expected) return false;

        Advance();
        return true;
    }

    private char Advance() => _source[_current++];

    private char Peek() => IsAtEnd() ? '\0' : _source[_current];
    private char PeekNext() => _current + 1 >= _source.Length ? '\0' : _source[_current + 1];

    private bool IsAtEnd() => _current >= _source.Length;

    private static bool IsDigit(char c) => c is >= '0' and <= '9';
}