using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

public class Program
{
   
    static void Main()
    {
        BenchmarkRunner.Run<Benchy>();
    }



    [MemoryDiagnoser]
    public class Benchy
    {
        public static readonly string  _dateString ="10-12-2023";
    
        [Benchmark]
        public (int, int, int) GetParts()
        {
            var dayString = _dateString.Substring(0, 2);
            var monthString = _dateString.Substring(3, 2);
            var  yearString = _dateString.Substring(6, 4);

            var day = int.Parse(dayString);
            var month = int.Parse(monthString);
            var year = int.Parse(yearString);
            return (day, month, year);
        }

    
        [Benchmark]
        public (int, int, int)  GetPartsSpanned()
        {
            ReadOnlySpan<char> span = _dateString;

            var dayString = span.Slice(0, 2);
            var monthString = span.Slice(3, 2);
            var  yearString = span.Slice(6, 4);
        
            var day = int.Parse(dayString);
            var month = int.Parse(monthString);
            var year = int.Parse(yearString);
            return (day, month, year);
        }    
    }
    
    
    
    
}










