using CommonComponents.Interfaces.Models;
using Ninject;
using ServiceClient.ClientModule;
using ServiceClient.Interface;

namespace ServiceClient.Providers
{
    public class MyModuleProvider
    {
        private static IKernel kernel = new StandardKernel(new ClientNinjectModule());
        internal static MyModule GetMyModule()
        {
            return new MyModule(kernel.Get<IHttpClient>(), kernel.Get<IMyModel>());
        }
    }
}
