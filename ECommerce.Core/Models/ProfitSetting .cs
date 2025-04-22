using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Models
{
    public class ProfitSetting :BaseEntity
    {
        public decimal Percentage { get; set; }
    }
}
