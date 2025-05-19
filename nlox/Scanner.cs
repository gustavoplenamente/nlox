namespace nlox;

public interface IScanner
{
    public List<Token> ScanTokens();
}

public class Scanner : IScanner
{
    private string Source { get; }
    private List<Token> Tokens { get; } = [];
    private int Start { get; set; }
    private int Current { get; set; }
    private int Line { get; set; } = 1;

    public Scanner(string source)
    {
        Source = source;
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            Start = Current;
            ScanToken();
        }

        Tokens.Add(new Token(TokenKind.Eof, "", null, Line));
        return Tokens;
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
                Line++;
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

                Lox.Error(Line, $"Unexpected character: '{c}'.");
                break;
        }
    }

    private void ScanString()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n') Line++;
            Advance();
        }

        if (IsAtEnd())
        {
            Lox.Error(Line, $"Unterminated string.");
            return;
        }

        // Capture the closing quotes
        Advance();

        // Get text inside quotes
        var str = Source.Substring(Start + 1, Current - Start - 1);
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

        var numStr = Source.Substring(Start, Current - Start);
        var num = double.Parse(numStr);
        AddToken(TokenKind.Number, num);
    }

    private void AddToken(TokenKind kind, object? literal = null)
    {
        var text = Source.Substring(Start, Current - Start);
        var token = new Token(kind, text, literal, Line);
        Tokens.Add(token);
    }

    private bool Match(char expected)
    {
        if (IsAtEnd()) return false;
        if (Peek() != expected) return false;

        Advance();
        return true;
    }

    private char Advance() => Source[Current++];

    private char Peek() => IsAtEnd() ? '\0' : Source[Current];
    private char PeekNext() => Current + 1 >= Source.Length ? '\0' : Source[Current + 1];

    private bool IsAtEnd() => Current >= Source.Length;

    private static bool IsDigit(char c) => c is >= '0' and <= '9';
}