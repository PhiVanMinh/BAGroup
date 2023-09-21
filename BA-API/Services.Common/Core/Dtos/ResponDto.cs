using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Common.Core.Dtos
{
    /// <summary>
    ///   <para>Dữ liệu phản hồi</para>
    /// </summary>
    /// <typeparam name="T">
    ///   <para>Kiểu dữ liệu của kết quả cần trả về</para>
    /// </typeparam>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/10/2023   created
    /// </Modified>
    public class ResponDto<T>
    {
        /// <summary>Trạng thái phản hồi</summary>
        public int? StatusCode { get; set; }

        /// <summary>Thông báo phản hồi</summary>
        public string? Message { get; set; }

        /// <summary>Kết quả trả về</summary>
        public virtual T Result { get; set; }

    }
}
