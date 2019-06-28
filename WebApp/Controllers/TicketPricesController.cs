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
    [RoutePrefix("api/TicketPrices")]
    public class TicketPricesController : ApiController
    {
       // private ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWork unitOfWork;

        public TicketPricesController(IUnitOfWork uw)
        {
            unitOfWork = uw;
        }
        // GET: api/TicketPrices
        [Route("GetAllTicketPrices")]
        public IEnumerable<TicketPrices> 
            Prices()
        {
            return unitOfWork.TicketPrices.GetAll().ToList();
        }

        [Route("GetValidPrices")]
        public TicketPricesHelpModel GetValidPrices(int id)
        {
            TicketPricesHelpModel tp = new TicketPricesHelpModel();
            var p = unitOfWork.TicketPrices.Find(x => x.PricelistId == id);

            TicketType tt = unitOfWork.TicketTypes.Find(m => m.Name == "Daily").FirstOrDefault();
            tp.Daily = (int)p.First(x => x.TicketTypeId == tt.Id).Price;
            tt = unitOfWork.TicketTypes.Find(m => m.Name == "Monthly").FirstOrDefault();
            tp.Monthly = (int)p.First(x => x.TicketTypeId == tt.Id).Price;
            tt = unitOfWork.TicketTypes.Find(m => m.Name == "Yearly").FirstOrDefault();
            tp.Yearly = (int)p.First(x => x.TicketTypeId == tt.Id).Price;
            tt = unitOfWork.TicketTypes.Find(m => m.Name == "Hourly").FirstOrDefault();
            tp.Hourly = (int)p.First(x => x.TicketTypeId == tt.Id).Price;
            tp.IdPriceList = id;

            return tp;
        }

        [Route("GetTicketPrice")]
        // GET: api/TicketPrices/5
        [ResponseType(typeof(TicketPrices))]
        public IHttpActionResult GetTicketPrices(int id)
        {
            TicketPrices ticketPrices = unitOfWork.TicketPrices.Get(id);
            if (ticketPrices == null)
            {
                return NotFound();
            }

            return Ok(ticketPrices);
        }

        //// PUT: api/TicketPrices/5
        //[ResponseType(typeof(void))]
        //public IHttpActionResult PutTicketPrices(int id, TicketPrices ticketPrices)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != ticketPrices.Id)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(ticketPrices).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!TicketPricesExists(id))
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

        // POST: api/TicketPrices
        [Route("AddTicketPrices")]
        [ResponseType(typeof(TicketPrices))]
        public IHttpActionResult PostTicketPrices([FromBody] TicketPricesHelpModel hm)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            try
            {
                TicketPrices tp = new TicketPrices();
                tp.TicketTypeId = unitOfWork.TicketTypes.Find(k => k.Name == "Hourly").FirstOrDefault().Id;
                tp.PricelistId = unitOfWork.PriceLists.Get(hm.IdPriceList).Id;
                tp.Price = hm.Hourly;

                unitOfWork.TicketPrices.Add(tp);
                tp = new TicketPrices();
                tp.TicketTypeId = unitOfWork.TicketTypes.Find(k => k.Name == "Daily").FirstOrDefault().Id;
                tp.PricelistId = unitOfWork.PriceLists.Get(hm.IdPriceList).Id;
                tp.Price = hm.Daily;
                unitOfWork.TicketPrices.Add(tp);
                tp = new TicketPrices();
                tp.TicketTypeId = unitOfWork.TicketTypes.Find(k => k.Name == "Monthly").FirstOrDefault().Id;
                tp.PricelistId = unitOfWork.PriceLists.Get(hm.IdPriceList).Id;
                tp.Price = hm.Monthly;
                unitOfWork.TicketPrices.Add(tp);
                tp = new TicketPrices();
                tp.TicketTypeId = unitOfWork.TicketTypes.Find(k => k.Name == "Yearly").FirstOrDefault().Id;
                tp.PricelistId = unitOfWork.PriceLists.Get(hm.IdPriceList).Id;
                tp.Price = hm.Yearly;

                unitOfWork.TicketPrices.Add(tp);

                unitOfWork.Complete();
                return Ok();
            }
            catch (Exception)
            {
                return NotFound();
            }

        }

        //// DELETE: api/TicketPrices/5
        //[ResponseType(typeof(TicketPrices))]
        //public IHttpActionResult DeleteTicketPrices(int id)
        //{
        //    TicketPrices ticketPrices = db.TicketPrices.Find(id);
        //    if (ticketPrices == null)
        //    {
        //        return NotFound();
        //    }

        //    db.TicketPrices.Remove(ticketPrices);
        //    db.SaveChanges();

        //    return Ok(ticketPrices);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }

        //private bool TicketPricesExists(int id)
        //{
        //    return db.TicketPrices.Count(e => e.Id == id) > 0;
        //}
    }
}