using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using Microsoft.Extensions.DependencyInjection;
using Mjml.AspNetCore;
using Services;

var services = new ServiceCollection();
services.AddMjmlServices(options => { });
var provider = services.BuildServiceProvider();
var mjmlEngine = provider.GetRequiredService<IMjmlServices>();

var mjmlTemplate = File.ReadAllText("template.mjml");
var textTemplate = File.ReadAllText("template.txt");

var mailData = new MailData {
    OrderReference = "ABCD12345",
    CustomerName = "Alice Aardvark",
    CustomerAddress = "123 Anything Street\nSomeplace\nAnytown\nABC123\nUnited Kingdom",
    OrderTotal = "£12350.00",
    Items = new List<OrderItem> {
        new OrderItem { Quantity = 2, Description = "All Access Pass", Price = "£9900" },
        new OrderItem { Quantity = 4, Description = "Conference Pass", Price = "£7700" },
        new OrderItem { Quantity = 3, Description = "Workshop Pass", Price = "£3300" },
    },
    Attendees = new List<Attendee> {
        new Attendee { TicketType = "All Access Pass", AttendeeName = "Alice Adelaide", AttendeeEmail = "alice@example.com" },
        new Attendee { TicketType = "Conference Pass", AttendeeName = "Bryan Brisbane", AttendeeEmail = "bryan@example.com" },
        new Attendee { TicketType = "Rocky Mountain Pass", AttendeeName = "Carol Canberra", AttendeeEmail = "carol@example.com" },
        new Attendee { TicketType = "Backstage Pass", AttendeeName = "David Delaware", AttendeeEmail = "david@example.com" },
    }
};

var renderer = new MailRenderer();
var htmlTemplate = renderer.Render(mjmlTemplate, mailData);
var result = await mjmlEngine.Render(htmlTemplate);
var htmlBody = result.Html;
var textBody = renderer.Render(textTemplate, mailData);

var client = new SmtpClient("smtp.mailtrap.io", 2525) {
    Credentials = new NetworkCredential("36192fbee550e6a2f", "ea20ee3fe3a9e5"),
    EnableSsl = true
};

var mailMessage = new MailMessage { From = new MailAddress("hello@example.com", "Mailjet Razor") };
mailMessage.To.Add("recipient@example.com");
mailMessage.Subject = "This is a test message";
mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(textBody, null, MediaTypeNames.Text.Plain));
mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html));
await client.SendMailAsync(mailMessage);

Console.WriteLine("Sent mail");
