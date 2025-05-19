using nlox.Tests.Dependencies;

namespace nlox.Tests;

public class Tests
{
    [Test]
    public void should_scan_script()
    {
        var text = File.ReadAllText("./Examples/script.lox");
        var scanner = new Scanner(text);
        var testWriter = new TestWriter();
        var lox = new Lox(scanner, testWriter);
        lox.Run();

        Assert.That(testWriter.WriteCount, Is.EqualTo(46));
        Assert.That(scanner.Line, Is.EqualTo(23));
    }
}