using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs;

public class LoginResponse
{
    public int UserId { get; set; }
    public bool RequiresTwoFactor { get; set; }
    public string Role { get; set; } = "teacher";
}
