using System.Text;
using VanillaSugar.Ini;

namespace Test.IniFile;

[TestFixture]
[TestOf(typeof(IniSerializer))]
public class IniSerializerTest
{
    [Test]
    public void SaveNormal()
    {
        Ini ini = new Ini();
        Section sec1 = ini.GetOrAdd("Sec1");
        sec1.AddComments("Comment before key1", "and Value1");
        sec1.Set("key1", "value1");
        sec1.AddComments("Comment before key2", "and Value2");
        sec1.Set("key2", "value2");
        sec1.Set("key3", "value3");
        Section sec2 = ini.GetOrAdd("Sec2");
        sec2.Set("key4", "value4");
        sec2.Set("key5", "value5");

        IniSerializer serializer = new IniSerializer();

        using (StringWriter writer = new StringWriter())
        {
            serializer.LineSeparator = "\n";
            serializer.AddSpaceAroundEqualizer = true;
            serializer.AddSpaceBeforeComment = true;
            serializer.Write(ini, writer);
            string result = TrimTailNewLine(writer);
            string expected = ExampleContents.Normal;
            Assert.AreEqual(expected, result);
        }

        using (StringWriter writer = new StringWriter())
        {
            serializer.LineSeparator = "\n";
            serializer.AddSpaceAroundEqualizer = false;
            serializer.AddSpaceBeforeComment = false;
            serializer.CommentPrefix = "#";
            serializer.Write(ini, writer);
            string result = TrimTailNewLine(writer);
            string expected = ExampleContents.Normal
                .Replace("; ", "#")
                .Replace(" = ", "=");
            Assert.AreEqual(expected, result);
        }
    }

    [Test]
    public void SaveTopDangling()
    {
        Ini ini = new Ini();
        Section sec1 = ini.GetOrAdd("Sec1");
        sec1.DanglingText = "  Dangling Content In Sec1\n\n  Next dangling line";
        sec1.AddComments("Comment1 before key1");
        sec1.Set("key1", "value1");
        sec1.Set("key2", "value2");
        ini.GetOrAdd("Sec2");

        IniSerializer serializer = new IniSerializer();
        using StringWriter writer = new StringWriter();
        serializer.LineSeparator = "\n";
        serializer.AddSpaceAroundEqualizer = true;
        serializer.AddSpaceBeforeComment = true;
        serializer.Write(ini, writer);
        string result = TrimTailNewLine(writer);
        string expected = ExampleContents.TopDangling;
        Assert.AreEqual(expected, result);
    }

    private static string TrimTailNewLine(StringWriter writer)
    {
        StringBuilder buffer = writer.GetStringBuilder();
        if (buffer[^1] == '\r')
        {
            buffer.Remove(buffer.Length - 1, 1);
        }

        if (buffer[^1] == '\n')
        {
            buffer.Remove(buffer.Length - 1, 1);
        }

        return buffer.ToString();
    }
}