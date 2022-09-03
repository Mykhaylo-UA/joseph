using Joseph.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Joseph.Context;

public class ApplicationContext: IdentityDbContext<User>
{
    public DbSet<Job> Jobs { get; set; }
    public DbSet<JobType> JobTypes { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<User> Users { get; set; }

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
}