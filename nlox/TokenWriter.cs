namespace nlox;

public interface IWriter
{
    void Write(Token token);
}

public class TokenWriter : IWriter
{
    public void Write(Token token)
    {
        Console.WriteLine(token);
    }
}