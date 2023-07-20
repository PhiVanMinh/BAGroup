using Application.Interfaces;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDBContext _dbContext;
        private IUserRepository _userRepository;
        private IAuthRepository _authRepository;


        public UnitOfWork(
            ApplicationDBContext dbContext
            )
        {
            _dbContext = dbContext;
        }


        public IUserRepository UserRepository
        {
            get { return _userRepository = _userRepository ?? new UserRepository(_dbContext); }
        }

        public IAuthRepository AuthRepository
        {
            get { return _authRepository = _authRepository ?? new AuthRepository(_dbContext); }
        }



        public void Save()
            => _dbContext.SaveChanges();


        public async Task SaveAsync()
            => await _dbContext.SaveChangesAsync();

        public void RollBack()
        {
            _dbContext.Dispose();
        }

        public async Task RollBackAsync()
        {
            await _dbContext.DisposeAsync();
        }
    }
}
