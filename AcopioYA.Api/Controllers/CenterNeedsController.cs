using AcopioYA.Api.Data;
using AcopioYA.Api.Domain.Entities;
using AcopioYA.Api.Domain.Enums;
using AcopioYA.Api.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcopioYA.Api.Controllers;

[ApiController]
[Route("centers/{centerId:guid}/needs")]
[Authorize(Roles = "Admin")]
public class CenterNeedsController : ControllerBase
{
    private readonly AppDbContext _db;
    public CenterNeedsController(AppDbContext db) => _db = db;

    [HttpPost]
    public async Task<ActionResult<CenterNeedResponseDto>> Create(Guid centerId, CenterNeedCreateDto dto)
    {
        var centerExists = await _db.CollectionCenters.AnyAsync(c => c.Id == centerId);
        if (!centerExists) return NotFound("El centro no existe.");

        if (!Enum.TryParse<NeedCategory>(dto.Category, out var category))
            return BadRequest($"Category inválida: {dto.Category}");

        if (!Enum.TryParse<UrgencyLevel>(dto.Urgency, out var urgency))
            return BadRequest($"Urgency inválida: {dto.Urgency}");

        var need = new CenterNeed
        {
            Id = Guid.NewGuid(),
            CollectionCenterId = centerId,
            Category = category,
            Note = dto.Note,
            Urgency = urgency
        };

        _db.CenterNeeds.Add(need);

        var center = await _db.CollectionCenters.FindAsync(centerId);
        center!.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        var response = new CenterNeedResponseDto(need.Id, need.Category.ToString(), need.Note, need.Urgency.ToString());
        return CreatedAtAction(nameof(Create), new { centerId }, response);
    }

    [HttpPut("{needId:guid}")]
    public async Task<ActionResult> Update(Guid centerId, Guid needId, CenterNeedUpdateDto dto)
    {
        var need = await _db.CenterNeeds.FirstOrDefaultAsync(n => n.Id == needId && n.CollectionCenterId == centerId);
        if (need is null) return NotFound();

        if (!Enum.TryParse<NeedCategory>(dto.Category, out var category))
            return BadRequest($"Category inválida: {dto.Category}");

        if (!Enum.TryParse<UrgencyLevel>(dto.Urgency, out var urgency))
            return BadRequest($"Urgency inválida: {dto.Urgency}");

        need.Category = category;
        need.Note = dto.Note;
        need.Urgency = urgency;

        var center = await _db.CollectionCenters.FindAsync(centerId);
        center!.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{needId:guid}")]
    public async Task<ActionResult> Delete(Guid centerId, Guid needId)
    {
        var need = await _db.CenterNeeds.FirstOrDefaultAsync(n => n.Id == needId && n.CollectionCenterId == centerId);
        if (need is null) return NotFound();

        _db.CenterNeeds.Remove(need);

        var center = await _db.CollectionCenters.FindAsync(centerId);
        center!.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return NoContent();
    }
}
