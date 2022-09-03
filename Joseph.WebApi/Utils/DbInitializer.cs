using Joseph.Context;
using Joseph.Data.Constants;
using Joseph.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace Joseph.WebApi.Utils;

public static class DbInitializer
    {
        public static void Initialize(ApplicationContext context, UserManager<User> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            if (!context.JobTypes.Any())
            {
                List<JobType> jobTypes = new()
                {
                    new(){CreationDate = DateTime.Now, Name = "Full time"},
                    new(){CreationDate = DateTime.Now, Name = "Partial"},
                    new(){CreationDate = DateTime.Now, Name = "Temporary"},
                    new(){CreationDate = DateTime.Now, Name = "Remote"},
                    new(){CreationDate = DateTime.Now, Name = "Internship"},
                    new(){CreationDate = DateTime.Now, Name = "Volunteering"}
                };

                context.JobTypes.AddRange(jobTypes);
            }
            if (!roleManager.RoleExistsAsync(UserRoles.Admin).GetAwaiter().GetResult())
                 roleManager.CreateAsync(new IdentityRole(UserRoles.Admin)).GetAwaiter().GetResult();
            if (!roleManager.RoleExistsAsync(UserRoles.Employer).GetAwaiter().GetResult())
                 roleManager.CreateAsync(new IdentityRole(UserRoles.Employer)).GetAwaiter().GetResult();
            if (!roleManager.RoleExistsAsync(UserRoles.Candidate).GetAwaiter().GetResult())
                 roleManager.CreateAsync(new IdentityRole(UserRoles.Candidate)).GetAwaiter().GetResult();

            User user = userManager.FindByEmailAsync("admin@gmail.com").GetAwaiter().GetResult();
            if (user is null)
            {
                user = new()
                {
                    Email = "admin@gmail.com",
                    UserName = "admin@gmail.com",
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                
                userManager.CreateAsync(user, "AdminPassword123456").GetAwaiter().GetResult();
            }

            if (!userManager.IsInRoleAsync(user, UserRoles.Admin).GetAwaiter().GetResult())
            {
                userManager.AddToRoleAsync(user, UserRoles.Admin).GetAwaiter().GetResult();
            }


            context.SaveChanges();
        }
    }