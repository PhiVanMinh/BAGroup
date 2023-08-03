using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Master
{
    public class Role : BaseEntity<int>
    {
        // Tên quyền
        public string? RoleName { get; set; }

        // Trạng thái quyền
        public byte? Status { get; set; }
    }
}
