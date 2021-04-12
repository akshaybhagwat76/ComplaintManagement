using ComplaintManagement.Helpers;
using Elmah;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ComplaintManagement.Repository;
using ComplaintManagement.ViewModel;

using Ionic.Zip;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace ComplaintManagement
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                bool isOpenAM = Convert.ToBoolean(ConfigurationManager.AppSettings["IsOpenAM"]);

                if (isOpenAM)
                {
                    if ((Request.Params["state"] == null) && (Request.Params["code"] == null))
                    {
                        Authorize();
                    }

                    if ((Request.Params["state"] != null) && (Request.Params["code"] != null))
                    {
                        string[] sToken = GenerateToken(Request.Params["code"]);

                        GetUserProfile(sToken);
                    }
                }
                //else
                //{
                //    LogMe log = new LogMe();
                //    log.LoadUserDetails(this, Session);

                //    if (Session["UserDetails"] != null)
                //    {
                //        Response.Redirect("~/Pages/Survey.aspx");
                //    }
                //    else
                //    {
                //        Response.Redirect("~/Pages/AccessDenied.aspx");
                //    }
                //}

            }
            catch(Exception ex)
            {
                Response.Write("error" + ex.Message);
            }
        }

        private void Authorize()
        {
            try
            {
                string clientID = ConfigurationManager.AppSettings["CLIENT_ID"];
                string appURL = ConfigurationManager.AppSettings["REDIRECT_URL"];
                string authServerURL = ConfigurationManager.AppSettings["authServerURL"];
                string state = Guid.NewGuid().ToString();

                HttpCookie stateCookie = new HttpCookie("OAuth2State", state);
                Response.Cookies.Add(stateCookie);

                string scope = "openid";
                string redirect = authServerURL + "?" +
                      "response_type=code" + "&" +
                      "client_id=" + System.Uri.EscapeDataString(clientID) + "&" +
                      "redirect_uri=" + System.Uri.EscapeDataString(appURL) + "&" +
                      "scope=" + System.Uri.EscapeDataString(scope) + "&" +
                      "state=" + state;

                Response.Redirect(redirect, true);
            }
            catch (Exception ex)
            {
                Response.Write("error" + ex.Message);
            }
        }

        public string[] GenerateToken(string code)
        {
            string[] token = new string[2];
            string accessToken = string.Empty;
            try
            {
                //ApplicationLogger.LogMessage(this, "GenerateToken - Start");

                string clientID = ConfigurationManager.AppSettings["CLIENT_ID"];
                string clientSecret = ConfigurationManager.AppSettings["CLIENT_SECRET"];
                string appURL = ConfigurationManager.AppSettings["REDIRECT_URL"];
                string accessURL = ConfigurationManager.AppSettings["accesstoken"];

                HttpClient client = new HttpClient();
                FormUrlEncodedContent form = null;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.MaxServicePointIdleTime = 2000;

                form = new FormUrlEncodedContent(new Dictionary<string, string> {
                                    { "code", code },
                                    { "client_id", clientID},
                                    { "client_secret", clientSecret},
                                    { "redirect_uri", appURL},
                                    { "grant_type", "authorization_code"}});

                System.Threading.Tasks.Task<HttpResponseMessage> message = client.PostAsync(accessURL, form);

                String result = message.Result.Content.ReadAsStringAsync().Result;

                dynamic dynObj = JsonConvert.DeserializeObject(result);

                accessToken = dynObj.access_token;
                string refreshtoken = dynObj.refresh_token;
                string idtoken = dynObj.id_token;
                System.Web.HttpContext.Current.Session["refresh_token"] = refreshtoken;

                token[0] = accessToken;
                token[1] = idtoken;
            }
            catch (Exception ex)
            {


                Response.Write("error" + ex.Message);
                //LogExceptionManager.LogException(ex, "GenerateToken", "");
            }
            return token;
        }

        private void GetUserProfile(string[] token)
        {
            try
            {
                string userProfileURL = ConfigurationManager.AppSettings["userinfo"];
                string url = userProfileURL + @"?access_token=" + HttpUtility.UrlEncode(token[0]);

                // get the user profile
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token[0]);
                System.Threading.Tasks.Task<HttpResponseMessage> message = client.GetAsync(url);

                String result = message.Result.Content.ReadAsStringAsync().Result;
                Response.Write("result=" + result);
                //dynamic dynObj = JsonConvert.DeserializeObject(result);

                // Response.Write("UPN=" + dynObj.upn)
                // Response.Write("sub=" + dynObj.sub)
                // Response.Write("uid=" + dynObj.uid)
                //    dynObj.upn;

                //if (dynObj != null && dynObj.sub != null && dynObj.sub.Value != null && dynObj.sub.Value != "")
                //{
                //    LogMe log = new LogMe();
                //    log.LoadUserDetails(Session, dynObj.sub.Value);

                //    if (Session["UserDetails"] != null)
                //    {
                //        Response.Redirect("~/Pages/Survey.aspx");
                //    }
                //    else
                //    {
                //        Response.Redirect("~/Pages/AccessDenied.aspx");
                //    }
                //}
                //else
                //{
                //    Response.Redirect("~/Pages/AccessDenied.aspx");
                //}
            }
            
            catch (Exception ex)
            {
                Response.Write("error" + ex.Message);
            }
        }

    

    }
}