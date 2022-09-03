using Microsoft.AspNetCore.Identity;

namespace Joseph.Data.Entities;

public class User: IdentityUser
{
    public List<Job> Jobs { get; set; }
    public List<Answer> Answers { get; set; }
    
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Other { get; set; }
    
    public DateTime? Birthday { get; set; }
    
}