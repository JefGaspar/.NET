using UI;

namespace EM.UI.CA.Extentions;

internal static class VisitorExtentions
{
    internal static string ToStringVisitor(this Visitor visitor)
    {
        string baseInfo = $"Visitor: {visitor.FirstName} {visitor.LastName}, Email: {visitor.Email}, Phone: {visitor.PhoneNumber}, City: {visitor.City}";

        if (visitor.Tickets.Any())
        {
            string eventInfo = string.Join(Environment.NewLine, visitor.Tickets
                .Select(ticket => $"  - {ticket.Event.EventName} "));
            return $"{baseInfo}{Environment.NewLine}{eventInfo}";
        }
        else
        {
            return $"{baseInfo}{Environment.NewLine}Events: No events attended.";
        }
    }

}