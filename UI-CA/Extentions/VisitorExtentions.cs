using UI;

namespace EM.UI.CA.Extentions;

internal static class VisitorExtentions
{
    internal static string ToStringVisitor(this Visitor visitor)
    {
        return $"Visitor: {visitor.FirstName} {visitor.LastName}, Email: {visitor.Email}, Phone: {visitor.PhoneNumber}, City: {visitor.City}";

    }
}