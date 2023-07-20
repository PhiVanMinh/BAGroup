using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dto.Users
{
    public class DeletedUserInput
    {
        public List<int> ListId { get; set; }
        public int CurrentUserId { get; set; }
    }
}
