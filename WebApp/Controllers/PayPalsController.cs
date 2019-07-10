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
    [RoutePrefix("api/PayPals")]
    public class PayPalsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private readonly IUnitOfWork unitOfWork;
        public PayPalsController(IUnitOfWork uw)
        {
            unitOfWork = uw;
        }

        [Route("Add")]
        [ResponseType(typeof(PayPal))]
        public IHttpActionResult PostPaypal(PayPal paypal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }



            try
            {
                PayPal t = new PayPal();
                t.CreateTime = paypal.CreateTime;
                t.CurrencyCode = paypal.CurrencyCode;
                t.PayementId = paypal.PayementId;
                t.PayerEmail = paypal.PayerEmail;
                t.PayerName = paypal.PayerName;
                t.PayerSurname = paypal.PayerSurname;
                t.Value = paypal.Value;



                unitOfWork.PayPals.Add(t);
                unitOfWork.Complete();
                return Ok(t.Id);
            }
            catch (Exception ex)
            {
                return NotFound();
            }

        }
        // GET: api/PayPals
        public IQueryable<PayPal> GetPayPals()
        {
            return db.PayPals;
        }

        // GET: api/PayPals/5
        [ResponseType(typeof(PayPal))]
        public IHttpActionResult GetPayPal(int id)
        {
            PayPal payPal = db.PayPals.Find(id);
            if (payPal == null)
            {
                return NotFound();
            }

            return Ok(payPal);
        }

        // PUT: api/PayPals/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPayPal(int id, PayPal payPal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != payPal.Id)
            {
                return BadRequest();
            }

            db.Entry(payPal).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PayPalExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/PayPals
        [ResponseType(typeof(PayPal))]
        public IHttpActionResult PostPayPal(PayPal payPal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.PayPals.Add(payPal);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = payPal.Id }, payPal);
        }

        // DELETE: api/PayPals/5
        [ResponseType(typeof(PayPal))]
        public IHttpActionResult DeletePayPal(int id)
        {
            PayPal payPal = db.PayPals.Find(id);
            if (payPal == null)
            {
                return NotFound();
            }

            db.PayPals.Remove(payPal);
            db.SaveChanges();

            return Ok(payPal);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PayPalExists(int id)
        {
            return db.PayPals.Count(e => e.Id == id) > 0;
        }
    }
}