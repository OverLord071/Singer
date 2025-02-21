﻿using Microsoft.AspNetCore.Mvc;
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
        try
        {
            var document = await _documentApplication.SaveDocument(documentDto);
            return Ok(new { message = "OK" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("resend-email/{documentId}")]
    public async Task<IActionResult> ResendEmail(string documentId)
    {
        try
        {
            await _documentApplication.ResendEmailForSignature(documentId);
            return Ok(new { message = "Correo reenviado exitosamente." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
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

    [HttpGet("GetAllDocuments")]
    public async Task<IActionResult> GetAllDocuments()
    {
        var documents = await _documentApplication.GetAllDocuments(id => Url.Action("GetDocumentFile", "Document", new { id = id }, Request.Scheme));
        return Ok(documents);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDocumentIsSigned(string id)
    {
        var result = await _documentApplication.UpdateDocumentIsSigned(id);
        if (!result)
        {
            return NotFound();
        }

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDocument(string id)
    {
        var result = await _documentApplication.DeleteDocument(id);
        if (!result)
        {
            return NotFound("Documento no encontrado");
        }

        return NoContent();
    }
}
