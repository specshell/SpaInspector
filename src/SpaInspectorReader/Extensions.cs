using System.Text;
using System.Text.RegularExpressions;

namespace SpaInspectorReader;

public static class Extensions
{
    public static Spa ReadSpa(this MemoryStream stream)
    {
        var read = new Read(stream);
        var spa = read.ReadBinaryToSpa();
        return spa;
    }

    public static void Position(this BinaryReader binaryReader, long position) => binaryReader.BaseStream.Position = position;

    private static Regex FixNewlinesRegex = new Regex(@"([^ \t\r\n])[ \t]+$", RegexOptions.Multiline | RegexOptions.Compiled);
    public static string ReadNullTerminatedString(this BinaryReader stream)
    {
        var stringBuilder = new StringBuilder();
        char ch;
        while ((ch = stream.ReadChar()) != 0)
            stringBuilder.Append(ch);
        var str = stringBuilder.ToString();
        var fixNewlines = Regex.Replace(str, @"\r\n?", "\n");

        return FixNewlinesRegex.Replace(fixNewlines, "$1");
    }

    /// <summary>
    /// Safely opens file stream and properly closes it while converting the file to memory stream
    /// </summary>
    /// <param name="file">Path to a file</param>
    /// <returns>MemoryStream</returns>
    public static async Task<MemoryStream> FileAsMemoryStream(this string file)
    {
        var memoryStream = new MemoryStream();
        await using (var fileStream = File.OpenRead(file))
        {
            await fileStream.CopyToAsync(memoryStream);
        }
        memoryStream.Position = 0;
        return memoryStream;
    }
}
