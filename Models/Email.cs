using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OzonTask.Models
{
    public class Email
    { 
        public string recipient { get; set; }
        public string subject { get; set; }
        public string text { get; set; }
        public string[] carbon_copy_recipients { get; set; }
    }
}
