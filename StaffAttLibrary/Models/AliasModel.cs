namespace StaffAttLibrary.Models;

/// <summary>
/// Hold Alias data. Properties Matchup with Aliases Table from our database, not showing PIN.
/// </summary>
public class AliasModel
{
    /// <summary>
    /// Aliases Id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Aliases Alias.
    /// </summary>
    public string Alias { get; set; }
}
