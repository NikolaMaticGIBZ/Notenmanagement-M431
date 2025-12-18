using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.DataAccess.Models;

public class Student_Subject
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int SubjectId { get; set; }
}
