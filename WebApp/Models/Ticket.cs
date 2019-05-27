﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static WebApp.Models.Enums;

namespace WebApp.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public TicketType Type { get; set; }
        public DateTime DateTime { get; set; } 

    }
}