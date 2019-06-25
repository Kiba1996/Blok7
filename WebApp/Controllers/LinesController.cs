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
    [RoutePrefix("api/Lines")]
    public class LinesController : ApiController
    {
        //private ApplicationDbContext db = new ApplicationDbContext();
        private readonly IUnitOfWork unitOfWork;

        private object locker = new object();
        public LinesController(IUnitOfWork uw)
        {
            unitOfWork = uw;
        }

        // GET: api/Lines
        [Route("GetLines")]
        public IEnumerable<Line> GetLines()
        {


            List<SerialNumberSL> sl = unitOfWork.SerialNumberSLs.GetAll().ToList();
            List<Line> stats = unitOfWork.Lines.GetAll().ToList();
           foreach(Line l in stats)
            {
                //List<Station> ss = l.Stations;
                //l.Stations.Clear();
                l.Stations = new List<Station>();
                List<SerialNumberSL> ll = sl.FindAll(c => c.LineId == l.Id);
                List<SerialNumberSL> ll1 = ll.OrderBy(x => x.SerialNumber).ToList();
                foreach(SerialNumberSL t in ll1)
                {
                    List<Station> s = unitOfWork.Stations.GetAll().Where(m => m.Id == t.StationId).ToList();
                    l.Stations.Add(s[0]);
                }
            }
            return stats;
        }
        [HttpGet]
        //GET: api/Lines/5
        [Route("FindVehicle")]
      //  [ResponseType(typeof(int))]
        public Int32 FindVehicle(int id)
        {
            List<Timetable> list = unitOfWork.Timetables.GetAllTimetablesWithVehicles().ToList();
            foreach(Timetable t in list)
            {
                if(t.LineId == id)
                {
                    if(t.Vehicles != null && t.Vehicles.Count != 0)
                    {
                        return t.Vehicles.FirstOrDefault().Id;
                    }
                }
            }
            return -1;
        }

        [Route("Change")]
        // PUT: api/Lines/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutLine(int id, Line line)
        {
            

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != line.Id)
            {
                return BadRequest();
            }

            //List<Line> stats = unitOfWork.Lines.GetAllLinesWithStations().ToList();
            if (line.Stations == null || line.Stations.Count < 2)
            {
                return Content(HttpStatusCode.BadRequest, "You must add at least two stations per line");
            }
            Line lineDb = unitOfWork.Lines.Get(id);


                if (lineDb != null)
                {
                    if (lineDb.Version > line.Version)
                    {
                        return Content(HttpStatusCode.Conflict, "CONFLICT You are trying to edit a Line that has been changed recently. Try again. ");
                    }

                    string poruka = unitOfWork.Lines.ReplaceStations(line.Id, line.Stations);
                    if(poruka == "notOk")
                    {
                        return Content(HttpStatusCode.Conflict, $" You are trying to edit a Line which station has been changed. Try again.");
                    }else if(poruka == "nullStation")
                    {
                        return Content(HttpStatusCode.Conflict, $" You are trying to edit a Line which station has been removed. Try again.");
                    }


                    List<SerialNumberSL> st = unitOfWork.SerialNumberSLs.GetAll().Where(sy => sy.LineId == line.Id).ToList();
                    unitOfWork.SerialNumberSLs.RemoveRange(st);
                    int i = 0;
                    foreach (Station s in line.Stations)
                    {
                        i++;
                        SerialNumberSL o = new SerialNumberSL();
                        o.LineId = line.Id;
                        o.StationId = s.Id;
                        o.SerialNumber = i;
                        unitOfWork.SerialNumberSLs.Add(o);

                    }

                }
                else
                {
                    return Content(HttpStatusCode.NotFound, "Line that you are trying to edit either does not exist or was previously deleted by another user.");
                }

                lineDb.Version++;
                unitOfWork.Lines.Update(lineDb);

            try
            {
               
                unitOfWork.Complete();
                return Ok(lineDb.Id);
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

        [Route("Add")]

        // POST: api/Lines
        [ResponseType(typeof(Line))]
        public IHttpActionResult PostLine(Line line)
        {
           
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

                if (LineExists(line.Id))
                {
                return Content(HttpStatusCode.Conflict, "CONFLICT Line already exists!");
                }

            if (line.Version != 0)
            {
                line.Version = 0;

            }

            if (line.LineNumber == "" || line.LineNumber == null)
            {
                return Content(HttpStatusCode.BadRequest, "You have to fill line number!");
            }

            List<Line> listOfLines = unitOfWork.Lines.GetAll().ToList();
            if (listOfLines.Exists(x => x.LineNumber == line.LineNumber))
            {
                return Content(HttpStatusCode.BadRequest, "Line with that number already exists!");
            }
            if (line.Stations == null || line.Stations.Count < 2)
            {
                return Content(HttpStatusCode.BadRequest, "You must add at least two stations per line");
            }



            Line l = new Line();
            l.Stations = new List<Station>();
            l.LineNumber = line.LineNumber;
            l.ColorLine = line.ColorLine;

            List<Station> stats = unitOfWork.Stations.GetAll().ToList();
            //line.Stations.Reverse();
            int i = 0;
            foreach (Station s in line.Stations)
            {

                Station stationAdd = stats.Find(x => x.Id.Equals(s.Id));
                if(stationAdd == null)
                {
                    return Content(HttpStatusCode.Conflict, "CONFLICT Station you want to add in line has been removed!");
                }
                else{
                    if(stationAdd.Version > s.Version)
                    {
                        return Content(HttpStatusCode.Conflict, "CONFLICT Station you want to add in line has been changed!");
                    }
                }
                i++;
                SerialNumberSL o = new SerialNumberSL();
                o.LineId = line.Id;
                o.StationId = s.Id;
                o.SerialNumber = i;
                unitOfWork.SerialNumberSLs.Add(o);
                l.Stations.Add(stats.Find(x => x.Id.Equals(s.Id)));
            }

            try
            {

                unitOfWork.Lines.Add(l);
                unitOfWork.Complete();

                return Ok(l.Id);
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
        // DELETE: api/Lines/5
        [ResponseType(typeof(Line))]
        public IHttpActionResult DeleteLine(int id)
        {
            if (id == 0)
            {
                return Content(HttpStatusCode.BadRequest, "You have to select line you want to remove!");
            }

            Line line = unitOfWork.Lines.Get(id);
            if (line == null)
            {
                 return Content(HttpStatusCode.NotFound, "Line that you are trying to delete either do not exist or was previously deleted by another user.");
            }

            try
            {
                //unitOfWork.Lines.Remove(line);
                unitOfWork.Lines.Delete(id);
                unitOfWork.Complete();

                return Ok(line);
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

        private bool LineExists(int id)
        {
            return unitOfWork.Lines.Get(id) != null;
        }
    }
}