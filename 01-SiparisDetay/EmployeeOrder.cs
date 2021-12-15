using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_SiparisDetay
{
    public class EmployeeOrder
    {
        public string OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShippedDate { get; set; }
        public string TotalPrice { get; set; }
    }
}
