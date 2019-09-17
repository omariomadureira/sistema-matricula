using System;
using System.Net;
using System.Web.Configuration;
using SistemaMatricula.Models;

namespace SistemaMatricula.Helpers
{
    public class API
    {
        public static string COBRANCA_API_URL = WebConfigurationManager.AppSettings["COBRANCA_API_URL"];

        public static bool Call(string type, string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = type;
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Accept = "text/json";
                request.ContentLength = 0;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                        return true;
                }
            }
            catch (Exception e)
            {
                object[] parameters = { type, url };
                string notes = LogHelper.Notes(parameters, e.Message);
                Log.Add(Log.TYPE_ERROR, "SistemaMatricula.Helpers.API.Call", notes);
            }

            return false;
        }
    }
}