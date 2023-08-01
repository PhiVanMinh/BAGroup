
namespace Application.Dto.Users
{
     public class GetAllUserDto
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? EmpName { get; set; }
        public DateTime? BirthDay { get; set; }
        public byte? Gender { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
