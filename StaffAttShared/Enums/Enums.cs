using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttShared.Enums;
/// <summary>
/// Enum for Staff's Approved status.
/// Values 0 and 1 are used in SQL queries.
/// </summary>
public enum ApprovedType
{
    All,
    Approved,
    NotApproved
}
