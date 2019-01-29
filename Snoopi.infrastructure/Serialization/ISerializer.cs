using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snoopi.infrastructure.Serialization
{
    public interface ISerializer
    {
        string SerializeToString<T>(T obj);
    }
}
