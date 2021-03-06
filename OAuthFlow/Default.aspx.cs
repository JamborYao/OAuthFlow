﻿using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OAuthFlow
{
    public partial class _Default : Page
    {
        private const string telnant_id = "e4162ad0-e9e3-4a16-bf40-0d8a906a06d4";
        private const string client_ID = "377657e9-8bc2-4fd8-8775-6f619ecb0e5d";
        private const string redirect_uri = "https://localhost:44300/";
        private const string resource = "https://karentest.onmicrosoft.com/OAuthFlow";
        private const string client_secret = "+ayIst0OG7T8ka82ro7RGOkqkJ807d24fIqLREHSSoM=";
        string password = "Doro0674";

        
        private string requestCodeUrl = 
            String.Format("https://login.microsoftonline.com/common/oauth2/authorize?response_type=code&client_id={0}&redirect_uri={1}&state={3}",
                  client_ID,
                  HttpUtility.UrlEncode(redirect_uri),
                   HttpUtility.UrlEncode(resource),
                  Guid.NewGuid().ToString()
                );
        private string _code = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["Code"] == null)
            {
                Response.Redirect(requestCodeUrl);
            }
            else
            {
                _code = Request.QueryString["code"].ToString();
                GetTokenFromCode();
            }

            // RequestCode();

        }
        public void RequestCode()
        {

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestCodeUrl);
            request.Method = "GET";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                string responseContent = reader.ReadToEnd();

            }

        }

        public void GetTokenFromCode()
        {
            string tokenUrl = string.Format("https://login.microsoftonline.com/{0}/oauth2/token", telnant_id);

            string postString = string.Format("grant_type=authorization_code&client_id={0}&code={1}&redirect_uri={2}$resource={3}&client_secret=Password01!",
                client_ID,
                _code,
                redirect_uri,
                resource);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(tokenUrl);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postString.Length;

            using (StreamWriter streamWrite = new StreamWriter(request.GetRequestStream()))
            {
                streamWrite.Write(postString);
            }

            using (var response = request.GetResponse())
            {
                StreamReader streamReader = new StreamReader(response.GetResponseStream());
                string responseData = streamReader.ReadToEnd();
            }
        }

        private static string GetAuthorizationHeader()
        {
            AuthenticationResult result = null;

            var context = new AuthenticationContext(string.Format(
              "https://login.windows.net/{0}",
              telnant_id));

            var thread = new Thread(() =>
            {
               
                result = context.AcquireToken(
                  "https://management.core.windows.net/",
                  client_ID,
                  new Uri(redirect_uri));
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Name = "AquireTokenThread";
            thread.Start();
            thread.Join();

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            string token = result.AccessToken;
            return token;
        }

    }
}