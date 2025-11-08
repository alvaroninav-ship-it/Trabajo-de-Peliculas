using System.Data;
using Movies.Core.Enum;

namespace Movies.Core.Interfaces
{
    public interface IDBConnectionFactory
    {
        DatabaseProvider Provider { get; }
        IDbConnection CreateConnection();
    }
}
