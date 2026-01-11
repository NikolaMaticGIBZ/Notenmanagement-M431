using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs;

public class Verify2FARequest
{
    public int UserId { get; set; }
    public string Code { get; set; } = string.Empty;
}
