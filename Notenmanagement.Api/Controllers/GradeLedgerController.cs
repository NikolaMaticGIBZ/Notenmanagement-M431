using Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Notenmanagement.Api.Controllers;

[Route("api/grade-ledger")]
[ApiController]
[Authorize]
public class GradeLedgerController : ControllerBase
{
    private readonly IGradeLedgerService _ledgerService;

    public GradeLedgerController(IGradeLedgerService ledgerService)
    {
        _ledgerService = ledgerService;
    }

    [HttpGet("{gradeId:int}")]
    [Authorize(Roles = "rektor,teacher")]
    public async Task<IActionResult> GetLedger(int gradeId)
    {
        List<Shared.DTOs.GradeLedgerEntryResponse> entries = await _ledgerService.GetLedgerAsync(gradeId);
        return Ok(entries);
    }

    [HttpGet("{gradeId:int}/verify")]
    [Authorize(Roles = "rektor,teacher")]
    public async Task<IActionResult> VerifyLedger(int gradeId)
    {
        Shared.DTOs.GradeLedgerVerificationResponse result = await _ledgerService.VerifyLedgerAsync(gradeId);
        return Ok(result);
    }
}
