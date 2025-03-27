using EM.BL;
using EM.BL.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EM.UI.MVC.Controllers.Api;
[Route("api/[controller]")]
[ApiController]
[Authorize] 
public class OrganisationsController: ControllerBase
{
    private readonly IManager _manager;

    public OrganisationsController(IManager manager)
    {
        _manager = manager;
    }

    // GET: api/Organisations
    [HttpGet]
    public IActionResult GetAllOrganisations()
    {
        var organisations = _manager.GetAllOrganisations();
        return Ok(organisations);
    }
    
    [HttpPost]
    public IActionResult AddOrganisation([FromBody] Organisation organisation)
    {
        if (organisation == null)
        {
            return BadRequest("Organisation data is required.");
        }

        // Voeg de organisatie toe via de manager
        var addedOrganisation = _manager.AddOrganisation(
            organisation.OrgName,
            organisation.OrgDescription,
            organisation.FoundedDate,
            organisation.ContactEmail
        );

        // Retourneer een 201 Created met de locatie van de nieuwe resource
        return CreatedAtAction(nameof(GetAllOrganisations), new { id = addedOrganisation.OrgId }, addedOrganisation);
    }

   
}