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
    [RoutePrefix("api/Pricelists")]
    public class PricelistsController : ApiController
    {
       // private ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWork unitOfWork;

        public PricelistsController(IUnitOfWork uw)
        {
            unitOfWork = uw;
        }
        [Route("GetPricelists")]
        // GET: api/Pricelists
        public IEnumerable<Pricelist> GetPricelists()
        {
            return unitOfWork.PriceLists.GetAllPricelists().ToList();
        }

        // GET: api/Pricelists/5
        [Route("GetPricelist")]
        [ResponseType(typeof(Pricelist))]
        public Pricelist GetPricelist()
        {
            Pricelist pricelist = unitOfWork.PriceLists.GetAllPricelists().ToList().FindLast(x=> x.EndOfValidity >= DateTime.Now && x.StartOfValidity<= DateTime.Now);
           
            return pricelist;
        }
        [Route("GetPricelistLast")]
        [ResponseType(typeof(Pricelist))]
        public Pricelist GetPricelistLast()
        {
            Pricelist pricelist = unitOfWork.PriceLists.GetAllPricelists().ToList().Last();
            return pricelist;
        }


        // POST: api/Pricelists
        [Route("Add")]
        [ResponseType(typeof(Pricelist))]
        public IHttpActionResult PostPricelist(TicketPricesHelpModel t)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (t.Hourly <= 0 || t.Daily <= 0 || t.Monthly <= 0 || t.Yearly <= 0)
            {
                return Content(HttpStatusCode.BadRequest, "Invalid value for prices.");
            }
            if (t.PriceList.StartOfValidity.ToString() == "" || t.PriceList.EndOfValidity.ToString() == "")
            {
                return Content(HttpStatusCode.BadRequest, "Invalid dates for validity period. Dates can't be empty.");
            }
            if(t.PriceList.StartOfValidity > t.PriceList.EndOfValidity)
            {
                return Content(HttpStatusCode.BadRequest, "Invalid dates for validity period. Start of validity period is greater than the end.");
            }
            if(t.PriceList.StartOfValidity.Value.Date < DateTime.Now.Date)
            {
                return Content(HttpStatusCode.BadRequest, "Invalid dates for validity period. Start of validity period can't be older than today.");
            }
            try
            {
                Pricelist prl = new Pricelist();
                prl = t.PriceList;
                prl.TicketPricess = new List<TicketPrices>();
                TicketPrices tp = new TicketPrices();
                tp.TicketTypeId = unitOfWork.TicketTypes.Find(k => k.Name == "Hourly").FirstOrDefault().Id;
     
                tp.Price = t.Hourly;

                prl.TicketPricess.Add(tp);
                tp = new TicketPrices();
                tp.TicketTypeId = unitOfWork.TicketTypes.Find(k => k.Name == "Daily").FirstOrDefault().Id;
                //tp.PricelistId = unitOfWork.PriceLists.Get(hm.IdPriceList).Id;
                tp.Price = t.Daily;
                prl.TicketPricess.Add(tp);
                tp = new TicketPrices();
                tp.TicketTypeId = unitOfWork.TicketTypes.Find(k => k.Name == "Monthly").FirstOrDefault().Id;
                //tp.PricelistId = unitOfWork.PriceLists.Get(hm.IdPriceList).Id;
                tp.Price = t.Monthly;
                prl.TicketPricess.Add(tp);
                tp = new TicketPrices();
                tp.TicketTypeId = unitOfWork.TicketTypes.Find(k => k.Name == "Yearly").FirstOrDefault().Id;
                //tp.PricelistId = unitOfWork.PriceLists.Get(hm.IdPriceList).Id;
                tp.Price = t.Yearly;
                prl.TicketPricess.Add(tp);

                unitOfWork.PriceLists.Add(prl);
                unitOfWork.Complete();

                return Ok();
            }
            catch (Exception ex)
            {

                return NotFound();
            }
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