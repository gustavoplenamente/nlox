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
    private Dictionary<string, TokenKind> KeywordsByIdentifier { get; } = new()
    {
        { "and", TokenKind.And },
        { "class", TokenKind.Class },
        { "else", TokenKind.Else },
        { "false", TokenKind.False },
        { "for", TokenKind.For },
        { "fun", TokenKind.Fun },
        { "if", TokenKind.If },
        { "nil", TokenKind.Nil },
        { "or", TokenKind.Or },
        { "print", TokenKind.Print },
        { "return", TokenKind.Return },
        { "super", TokenKind.Super },
        { "this", TokenKind.This },
        { "true", TokenKind.True },
        { "var", TokenKind.Var },
        { "while", TokenKind.While }
    };

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
                if (Match('/')) ScanComment();
                else AddToken(TokenKind.Slash);
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

                if (IsAlpha(c))
                {
                    ScanIdentifier();
                    break;
                }

                Lox.Error(Line, $"Unexpected character: '{c}'.");
                break;
        }
    }

    private void ScanComment()
    {
        while (Peek() != '\n' && !IsAtEnd())
            Advance();
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

    private void ScanIdentifier()
    {
        while (IsAlphanumeric(Peek()))
            Advance();

        var text = Source.Substring(Start, Current - Start);

        if (KeywordsByIdentifier.TryGetValue(text, out var keyword))
        {
            AddToken(keyword);
            return;
        }

        AddToken(TokenKind.Identifier);
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

    private static bool IsAlpha(char c)
    {
        return c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_';
    }

    private static bool IsAlphanumeric(char c)
    {
        return IsAlpha(c) || IsDigit(c);
    }
}
