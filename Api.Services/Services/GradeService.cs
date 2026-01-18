using Api.DataAccess;
using Api.DataAccess.Models;
using Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;

namespace Api.Services.Services;

public class GradeService : IGradeService
{
    private readonly AppDBContext _db;

    public GradeService(AppDBContext db)
    {
        _db = db;
    }

    public async Task<GradeResponse> CreateGradeAsync(CreateGradeRequest request, int teacherId)
    {
        // Validate teacher exists
        var teacher = await _db.Users.FirstOrDefaultAsync(u => u.id == teacherId);
        if (teacher == null)
            throw new InvalidOperationException("Teacher not found.");

        // Validate rektor exists
        var rektor = await _db.Rektors.FirstOrDefaultAsync(r => r.id == request.RektorId);
        if (rektor == null)
            throw new InvalidOperationException("Rektor not found.");

        var entity = new Grades
        {
            course_name = request.CourseName,
            module_name = request.ModuleName,
            student_name = request.StudentName,
            grade_value = request.GradeValue,
            comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim(),


            teacher_id = teacherId,
            rektor_id = request.RektorId,

            // workflow defaults
            status = "pending",
            created_at = DateTime.UtcNow,
            decision_note = null,
            prorektor_id = null
        };

        _db.Grades.Add(entity);
        await _db.SaveChangesAsync();

        // Reload with navigation props for names
        var saved = await _db.Grades
            .Include(g => g.teacher)
            .Include(g => g.rektor)
            .FirstAsync(g => g.id == entity.id);

        return MapToResponse(saved);
    }

    public async Task<List<GradeResponse>> GetGradesAsync(string? status)
    {
        var query = _db.Grades
            .Include(g => g.teacher)
            .Include(g => g.rektor)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            var s = status.Trim().ToLower();

            // only allow known values (optional but good)
            if (s is not ("pending" or "approved" or "rejected"))
                return new List<GradeResponse>();

            query = query.Where(g => g.status.ToLower() == s);
        }

        var grades = await query
            .OrderByDescending(g => g.created_at)
            .ToListAsync();

        return grades.Select(MapToResponse).ToList();
    }

    public async Task<GradeResponse?> GetByIdAsync(int id)
    {
        var grade = await _db.Grades
            .Include(g => g.teacher)
            .Include(g => g.rektor)
            .FirstOrDefaultAsync(g => g.id == id);

        if (grade == null) return null;

        return MapToResponse(grade);
    }

    public async Task<bool> DecideAsync(int gradeId, string status, string? decisionNote)
    {
        var grade = await _db.Grades.FirstOrDefaultAsync(g => g.id == gradeId);
        if (grade == null) return false;

        var s = status?.Trim().ToLower();
        if (s is not ("approved" or "rejected"))
            throw new ArgumentException("Status must be 'approved' or 'rejected'.");

        grade.status = s;
        grade.decision_note = string.IsNullOrWhiteSpace(decisionNote) ? null : decisionNote.Trim();

        // Optional: store who decided (requires prorektor_id + JWT sub in controller)
        // You can pass prorektorId into DecideAsync if you want to save it.
        // grade.prorektor_id = prorektorId;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteGradeAsync(int id)
    {
        var grade = await _db.Grades.FirstOrDefaultAsync(g => g.id == id);
        if (grade == null) return false;

        _db.Grades.Remove(grade);
        await _db.SaveChangesAsync();
        return true;
    }

    private static GradeResponse MapToResponse(Grades g) => new()
    {
        Id = g.id,
        CourseName = g.course_name,
        ModuleName = g.module_name,
        StudentName = g.student_name,
        GradeValue = g.grade_value,

        TeacherName = g.teacher?.username ?? "",
        RektorName = g.rektor?.name ?? "",

        Status = g.status,
        CreatedAt = g.created_at,
        Comment = g.comment,
        DecisionNote = g.decision_note
    };
    public async Task<List<GradeResponse>> GetMyGradesAsync(int teacherId)
    {
        var grades = await _db.Grades
            .Include(g => g.teacher)
            .Include(g => g.rektor)
            .Where(g => g.teacher_id == teacherId)
            .OrderByDescending(g => g.created_at)
            .ToListAsync();

        return grades.Select(MapToResponse).ToList();
    }

    public async Task<bool> UpdateMyGradeAsync(int gradeId, int teacherId, UpdateGradeRequest request)
    {
        var grade = await _db.Grades.FirstOrDefaultAsync(g => g.id == gradeId && g.teacher_id == teacherId);
        if (grade == null) return false;

        // only editable if pending
        if (!string.Equals(grade.status, "pending", StringComparison.OrdinalIgnoreCase))
            return false;

        // validate rektor exists (optional but good)
        var rektorExists = await _db.Rektors.AnyAsync(r => r.id == request.RektorId);
        if (!rektorExists) throw new InvalidOperationException("Rektor not found.");

        grade.course_name = request.CourseName;
        grade.module_name = request.ModuleName;
        grade.student_name = request.StudentName;
        grade.grade_value = request.GradeValue;
        grade.rektor_id = request.RektorId;

        // store teacher comment (make sure your Grades entity has comment column)
        grade.comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim();

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteMyGradeAsync(int id, int teacherId)
    {
        var grade = await _db.Grades.FirstOrDefaultAsync(g => g.id == id);
        if (grade == null) return false;

        // must be owner
        if (grade.teacher_id != teacherId) return false;

        // only allow delete while pending
        if (!string.Equals(grade.status, "pending", StringComparison.OrdinalIgnoreCase))
            return false;

        _db.Grades.Remove(grade);
        await _db.SaveChangesAsync();
        return true;
    }

}
