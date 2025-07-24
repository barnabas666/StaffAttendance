namespace StaffAttLibrary.Models;

/// <summary>
/// Hold PhoneNumber data. Properties Matchup with PhoneNumbers Table from our database.
/// </summary>
public class PhoneNumberModel
{
    /// <summary>
    /// PhoneNumber's Id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// PhoneNumber's value.
    /// </summary>
    public string PhoneNumber { get; set; }

    public override bool Equals(object obj)
    {
        // If the passed object is null or is not a PhoneNumberModel object, return false.
        if (obj == null || ((obj is PhoneNumberModel) == false))
            return false;

        return (PhoneNumber == ((PhoneNumberModel)obj).PhoneNumber);
    }

    public override int GetHashCode()
    {
        return PhoneNumber.GetHashCode();
    }
}
