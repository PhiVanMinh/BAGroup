using Domain.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Login
{
    public class ResponLoginDto
    {
        public User? User { get; set; }
        public List<string> Roles { get; set; }
    }
}
