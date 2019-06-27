using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using WebApp.Models;

namespace WebApp.Persistence.Repository
{
    public class TicketRepository : Repository<Ticket, int>, ITicketRepository
    {
        protected ApplicationDbContext Context { get { return context as ApplicationDbContext; } }
        public TicketRepository(DbContext context) : base(context)
        {
        }

        public Ticket GetTicketWithInclude(int id)
        {
            List<Ticket> t = Context.Tickets.Include(x=> x.ApplicationUser).ToList();
            Ticket tt = t.Find(x => x.Id == id);
            return tt;
        }

        public bool NotifyViaEmail(string targetEmail, string subject, string body)
        {
            string mailTo = targetEmail;
            string mailFrom = "pusgs2019app@gmail.com";
            string pass = "12345Aa.";

            try
            {
                SmtpClient client = new SmtpClient();
                client.Port = 587;
                client.Host = "smtp.gmail.com";
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Timeout = 10000;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(mailFrom, pass);

                MailMessage mm = new MailMessage(mailFrom, mailTo);
                mm.BodyEncoding = UTF8Encoding.UTF8;
                mm.Subject = subject;
                mm.Body = body;
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                client.Send(mm);
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("Error, mail did not send");
                return false;
            }
        }

        public IEnumerable<Ticket> getAllTicketsWithUser(string id)
        {
            List<Ticket> lista = Context.Tickets.Include(x => x.ApplicationUser).Where(x => x.ApplicationUser!=null).ToList();
            List<Ticket> ret = lista.FindAll(m => m.ApplicationUser.Email == id);

            return ret;
        }
    }
}