using RazorEngine;
using RazorEngine.Templating;

namespace Services;
public class MailRenderer {
    private IRazorEngineService razor;

    public MailRenderer() {
        razor = Engine.Razor;
    }

    public string Render(string template, object model) {
        var viewBag = new DynamicViewBag();
        viewBag.AddValue("SomeKey1", "Some value 1");
        viewBag.AddValue("SomeKey2", $"Some value 2");
        var templateKey = template.GetHashCode().ToString();
        razor.AddTemplate(templateKey, template);
        var writer = new StringWriter();
        razor.RunCompile(templateKey, writer, model.GetType(), model, viewBag);
        return writer.ToString();
    }
}


public class MailData {
    public String OrderReference { get; set; } = String.Empty;
    public List<OrderItem> Items { get; set; } = new();
    public string CustomerName { get; set; } = String.Empty;
    public string CustomerEmail { get; set; } = String.Empty;
    public string CustomerAddress { get; set; } = String.Empty;
    public List<Attendee> Attendees { get; set; } = new();

    public string OrderTotal { get; set; }
}

public class OrderItem {
    public int Quantity { get; set; }
    public string Description { get; set; } = String.Empty;
    public string Price { get; set; } = String.Empty;
}

public class Attendee {
    public string TicketType { get; set; } = String.Empty;
    public string AttendeeName { get; set; } = String.Empty;
    public string AttendeeEmail { get; set; } = String.Empty;
    public bool HasInfo => !(String.IsNullOrEmpty(AttendeeName) && String.IsNullOrEmpty(AttendeeEmail));
}