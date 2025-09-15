using System.Text.Encodings.Web;

namespace StaffAtt.Web.Helpers;

public static class EmailMessages
{
    public static string GetConfirmLinkMessage(string callbackUrl)
    {
        string encodedUrl = HtmlEncoder.Default.Encode(callbackUrl);

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

}
