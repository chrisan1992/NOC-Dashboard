using NOC_Macro.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace NOC_Macro
{
    public class EmailSender
    {

        public EmailSender()
        {

        }

        /// <summary>
        /// sends email to the recipients
        /// </summary>
        /// <returns></returns>
        public String SendMacroEmail(MajorIncidents i, string lyncCall="0")
        {
            MailMessage msg = new MailMessage();
            msg.To.Add(new MailAddress("chris.sanchezc@hpe.com", "SomeOne"));
            msg.From = new MailAddress(HttpContext.Current.Session["user_email"].ToString(), HttpContext.Current.Session["username"].ToString());
            msg.Subject = "Incident #"+i.incidentNumber+" "+i.descr;
            if (i.customerType == 1)
            {
                //TOP Customer
                msg.Body = "Hi All, <br/>We are currently experiencing a TOP Customer incident, please see the following details: <br /><br /><table id='main-table' style=width:100%><tr><td style='width:40%;'>Incident #</td><td>" + i.incidentNumber + "</td></tr>";
            }
            else
            {
                //Multi Customer
                msg.Body = "Hi All, <br/>We are currently experiencing a Multi-Customer incident, please see the following details: <br /><br /><table id='main-table' style=width:100%><tr><td style='width:50%;'>Incident #</td><td>" + i.incidentNumber + "</td></tr>";
            }

            msg.Body += "<tr><td style='width:40%;'>Description</td><td>" + i.descr + "</td></tr>";
            msg.Body += "<tr><td style='width:40%;'>Product (s) impacted</td><td>" + i.product + "</td></tr>";
            msg.Body += "<tr><td style='width:40%;'>Data center (s)</td><td>" + i.dataCenter + "</td></tr>";

            if (i.categorization == 1)
            {
                //Availability
                msg.Body += "<tr><td style='width:40%;'>Impact type</td><td>Availability</td></tr>";
            }
            else
            {
                //Major Functionality
                msg.Body += "<tr><td style=width:40%>Impact type</td><td>Major Functionality</td></tr>";
            }

            //closing the table
            msg.Body += "</table>";
            
            //full html email message
            //msg.Body = "<html><head><style>#format-table,#format-table th,#format-table td{border: 0px;font-family: Calibri; font-size:13;padding: 3px;margin:0 auto;}#main-table, #main-table tr, #main-table td{border: 1px solid black;font-family: Calibri; font-size:13;padding: 3px;}</style></head><body><table id='format-table' width='600px'><tr style='background-color:#61476D;color:#fff;'><td style='font-size: 20px; padding: 10px;'>Hewlett Packard Enterprise</td></tr><tr><td>Hi All,<br/><br/>" + msg.Body + "<br/><br/> We are working with the relevant teams to solve the issue.<br/><br/>Incident #" + i.incidentNumber + " is opened<br/><br/>More Updates will follow.<br/></td></tr></table>";
            //msg.Body = "<HTML><HEAD><style>table,th,td{border: 1px solid black;font-family: Calibri; font-size:13;padding: 3px;}</style></HEAD><BODY><br/><font face=Calibri size=2>Hi All,<br/><br/>" + msg.Body + "<br/><br/> We are working with the relevant teams to solve the issue.<br/><br/>Incident #" + i.incidentNumber + " is opened<br/><br/>More Updates will follow.<br/>";

            if (lyncCall == "1")
            {
                msg.Body += " Conference ID: 974237606 " + "<br/><br/>Thanks,<br/>" + HttpContext.Current.Session["username"].ToString() + " - " + HttpContext.Current.Session["user_email"].ToString();
            }
            else
            {
                msg.Body += "<br/><br/>Thanks,<br/>" + HttpContext.Current.Session["username"].ToString() + " - " + HttpContext.Current.Session["user_email"].ToString();
            }

            //Reading the email template files in order to send
            String htmlEmailStart = System.IO.File.ReadAllText(HostingEnvironment.MapPath(@"~/Content/email-start.html"));
            String htmlEmailEnd = System.IO.File.ReadAllText(HostingEnvironment.MapPath(@"~/Content/email-end.html"));
            msg.Body = htmlEmailStart + msg.Body + htmlEmailEnd;

            msg.IsBodyHtml = true;

            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(HttpContext.Current.Session["user_email"].ToString(), HttpContext.Current.Session["user_pass"].ToString());
            client.Port = 587; // You can use Port 25 if 587 is blocked (mine is!)
            client.Host = "smtp.office365.com";
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            try
            {
                client.Send(msg);
                if (lyncCall == "1")
                {
                    sendCallInvite(i.incidentNumber, i.descr);
                }
                return "Message Sent Succesfully";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private void sendCallInvite(int incidentNumber, String descr)
        {
            MailMessage msg = new MailMessage();
            msg.To.Add(new MailAddress("chris.sanchezc@hpe.com", "SomeOne"));
            msg.From = new MailAddress(HttpContext.Current.Session["user_email"].ToString(), HttpContext.Current.Session["username"].ToString());
            msg.Subject = "Join Lync Meeting on Incident #" + incidentNumber + " " + descr;

            msg.Body = "<HTML><BODY><br/><font face=Calibri size=1>....................................................................</font><br/><font face=Calibri size=5><a color=00b388 href=https://join.ucrtc.hpe.com/meet/maroon.shalbak/4GVJFCHR> Join Lync Meeting </a></font><br/><br/>" + "<font face=Calibri size=4>Join by phone</font> <br/><br/><br/> <font face=Calibri size=3> <A href=https://join.ucrtc.hpe.com/dialin>Find a local number</A><br/><br/>Conference ID: 974237606 </font><br/><br/><font face=Calibri font=3><A href=https://join.ucrtc.hpe.com/dialin>Forgot your dial-in PIN?</A> | <A href=http://o15.officeredir.microsoft.com/r/rlidLync15?clid=1033&p1=5&p2=2009>Help</a></font><br/><br/><font face=Calibri size=1>....................................................................</HTML></BODY>";

            msg.IsBodyHtml = true;

            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(HttpContext.Current.Session["user_email"].ToString(), HttpContext.Current.Session["user_pass"].ToString());
            client.Port = 587; // You can use Port 25 if 587 is blocked (mine is!)
            client.Host = "smtp.office365.com";
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;

            try
            {
                client.Send(msg);
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}