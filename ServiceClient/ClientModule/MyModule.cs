using CommonComponents.Interfaces.Models;
using Newtonsoft.Json;
using ServiceClient.Interface;
using System;

namespace ServiceClient.ClientModule
{
    internal class MyModule
    {
        IMyModel m; IHttpClient client;
        internal MyModule(IHttpClient client, IMyModel model)
        {
            m = model;
            this.client = client;
        }

        public IMyModel GetMyModel()
        {
            return m;
        }

        public string PostModel()
        {
            string rtn;
            if (null != m)
            {
                m.FName = "FirstName";
                m.Date = new DateTime(2000, 01, 1);
                m.Id = 456;
                rtn = client.Post(@"http://localhost:5435/My/SaveInfo", JsonConvert.SerializeObject(m));
                return rtn;
            }
            return "";
        }

        public string GetModel(int val)
        {
            return client.Get(@"http://localhost:5435/My/intConstraint/" + val.ToString());
        }
    }
}
