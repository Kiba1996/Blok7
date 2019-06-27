﻿using Microsoft.AspNet.Identity;
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
using WebApp.Models.HelpModels;
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

        [Route("GetTickets")]
        [HttpGet]
        //GET: api/Tickets
        public IEnumerable<TicketHelpModel> GetTicketsForOneUser(string id)
        {
            ApplicationUser ap = UserManager.FindByEmail(id);
            PassengerType pt = unitOfWork.PassengerTypes.Get((int)ap.PassengerTypeId);

            List<Ticket> lista = unitOfWork.Tickets.getAllTicketsWithUser(id).ToList();

            List<TicketHelpModel> ret = new List<TicketHelpModel>();
            foreach (Ticket t in lista)
            {
                TicketHelpModel st = new TicketHelpModel();
                st.PurchaseTime = t.PurchaseTime;
                double price = unitOfWork.TicketPrices.Get(t.TicketPricesId).Price;
                st.TicketPrice = price - (price * pt.Coefficient);
                st.TicketType = unitOfWork.TicketTypes.Get((int)t.TicketTypeId).Name;
                DateTime d;
                if (t.TicketTypeId == 1)
                {
                    d = new DateTime(t.PurchaseTime.Value.Year, t.PurchaseTime.Value.Month, t.PurchaseTime.Value.Day,
                        t.PurchaseTime.Value.Hour + 1, t.PurchaseTime.Value.Minute, 0);
                    st.ExparationTime = d.ToString();
                }
                if (t.TicketTypeId == 2)
                { d = new DateTime(t.PurchaseTime.Value.Year, t.PurchaseTime.Value.Month, t.PurchaseTime.Value.Day+1,
                      0, 0, 0);
                    st.ExparationTime = d.ToString(); 
                }
                if (t.TicketTypeId == 3)
                {
                    if ((t.PurchaseTime.Value.Month < 8 && t.PurchaseTime.Value.Month % 2 == 0) || (t.PurchaseTime.Value.Month >= 8 && t.PurchaseTime.Value.Month % 2 != 0))
                    {
                        if(t.PurchaseTime.Value.Month == 2)
                        {
                            d = new DateTime(t.PurchaseTime.Value.Year, t.PurchaseTime.Value.Month, 28,
                        0, 0, 0);
                        }
                        else
                        {
                            d = new DateTime(t.PurchaseTime.Value.Year, t.PurchaseTime.Value.Month, 30,
                                                   0, 0, 0);
                        }
                       
                    }
                    else {
                        d = new DateTime(t.PurchaseTime.Value.Year, t.PurchaseTime.Value.Month,31,
                     0, 0, 0);
                    }
                    st.ExparationTime = d.ToString();
                }
                if (t.TicketTypeId == 4)
                {
                    d = new DateTime(t.PurchaseTime.Value.Year, 12, 31,
                       0, 0, 0);
                    st.ExparationTime = d.ToString();
                }
                ret.Add(st);
            }
            return ret;
        }

        // POST: api/Tickets
        [Route("Add")]
        [ResponseType(typeof(Ticket))]
        public IHttpActionResult PostTicket(Ticket ticket)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (ticket.Name != null && ticket.Name != "")
            {
                if (ticket.TicketTypeId != 1)
                {
                    return Content(HttpStatusCode.BadRequest, "Only signedIn users can buy this type of ticket!");
                }
            }
            else
            {
                ApplicationUser appu = UserManager.FindById(ticket.ApplicationUserId);
                if ((appu.Activated == "Deactivated" || appu.Activated == "Pending") && ticket.TicketTypeId != 1)
                {
                    return Content(HttpStatusCode.BadRequest, "Only authorized users can buy this type of ticket!");
                }
            }

            try
            {
                Ticket t = new Ticket();
                t.PurchaseTime = ticket.PurchaseTime;
                t.TicketPricesId = unitOfWork.TicketPrices.Get(ticket.TicketPricesId).Id;

                t.TicketTypeId = unitOfWork.TicketTypes.Get(ticket.TicketTypeId.GetValueOrDefault()).Id;
                t.Name = "Karta";
                if(ticket.ApplicationUserId  != null && ticket.ApplicationUserId != "")
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
    }
}