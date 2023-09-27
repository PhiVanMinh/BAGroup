using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;
using System.Text;
using ReportDataGrpcService.DBContext;
using Dapper;
using ReportDataGrpcService.Interfaces.Common;

namespace ReportDataGrpcService.Common
{
    /// <summary>Khai báo các hàm chung cho repository</summary>
    /// <typeparam name="T">Tên bảng</typeparam>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    9/27/2023   created
    /// </Modified>
    public class Repository<T> : IRepository<T> where T : class
    {
        IDbConnection _connection;

        private readonly DapperDbContext _dapperContext;
        private readonly ILogger<Repository<T>> _logger;

        public Repository(
                DapperDbContext dapperContext,
                ILogger<Repository<T>> logger
            )
        {
            _dapperContext = dapperContext;
            _logger = logger;
            _connection = _dapperContext.CreateConnection("ServerLab3");
        }


        /// <summary>Thêm thông tin bảng</summary>
        /// <param name="entity">Kiểu dữ liệu</param>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/18/2023   created
        /// </Modified>
        public bool Add(T entity)
        {
            int rowsEffected = 0;
            try
            {
                string tableName = GetTableName();
                string columns = GetColumns(excludeKey: true);
                string properties = GetPropertyNames(excludeKey: true);
                string query = $"INSERT INTO [{tableName}] ({columns}) VALUES ({properties})";

                rowsEffected = _connection.Execute(query, entity);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Insert_{GetTableName()}_{ex.Message}");
            }

            return rowsEffected > 0 ? true : false;
        }

        /// <summary>Xóa thông tin</summary>
        /// <param name="entity">Kiểu dữ liệu</param>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/18/2023   created
        /// </Modified>
        public bool Delete(T entity)
        {
            int rowsEffected = 0;
            try
            {
                string tableName = GetTableName();
                string keyColumn = GetKeyColumnName();
                string keyProperty = GetKeyPropertyName();
                string query = $"DELETE FROM [{tableName}] WHERE {keyColumn} = @{keyProperty}";

                rowsEffected = _connection.Execute(query, entity);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Deleted_{GetTableName()}_{ex.Message}");
            }

            return rowsEffected > 0 ? true : false;
        }

        /// <summary>Lấy tất cả thông tin</summary>
        /// <returns>Tất cả thông tin theo kiểu model</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/18/2023   created
        /// </Modified>
        public IEnumerable<T> GetAll()
        {
            IEnumerable<T> result = null;
            try
            {
                string tableName = GetTableName();
                string query = $"SELECT * FROM [{tableName}]";

                result = _connection.Query<T>(query);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GetAll_{GetTableName()}_{ex.Message}");
            }

            return result;
        }

        /// <summary>Lấy tất cả thông tin theo ngày</summary>
        /// <param name="columnName">Tên cột so sánh</param>
        /// <param name="fromDate">Từ ngày</param>
        /// <param name="toDate">Đến ngày</param>
        /// <param name="hasIsDeleted">Điều kiện xóa</param>
        /// <returns>Thông tin các bản ghi</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/18/2023   created
        /// </Modified>
        public IEnumerable<T> GetAllByDate(string columnName, DateTime fromDate, DateTime toDate, bool hasIsDeleted)
        {
            IEnumerable<T> result = null;
            try
            {
                string tableName = GetTableName();
                string checkDeleted = hasIsDeleted ? "AND IsDeleted = 0" : "";
                string query = $"SELECT * FROM [{tableName}] WHERE {columnName} BETWEEN '{fromDate}' AND '{toDate}' {checkDeleted}";

                result = _connection.Query<T>(query);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GetAllByDate_{GetTableName()}_{ex.Message}");
            }

            return result;
        }

        /// <summary>Lấy tất cả thông tin theo điều kiện lọc</summary>
        /// <param name="columnName">Tên cột</param>
        /// <param name="value">Giá trị lọc</param>
        /// <param name="hasIsDeleted">Điều kiện xóa</param>
        /// <returns>Các thông tin cần tìm</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/18/2023   created
        /// </Modified>
        public IEnumerable<T> GetAllByColumnId(string columnName, int value, bool hasIsDeleted)
        {
            IEnumerable<T> result = null;
            try
            {
                string tableName = GetTableName();
                string checkDeleted = hasIsDeleted ? "AND IsDeleted = 0" : "";
                string query = $"SELECT * FROM [{tableName}] WHERE {columnName} = {value}  {checkDeleted}";

                result = _connection.Query<T>(query);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GetAllByColumnId_{GetTableName()}_{columnName}_{ex.Message}");
            }

            return result;
        }

        /// <summary>Lấy thông tin bản ghi theo mã</summary>
        /// <param name="Id">Mã bản ghi</param>
        /// <returns>Thông tin bản ghi cần tìm</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/18/2023   created
        /// </Modified>
        public T GetById(int Id)
        {
            IEnumerable<T> result = null;
            try
            {
                string tableName = GetTableName();
                string keyColumn = GetKeyColumnName();
                string query = $"SELECT * FROM [{tableName}] WHERE {keyColumn} = '{Id}'";

                result = _connection.Query<T>(query);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"GetById_{GetTableName()}_{ex.Message}");
            }

            return result.FirstOrDefault();
        }

        /// <summary>Cập nhật bản ghi</summary>
        /// <param name="entity">Entity</param>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/18/2023   created
        /// </Modified>
        public bool Update(T entity)
        {
            int rowsEffected = 0;
            try
            {
                string tableName = GetTableName();
                string keyColumn = GetKeyColumnName();
                string keyProperty = GetKeyPropertyName();

                StringBuilder query = new StringBuilder();
                query.Append($"UPDATE [{tableName}] SET ");

                foreach (var property in GetProperties(true))
                {
                    var columnAttr = property.GetCustomAttribute<ColumnAttribute>();

                    string propertyName = property.Name;
                    string columnName = columnAttr.Name;

                    query.Append($"{columnName} = @{propertyName},");
                }

                query.Remove(query.Length - 1, 1);

                query.Append($" WHERE {keyColumn} = @{keyProperty}");

                rowsEffected = _connection.Execute(query.ToString(), entity);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Update_{GetTableName()}_{ex.Message}");
            }

            return rowsEffected > 0 ? true : false;
        }

        /// <summary>Lấy thông tin tên bảng</summary>
        /// <returns>Tên bảng</returns>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/18/2023   created
        /// </Modified>
        private string GetTableName()
        {
            string tableName = "";
            var type = typeof(T);
            var tableAttr = type.GetCustomAttribute<TableAttribute>();
            if (tableAttr != null)
            {
                tableName = tableAttr.Name;
                return tableName;
            }

            return type.Name.Replace("_", ".");
        }

        /// <summary>Lấy tên khóa trong bảng</summary>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/18/2023   created
        /// </Modified>
        public static string GetKeyColumnName()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                object[] keyAttributes = property.GetCustomAttributes(typeof(KeyAttribute), true);

                if (keyAttributes != null && keyAttributes.Length > 0)
                {
                    object[] columnAttributes = property.GetCustomAttributes(typeof(ColumnAttribute), true);

                    if (columnAttributes != null && columnAttributes.Length > 0)
                    {
                        ColumnAttribute columnAttribute = (ColumnAttribute)columnAttributes[0];
                        return columnAttribute.Name;
                    }
                    else
                    {
                        return property.Name;
                    }
                }
            }

            return "";
        }


        /// <summary>Lấy tên cột</summary>
        /// <param name="excludeKey"></param>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/18/2023   created
        /// </Modified>
        private string GetColumns(bool excludeKey = false)
        {
            var type = typeof(T);
            var columns = string.Join(", ", type.GetProperties()
                .Where(p => !excludeKey || !p.IsDefined(typeof(KeyAttribute)))
                .Select(p =>
                {
                    var columnAttr = p.GetCustomAttribute<ColumnAttribute>();
                    return columnAttr != null ? columnAttr.Name : p.Name;
                }));

            return columns;
        }

        /// <summary>Lấy tên thuộc tính</summary>
        /// <param name="excludeKey">if set to <c>true</c> [exclude key].</param>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/18/2023   created
        /// </Modified>
        protected string GetPropertyNames(bool excludeKey = false)
        {
            var properties = typeof(T).GetProperties()
                .Where(p => !excludeKey || p.GetCustomAttribute<KeyAttribute>() == null);

            var values = string.Join(", ", properties.Select(p =>
            {
                return $"@{p.Name}";
            }));

            return values;
        }

        /// <summary>Lấy tất cả thuộc tính</summary>
        /// <param name="excludeKey">if set to <c>true</c> [exclude key].</param>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/18/2023   created
        /// </Modified>
        protected IEnumerable<PropertyInfo> GetProperties(bool excludeKey = false)
        {
            var properties = typeof(T).GetProperties()
                .Where(p => !excludeKey || p.GetCustomAttribute<KeyAttribute>() == null);

            return properties;
        }

        /// <summary>Lấy tên các thuộc tính</summary>
        /// <Modified>
        /// Name       Date       Comments
        /// minhpv    9/18/2023   created
        /// </Modified>
        protected string GetKeyPropertyName()
        {
            var properties = typeof(T).GetProperties()
                .Where(p => p.GetCustomAttribute<KeyAttribute>() != null);

            if (properties.Any())
            {
                return properties.FirstOrDefault().Name;
            }

            return "";
        }
    }
}
