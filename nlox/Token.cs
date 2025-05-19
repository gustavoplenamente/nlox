namespace nlox;

public record Token(TokenKind Kind, string Lexeme, object? Literal, int Line)
{
    public override string ToString() => $"{Kind} {Lexeme} {Literal}";
}

public enum TokenKind
{
    // Single-character tokens
    LeftParen,
    RightParen,
    LeftBrace,
    RightBrace,
    Comma,
    Dot,
    Minus,
    Plus,
    Semicolon,
    Slash,
    Star,
    
    // One or two character tokens
    Bang,
    BangEqual,
    Equal,
    EqualEqual,
    Greater,
    GreaterEqual,
    Less,
    LessEqual,
    
    // Literals
    Identifier,
    String,
    Number,
    
    // Keywords
    And,
    Or,
    Class,
    If,
    Else,
    True,
    False,
    Nil,
    Print,
    Return,
    Super,
    This,
    Var,
    For,
    While,
    
    Eof,
}