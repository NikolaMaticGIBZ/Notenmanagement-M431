using Api.DataAccess.Interfaces;
using Api.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;


namespace Api.DataAccess.Repositories;

public class GradeRepository : IGradesRepository
{
    private readonly AppDBContext _context;

    public GradeRepository(AppDBContext context)
    {
        _context = context;
    }

    public async Task<Grades> CreateAsync(Grades grade)
    {
        _context.Grades.Add(grade);
        await _context.SaveChangesAsync();
        return grade;
    }

    public async Task<IEnumerable<Grades>> GetAllAsync()
    {
        return await _context.Grades
            .Include(g => g.teacher)
            .Include(g => g.rektor)
            .ToListAsync();
    }

    public async Task<Grades?> GetByIdAsync(int id)
    {
        return await _context.Grades
            .Include(g => g.teacher)
            .Include(g => g.rektor)
            .FirstOrDefaultAsync(g => g.id == id);
    }

    public async Task<bool> UpdateAsync(Grades grade)
    {
        _context.Grades.Update(grade);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var grade = await _context.Grades.FindAsync(id);
        if (grade == null) return false;
        _context.Grades.Remove(grade);
        return await _context.SaveChangesAsync() > 0;
    }
    public async Task<IEnumerable<Grades>> GetByRektorIdAsync(int rektorId)
    {
        return await _context.Grades
            .Include(g => g.teacher)
            .Where(g => g.rektor_id == rektorId)
            .ToListAsync();
    }
}
