using EM.BL.Domain;

namespace EM.UI.CA.Extentions;

internal static class OrganisationExtentions
{
    internal static string ToStringOrganisation(this Organisation organisation)
    {
        return $"Organisation: {organisation.OrgName} (ID: {organisation.OrgId}), Founded: {organisation.FoundedDate.ToShortDateString()}, Contact: {organisation.ContactEmail}" +
               (organisation.OrgDescription != null ? $", Description: {organisation.OrgDescription}" : "");
    }
}