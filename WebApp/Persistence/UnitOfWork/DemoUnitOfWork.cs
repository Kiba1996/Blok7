using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using Unity;
using WebApp.Persistence.Repository;

namespace WebApp.Persistence.UnitOfWork
{
    public class DemoUnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
     
        public DemoUnitOfWork(DbContext context)
        {
            _context = context;
        }

        [Dependency]
        public IDayTypeRepository DayTypes { get; set; }
        [Dependency]
        public ILineRepository Lines { get; set; }
        [Dependency]
        public IPassengerTypeRepository PassengerTypes { get; set; }
        [Dependency]
        public IPricelistRepository PriceLists { get; set; }
        [Dependency]
        public IStationRepository Stations { get; set; }
        [Dependency]
        public ITicketRepository Tickets { get; set; }
        [Dependency]
        public ITicketPricesRepository TicketPrices { get; set; }
        [Dependency]
        public ITicketTypeRepository TicketTypes { get; set; }
        [Dependency]
        public ITimetableRepository Timetables { get; set; }
        [Dependency]
        public IVehicleRepository Vehicles { get; set; }
        [Dependency]
        public ISerialNumberSLRepository SerialNumberSLs { get; set; }
        [Dependency]
        public IPayPalRepository PayPals { get; set; }

        public int Complete()
        {
           // return _context.SaveChanges();
            
            
                return _context.SaveChanges();
            
           
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}