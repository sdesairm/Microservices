using Nancy;
using Nancy.Responses;
using System.Collections.Generic;
using System.Security.Principal;

namespace ServiceHost
{
    public class BaseModule : NancyModule
    {
        protected IIdentity CurrentUser;
        public BaseModule()
        {
            HtmlResponse unAuthorizedResponse = new HtmlResponse(HttpStatusCode.Unauthorized);
            unAuthorizedResponse.Headers.Add("WWW-Authenticate", "NTLM");
            Before += x => { return GetUser(this.Context) == null ? unAuthorizedResponse : null; };
        }
        private IIdentity GetUser(NancyContext Context)
        {
            var env = ((IDictionary<string, object>)Context.Items["OWIN_REQUEST_ENVIRONMENT"]);
            var user = (IPrincipal)env["server.User"];
            CurrentUser = user.Identity;
            return user.Identity;
        }
    }
}
