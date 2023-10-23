using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Common
{
    /// <summary>Repository chung</summary>
    /// <typeparam name="T">Các entity</typeparam>
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/11/2023   created
    /// </Modified>
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDBContext _dbContext;
        private readonly DbSet<T> _entitiySet;


        public GenericRepository(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
            _entitiySet = _dbContext.Set<T>();
        }


        /// <summary>Thêm dữ liệu</summary>
        /// <param name="entity">Thông tin bảng</param>
        public void Add(T entity)
      => _dbContext.Add(entity);


        /// <summary>Thêm dữ liệu bất đồng bộ</summary>
        /// <param name="entity">Thông tin bảng</param>
        /// <param name="cancellationToken">Mã thông báo hủy.</param>
        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
            => await _dbContext.AddAsync(entity, cancellationToken);


        /// <summary>Thêm nhiều dữ liệu 1 lúc</summary>
        /// <param name="entities">Danh sách dữ liệu</param>
        public void AddRange(IEnumerable<T> entities)
            => _dbContext.AddRange(entities);


        public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
            => await _dbContext.AddRangeAsync(entities, cancellationToken);

        /// <summary>Kiểm tra dữ liệu bất đồng bộ</summary>
        /// <param name="expression">The expression.</param>
        public Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
             => _entitiySet.AnyAsync(expression);

        /// <summary>Lấy bản ghi hợp lệ đầu tiền</summary>
        /// <param name="expression">The expression.</param>
        public T FirstOrDefault(Expression<Func<T, bool>> expression)
            => _entitiySet.FirstOrDefault(expression);


        /// <summary>Lấy tất cả</summary>
        public IQueryable<T> GetAll()
            => _entitiySet.AsNoTracking().AsQueryable();

        /// <summary>Lấy tất cả dữ liệu thỏa mãn điền kiện</summary>
        /// <param name="expression">The expression.</param>
        public IEnumerable<T> GetAll(Expression<Func<T, bool>> expression)
            => _entitiySet.Where(expression).AsEnumerable();

        /// <summary>Chạy bât đồng bộ lấy DL</summary>
        /// <param name="cancellationToken">Mã hủy</param>
        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _entitiySet.ToListAsync(cancellationToken);

        /// <summary>Chạy bât đồng bộ lấy DL theo diều kiện</summary>
        /// <param name="expression">The expression.</param>
        /// <param name="cancellationToken">Mã hủy</param>
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
            => await _entitiySet.Where(expression).ToListAsync(cancellationToken);

        /// <summary>Chạy bât đồng bộ lấy bản ghi đầu tiên</summary>
        /// <param name="expression">The expression.</param>
        /// <param name="cancellationToken">Mã hủy</param>
        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
            => await _entitiySet.FirstOrDefaultAsync(expression, cancellationToken);


        /// <summary>Đánh cờ xóa 1 bản ghi</summary>
        /// <param name="entity">Bảng</param>
        public void Remove(T entity)
            => _dbContext.Remove(entity);


        /// <summary>Đánh cờ nhiều bản ghi cùng lúc</summary>
        /// <param name="entities">Danh sách bản ghi theo bảng</param>
        public void RemoveRange(IEnumerable<T> entities)
            => _dbContext.RemoveRange(entities);


        /// <summary>Cập nhật 1 bản ghi</summary>
        /// <param name="entity">Bảng</param>
        public void Update(T entity)
            => _dbContext.Update(entity);


        /// <summary>Cập nhật 1 bản ghi</summary>
        /// <param name="entities">Danh sách bản ghi theo bảng</param>
        public void UpdateRange(IEnumerable<T> entities)
            => _dbContext.UpdateRange(entities);
    }
}
