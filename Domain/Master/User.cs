﻿
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Domain.Master
{
    /// <summary>Bảng thông tin người dùng</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/11/2023   created
    /// </Modified>
    public class User
    {
        /// <summary>Mã người dùng</summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid UserId { get; set; }

        /// <summary>Tên đăng nhập</summary>
        [StringLength(50)]
        public string? UserName { get; set; }

        /// <summary>Email</summary>
        [StringLength(500)]
        public string? Email { get; set; }

        /// <summary>Tên người dùng</summary>
        [StringLength(200)]
        public string? EmpName { get; set; }

        /// <summary>Ngày sinh</summary>
        [Column(TypeName = "datetime")]
        public DateTime? BirthDay { get; set; }

        /// <summary>Giới tính</summary>
        public byte? Gender { get; set; }

        /// <summary>Số điện thoại</summary>
        [StringLength(10)]
        public string? PhoneNumber { get; set; }

        /// <summary>Mật khẩu đã mã hóa</summary>
        public string? PasswordHash { get; set; }

        public byte[]? PasswordSalt { get; set; }

        /// <summary>Loại người dùng</summary>
        public byte? UserType { get; set; }

        /// <summary>Mã công ty</summary>
        public int? CompanyId { get; set; }

        /// <summary>Người tạo</summary>
        public string? CreatorUserId { get; set; }

        /// <summary>Người sửa cuối cùng</summary>
        public string? LastModifyUserId { get; set; }

        /// <summary>Ngày tạo</summary>
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }


        /// <summary>Ngày sửa cuối cùng</summary>
        [Column(TypeName = "datetime")]
        public DateTime? LastModifyDate { get; set; }

        /// <summary>Cờ xóa</summary>
        public bool IsDeleted { get; set; }

        /// <summary>Người xóa</summary>
        public string? DeletedUserId { get; set; }

        /// <summary>Ngày xóa</summary>
        [Column(TypeName = "datetime")]
        public DateTime? DeletedDate { get; set; }

    }
}
