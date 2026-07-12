using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Application.Responses
{
    public class UserResponse
    {
        public Guid Id { get; set; }

        public string UserName { get; set; } = null!;

        public string Role { get; set; } = null!;

        public DateTimeOffset CreatedAtUtc { get; set; }
    }
}
