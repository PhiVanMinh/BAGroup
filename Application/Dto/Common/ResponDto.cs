using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.Common
{
    // Dữ liệu phản hồi
    public class ResponDto<T>
    {
        // Trạng thái code
        public int? StatusCode { get; set; }
        // Thông báo
        public string? Message { get; set; }
        public virtual T Result { get; set; }

    }
}
