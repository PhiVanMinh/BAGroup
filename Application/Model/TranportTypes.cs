using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Model
{
    /// <summary>Loại hình vận tải</summary>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/8/2023   created
    /// </Modified>
    public class TranportTypes
    {
        /// <summary>Mã loại phương tiện</summary>
        public long PK_TransportTypeID { get; set; }

        /// <summary>Mã đơn vị vận tải</summary>
        [StringLength(255)]
        public string DisplayName { get; set; }

        /// <summary>Trạng thái hoạt động</summary>
        public bool IsActivated { get; set; }
    }
}
