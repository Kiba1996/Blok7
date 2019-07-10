using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebApp.Models;

namespace WebApp.Persistence.Repository
{
    public class PayPalRepository : Repository<PayPal, int>, IPayPalRepository
    {
        protected ApplicationDbContext Context { get { return context as ApplicationDbContext; } }
        public PayPalRepository(DbContext context) : base(context)
        {
        }


        public int GetPayPal(string s)
        {
            List<PayPal> lista = Context.PayPals.ToList();
            return lista.Find(p => p.PayementId == s).Id;
        }
    }
}