using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.Repositories
{
    public interface IRepository<Type>
    {
        Type? Load(int id);
        void Save(int id, Type data);
    }
}
