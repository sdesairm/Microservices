using ServiceClient.Interface;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace ServiceClient.Concrete
{
    internal class HttpClient: IHttpClient

    {
        public string Get(string Url)
        {
            Uri uri = new Uri(Url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri) as HttpWebRequest;
            request.Method = "GET";            
            CredentialCache cc = new CredentialCache();
            cc.Add(uri, "NTLM", CredentialCache.DefaultNetworkCredentials);
            request.Credentials = cc;
            request.PreAuthenticate = true;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            return readStream.ReadToEnd();
        }

        public string Post(string Url, string requestContent)
        {
            Uri uri = new Uri(Url);
            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
            request.Method = "POST";
            request.UseDefaultCredentials = true;
            CredentialCache cc = new CredentialCache();
            cc.Add(uri, "NTLM", CredentialCache.DefaultNetworkCredentials);
            request.Credentials = cc;
            request.ContentType = "application/json; charset=utf-8";
            HttpWebResponse response;
            string result = String.Empty;
            using (Stream writer = request.GetRequestStream())
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(requestContent);
                writer.Write(byteArray, 0, byteArray.Length);
                writer.Close();
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        result = reader.ReadToEnd();
                        reader.Close();
                    }
                }
                catch (WebException webEx)
                {
                    throw webEx;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return result;
        }
    }
}
