namespace Bug_Tracker.Migrations
{
    using Bug_Tracker.Models;
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
                roleManager.Create(new IdentityRole { Name = "Admin" });
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

            //demo admin user
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

            //demo project manager
            ApplicationUser pmUser = new ApplicationUser();

            if (!context.Users.Any(item => item.UserName == "pManager@bgTracker.com"))
            {
                pmUser.UserName = "pManager@bgTracker.com";
                pmUser.Email = "pManager@bgTracker.com";
                pmUser.Name = "Project Manager";
                pmUser.LastName = "Manager";
                pmUser.FirstName = "Project";
                pmUser.DisplayName = "Project Manager";
                userManager.Create(pmUser, "Password-1");
            }
            else
            {
                pmUser = context.Users.FirstOrDefault(item => item.UserName == "pManager@bgTracker.com");
            }
            if (!userManager.IsInRole(pmUser.Id, "Project Manager"))
            {
                userManager.AddToRole(pmUser.Id, "Project Manager");
            }
            //demo developer

            ApplicationUser devUser = new ApplicationUser();

            if (!context.Users.Any(item => item.UserName == "dev@bgTracker.com"))
            {
                devUser.UserName = "dev@bgTracker.com";
                devUser.Email = "dev@bgTracker.com";
                devUser.Name = "Developer";
                devUser.LastName = "dev";
                devUser.FirstName = "Demo";
                devUser.DisplayName = "Developer";
                userManager.Create(devUser, "Password-1");
            }
            else
            {
                devUser = context.Users.FirstOrDefault(item => item.UserName == "dev@bgTracker.com");
            }
            if (!userManager.IsInRole(devUser.Id, "Developer"))
            {
                userManager.AddToRole(devUser.Id, "Developer");
            }


            //demo submitter
            ApplicationUser subUser = new ApplicationUser();

            if (!context.Users.Any(item => item.UserName == "sub@bgTracker.com"))
            {
                subUser.UserName = "sub@bgTracker.com";
                subUser.Email = "sub@bgTracker.com";
                subUser.Name = "Submitter";
                subUser.LastName = "sub";
                subUser.FirstName = "Demo";
                subUser.DisplayName = "Submitter";
                userManager.Create(subUser, "Password-1");
            }
            else
            {
                subUser = context.Users.FirstOrDefault(item => item.UserName == "sub@bgTracker.com");
            }
            if (!userManager.IsInRole(subUser.Id, "Submitter"))
            {
                userManager.AddToRole(subUser.Id, "Submitter");
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
