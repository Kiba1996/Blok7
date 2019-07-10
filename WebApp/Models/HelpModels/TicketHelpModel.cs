using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models.HelpModels
{
    public class TicketHelpModel
    {
        public DateTime? PurchaseTime { get; set; }
        public string ExparationTime { get; set; }
        public string TicketType { get; set; }
        public double TicketPrice { get; set; }
    }
    public class ModelHelpTicketValidation
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class TicketHelpModel1
    {
        public Ticket Ticket { get; set; }
        public string PayementId { get; set; }

        public TicketHelpModel1()
        {

        }

        public TicketHelpModel1(Ticket t, string s)
        {
            Ticket = t;
            PayementId = s;
        }
    }
}