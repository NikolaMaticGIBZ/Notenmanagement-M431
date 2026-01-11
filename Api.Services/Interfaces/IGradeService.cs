using Shared.DTOs;

namespace Api.Services.Interfaces;

public interface IGradeService
{
    Task<GradeResponse> CreateGradeAsync(CreateGradeRequest request, int teacherId);
    Task<IEnumerable<GradeResponse>> GetAllGradesAsync();
    Task<bool> DeleteGradeAsync(int id);
    Task<IEnumerable<GradeResponse>> GetForRektorAsync(int rektorId);

}
