using System;
using System.Threading.Tasks;

namespace Services.Services
{
    public interface ILogService
    {
        Task Exception(Exception ex);
    }
}