using System.Text.Json.Serialization;

namespace Joseph.Data.Entities;

public class JobType: BaseEntity
{
    public string Name { get; set; }
    
    [JsonIgnore]
    public List<Job> Jobs { get; set; }
}