using System.Text;

namespace Test.IniFile;

public static class ExampleContents
{
    public static Stream AsStream(string content)
    {
        var stream = new MemoryStream();
        var bytes = Encoding.UTF8.GetBytes(content);
        stream.Write(bytes, 0, bytes.Length);
        stream.Position = 0;
        return stream;
    }

    public static readonly string Normal =
        """
        [Sec1]
        ; Comment before key1
        ; and Value1
        key1 = value1
        ; Comment before key2
        ; and Value2
        key2 = value2
        key3 = value3
        [Sec2]
        key4 = value4
        key5 = value5
        """;

    public static readonly string Abnormal =
        """
        untitled-key1 = untitled-value1
        ; Comment before untitled key2
        and Value2
        untitled key 2 = untitled value 2
        [Sec1]
          Dangling Content In Sec1
        
          Next dangling line
        ; Comment1 before key1
        ;    Comment2 before key1
        key1 = value1
        key2 = value2
            value2 next line
            ; Comment before key3
        key3  =  value3
        [Sec2]
          [  Sec3 ]

        key4  =  value4

        key5 = value5
        """;

    public static readonly string TopDangling =
        """
        [Sec1]
          Dangling Content In Sec1
        
          Next dangling line
        ; Comment1 before key1
        key1 = value1
        key2 = value2
        [Sec2]
        """;
}