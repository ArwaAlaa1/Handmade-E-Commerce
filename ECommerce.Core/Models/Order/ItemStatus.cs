﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Models.Order
{
    public enum ItemStatus
    {

        [EnumMember(Value = "Pending")]
        Pending,
        [EnumMember(Value = "InProgress")]
        InProgress,
        [EnumMember(Value = "Ready")]
        Ready,
        [EnumMember(Value = "Cancelled")]
        Cancelled

    }
}
