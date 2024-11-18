namespace UI.Extentions;

internal static class ToStringExtentions
{
    internal static string ToStringEvent(this Event evnt)
    {
        string ticketPriceStr = evnt.TicketPrice.HasValue ? $"€{evnt.TicketPrice.Value:F2}" : "Free";
        string description = evnt.EventDescription ?? "No description available";

        return $"{evnt.EventName}: Date: {evnt.EventDate:yyyy-MM-dd HH:mm}, Description: {description}, Price: {ticketPriceStr}, Category: {evnt.Category}";

    }

    internal static string ToStringOrganisation(this Organisation organisation)
    {
        return $"Organisation: {organisation.OrgName} (ID: {organisation.OrgId}), Founded: {organisation.FoundedDate.ToShortDateString()}, Contact: {organisation.ContactEmail}" +
               (organisation.OrgDescription != null ? $", Description: {organisation.OrgDescription}" : "");
    }

    internal static string ToStringVisitor(this Visitor visitor)
    {
        return $"Visitor: {visitor.FirstName} {visitor.LastName}, Email: {visitor.Email}, Phone: {visitor.PhoneNumber}, City: {visitor.City}";

    }
}