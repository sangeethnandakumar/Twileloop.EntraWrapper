using Twileloop.EntraWrapper.ConfigModels;

namespace Twileloop.EntraWrapper.Abstractions
{
    public interface IEntraConfigurationResolver
    {
        EntraConfig Resolve();
    }


}
