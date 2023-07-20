using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto.Users
{
    public class CreateOrEditUserDto
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        //public DateTime? CreateDate { get; set; }
        //public DateTime? LastModifyDate { get; set; }
        public string? EmpName { get; set; }
        public DateTime? BirthDay { get; set; }
        public int? CreatorUserId { get; set; }
        //public int? LastModifyUserId { get; set; }
        public byte? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public int? CurrentUserId { get; set; }
    }
}
