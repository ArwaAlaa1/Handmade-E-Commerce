﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Models
{
    public class Sale : BaseEntity
    {

        [Range(1, 100)]
        public int Percent { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }

        // Foreign key
        public int ProductId { get; set; }

        // Navigation
        public Product Product { get; set; }
    }

}
