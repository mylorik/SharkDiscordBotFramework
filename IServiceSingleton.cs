using System.Threading.Tasks;

namespace SharkFramework
{
    public interface IServiceSingleton
    {
        Task InitializeAsync();
    }
}
