namespace nlox.Tests.Dependencies;

public class TestWriter : IWriter
{
    public int WriteCount { get; private set; }

    public void Write(Token token)
    {
        WriteCount++;
    }
}