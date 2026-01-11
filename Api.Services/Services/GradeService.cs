using Api.DataAccess.Interfaces;
using Api.DataAccess.Models;
using Api.Services.Interfaces;
using Shared.DTOs;

namespace Api.Services.Services;

public class GradeService : IGradeService
{
    private readonly IGradesRepository _gradesRepo;

    public GradeService(IGradesRepository gradesRepo)
    {
        _gradesRepo = gradesRepo;
    }

    public async Task<GradeResponse> CreateGradeAsync(CreateGradeRequest request, int teacherId)
    {
        var grade = new Grades
        {
            course_name = request.CourseName,
            grade_value = request.GradeValue,
            student_name = request.StudentName,
            teacher_id = teacherId,
            rektor_id = request.RektorId
        };

        var created = await _gradesRepo.CreateAsync(grade);

        return new GradeResponse
        {
            Id = created.id,
            CourseName = created.course_name,
            GradeValue = created.grade_value,
            StudentName = created.student_name,
            TeacherName = created.teacher?.username ?? string.Empty,
            RektorName = created.rektor?.name ?? string.Empty
        };
    }
    public async Task<IEnumerable<GradeResponse>> GetForRektorAsync(int rektorId)
    {
        var grades = await _gradesRepo.GetByRektorIdAsync(rektorId);

        return grades.Select(g => new GradeResponse
        {
            Id = g.id,
            CourseName = g.course_name,
            GradeValue = g.grade_value,
            StudentName = g.student_name,
            TeacherName = g.teacher.username
        });
    }

    public async Task<IEnumerable<GradeResponse>> GetAllGradesAsync()
    {
        var grades = await _gradesRepo.GetAllAsync();
        return grades.Select(g => new GradeResponse
        {
            Id = g.id,
            CourseName = g.course_name,
            GradeValue = g.grade_value,
            StudentName = g.student_name,
            TeacherName = g.teacher?.username ?? string.Empty,
            RektorName = g.rektor?.name ?? string.Empty
        });
    }

    public async Task<bool> DeleteGradeAsync(int id)
    {
        return await _gradesRepo.DeleteAsync(id);
    }
}
