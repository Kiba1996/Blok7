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
    [RoutePrefix("api/Vehicles")]
    public class VehiclesController : ApiController
    {
        //private ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWork unitOfWork;

        public VehiclesController(IUnitOfWork uw)
        {
            unitOfWork = uw;
        }

        // GET: api/Vehicles
        [Route("GetVehicles")]
        public IEnumerable<Vehicle> GetVehicles()
        {
            return unitOfWork.Vehicles.GetAll().ToList();
        }
        [Route("GetAvailableVehicles")]
        public IEnumerable<Vehicle> GetAvailableVehicles()
        {
            List<Vehicle> list = unitOfWork.Vehicles.GetAllAvailables().ToList();
            List<Vehicle> ret = new List<Vehicle>();
            foreach(Vehicle v in list)
            {
                if(v.Timetables == null || v.Timetables.Count() == 0)
                {
                    ret.Add(v);
                }
            }
            return ret;
        }

        // POST: api/Vehicles
        [Route("Add")]
        [ResponseType(typeof(Vehicle))]
        public IHttpActionResult PostVehicle(Vehicle vehicle)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (vehicle.Type == "" || vehicle.Type == null)
            {
                return Content(HttpStatusCode.BadRequest, "You have to select type of vehicle!");
            }

            try
            {

                unitOfWork.Vehicles.Add(vehicle);
                unitOfWork.Complete();

                return Ok(vehicle.Id);
            }
            catch (Exception)
            {
                return NotFound();
            }

        }
        [Route("Delete")]
        // DELETE: api/Vehicles/5
        [ResponseType(typeof(Vehicle))]
        public IHttpActionResult DeleteVehicle(int id)
        {
            if (id == 0)
            {
                return Content(HttpStatusCode.BadRequest, "You have to select vehicle you want to remove!");
            }

            Vehicle vehicle = unitOfWork.Vehicles.Get(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            unitOfWork.Vehicles.Remove(vehicle);
            unitOfWork.Complete();

            return Ok(vehicle);
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