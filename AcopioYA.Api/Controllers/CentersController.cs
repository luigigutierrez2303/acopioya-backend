using AcopioYA.Api.Data;
using AcopioYA.Api.Domain.Entities;
using AcopioYA.Api.Domain.Enums;
using AcopioYA.Api.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AcopioYA.Api.Controllers;

[ApiController]
[Route("centers")]
public class CentersController : ControllerBase
{
    private readonly AppDbContext _db;
    public CentersController(AppDbContext db) => _db = db;

    // GET /centers?state=Zulia  (público)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CenterListItemDto>>> GetAll([FromQuery] string? state)
    {
        var query = _db.CollectionCenters
            .AsNoTracking()
            .Where(c => c.Status != CenterStatus.Inactive);

        if (!string.IsNullOrWhiteSpace(state))
            query = query.Where(c => c.State == state);

        var centers = await query
            .OrderByDescending(c => c.IsVerified)
            .ThenByDescending(c => c.CreatedAt)
            .Select(c => new CenterListItemDto(
                c.Id, c.Name, c.State, c.Address,
                c.Latitude, c.Longitude,
                c.Status.ToString(),
                c.IsVerified,
                c.OrganizationName,
                c.Needs.Select(n => new NeedDto(n.Id, n.Category.ToString(), n.Urgency.ToString(), n.Note))
            ))
            .ToListAsync();

        return Ok(centers);
    }

    // GET /centers/{id}  (público)
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CenterDetailDto>> GetById(Guid id)
    {
        var c = await _db.CollectionCenters.AsNoTracking()
            .Include(x => x.Needs)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (c is null) return NotFound();

        return Ok(new CenterDetailDto(
            c.Id, c.Name, c.Description, c.Address, c.State, c.Municipality,
            c.Latitude, c.Longitude, c.Phone, c.WhatsApp, c.OpeningHours, c.OrganizationName,
            c.Status.ToString(), c.IsVerified, c.VerifiedAt, c.CreatedAt, c.UpdatedAt,
            c.Needs.Select(n => new NeedDto(n.Id, n.Category.ToString(), n.Urgency.ToString(), n.Note))
        ));
    }

    // POST /centers  (Admin)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CenterDetailDto>> Create(CenterCreateDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User.FindFirstValue("sub")
                     ?? "unknown";

        var center = new CollectionCenter
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            Address = dto.Address,
            State = dto.State,
            Municipality = dto.Municipality,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Phone = dto.Phone,
            WhatsApp = dto.WhatsApp,
            OpeningHours = dto.OpeningHours,
            OrganizationName = dto.OrganizationName,
            Status = CenterStatus.Active,
            IsVerified = false,
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.CollectionCenters.Add(center);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = center.Id }, center);
    }

    // PUT /centers/{id}  (Admin)
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Update(Guid id, CenterUpdateDto dto)
    {
        var center = await _db.CollectionCenters.FindAsync(id);
        if (center is null) return NotFound();

        if (!Enum.TryParse<CenterStatus>(dto.Status, out var status))
            return BadRequest($"Status inválido: {dto.Status}");

        center.Name = dto.Name;
        center.Description = dto.Description;
        center.Address = dto.Address;
        center.State = dto.State;
        center.Municipality = dto.Municipality;
        center.Latitude = dto.Latitude;
        center.Longitude = dto.Longitude;
        center.Phone = dto.Phone;
        center.WhatsApp = dto.WhatsApp;
        center.OpeningHours = dto.OpeningHours;
        center.OrganizationName = dto.OrganizationName;
        center.Status = status;
        center.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // PATCH /centers/{id}/verify  (Admin)
    [HttpPatch("{id:guid}/verify")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Verify(Guid id)
    {
        var center = await _db.CollectionCenters.FindAsync(id);
        if (center is null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User.FindFirstValue("sub")
                     ?? "unknown";

        center.IsVerified = true;
        center.VerifiedAt = DateTime.UtcNow;
        center.VerifiedByUserId = userId;
        center.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /centers/{id}  (Admin)
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var center = await _db.CollectionCenters.FindAsync(id);
        if (center is null) return NotFound();

        _db.CollectionCenters.Remove(center);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
