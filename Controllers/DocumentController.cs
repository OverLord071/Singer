using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Singer.Domain;
using Singer.Domain.Dtos;
using Singer.Interfaces;

namespace Singer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DocumentController : ControllerBase
{
    private readonly IDocumentApplication _documentApplication;
    private readonly IUrlHelper _urlHelper;

    public DocumentController(IDocumentApplication documentApplication, IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor)
    {
        _documentApplication = documentApplication;
        _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
    }

    [HttpPost]
    public async Task<IActionResult> SaveDocument([FromBody] DocumentDto documentDto)
    {
        var document = await _documentApplication.SaveDocument(documentDto);
        return Ok();
    }

    [HttpGet("{email}")]
    public async Task<IActionResult> GetDocumentsByUser(string email)
    {
        var documents = await _documentApplication.GetDocumentsByUser(email, id => Url.Action("GetDocumentFile", "Document", new { id = id }, Request.Scheme));
        return Ok(documents);
    }

    [HttpGet("GetDocumentFile/{id}")]
    public async Task<IActionResult> GetDocumentFile(string id)
    {
        var document = await _documentApplication.GetDocumentFile(id);

        if (document == null)
        {
            return NotFound();
        }

        var memory = new MemoryStream(document);
        return File(memory, "application/pdf");
    }
}
