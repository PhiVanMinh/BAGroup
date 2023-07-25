using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Master
{
    public class User : BaseEntity<int>
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? EmpName { get; set; }
        public DateTime? BirthDay { get; set; }
        public byte? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public byte? Role { get; set; }

        public int? CreatorUserId { get; set; }
        public int? LastModifyUserId { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? LastModifyDate { get; set; }
        public bool IsDeleted { get; set; }
        public int DeletedUserId { get; set; }

    }
}
