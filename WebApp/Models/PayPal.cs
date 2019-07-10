using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class PayPal
    {
        public int Id { get; set; }
        public string PayementId { get; set; }

        public DateTime? CreateTime { get; set; }

        public string PayerEmail { get; set; }
        public string PayerName { get; set; }
        public string PayerSurname { get; set; }

        public string CurrencyCode { get; set; }
        public double Value { get; set; }
    }
}