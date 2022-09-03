using System.Text.Json.Serialization;
using Joseph.Enums;

namespace Joseph.Data.Entities;

public class Job: BaseEntity
{
    public string Name { get; set; }
    public string Location { get; set; }
    
    public List<JobType> JobTypes { get; set; }
    
    public int HoursWeek { get; set; }
    public NumberOfHiredPeople NumberOfHiredPeople { get; set; }
    
    public string Description { get; set; }
    
    public List<Answer> Answers { get; set; }
    
    public string UserId { get; set; }
    
    [JsonIgnore]
    public User User { get; set; }
}