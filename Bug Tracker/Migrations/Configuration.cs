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
        }
    }
}
