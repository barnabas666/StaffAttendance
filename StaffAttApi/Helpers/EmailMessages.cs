using System.Text.Encodings.Web;

namespace StaffAttApi.Helpers;

/// <summary>
/// Provides email message templates for account confirmation and password reset.
/// </summary>
public static class EmailMessages
{
    public static string GetConfirmLinkMessage(string callbackUrl)
    {
        // Encode the URL to ensure it is safe for use in HTML
        var encodedUrl = Uri.EscapeDataString(callbackUrl);

        return $"""
        <html>
        <body>
            <p>Thank you for registering for <strong>Polaris Staff Attendance</strong> system! We're excited to have you join us.</p> <br/>

            <p>Please confirm your account by 
            <a href='{encodedUrl}'>clicking here</a>.</p> <br/>

            <p>We look forward to seeing you!</p> <br/>

            <p>Sincerely, </p> <br/>

            <p>Ginevra<br/>
            Staff Attendance Success Manager, Polaris</p>
        </body>
        </html>
        """;
    }

    public static string GetPasswordResetMessage(string resetUrl)
    {
        // Encode the URL to ensure it is safe for use in HTML
        string encodedUrl = HtmlEncoder.Default.Encode(resetUrl);

        return $"""
        <p>You requested to reset your password for <strong>Polaris Staff Attendance</strong> system!</p> <br/>

        <p>Please reset your password by 
        <a href='{encodedUrl}'>clicking here</a>.</p> <br/>        
        """;
    }
}
