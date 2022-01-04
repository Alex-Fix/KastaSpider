using System.Threading.Tasks;

namespace Services.Workers
{
    public interface IWorker<TWorker, TData> where TWorker : class
    {
        Task Execute(TData data = default);
    }
}