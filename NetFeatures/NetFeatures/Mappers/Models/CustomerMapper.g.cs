using System.Collections.Generic;

public static partial class CustomerMapper
{
    public static CustomerCodeGenDto AdaptToCodeGenDto(this Customer p1)
    {
        return p1 == null ? null : new CustomerCodeGenDto()
        {
            FirstName = p1.FirstName,
            Age = p1.Age,
            BirthDate = p1.BirthDate,
            Contacts = funcMain1(p1.Contacts)
        };
    }
    public static CustomerCodeGenDto AdaptTo(this Customer p3, CustomerCodeGenDto p4)
    {
        if (p3 == null)
        {
            return null;
        }
        CustomerCodeGenDto result = p4 ?? new CustomerCodeGenDto();
        
        result.FirstName = p3.FirstName;
        result.Age = p3.Age;
        result.BirthDate = p3.BirthDate;
        result.Contacts = funcMain2(p3.Contacts, result.Contacts);
        return result;
        
    }
    
    private static List<ContactCodeGenDto> funcMain1(List<Contact> p2)
    {
        if (p2 == null)
        {
            return null;
        }
        List<ContactCodeGenDto> result = new List<ContactCodeGenDto>(p2.Count);
        
        int i = 0;
        int len = p2.Count;
        
        while (i < len)
        {
            Contact item = p2[i];
            result.Add(item == null ? null : new ContactCodeGenDto()
            {
                Type = item.Type,
                Value = item.Value
            });
            i++;
        }
        return result;
        
    }
    
    private static List<ContactCodeGenDto> funcMain2(List<Contact> p5, List<ContactCodeGenDto> p6)
    {
        if (p5 == null)
        {
            return null;
        }
        List<ContactCodeGenDto> result = new List<ContactCodeGenDto>(p5.Count);
        
        int i = 0;
        int len = p5.Count;
        
        while (i < len)
        {
            Contact item = p5[i];
            result.Add(item == null ? null : new ContactCodeGenDto()
            {
                Type = item.Type,
                Value = item.Value
            });
            i++;
        }
        return result;
        
    }
}