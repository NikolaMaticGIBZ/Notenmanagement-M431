using Api.DataAccess.Models;
using Shared.DTOs;

namespace Api.Services.Interfaces;

public interface IGradeLedgerService
{
    Task AddEntryAsync(Grade grade, string action, int actorUserId, string actorRole);
    Task<List<GradeLedgerEntryResponse>> GetLedgerAsync(int gradeId);
    Task<GradeLedgerVerificationResponse> VerifyLedgerAsync(int gradeId);
}
