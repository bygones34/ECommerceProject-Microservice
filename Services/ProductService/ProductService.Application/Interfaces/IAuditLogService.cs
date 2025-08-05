using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Application.Interfaces
{
    public interface IAuditLogService
    {
        Task AuditLog(string action, string entityName, string entityId, string performedBy);
    }
}
