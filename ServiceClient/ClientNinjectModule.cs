using CommonComponents;
using ServiceClient.Interface;

namespace ServiceClient
{
    public class ClientNinjectModule : CommonNinjectModule
    {
        public override void Load()
        {
            base.Load();            
            Bind<IHttpClient>().To<Concrete.HttpClient>().InThreadScope();
        }
    }
}
