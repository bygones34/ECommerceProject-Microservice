using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Entities
{
    public class BlacklistedToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = null!;
        public DateTime Expiration { get; set; }
    }
}
