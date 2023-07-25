using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Users
{
    public class GetAllUserInput
    {
        public string? ValueFilter { get; set; }
        public byte? TypeFilter { get; set; }
        public byte? Gender { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set;}
        public int PageSize { get; set; }
    }
}
