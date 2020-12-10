using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using ComplaintManagement.Repository;
using ComplaintManagement.ViewModel;
using System.Web.Mvc;
using Elmah;
using ComplaintManagement.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using ComplaintManagement.Models;

namespace ComplaintManagement.Controllers
{
    public class AccountController : Controller
    {
        private DB_A6A061_complaintuserEntities db = new DB_A6A061_complaintuserEntities();
        // GET: Account     
        [HttpGet]
        public ActionResult Index(string returnUrl)
        {
            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                int userId = Convert.ToInt32(identity.Claims.Where(c => c.Type == ClaimTypes.Sid)
                       .Select(c => c.Value).SingleOrDefault());
                var user = (dynamic)null;
                if (userId > 0)
                {
                    user = new UserMastersRepository().GetAll();
                }
                if (Request.IsAuthenticated && userId > 0 && user != null)
                {
                    return Redirect(returnUrl ?? "/Category/Index");
                }
                ViewBag.returnUrl = returnUrl;
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View();

        }
        [HttpPost]
        public ActionResult Login(LoginVM LoginVM)
        {
            try
            {
                var user = new UserMastersRepository().Login(LoginVM);
                Session["EmployeeId"] = user.EmployeeId;
                Session["id"] = user.Id;
                var CommitteeMemberData= (from u in db.CommitteeMasters
                                          where u.IsActive
                                          select u).FirstOrDefault();
                var isCommitteeUserAssigned = CommitteeMemberData.UserId.Split(',').Where(i => i.ToString() == user.Id.ToString()).Count() > 0;
                if (isCommitteeUserAssigned)
                {
                    user.Type = Messages.Committee;
                }

                SignInUser(user.WorkEmail, user.Id.ToString(), user.EmployeeName, user.ImagePath, user.Type, false);
                return new ReplyFormat().Success(Messages.SUCCESS, null);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return new ReplyFormat().Error(ex.Message.ToString());
            }
        }
        private void SignInUser(string useremail, string userid, string username, string image,string userrole, bool isPersistent)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Sid, userid));
            claims.Add(new Claim(ClaimTypes.Email, useremail));
            claims.Add(new Claim(ClaimTypes.Role, userrole));
            claims.Add(new Claim(ClaimTypes.Name, username));
            if (!string.IsNullOrEmpty(image) && new Common().GetFilePathExist(image))
            {
                claims.Add(new Claim(ClaimTypes.UserData, "/Images/profile_pics/"+image));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.UserData, "/Images/profile.png"));
            }
            var claimIdenties = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            // Sign In.    
            authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, claimIdenties);
        }

        public ActionResult LogOff()
        {
            var ctx = Request.GetOwinContext();
            var auth = ctx.Authentication;
            auth.SignOut();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index");
        }
    }
}