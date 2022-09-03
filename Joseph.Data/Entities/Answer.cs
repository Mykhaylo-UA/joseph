using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Joseph.Data.Entities;

public class Answer: BaseEntity
{
    public string Description { get; set; }
    
    public string UserId { get; set; }
    [JsonIgnore]
    public User User { get; set; }
    [NotMapped]
    public string UserDisplay { get; set; }
    
    public Guid JobId { get; set; }
    [JsonIgnore]
    public Job Job { get; set; }
}