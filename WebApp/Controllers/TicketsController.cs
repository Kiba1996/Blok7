using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebApp.Models;
using WebApp.Persistence;
using WebApp.Persistence.UnitOfWork;

namespace WebApp.Controllers
{
    [RoutePrefix("api/Tickets")]
    public class TicketsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private readonly IUnitOfWork unitOfWork;
        private ApplicationUserManager _userManager;
        public TicketsController(ApplicationUserManager userManager,IUnitOfWork uw)
        {
            UserManager = userManager;
            unitOfWork = uw;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [Route("GetTicketTypes")]
        //GET: api/Tickets
        public IEnumerable<TicketType> GetTicketTypes()
        {
            return unitOfWork.TicketTypes.GetAll().ToList();
        }

        // GET: api/Tickets
        //public IQueryable<Ticket> GetTickets()
        //{
        //    return db.Tickets;
        //}

        [Route("GetTicket")]
        // GET: api/Tickets/5
        [ResponseType(typeof(Ticket))]
        public IHttpActionResult GetTicket(int id)
        {

            Ticket ticket = unitOfWork.Tickets.GetTicketWithInclude(id);
            
            if (ticket == null)
            {
                return NotFound();
            }
            if(ticket.ApplicationUser != null)
            {
                ticket.ApplicationUserId = ticket.ApplicationUser.Email;
            }
            

            return Ok(ticket);
        }

        //// PUT: api/Tickets/5
        //[ResponseType(typeof(void))]
        //public IHttpActionResult PutTicket(int id, Ticket ticket)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != ticket.Id)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(ticket).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!TicketExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        // POST: api/Tickets
        [Route("Add")]
        [ResponseType(typeof(Ticket))]
        public IHttpActionResult PostTicket(Ticket ticket)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                Ticket t = new Ticket();
                t.PurchaseTime = ticket.PurchaseTime;
                t.TicketPricesId = unitOfWork.TicketPrices.Get(ticket.TicketPricesId).Id;

                t.TicketTypeId = unitOfWork.TicketTypes.Get(ticket.TicketTypeId.GetValueOrDefault()).Id;
                t.Name = "Karta";
                if(ticket.ApplicationUserId  != null)
                {
                    t.ApplicationUserId = UserManager.FindById(ticket.ApplicationUserId).Id;
                }
               
                

                unitOfWork.Tickets.Add(t);
                unitOfWork.Complete();
                return Ok(t.Id);
            }
            catch (Exception)
            {
                return NotFound();
            }

           
        }

        //// DELETE: api/Tickets/5
        //[ResponseType(typeof(Ticket))]
        //public IHttpActionResult DeleteTicket(int id)
        //{
        //    Ticket ticket = db.Tickets.Find(id);
        //    if (ticket == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Tickets.Remove(ticket);
        //    db.SaveChanges();

        //    return Ok(ticket);
        //}


        [Route("SendMail")]
        public string SendMail(Ticket ticket)
        {
           
                //if (!ModelState.IsValid)
                //{
                //    return BadRequest(ModelState).ToString();
                //}
            //Get user data, and update activated to true

            try
            {
               // Ticket t = new Ticket();
               // t.PurchaseTime = ticket.PurchaseTime;
               // t.TicketPricesId = unitOfWork.TicketPrices.Get(ticket.TicketPricesId).Id;
               // t.TicketTypeId = unitOfWork.TicketTypes.Get((int)ticket.TicketTypeId).Id;
               // t.Name = "Karta";
               ////  t.ApplicationUserId = "unknown";

               // unitOfWork.Tickets.Add(ticket);
               // unitOfWork.Complete();
                try
                {
                    string subject = "Ticket purchase";
                    string desc = $"Dear {ticket.Name}, Your purchase is successfull.\n Ticket price: {unitOfWork.TicketPrices.Get(ticket.TicketPricesId).Price}\n " +
                $"Ticket type:{unitOfWork.TicketTypes.Get((int)ticket.TicketTypeId).Name}\n" +
                $"Time of purchase: {ticket.PurchaseTime}\n" +
                $"Ticket is valid for the next hour.\n\n" +
                $"Thank you.";
                    var email = ticket.Name;
                    unitOfWork.Tickets.NotifyViaEmail(email, subject, desc);
                }
                catch { }
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest().ToString();
            }
            
                return "Ok";
            
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }

        //private bool TicketExists(int id)
        //{
        //    return db.Tickets.Count(e => e.Id == id) > 0;
        //}
    }
}