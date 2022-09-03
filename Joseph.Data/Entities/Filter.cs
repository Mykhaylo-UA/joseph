using Joseph.Enums;

namespace Joseph.Data.Entities;

public class Filter
{
    public string Name { get; set; }
    public string Property { get; set; }
    public List<string> Values { get; set; }
    public string Additional { get; set; }
    public TypeFilter TypeFilter { get; set; }
}