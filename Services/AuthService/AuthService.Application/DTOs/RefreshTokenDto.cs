using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.DTOs
{
    public class RefreshTokenDto
    {
        public string UserName { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
