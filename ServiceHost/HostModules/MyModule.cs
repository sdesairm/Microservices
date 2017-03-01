using CommonComponents.Interfaces.Models;
using Nancy.Extensions;
using Newtonsoft.Json;
using System;

namespace ServiceHost.HostModules
{
    public class MyModule : BaseModule
    {
        IMyModel model;
        public MyModule(IMyModel m) 
        {
            model = m;

            Get["My/intConstraint/{value}"] = parameters => string.Format("Hello {0}: Value {1} is an integer", this.CurrentUser.Name, parameters.value);
            Post["My/SaveInfo"] = (x) =>
            {
                Type t = model.GetType();
                string tx = this.Request.Body.AsString();
                IMyModel myModel = JsonConvert.DeserializeObject(tx, t) as IMyModel;
                return string.Format("Your Number is {0}", myModel.FName);
            };

            Get["GetMyModel"] = _ =>
            {
                return JsonConvert.SerializeObject(model, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects });
            };
        }
    }
}
