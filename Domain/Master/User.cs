
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Domain.Master
{
    public class User : BaseEntity<int>
    {
        [StringLength(50)]
        public string? UserName { get; set; }
        [StringLength(500)]
        public string? Email { get; set; }
        [StringLength(200)]
        public string? EmpName { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? BirthDay { get; set; }
        public byte? Gender { get; set; }
        [StringLength(10)]
        public string? PhoneNumber { get; set; }
        public string? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public byte? Role { get; set; }

        public int? CreatorUserId { get; set; }
        public int? LastModifyUserId { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? CreateDate { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? LastModifyDate { get; set; }
        public bool IsDeleted { get; set; }
        public int DeletedUserId { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? DeletedDate { get; set; }

    }
}
