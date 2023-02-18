using System;
using System.Collections.Generic;

public partial class CustomerCodeGenDto
{
    public string FirstName { get; set; }
    public int Age { get; set; }
    public DateTime BirthDate { get; set; }
    public List<ContactCodeGenDto> Contacts { get; set; }
}