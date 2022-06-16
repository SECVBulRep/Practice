using System.Collections.Specialized;

public class QuartzConfig : Dictionary<string, string>
{
    public NameValueCollection ToNameValueCollection()
    {
        return this.Aggregate(new NameValueCollection(), (seed, current) =>
        {
            seed.Add(current.Key, current.Value);
            return seed;
        });
    }
}