namespace Bug_Tracker.Migrations
{
    using Bug_Tracker.Models;
    using Bug_Tracker.Models.Classes;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Bug_Tracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "Bug_Tracker.Models.ApplicationDbContext";
        }

        protected override void Seed(Bug_Tracker.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
                {
                    roleManager.Create(new IdentityRole("Admin" ));
                    var role = new IdentityRole("Admin");

                    roleManager.Create(role);
            }
            if (!context.Roles.Any(r => r.Name == "Project Manager"))
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager" });
            }
            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }
            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }

            ApplicationUser adminUser = new ApplicationUser();

            if (!context.Users.Any(item => item.UserName == "admin@bgTracker.com"))
            {
                adminUser.UserName = "admin@bgTracker.com";
                adminUser.Email = "admin@bgTracker.com";
                adminUser.Name = "Admin";
                adminUser.LastName = "User";
                adminUser.FirstName = "Admin";
                adminUser.DisplayName = "Admin";
                userManager.Create(adminUser, "Password-1");
            }
            else
            {
                adminUser = context.Users.FirstOrDefault(item => item.UserName == "admin@bgTracker.com");
            }
            if (!userManager.IsInRole(adminUser.Id, "Admin"))
            {
                userManager.AddToRole(adminUser.Id, "Admin");
            }

            context.TicketTypes.AddOrUpdate(x => x.Id,
                new Models.Classes.TicketType() { Id = 1, Name = "Bug Fixes" },
                new Models.Classes.TicketType() { Id = 2, Name = "Software Update" },
                new Models.Classes.TicketType() { Id = 3, Name = "Adding Helpers" },
                new Models.Classes.TicketType() { Id = 4, Name = "Database errors" });

            context.TicketPriorities.AddOrUpdate(x => x.Id,
                new Models.Classes.TicketPriority() { Id = 1, Name = "High" },
                new Models.Classes.TicketPriority() { Id = 2, Name = "Medium" },
                new Models.Classes.TicketPriority() { Id = 3, Name = "Low" },
                new Models.Classes.TicketPriority() { Id = 4, Name = "Urgent" });

            context.TicketStatuses.AddOrUpdate(x => x.Id,
                new Models.Classes.TicketStatus() { Id = 1, Name = "Finished" },
                new Models.Classes.TicketStatus() { Id = 2, Name = "Started" },
                new Models.Classes.TicketStatus() { Id = 3, Name = "Not Started" },
                new Models.Classes.TicketStatus() { Id = 4, Name = "In progress" });

            context.SaveChanges();
        }
    }
}
