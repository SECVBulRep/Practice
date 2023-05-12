// See https://aka.ms/new-console-template for more information

// #if DEBUG
// System.Console.WriteLine("Hello, World!");
// #endif


using System.Diagnostics;

Console.WriteLine("Hello, World!");

string? s = null;

MyCondition(s is null);

[Conditional("DEBUG")]
static void MyCondition(bool condition)
{
    
}