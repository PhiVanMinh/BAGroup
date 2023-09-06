using Application.Interfaces;
using Infrastructure.Contexts;
using Infrastructure.Persistence;
using Persistence.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    /// <Modified>
    /// Name       Date       Comments
    /// minhpv    8/11/2023   created
    /// minhpv    09/05/2023   modified
    /// </Modified>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly DapperContext _dapperContext;
        private IUserRepository _userRepository;
        private IAuthRepository _authRepository;
        private IRoleRepository _roleRepository;
        private IUserRoleRepository _userRoleRepository;
        private ISpeedViolationRepository _speedViolationRepository;


        public UnitOfWork(
            ApplicationDBContext dbContext,
            DapperContext dapperContext
            )
        {
            _dbContext = dbContext;
            _dapperContext = dapperContext;
        }


        public IUserRepository UserRepository
        {
            get { return _userRepository = _userRepository ?? new UserRepository(_dbContext, _dapperContext); }
        }

        public IAuthRepository AuthRepository
        {
            get { return _authRepository = _authRepository ?? new AuthRepository(_dbContext); }
        }

        public IRoleRepository RoleRepository
        {
            get { return _roleRepository = _roleRepository ?? new RoleRepository(_dbContext); }
        }

        public IUserRoleRepository UserRoleRepository
        {
            get { return _userRoleRepository = _userRoleRepository ?? new UserRoleRepository(_dbContext); }
        }
        public ISpeedViolationRepository SpeedViolationRepository
        {
            get { return _speedViolationRepository = _speedViolationRepository ?? new SpeedViolationRepository(_dapperContext); }
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
