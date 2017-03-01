namespace ServiceClient.Interface
{
    public interface IHttpClient
    {
        string Get(string Url);
        string Post(string Url, string requestContent);
    }
}
