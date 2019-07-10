using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Name { get; set; }
       
        public DateTime? PurchaseTime { get; set; }

        //[ForeignKey("TicketType")]
        public int? TicketTypeId { get; set; }
        public TicketType TicketType { get; set; }

        [ForeignKey("TicketPrices")]
        public int TicketPricesId { get; set; }
        public TicketPrices TicketPrices { get; set; }

        [ForeignKey("PayPal")]
        public int PayPalId { get; set; }
        public PayPal PayPal { get; set; }

        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}