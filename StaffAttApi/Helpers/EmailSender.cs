using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using MimeKit.Text;

namespace StaffAttApi.Helpers;

/// <summary>
/// Implements email sending functionality using SMTP.
/// </summary>
public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string confirmLink)
    {
        MimeMessage message = new MimeMessage();

        string senderEmail = _configuration?["Email:Sender"] ?? throw new InvalidOperationException("Sender email not configured.");
        string password = _configuration?["Email:Password"] ?? throw new InvalidOperationException("Email password not configured.");

        var from = new MailboxAddress("Staff Attendance", senderEmail);
        message.From.Add(from);

        var to = MailboxAddress.Parse(email);
        message.To.Add(to);
        message.Subject = subject;

        message.Body = new TextPart(TextFormat.Html)
        {
            Text = confirmLink
        };

        using SmtpClient smtp = new SmtpClient();

        try
        {
            Console.WriteLine($"EmailSender: sending to {email}, link={confirmLink}");

            await smtp.ConnectAsync("smtp.gmail.com", 465, true);
            await smtp.AuthenticateAsync(senderEmail, password);
            await smtp.SendAsync(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"EmailSender ERROR: {ex.Message}");
        }

        finally
        {
            await smtp.DisconnectAsync(true);
        }
    }
}
