
using System.Net.Security;
using System.Security.Cryptography;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using TicketSystem.Common.Interface;

namespace TicketSystem.Infrastructure.Services;

public class EmailServices(IConfiguration configuration) : IEmailSerivces
{
    private readonly IConfiguration configuration = configuration;

public async Task SendEmailAaync(string ToEmail, string Subject, string Body, string Format = "plain", CancellationToken ct = default)
    {
        var emailSettings = configuration.GetSection("EmailSettings");
        var SmtpServer = emailSettings["SmtpServer"]!;
        var SmtpPort = int.Parse(emailSettings["SmtpPort"]!);
        var SenderEmail = emailSettings["SenderEmail"]!;
        var SenderName = emailSettings["SenderName"];
        var Password = emailSettings["Password"]!;
        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(new MailboxAddress(SenderName, SenderEmail!));
        mimeMessage.To.Add(new MailboxAddress("", ToEmail));
        mimeMessage.Subject = Subject;
        mimeMessage.Body = new TextPart(Format) { Text = Body };

        using (var client = new SmtpClient())
            try
            {
                await client.ConnectAsync(SmtpServer, SmtpPort, SecureSocketOptions.SslOnConnect, ct);
                await client.AuthenticateAsync(SenderEmail , Password);
                await client.SendAsync(mimeMessage, ct);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Send Email Error{ex.Message}");
                throw;

            }
            finally
            {
               await client.DisconnectAsync(true ,ct);
            }

    }
}