using Api.DataAccess;
using Api.DataAccess.Models;
using Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Api.Services.Services;

public class GradeLedgerService : IGradeLedgerService
{
    private const string GenesisHash = "GENESIS";
    private readonly AppDBContext _db;

    public GradeLedgerService(AppDBContext db)
    {
        _db = db;
    }

    public async Task AddEntryAsync(Grade grade, string action, int actorUserId, string actorRole)
    {
        var previousHash = await _db.GradeLedger
            .Where(e => e.Grade_id == grade.Id)
            .OrderByDescending(e => e.Created_at)
            .ThenByDescending(e => e.Id)
            .Select(e => e.Block_hash)
            .FirstOrDefaultAsync();

        var snapshot = new GradeSnapshot
        {
            GradeId = grade.Id,
            CourseName = grade.Course_name,
            ModuleName = grade.Module_name,
            StudentName = grade.Student_name,
            GradeValue = grade.Grade_value,
            Status = grade.Status,
            Comment = grade.Comment,
            DecisionNote = grade.Decision_note,
            TeacherId = grade.Teacher_id,
            RektorId = grade.Rektor_id,
            ProrektorId = grade.prorektor_id,
            CreatedAt = grade.Created_at,
            Action = action
        };

        var snapshotJson = JsonSerializer.Serialize(snapshot);
        var payloadHash = ComputeHash(snapshotJson);
        var createdAt = DateTime.UtcNow;
        var blockHash = ComputeHash($"{previousHash ?? GenesisHash}|{payloadHash}|{action}|{createdAt:o}|{actorUserId}|{actorRole}|{grade.Id}");

        var entry = new GradeLedgerEntry
        {
            Grade_id = grade.Id,
            Action = action,
            Snapshot_json = snapshotJson,
            Payload_hash = payloadHash,
            Previous_hash = previousHash ?? GenesisHash,
            Block_hash = blockHash,
            Created_at = createdAt,
            Actor_user_id = actorUserId,
            Actor_role = actorRole
        };

        _db.GradeLedger.Add(entry);
        await _db.SaveChangesAsync();
    }

    public async Task<List<GradeLedgerEntryResponse>> GetLedgerAsync(int gradeId)
    {
        var entries = await _db.GradeLedger
            .Where(e => e.Grade_id == gradeId)
            .OrderBy(e => e.Created_at)
            .ThenBy(e => e.Id)
            .ToListAsync();

        return entries.Select(e => new GradeLedgerEntryResponse
        {
            Id = e.Id,
            GradeId = e.Grade_id,
            Action = e.Action,
            SnapshotJson = e.Snapshot_json,
            PayloadHash = e.Payload_hash,
            PreviousHash = e.Previous_hash,
            BlockHash = e.Block_hash,
            CreatedAt = e.Created_at,
            ActorUserId = e.Actor_user_id,
            ActorRole = e.Actor_role
        }).ToList();
    }

    public async Task<GradeLedgerVerificationResponse> VerifyLedgerAsync(int gradeId)
    {
        var entries = await _db.GradeLedger
            .Where(e => e.Grade_id == gradeId)
            .OrderBy(e => e.Created_at)
            .ThenBy(e => e.Id)
            .ToListAsync();

        if (entries.Count == 0)
        {
            return new GradeLedgerVerificationResponse
            {
                IsValid = false,
                Message = "No ledger entries found for this grade."
            };
        }

        string? previousHash = null;
        foreach (var entry in entries)
        {
            var payloadHash = ComputeHash(entry.Snapshot_json);
            if (!string.Equals(payloadHash, entry.Payload_hash, StringComparison.OrdinalIgnoreCase))
            {
                return new GradeLedgerVerificationResponse
                {
                    IsValid = false,
                    Message = "Payload hash mismatch detected.",
                    InvalidEntryId = entry.Id
                };
            }

            var expectedPrevious = previousHash ?? GenesisHash;
            if (!string.Equals(expectedPrevious, entry.Previous_hash, StringComparison.OrdinalIgnoreCase))
            {
                return new GradeLedgerVerificationResponse
                {
                    IsValid = false,
                    Message = "Previous hash mismatch detected.",
                    InvalidEntryId = entry.Id
                };
            }

            var expectedBlockHash = ComputeHash($"{entry.Previous_hash}|{entry.Payload_hash}|{entry.Action}|{entry.Created_at:o}|{entry.Actor_user_id}|{entry.Actor_role}|{entry.Grade_id}");
            if (!string.Equals(expectedBlockHash, entry.Block_hash, StringComparison.OrdinalIgnoreCase))
            {
                return new GradeLedgerVerificationResponse
                {
                    IsValid = false,
                    Message = "Block hash mismatch detected.",
                    InvalidEntryId = entry.Id
                };
            }

            previousHash = entry.Block_hash;
        }

        return new GradeLedgerVerificationResponse
        {
            IsValid = true,
            Message = "Ledger chain is valid.",
            LatestBlockHash = previousHash
        };
    }

    private static string ComputeHash(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        var builder = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }

        return builder.ToString();
    }

    private sealed class GradeSnapshot
    {
        public int GradeId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string ModuleName { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public decimal GradeValue { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Comment { get; set; }
        public string? DecisionNote { get; set; }
        public int TeacherId { get; set; }
        public int RektorId { get; set; }
        public int? ProrektorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Action { get; set; } = string.Empty;
    }
}
