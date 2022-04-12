using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PNS_Prototype.Models
{
    public class TempList
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string ProCode { get; set; }
        public string Description { get; set; }
        public Nullable<int> SKU { get; set; }
        public Nullable<int> Order { get; set; }
        public Nullable<int> Supplied { get; set; }
        public Nullable<double> Rate { get; set; }
        public Nullable<double> Total { get; set; }
    }
}