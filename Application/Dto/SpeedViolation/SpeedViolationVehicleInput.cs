using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.SpeedViolation
{
    public class SpeedViolationVehicleInput
    {
        public List<long> ListVhcId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        /// <summary>Số trang</summary>
        public int Page { get; set; }

        /// <summary>Kích thước trang</summary>
        public int PageSize { get; set; }
    }
}
