using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.DataAccess.Models;

public class Grade
{
    public int Id { get; set; }

    public decimal GradeValue { get; set; }
    public DateTime ExamDate { get; set; }
    public DateTime CreationDate { get; set; }

    public int StudentId { get; set; }
    public int SubjectId { get; set; }
    public int DetailId { get; set; }
}
