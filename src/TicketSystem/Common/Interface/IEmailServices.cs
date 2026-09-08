namespace TicketSystem.Common.Interface;

public interface IEmailSerivces
{
     public  Task SendEmailAaync(string ToEmail, string Subject, string Body, string Format = "html", CancellationToken ct = default);
}