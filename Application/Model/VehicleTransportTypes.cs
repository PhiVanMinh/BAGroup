using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Model
{
    /// Name       Date       Comments
    /// minhpv    9/8/2023   created
    /// </Modified>
    public class VehicleTransportTypes
    {
        /// <summary>Mã phương tiện</summary>
        public long FK_VehicleID { get; set; }

        /// <summary>Mã loại phương tiện</summary>
        public long FK_TransportTypeID { get; set; }

        /// <summary>Mã đơn vị vận tải</summary>
        public int FK_CompanyID { get; set; }

        /// <summary>Xoá bản ghi</summary>
        public bool? IsDeleted { get; set; }
    }
}
