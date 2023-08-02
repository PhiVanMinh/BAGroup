
namespace Application.Dto.Users
{
    public class DeletedUserInput
    {
        public List<int> ListId { get; set; }
        public int CurrentUserId { get; set; }
        public string? CurrentUserName { get; set; }
    }
}
