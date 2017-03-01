using Ninject.Modules;

namespace CommonComponents
{
    public class CommonNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<CommonComponents.Interfaces.Models.IMyModel>().To<CommonComponents.Concrete.Models.MyModel>().InThreadScope();
            Bind<Interfaces.Utilities.IDataAccess>().To<CommonComponents.Concrete.Utilities.DataAccess>().InThreadScope();
        }
    }
}
