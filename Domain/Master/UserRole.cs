using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Master
{
    public class UserRole : BaseEntity<int>
    {
        // Mã người dùng
        public int UserId { get; set; }

        // Mã quyền
        public int RoleId { get; set; }
    }
}
