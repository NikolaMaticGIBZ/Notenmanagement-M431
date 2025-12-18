using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs;

public class UpdateGradeResponse
{
    public string CourseName { get; set; } = string.Empty;
    public decimal GradeValue { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;
    public int RektorId { get; set; }
}
