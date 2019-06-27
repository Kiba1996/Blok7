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
}