using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace HOTP
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            if (exception == null)
                return;
            var mail = new MailMessage { From = new MailAddress("postmaster@hospiceotpgoals.org") };
            mail.To.Add(new MailAddress("daniel.cogswell@gmail.com"));
            mail.Subject = "Site Error at " + DateTime.Now.AddHours(3);
            mail.Body = "Error Description: " + exception.Message;
            mail.Body += "<br>User: " + User.Identity.Name;
            var server = new SmtpClient { Host = "localhost" };
            server.Send(mail);

            // Clear the error
            Server.ClearError();

            // Redirect to a landing page
            Response.Redirect("~/Home/Error");            
        }

    }
}
