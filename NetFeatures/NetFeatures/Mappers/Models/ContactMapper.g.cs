public static partial class ContactMapper
{
    public static ContactCodeGenDto AdaptToCodeGenDto(this Contact p1)
    {
        return p1 == null ? null : new ContactCodeGenDto()
        {
            Type = p1.Type,
            Value = p1.Value
        };
    }
    public static ContactCodeGenDto AdaptTo(this Contact p2, ContactCodeGenDto p3)
    {
        if (p2 == null)
        {
            return null;
        }
        ContactCodeGenDto result = p3 ?? new ContactCodeGenDto();
        
        result.Type = p2.Type;
        result.Value = p2.Value;
        return result;
        
    }
}