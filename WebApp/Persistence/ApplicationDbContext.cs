using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Reflection.Emit;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using WebApp.Models;

namespace WebApp.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public DbSet<Line> Lines { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Timetable> Timetables { get; set; }
        public DbSet<DayType> DayTypes { get; set; }
        public DbSet<PassengerType> PassengerTypes { get; set; }
        public DbSet<Pricelist> Pricelists { get; set; }
        public DbSet<TicketPrices> TicketPrices { get; set; }
        public DbSet<TicketType> TicketTypes { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<SerialNumberSL> SerialNumberSLs { get; set; }
        public DbSet<PayPal> PayPals { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }



        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

        //    //modelBuilder.Entity<TicketPrices>()
        //    //.HasRequired(f => f.TicketType)
        //    //.WithMany()
        //    //.WillCascadeOnDelete(false);


        //}

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Ticket>().HasOptional(a => a.ApplicationUser)
        //                        .WithMany(au => au.Tickets)
        //                        .HasForeignKey(a => a.ApplicationUserId);
        //}

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

       
    }
}