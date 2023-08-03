
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Domain.Master
{
    public class User : BaseEntity<int>
    {
        // Tên đăng nhập
        [StringLength(50)]
        public string? UserName { get; set; }

        // Email
        [StringLength(500)]
        public string? Email { get; set; }

        // Tên người dùng
        [StringLength(200)]
        public string? EmpName { get; set; }

        // Ngày sinh
        [Column(TypeName = "datetime2")]
        public DateTime? BirthDay { get; set; }

        // Giới tính
        public byte? Gender { get; set; }

        // Số điện thoại
        [StringLength(10)]
        public string? PhoneNumber { get; set; }

        // Password mã hóa
        public string? PasswordHash { get; set; }

        public byte[]? PasswordSalt { get; set; }

        // Quyền
        public byte? Role { get; set; }

        // Người tạo
        public int? CreatorUserId { get; set; }

        // Người sửa cuối cùng
        public int? LastModifyUserId { get; set; }
        [Column(TypeName = "datetime2")]

        // Ngày tạo
        public DateTime? CreateDate { get; set; }
        [Column(TypeName = "datetime2")]

        // Ngày sửa cuối cùng
        public DateTime? LastModifyDate { get; set; }

        // Xóa
        public bool IsDeleted { get; set; }

        // Người xóa
        public int DeletedUserId { get; set; }

        // Ngày xóa
        [Column(TypeName = "datetime2")]
        public DateTime? DeletedDate { get; set; }

    }
}
