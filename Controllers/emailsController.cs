using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OzonTask.Models;
using System.Net;
using System.Net.Mail;
using Dapper;
using Npgsql;
using System.Data;
using Microsoft.Data.SqlClient;

namespace OzonTask.Controllers
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class emailsController : ControllerBase
    {
        EmailsContext ec;
        public emailsController(EmailsContext ec_)
        {
            ec = ec_;
        }


        [HttpPost]
        public ActionResult Post(Email email)
        {
            if (email.recipient == null || email.recipient == "")
                return BadRequest();
            if (email.subject == null || email.subject == "")
                return BadRequest();
            if (email.text == null || email.text == "")
                return BadRequest();

            if (email.carbon_copy_recipients == null || email.carbon_copy_recipients.Length == 0)
            {
                if (SendMail(email.recipient, email.subject, email.text))
                    ec.AddEmail(email, ec.user.EmailAddress , true);
                else
                    ec.AddEmail(email, ec.user.EmailAddress , false);
            }

            else
            {
                if (SendMail(email.recipient, email.subject, email.text))
                    ec.AddEmail(email, email.recipient, ec.user.EmailAddress, true);
                else
                    ec.AddEmail(email, email.recipient, ec.user.EmailAddress , false);

                // дял каждого получателя копии в списке получателей ....
                foreach(var address in email.carbon_copy_recipients)
                {
                    if (SendMail(address, email.subject, email.text))
                        ec.AddEmail(email, address, ec.user.EmailAddress , true);
                    else
                        ec.AddEmail(email, address, ec.user.EmailAddress , false);
                }
            }
            
            return Ok();
        }


        [HttpGet]
        public JsonResult GetEmails()
        {
            var emails = ec.GetEmails();
            if (emails == null)
                NotFound();
            return new JsonResult(emails);
        }

        public bool SendMail(string recipient, string subject, string text)
        {
            try
            {
                var fromAddress = new MailAddress(ec.user.EmailAddress , "From Nikolay");
                var toAddress = new MailAddress(recipient);
                string fromPassword = ec.user.Pass;

                var smtpClient = new SmtpClient
                {
                    Host = "smtp.mail.ru",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = text
                })
                {
                    smtpClient.Send(message);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
