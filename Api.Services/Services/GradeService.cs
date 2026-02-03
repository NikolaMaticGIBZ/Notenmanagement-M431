using Api.DataAccess;
using Api.DataAccess.Models;
using Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;

namespace Api.Services.Services;

/// <summary>
/// Service for grades
/// </summary>
/// <seealso cref="Api.Services.Interfaces.IGradeService" />
public class GradeService : IGradeService
{
    private readonly AppDBContext _db;
    private readonly IGradeLedgerService _ledgerService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GradeService"/> class.
    /// </summary>
    /// <param name="db">The database.</param>
    public GradeService(AppDBContext db, IGradeLedgerService ledgerService)
    {
        _db = db;
        _ledgerService = ledgerService;
    }

    /// <inheritdoc/>
    public async Task<GradeResponse> CreateGradeAsync(CreateGradeRequest request, int teacherId)
    {
        // Validate teacher exists
        var teacher = await _db.User.FirstOrDefaultAsync(u => u.Id == teacherId);
        if (teacher == null)
            throw new InvalidOperationException("Lehrer nicht gefunden.");

        // Validate rektor exists
        var rektor = await _db.Rektor.FirstOrDefaultAsync(r => r.Id == request.RektorId);
        if (rektor == null)
            throw new InvalidOperationException("Prorektor nocht gefunden.");

        var entity = new Grade
        {
            Course_name = request.CourseName,
            Module_name = request.ModuleName,
            Student_name = request.StudentName,
            Grade_value = request.GradeValue,
            Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim(),


            Teacher_id = teacherId,
            Rektor_id = request.RektorId,

            // workflow defaults
            Status = "pending",
            Created_at = DateTime.UtcNow,
            Decision_note = null,
            prorektor_id = null
        };

        _db.Grade.Add(entity);
        await _db.SaveChangesAsync();

        await _ledgerService.AddEntryAsync(entity, "created", teacherId, "teacher");

        // Reload with navigation props for names
        var saved = await _db.Grade
            .Include(g => g.Teacher)
            .Include(g => g.Rektor)
            .FirstAsync(g => g.Id == entity.Id);

        return MapToResponse(saved);
    }

    /// <inheritdoc/>
    public async Task<List<GradeResponse>> GetGradesAsync(string? status)
    {
        var query = _db.Grade
            .Include(g => g.Teacher)
            .Include(g => g.Rektor)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            var s = status.Trim().ToLower();

            // only allow known values (optional but good)
            if (s is not ("pending" or "approved" or "rejected"))
                return new List<GradeResponse>();

            query = query.Where(g => g.Status.ToLower() == s);
        }

        var grades = await query
            .OrderByDescending(g => g.Created_at)
            .ToListAsync();

        return grades.Select(MapToResponse).ToList();
    }

    /// <inheritdoc/>
    public async Task<GradeResponse?> GetByIdAsync(int id)
    {
        var grade = await _db.Grade
            .Include(g => g.Teacher)
            .Include(g => g.Rektor)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (grade == null) return null;

        return MapToResponse(grade);
    }

    /// <inheritdoc/>
    public async Task<bool> DecideAsync(int gradeId, int rektorId, string status, string? decisionNote)
    {
        var grade = await _db.Grade.FirstOrDefaultAsync(g => g.Id == gradeId);
        if (grade == null) return false;

        var s = status?.Trim().ToLower();
        if (s is not ("approved" or "rejected"))
            throw new ArgumentException("Status must be 'approved' or 'rejected'.");

        grade.Status = s;
        grade.Decision_note = string.IsNullOrWhiteSpace(decisionNote) ? null : decisionNote.Trim();
        grade.prorektor_id = rektorId;

        await _db.SaveChangesAsync();
        await _ledgerService.AddEntryAsync(grade, "decided", rektorId, "rektor");
        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteGradeAsync(int id, int rektorId)
    {
        var grade = await _db.Grade.FirstOrDefaultAsync(g => g.Id == id);
        if (grade == null) return false;

        await _ledgerService.AddEntryAsync(grade, "deleted", rektorId, "rektor");
        _db.Grade.Remove(grade);
        await _db.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public async Task<List<GradeResponse>> GetMyGradesAsync(int teacherId)
    {
        var grades = await _db.Grade
            .Include(g => g.Teacher)
            .Include(g => g.Rektor)
            .Where(g => g.Teacher_id == teacherId)
            .OrderByDescending(g => g.Created_at)
            .ToListAsync();

        return grades.Select(MapToResponse).ToList();
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateMyGradeAsync(int gradeId, int teacherId, UpdateGradeRequest request)
    {
        var grade = await _db.Grade.FirstOrDefaultAsync(g => g.Id == gradeId && g.Teacher_id == teacherId);
        if (grade == null) return false;

        // only editable if pending
        if (!string.Equals(grade.Status, "pending", StringComparison.OrdinalIgnoreCase))
            return false;

        // validate rektor exists (optional but good)
        var rektorExists = await _db.Rektor.AnyAsync(r => r.Id == request.RektorId);
        if (!rektorExists) throw new InvalidOperationException("Prorektor nicht gefunden.");

        grade.Course_name = request.CourseName;
        grade.Module_name = request.ModuleName;
        grade.Student_name = request.StudentName;
        grade.Grade_value = request.GradeValue;
        grade.Rektor_id = request.RektorId;

        // store teacher comment (make sure your Grades entity has comment column)
        grade.Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim();

        await _db.SaveChangesAsync();
        await _ledgerService.AddEntryAsync(grade, "updated", teacherId, "teacher");
        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteMyGradeAsync(int id, int teacherId)
    {
        var grade = await _db.Grade.FirstOrDefaultAsync(g => g.Id == id);
        if (grade == null) return false;

        // must be owner
        if (grade.Teacher_id != teacherId) return false;

        // only allow delete while pending
        if (!string.Equals(grade.Status, "pending", StringComparison.OrdinalIgnoreCase))
            return false;

        await _ledgerService.AddEntryAsync(grade, "deleted", teacherId, "teacher");
        _db.Grade.Remove(grade);
        await _db.SaveChangesAsync();
        return true;
    }

    private static GradeResponse MapToResponse(Grade g) => new()
    {
        Id = g.Id,
        CourseName = g.Course_name,
        ModuleName = g.Module_name,
        StudentName = g.Student_name,
        GradeValue = g.Grade_value,

        TeacherName = g.Teacher?.Username ?? "",
        RektorName = g.Rektor?.Name ?? "",

        Status = g.Status,
        CreatedAt = g.Created_at,
        Comment = g.Comment,
        DecisionNote = g.Decision_note
    };
}
