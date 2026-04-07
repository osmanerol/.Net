using System;
using System.Collections.Generic;
using System.Text;

namespace HL.Core.Interface
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
    }
}
