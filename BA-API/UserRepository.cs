using Application.Interfaces;
using Domain.Master;
using Infrastructure.Persistence;
using Library.Infrastructure;
using System;

public class UserRepository : GenericRepository<User>, IUserRepository
{
	public UserRepository(ApplicationDBContext _dbContext) : base(_dbContext)
	{
	}
}
