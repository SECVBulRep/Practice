namespace AllocFree;

public class Guider
{
    
    private const char Equal = '=';
    private const char Underscore = '_';
    private const char Slash = '/';
    private const char PLus = '+';
    private const char Hyphen = '-';
    
    
    
    
    public static string ToStringFromGuid(Guid id)
    {
        return Convert.ToBase64String(id.ToByteArray())
            .Replace("/", "-")
            .Replace("+", "_")
            .Replace("=", string.Empty);
    }

    public static Guid ToGuidFromString(string id)
    {
        byte[] base64 = Convert.FromBase64String(
            id
                .Replace("-", "/")
                .Replace("_", "+")
            + "==");
        return new Guid(base64);
    }

   
    
    
    
    public static Guid ToGuidFromStringOpt(ReadOnlySpan<char> id)
    {
        Span<char> base64Char = stackalloc char[24];


        for (int i = 0; i < 22; i++)
        {
            base64Char[i] = id[i] switch
            {
                Hyphen => Slash,
                Underscore => PLus,
                _ => id[i]
            };
        }

        base64Char[22] = Equal;
        base64Char[23] = Equal;

        // я знаю что GUID 16 байт
        Span<byte> bytes = stackalloc byte[16];

        Convert.TryFromBase64Chars(base64Char, bytes, out _);
        
        return new Guid(bytes);
    }

}