using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.SpeedViolation
{
    public class GetVehicleListDto
    {
        public int VehicleID { get; set; }
        public string? PrivateCode { get; set; }
    }
}
