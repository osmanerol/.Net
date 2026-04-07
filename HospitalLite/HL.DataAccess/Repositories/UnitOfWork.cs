using HL.Core.Interface;
using HL.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace HL.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context; 

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
