using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OzonTask.Models
{
    public class EmailResponse : Email
    {
        public int id { get; set; }
        public string sender { get; set; }
        public bool result { get; set; }
    }
}
