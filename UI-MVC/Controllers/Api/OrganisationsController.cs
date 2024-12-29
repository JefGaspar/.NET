using EM.BL;
using EM.BL.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EM.UI.MVC.Controllers.Api;
[Route("api/[controller]")]
[ApiController]
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
    public IActionResult AddOrganisation(Organisation newOrganisation)
    {
        var addedOrganisation = _manager.AddOrganisation(
            newOrganisation.OrgName,
            newOrganisation.OrgDescription,
            newOrganisation.FoundedDate,
            newOrganisation.ContactEmail
        );

        // Return een response met status 201 (Created) en het nieuwe object
        return CreatedAtAction(nameof(GetAllOrganisations), new { id = addedOrganisation.OrgId }, addedOrganisation);
    }

}