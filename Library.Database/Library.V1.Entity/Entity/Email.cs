using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.IO;

namespace Library.V1.Entity
{

    public delegate void callback_email(MailMessage mailMessage);
    public delegate void callerr_email(Exception err);

    public class MMEmail
    {
        public event callback_email onCompleted;
        public event callerr_email onError;
        public event callback_email onCancelled;

        private MailMessage mailMessage;
        private SmtpClient smtpClient;

        #region email settings property
        private string _user;
        public string User
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
            }
        }

        private string _pasword;
        public string Password
        {
            get
            {
                return _pasword;
            }
            set
            {
                _pasword = value;
            }
        }

        public ICredentialsByHost Credentials
        {
            get
            {
                return this.smtpClient.Credentials;
            }
            set
            {
                this.smtpClient.Credentials = new NetworkCredential(this.User, this.Password);
            }
        }

        public string Host
        {
            get
            {
                return this.smtpClient.Host;
            }
            set
            {
                this.smtpClient.Host = value;
            }
        }

        public int Port
        {
            get
            {
                return this.smtpClient.Port;
            }
            set
            {
                this.smtpClient.Port = value;
            }
        }

        public bool enableSSL
        {
            get
            {
                return this.smtpClient.EnableSsl;
            }
            set
            {
                this.smtpClient.EnableSsl = value;
            }
        }
        #endregion email setttings property

        #region email message
        public bool isBodyHtml
        {
            /*
             如果是 HTML 和 不是 HTML:  html: <br>  Text: \n \t
            */
            get
            {
                return this.mailMessage.IsBodyHtml;
            }
            set
            {
                this.mailMessage.IsBodyHtml = value;
            }
        }

        public string Subject
        {
            get
            {
                return this.mailMessage.Subject;
            }
            set
            {
                this.mailMessage.Subject = value;
            }

        }

        public string Content
        {
            get
            {
                return this.mailMessage.Body;
            }
            set
            {
                this.mailMessage.Body = value;

            }
        }

        public void addFrom(string email_address, string display_name = "")
        {
            if (string.IsNullOrEmpty(display_name))
            {
                this.mailMessage.From = new MailAddress(email_address);
            }
            else
            {
                this.mailMessage.From = new MailAddress(email_address, display_name, UnicodeEncoding.UTF8);
            }
        }

        public void addTo(string email_address, string display_name = "")
        {
            if (string.IsNullOrEmpty(display_name))
            {
                this.mailMessage.To.Add(new MailAddress(email_address));
            }
            else
            {
                this.mailMessage.To.Add(new MailAddress(email_address, display_name, UnicodeEncoding.UTF8));
            }
        }

        public void clearTo()
        {
            this.mailMessage.To.Clear();
        }

        public void addCC(string email_address, string display_name = "")
        {
            if (string.IsNullOrEmpty(display_name))
            {
                this.mailMessage.CC.Add(new MailAddress(email_address));
            }
            else
            {
                this.mailMessage.CC.Add(new MailAddress(email_address, display_name, UnicodeEncoding.UTF8));
            }
        }
        public void clearCC()
        {
            this.mailMessage.CC.Clear();
        }

        public void addBCC(string email_address, string display_name = "")
        {
            if (string.IsNullOrEmpty(display_name))
            {
                this.mailMessage.Bcc.Add(new MailAddress(email_address));
            }
            else
            {
                this.mailMessage.Bcc.Add(new MailAddress(email_address, display_name, UnicodeEncoding.UTF8));
            }
        }
        public void addReply(string email_address, string display_name = "")
        {
            this.mailMessage.ReplyToList.Add(email_address);
        }
        public void clearBCC()
        {
            this.mailMessage.Bcc.Clear();
        }

        public void Send()
        {
            try
            {
                this.smtpClient.Send(this.mailMessage);
            }
            catch (Exception err)
            {
                if (this.onError != null) this.onError(err);
            }
        }

        public void SendAsync()
        {
            try
            {
                this.smtpClient.SendAsync(this.mailMessage, null);
            }
            catch (Exception err)
            {
                if (this.onError != null) this.onError(err);
            }
        }

        public void SendCancel()
        {
            this.smtpClient.SendAsyncCancel();
        }

        void smtpClient_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                if (this.onCancelled != null) this.onCancelled(this.mailMessage);
            }
            else
            {
                if (this.onCompleted != null) this.onCompleted(this.mailMessage);
            }
        }
        #endregion email message

        #region Attachments
        /*
            嵌入式图片： 
            myemail.Content = "Hello World<br><br>Are you ok recently?<br><img src='cid:11001' /><br><br>William Liu<br>778-888-6068";
            myemail.addAttachment(@"C:\Apps\Source\FM\FM_2013.10.23_11.23.44.jpg", "", "11001");   嵌入式图片
            myemail.addAttachment(@"C:\Apps\Source\Other\TM_2014.08.19_23.47.18.jpg", "", "");     附件形式

            此时 Attachment 将不是附件形式， 而是作为嵌入式图片， 把图片原始内容当成邮件内容的一部分。
        */
        public AttachmentCollection Attachments
        {
            get
            {
                return this.mailMessage.Attachments;
            }
        }

        public void clearAttachments()
        {
            this.mailMessage.Attachments.Clear();
        }

        public void addAttachment(string file_name, string media_type = "", string content_id = "")
        {
            Attachment att = new Attachment(file_name, media_type);
            //att.Name = "lwh" + content_id + ".jpg";   指定新的名字；
            //没有提供，使用 file_name 的文件名
            att.ContentId = content_id;
            this.mailMessage.Attachments.Add(att);
        }

        public void addAttachment(string file_name, ContentType content_type, string content_id = "")
        {
            Attachment att = new Attachment(file_name, content_type);
            att.ContentId = content_id;
            this.mailMessage.Attachments.Add(att);
        }

        public void addAttachment(Stream file_stream, string file_name, string media_type = "", string content_id = "")
        {
            Attachment att = new Attachment(file_stream, media_type);
            att.Name = file_name;
            att.ContentId = content_id;
            this.mailMessage.Attachments.Add(att);
        }

        public void addAttachment(Stream file_stream, string file_name, ContentType content_type, string content_id = "")
        {
            Attachment att = new Attachment(file_stream, content_type);
            att.Name = file_name;
            att.ContentId = content_id;
            this.mailMessage.Attachments.Add(att);
        }



        // other way to add attachments,  add embed image;
        /*
        string constr = "Hello 你好<br>\n\tAre you ok recently?<br><img src='cid:11003' /><br>Other Image:<br><img src='cid:11007' /><br>William Liu<br>778-888-6068";
        myemail.addViews(constr, "v1000");
        myemail.addResources("v1000", @"C:\Apps\Source\FM\FM_2013.10.23_11.23.44.jpg", "", "11003");
        myemail.addResources("v1000", @"C:\Apps\Source\Other\TM_2014.08.19_23.45.14.jpg", "", "11007");
        
        Only allow one AlternateView, below will become xxx.bin attachment:
        
        myemail.addViews("<br>Good Friday:<br><br>Image:<img src='cid:20013' /><br>Long Weekend", "v2000");
        myemail.addResources("v2000", @"C:\Apps\Source\Other\FM_2013.01.01_14.46.38.jpg", "", "20013");
        */
        public void addViews(string html_content, string content_id)
        {
            ContentType mimeType = new System.Net.Mime.ContentType("text/html");
            AlternateView alv = AlternateView.CreateAlternateViewFromString(html_content, mimeType);
            alv.ContentId = content_id;
            /*
            ContentType contentType = new ContentType("html/text");
            contentType.Boundary = string.Format("-----{0}-{1}------", DateTime.Now.ToLocalTime(), content_id); 
            contentType.CharSet = "UTF-8";
            contentType.Name = Path.GetFileName(file_name);
            AlternateView alv = AlternateView.CreateAlternateViewFromString(html_content, contentType);
            */


            /*
            AlternateView alv = AlternateView.CreateAlternateViewFromString(html_content, UnicodeEncoding.UTF8, "html/text");
            // embed image or document
            LinkedResource lr = new LinkedResource(file_name, media_type);
            lr.ContentId = content_id;

            alv.LinkedResources.Add(lr);
            */

            this.mailMessage.AlternateViews.Add(alv);
        }

        // this doesn't work for attachement or embed image
        public void addImage(string file_name, string content_id)
        {
            ContentType mimeType = new System.Net.Mime.ContentType("image/jpeg");
            AlternateView alv = new AlternateView(file_name);
            alv.ContentId = content_id;

            this.mailMessage.AlternateViews.Add(alv);
        }

        public void addResources(string view_id, string file_name, string media_type = "", string resource_id = "")
        {
            AlternateView alv = this.mailMessage.AlternateViews.FirstOrDefault(p => p.ContentId == view_id);
            if (alv != null)
            {
                LinkedResource lr = new LinkedResource(file_name, media_type);
                lr.ContentId = resource_id;
                alv.LinkedResources.Add(lr);
            }
        }
        #endregion Attachments

        public MMEmail()
        {
            this.mailMessage = new MailMessage();
            this.mailMessage.BodyEncoding = UnicodeEncoding.UTF8;
            this.isBodyHtml = true;

            this.smtpClient = new SmtpClient();
            this.smtpClient.Timeout = 10000;
            this.smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            this.smtpClient.UseDefaultCredentials = false;
            this.smtpClient.SendCompleted += new SendCompletedEventHandler(smtpClient_SendCompleted);

            this.Port = 587; //25;   //587 default
            this.enableSSL = true;  // most required
        }

        // stmp :  hotmail: smtp.live.com 
        public MMEmail(string smtp) : this()
        {
            this.Host = smtp;
        }

        public MMEmail(string smtp, string user, string password) : this(smtp)
        {
            this.User = user;
            this.Password = password;
            this.Credentials = new NetworkCredential(this.User, this.Password);
        }
    }
}

/* sample :
MMEmail myemail = new MMEmail("smtp-mail.outlook.com", "william_lwh@hotmail.com", "liu20060923");
myemail.addFrom("william_lwh@hotmail.com", "William Motion");

myemail.addCC("185290926@qq.com");
myemail.addCC("william_lwh@hotmail.com");
myemail.addCC("william@motionmetrics.com");
myemail.addBCC("william.liuweihui@gmail.com");
myemail.onCompleted += new callback_email(myemail_onCompleted);
myemail.onError += new callerr_email(myemail_onError);
myemail.onCancelled += new callback_email(myemail_onCancelled);

myemail.Subject = "Test from C# William in Motion Metrics";

// first image is embed image ;  second image is attachment;
string body = "Hello 你好<br>\n\tAre you ok recently?<br><img src='cid:11003' /><br><br><br>William Liu<br>778-888-6068";
myemail.Content = body;
myemail.addAttachment(@"C:\Apps\Source\FM\FM_2013.10.23_11.23.44.jpg", "", "11003");
myemail.addAttachment(@"C:\Apps\Source\FM\FM_2013.10.23_11.23.44.jpg", "", "11005");

myemail.SendAsync();
*/



/*
Solve Problem  Attachment Filename has no quote "",  Issued by Michael Zhaojianhong

string boundary = Guid.NewGuid().ToString();
mail.Headers.Add("Content-Type", "multipart/mixed; boundary=--" + boundary);
mail.Headers.Add("", Environment.NewLine);  \\  NewLine "\r\n"
mail.Headers.Add("", Environment.NewLine);

mail.Headers.Add("", "--" + boundary);
mail.Headers.Add("Content-Type", "text/html; charset=utf-8;");
mail.Headers.Add("Content-Disposition", "attachment; filename=\"" + FileName + "\"");
mail.Headers.Add("Content-Transfer-Encoding", "base64; ");
mail.Headers.Add("", Environment.NewLine);
var bytes = Encoding.UTF8.GetBytes(html_body);
var base64 = Convert.ToBase64String(bytes);
mail.Headers.Add("", base64.ToString());
mail.Headers.Add("", "--" + boundary);
*/

/*
// Use a new MailMessage
using (MailMessage mailMessage = new MailMessage(mailFrom, cm.MailToAdr))
{
    string fullPath = Path.Combine("C:\\MZTest\\", fileName);
    mailMessage.Priority = MailPriority.High;
    mailMessage.IsBodyHtml = true;
    mailMessage.BodyEncoding = Encoding.UTF8;
    mailMessage.BodyTransferEncoding=TransferEncoding.Base64;
    mailMessage.SubjectEncoding = Encoding.UTF8;
    mailMessage.Subject = subject;
    mailMessage.HeadersEncoding= Encoding.UTF8;

    // Generate boundary string
    string boundary = Guid.NewGuid().ToString();
                           
    // Create content type for the main view
    ContentType mainContentContent = new ContentType()
    {
        Boundary = boundary,
        CharSet = "utf-8",
        MediaType = "multipart/mixed"
    };

    // Create the main view
    AlternateView mainView = AlternateView.CreateAlternateViewFromString("", mainContentContent);
    mainView.TransferEncoding = TransferEncoding.Base64;
    mailMessage.AlternateViews.Add(mainView);

    // Create attachment content type
    ContentType attchmentContent = new ContentType()
    {
        Boundary = boundary,
        CharSet = "utf-8",
        MediaType = "text/plain",
        Name = "\"" + fileName + "\""
    };

    // Create attachment                       
    Attachment attach = new Attachment(fullPath, attchmentContent);
    attach.ContentDisposition.FileName = "\"" + fileName + "\"";
                            
    mailMessage.Attachments.Add(attach);
    client.Send(mailMessage);
}
*/
