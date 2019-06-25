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
    [RoutePrefix("api/Timetables")]
    public class TimetablesController : ApiController
    {
        //private ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWork unitOfWork;

        private object locker = new object();
        public TimetablesController(IUnitOfWork uw)
        {
            unitOfWork = uw;
        }
        [Route("GetTimetables")]
        // GET: api/Timetables
        public IEnumerable<Timetable> GetTimetables()
        {
            return unitOfWork.Timetables.GetAll().ToList();
            //return db.Timetables;
        }
     

        // GET: api/Timetables/5
        [ResponseType(typeof(Timetable))]
        public IHttpActionResult GetTimetable(int id)
        {
            Timetable timetable = unitOfWork.Timetables.Get(id);
            if (timetable == null)
            {
                return NotFound();
            }

            return Ok(timetable);
        }

        // PUT: api/Timetables/5
        [Route("Change")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTimetable(int id, Timetable timetable)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != timetable.Id)
            {
                return BadRequest();
            }

            if(timetable.Departures == null || timetable.Departures == "")
            {
                return Content(HttpStatusCode.BadRequest, "Can't add timetable without departures.");
            }

            Timetable tt = unitOfWork.Timetables.Get(id);
            if (tt != null)
            {
                if (tt.Version > timetable.Version)
                {
                    return Content(HttpStatusCode.Conflict, $" You are trying to edit timetable  that has been changed recently. Try again.");
                }
            }
            else
            {
                return Content(HttpStatusCode.NotFound, "Timetable that you are trying to edit either do not exist or was previously deleted by another user.");
            }
            tt.Departures = timetable.Departures;
            tt.Version++;
            try
            {
                unitOfWork.Timetables.Update(tt);
                unitOfWork.Complete();
                return Ok(tt.Id);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Content(HttpStatusCode.Conflict, ex);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }


        }

        // POST: api/Timetables
        [Route("Add")]
        [ResponseType(typeof(Timetable))]
        public IHttpActionResult PostTimetable(Timetable timetable)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (TimetableExists(timetable.Id))
            {
                return Content(HttpStatusCode.Conflict, "CONFLICT Timetable already exists!");
            }

            if (timetable.Departures==null || timetable.Departures == "")
            {
                return Content(HttpStatusCode.BadRequest, "Can't add timetable without departures");
            }
            if (timetable.Vehicles.FirstOrDefault().Id == -1)
            {
                return Content(HttpStatusCode.BadRequest, "Can't add timetable without vehicle");
            }

            if (timetable.Version != 0)
            {
                timetable.Version = 0;

            }

            Timetable t = new Timetable();
            t.Departures = timetable.Departures;
            t.Version = timetable.Version;
            t.DayTypeId = unitOfWork.DayTypes.Get(timetable.DayTypeId).Id;
            t.LineId = unitOfWork.Lines.Get(timetable.LineId).Id;
            t.Vehicles = new List<Vehicle>();
            t.Vehicles.Add(unitOfWork.Vehicles.Get(timetable.Vehicles.FirstOrDefault().Id));
            try
            {

                unitOfWork.Timetables.Add(t);
                unitOfWork.Complete();

                return Ok(t.Id);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Content(HttpStatusCode.Conflict, ex);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
            
        }


        [Route("Delete")]
        // DELETE: api/Timetables/5
        [ResponseType(typeof(Timetable))]
        public IHttpActionResult DeleteTimetable(int id)
        {
            if (id == 0)
            {
                return Content(HttpStatusCode.BadRequest, "You have to select timetable you want to delete!");
            }

            Timetable timetable = unitOfWork.Timetables.Get(id);
            if (timetable == null)
            {
                return Content(HttpStatusCode.NotFound, "Timetable that you are trying to delete either do not exist or was previously deleted by another user.");
            }
            try
            {
                unitOfWork.Timetables.Remove(timetable);
                unitOfWork.Complete();

                return Ok(timetable);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Content(HttpStatusCode.Conflict, ex);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
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

        private bool TimetableExists(int id)
        {
            return unitOfWork.Timetables.Get(id) != null;
        }
    }
}