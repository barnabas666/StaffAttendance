using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffAttLibrary.Models;

/// <summary>
/// Hold Department data. Properties Matchup with Departments Table from our database.
/// </summary>
public class DepartmentModel
{
    /// <summary>
    /// Department's Id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Department's Title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Department's Description.
    /// </summary>
    public string Description { get; set; }
}
