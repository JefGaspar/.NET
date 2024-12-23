using UI;

namespace EM.UI.CA.Extentions;

internal static class EventExtentions
{
    internal static string ToStringEvent(this Event evnt)
    {
        string ticketPriceStr = evnt.TicketPrice.HasValue ? $"€{evnt.TicketPrice.Value:F2}" : "Free";
        string description = evnt.EventDescription ?? "No description available";
        string organisationName = evnt.Organisation?.OrgName ?? "No organisation";


        return
            $"{evnt.EventName} [organized by {organisationName}] : Date: {evnt.EventDate:yyyy-MM-dd HH:mm}, Description: {description}, Price: {ticketPriceStr}, Category: {evnt.Category}";
    }
}