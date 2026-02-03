using Api.DataAccess.Interfaces;
using Api.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.DataAccess.Repositories;

/// <summary>
/// Repository for grades
/// </summary>
/// <seealso cref="Api.DataAccess.Interfaces.IGradesRepository" />
public class GradeRepository : IGradesRepository
{
    private readonly AppDBContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GradeRepository"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    public GradeRepository(AppDBContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<Grade> CreateAsync(Grade grade)
    {
        _context.Grade.Add(grade);
        await _context.SaveChangesAsync();
        return grade;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Grade>> GetAllAsync()
    {
        return await _context.Grade
            .Include(g => g.Teacher)
            .Include(g => g.Rektor)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<Grade?> GetByIdAsync(int id)
    {
        return await _context.Grade
            .Include(g => g.Teacher)
            .Include(g => g.Rektor)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateAsync(Grade grade)
    {
        _context.Grade.Update(grade);
        return await _context.SaveChangesAsync() > 0;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id)
    {
        Grade? grade = await _context.Grade.FindAsync(id);
        if (grade == null) return false;
        _context.Grade.Remove(grade);
        return await _context.SaveChangesAsync() > 0;
    }
}