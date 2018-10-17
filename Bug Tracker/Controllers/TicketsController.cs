using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Bug_Tracker.Models;
using Bug_Tracker.Models.Classes;
using Bug_Tracker.Models.Helpers;
using Microsoft.AspNet.Identity;

namespace Bug_Tracker.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRoleHelper UserRoleHelper { get; set; }

        // GET: Tickets
        public ActionResult Index(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var userRoleHelper = new UserRoleHelper();
                var userId = User.Identity.GetUserId();
                var role = UserRoleHelper.GetUserRoles(userId);
                ViewBag.User = "User";

                if (role.Contains("Project Manager"))
                {
                    var dbUSer = db.Users.FirstOrDefault(p => p.Id == userId);
                    var myProject = dbUSer.Projects.Select(p => p.Id);
                    var ticket = db.Tickets.Where(p => myProject.Contains(p.ProjectId)).ToList();
                    return View(ticket);
                }
                else if (role.Contains("Developer"))
                {
                    if (id == "projectsTicket")
                    {
                        var dbUSer = db.Users.FirstOrDefault(p => p.Id == userId);
                        var myProject = dbUSer.Projects.Select(p => p.Id);
                        var ticket = db.Tickets.Where(p => myProject.Contains(p.ProjectId)).ToList();
                        return View(ticket);
                    }
                    else if (id == "devTicket")
                    {
                        return View(db.Tickets.Include(t => t.TicketPriority).Include(t => t.Project).Include(t => t.TicketStatus).Include(t => t.TicketType).Where(p => p.AssigneeId == userId).ToList());
                    }
                }
                else if (role.Contains("Submitter"))
                {
                    return View(db.Tickets.Include(t => t.TicketPriority).Include(t => t.Project).Include(t => t.TicketStatus).Include(t => t.TicketType).Where(p => p.CreatorId == userId).ToList());
                }
            }
            ViewBag.User = "";
            return View(db.Tickets.Include(t => t.TicketPriority).Include(t => t.Project).Include(t => t.TicketStatus).Include(t => t.TicketType).ToList());
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);

            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        //Post Details
        [HttpPost]
        [Authorize]
        public ActionResult CreateComment(int id, string body)
        {
            var tickets = db.Tickets
           .Where(p => p.Id == id)
           .FirstOrDefault();
            if (tickets == null)
            {
                return HttpNotFound();
            }
            if (string.IsNullOrWhiteSpace(body))
            {
                ViewBag.ErrorMessage = "Comment is required";
                return View("Details", new { tickets.Id });
            }
            var comment = new TicketComment();
            comment.UserId = User.Identity.GetUserId();
            comment.TicketId = tickets.Id;
            comment.Created = DateTime.Now;
            comment.Comment = body;
            db.TicketComments.Add(comment);
            db.SaveChanges();
            return RedirectToAction("Details", new { id });
        }

        //Submitter Tickets
        //public ActionResult SubmitterTickets()
        //{
        //    string submitterId = User.Identity.GetUserId();
        //    var tickets = db.Tickets.Where(t => t.CreatorId == submitterId).Include(t => t.Creator).Include(t => t.Assignee).Include(t => t.Project);
        //    return View("Index", tickets.ToList());
        //}

        //[Authorize(Roles = "Project Manager, Developer")]
        ////Project Manager and Developer Tickets
        //public ActionResult ProjectManagerTickets()
        //{
        //    var userId = User.Identity.GetUserId();
        //    var dbUSer = db.Users.FirstOrDefault(p => p.Id == userId);
        //    var myProject = dbUSer.Projects.Select(p => p.Id);
        //    var ticket = db.Tickets.Where(p => myProject.Contains(p.Id)).ToList();
        //    return View(ticket);
        //}

        // GET: assign developer to ticket
        public ActionResult AssignDevelopers(int ticketId)
        {
            var model = new AssignTicketToDeveloper();
            model.Id = ticketId;
            var ticket = db.Tickets.FirstOrDefault(p => p.Id == ticketId);
            var userRoleHelper = new UserRoleHelper();
            var users = userRoleHelper.UsersInRole("Developer");
            model.UserList = new SelectList(users, "Id", "Name");
            return View(model);
        }
        [HttpPost]
        public ActionResult AssignDevelopers(AssignTicketToDeveloper model)
        {
            var ticket = db.Tickets.FirstOrDefault(p => p.Id == model.Id);
            ticket.AssigneeId = model.SelectedUser;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Submitter")]
        public ActionResult Create()
        {
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Submitter")]
        public ActionResult Create([Bind(Include = "Id,Name,Description,AssignId,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.CreatorId = User.Identity.GetUserId();
                ticket.TicketStatusId = 3;
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AssigneeId = new SelectList(db.Users, "Id", "Name", ticket.AssigneeId);
            ViewBag.CreatorId = new SelectList(db.Users, "Id", "Name", ticket.CreatorId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAttachment(int id, [Bind(Include = "Id,Description,TicketId")] TicketAttachment ticketAttachment, HttpPostedFileBase image)
        {

            if (ModelState.IsValid)
            {

                if (image == null)
                {
                    return HttpNotFound();
                }
                if (!ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    ViewBag.ErrorMessage = "Please upload an image";

                }
                var fileName = Path.GetFileName(image.FileName);
                image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                ticketAttachment.FilePath = "/Uploads/" + fileName;
                ticketAttachment.UserId = User.Identity.GetUserId();
                ticketAttachment.Created = DateTime.Now;
                ticketAttachment.TicketId = id;
                db.TicketAttachments.Add(ticketAttachment);
                db.SaveChanges();
                return RedirectToAction("Details", new { id });
            }
            return View(ticketAttachment);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.AssigneeId = new SelectList(db.Users, "Id", "Name", ticket.AssigneeId);
            ViewBag.CreatorId = new SelectList(db.Users, "Id", "Name", ticket.CreatorId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Updated,ProjectId,TicketTypeId,TicketPriorityId,CreatorId,TicketStatusId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                var DBTicket = db.Tickets.FirstOrDefault(p => p.Id == ticket.Id);
                DBTicket.Name = ticket.Name;
                DBTicket.Description = ticket.Description;
                DBTicket.TicketStatusId = ticket.TicketStatusId;
                DBTicket.Updated = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AssigneeId = new SelectList(db.Users, "Id", "Name", ticket.AssigneeId);
            ViewBag.CreatorId = new SelectList(db.Users, "Id", "Name", ticket.CreatorId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
