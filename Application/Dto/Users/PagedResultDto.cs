using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Users
{
    public class PagedResultDto
    {
        public int TotalCount { get; set; }
        public List<GetAllUserDto> Result { get; set; }
    }
}
