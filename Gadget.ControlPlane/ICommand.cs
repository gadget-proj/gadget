using System.Threading.Tasks;

namespace Gadget.ControlPlane
{
    public interface ICommand
    {
        Task Execute();
    }
}