using System;
using System.Net;
using System.Net.Mail;
using System.ComponentModel;
using System.Windows.Forms;
// 23.07.2025: 
//using Outlook = Microsoft.Office.Interop.Outlook;
using System.Collections.Generic;
using System.IO;

#if MailKit_cemo_poceti_koristiti_umj_smtpclient
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

/// <summary>
/// Ovo mi je gpt napravio
/// </summary>

public struct EmailMessageData_GPT
{
   // SMTP settings
   public string SmtpServer { get; set; }
   public int SmtpPort { get; set; }
   public string SmtpUser { get; set; }
   public string SmtpPass { get; set; }

   // Email addresses
   public string FromAddress { get; set; }
   public string FromName { get; set; }
   public List<string> ToAddresses { get; set; }
   public List<string> CcAddresses { get; set; }
   public List<string> BccAddresses { get; set; }

   // Email content
   public string Subject { get; set; }
   public string Body { get; set; }
   public bool IsHtml { get; set; }

   // Attachments
   public List<string> AttachmentPaths { get; set; }
}

public static class EmailHelper_GPT
{
   public static void SendEmail(EmailMessageData_GPT data)
   {
      SendEmail(
       data.SmtpServer,
       data.SmtpPort,
       data.SmtpUser,
       data.SmtpPass,
       data.FromAddress,
       data.FromName,
       data.ToAddresses,
       data.CcAddresses,
       data.BccAddresses,
       data.Subject,
       data.Body,
       data.IsHtml = false,
       data.AttachmentPaths = null);
   }
   public static void SendEmail(
       string smtpServer,
       int smtpPort,
       string smtpUser,
       string smtpPass,
       string fromAddress,
       string fromName,
       IEnumerable<string> toAddresses,
       IEnumerable<string> ccAddresses,
       IEnumerable<string> bccAddresses,
       string subject,
       string body,
       bool isHtml = false,
       IEnumerable<string> attachmentPaths = null)
   {
      var message = new MimeMessage();

      // From
      message.From.Add(new MailboxAddress(fromName, fromAddress));

      // To
      if(toAddresses != null) { foreach(var to in toAddresses) message.To.Add(MailboxAddress.Parse(to)); }

      // CC
      if(ccAddresses != null) { foreach(var cc in ccAddresses) message.Cc.Add(MailboxAddress.Parse(cc)); }

      // BCC
      if(bccAddresses != null) { foreach(var bcc in bccAddresses) message.Bcc.Add(MailboxAddress.Parse(bcc)); }

      message.Subject = subject;

      var bodyBuilder = new BodyBuilder();

      if(isHtml) bodyBuilder.HtmlBody = body;
      else       bodyBuilder.TextBody = body;

      // Attachments
      if(attachmentPaths != null)
      {
         foreach(var filePath in attachmentPaths)
         {
            if(File.Exists(filePath)) bodyBuilder.Attachments.Add(filePath);
            else                      Console.WriteLine($"Warning: Attachment not found: {filePath}");
         }
      }

      message.Body = bodyBuilder.ToMessageBody();

      using(var client = new MailKit.Net.Smtp.SmtpClient())
      {
         // For testing only - bypass cert validation (remove for production)
         client.ServerCertificateValidationCallback = (s, c, h, e) => true;

         client.Connect(smtpServer, smtpPort, SecureSocketOptions.SslOnConnect);
         client.Authenticate(smtpUser, smtpPass);
         client.Send(message);
         client.Disconnect(true);
      }
   }
}

#endif
public sealed class VvMailClient
{
   #region Propertiz, Fieldz, defaults 

   private MailMessage MailMessage { get; set; }
   private System.Net.Mail.SmtpClient  SmtpClient  { get; set; }

 //public System.Net.Mail.Attachment[] MailAttachmentList { get; set; }
   public string[] MailAttachmentFileNameList { get; set; }
 //private MailAddress From           { get; set; }
 //private MailAddress To             { get; set; }

   public string EmailFromAddress     { get; set; } 
   public string EmailFromPasswd      { get; set; } 
   public string EmailFromUserName    { get; set; } 
   public string EmailFromDisplayName { get; set; } 
 //public string EmailTo              { get; set; }

   public string MailHost       { get; set; }
   public string MessageSubject { get; set; }
   public string MessageBody    { get; set; }

 //int    portNumber                 = 587; // a 25 je default?! 
   public bool   EnableSSL      { get; set; }
   public bool   IsBodyHtml     { get; set; } // Can set to false, if you are sending pure text.
   public bool   IsCcToMySelf   { get; set; } 
   public int    PortNo         { get; set; } 
 //bool   useDefaultCredentials      = false; // ovo mora biti false! 
 //string vektorEmailPasswd          = "qwe1qwe2qwe3".Replace("qwe3", "84").Replace("qwe2", "ix").Replace("qwe1", "Nu"); // pokusavamo kamuflirati string anti reverse engeneering 

   bool mailSent   ;
   bool isNotSilent;

   #endregion Propertiz, Fieldz, defaults

   #region Constructorz

   public VvMailClient() 
   {
   }

   #endregion Constructorz

   #region Methodz

   public bool SendMail_SUPPORT(bool isNotSilent, string messageSubject, string messageBody, string emailFromDisplayName_addition)
   {
      this.SetVvSupportMailData(emailFromDisplayName_addition);

      this.MessageSubject = messageSubject;

      this.MessageBody = String.Format("{0}\n\rProjekt/Year: {1}\n\r{2}\n\rTheVvDataRecord: {3}\n\r~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n\r", 
         ZXC.TheVvForm.Text, 
         ZXC.CURR_prjkt_rec.ToString() + " - " + ZXC.projectYear,

         (ZXC.TheVvForm.TheVvTabPage != null && ZXC.TheVvForm.TheVvUC != null) ? ZXC.TheVvForm.TheVvUC.ToString() : "",
                  ZXC.TheVvForm.TheVvTabPage != null && ZXC.TheVvForm.TheVvUC != null && ZXC.TheVvForm?.TheVvDataRecord != null ? ZXC.TheVvForm.TheVvDataRecord.ToString() : "");

      this.MessageBody += messageBody;

      return this.SendMail_Normal(isNotSilent, ZXC.VektorEmailAddress);
   }

   public void SetVvSupportMailData(string emailFromDisplayName_addition)
   {
      this.EmailFromPasswd      = ZXC.SkyLabEmailPassword;
      this.MailHost             = ZXC.ViperMailHost      ;
      this.EmailFromAddress     = 
      this.EmailFromUserName    = ZXC.SkyLabEmailAddress ;

      this.EmailFromDisplayName = "[" + ZXC.vvDB_VvDomena + "] - " + ZXC.VvDeploymentSite + emailFromDisplayName_addition;
   }

   #region MS Outlook 2010 

   //public static void pero() // MSDN primjer
   //{
   //   // Command line argument must the the SMTP host.
   //   SmtpClient smtpClient = new SmtpClient(viperMailHost);
   //   // Specify the e-mail sender.
   //   // Create a mailing address that includes a UTF8 character
   //   // in the display name.
   //   MailAddress from = new MailAddress(vektorEmailFromAddress, "VvSkyDiag", ZXC.VvUTF8Encoding_noBOM);
   //   // Set destinations for the e-mail message.
   //   MailAddress to = new MailAddress(robertEmailTo);
   //   // Specify the message content.
   //   MailMessage mailMessage = new MailMessage(from, to);
   //   mailMessage.Body = "This is a test e-mail message sent by an application. ";
   //   // Include some non-ASCII characters in body and subject.
   //   string someArrows = new string(new char[] { '\u2190', '\u2191', '\u2192', '\u2193' });
   //   mailMessage.Body += System.Environment.NewLine + someArrows;
   //   mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
   //   mailMessage.Subject = "test message 1" + someArrows;
   //   mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
   //   // Set the method that is called back when the send operation ends.
   //   //client.SendCompleted += new
   //   //SendCompletedEventHandler(SendCompletedCallback);
   //   // The userState can be any object that allows your callback 
   //   // method to identify this send operation.
   //   // For this example, the userToken is a string constant.
   //   smtpClient.Send(mailMessage);
   //   // Clean up.
   //   mailMessage.Dispose();
   //}

   public static List<ZXC.CdAndName_CommonStruct> GetOutlookContacts_Fill_LuiListaOutlookItems()
   {
      ZXC.luiListaOutlookItems.Clear();
      List<ZXC.CdAndName_CommonStruct> contactList = new List<ZXC.CdAndName_CommonStruct>();

      // 23.07.2025: gasim ovo da vise ne trebamo         
      // using Outlook = Microsoft.Office.Interop.Outlook;

#if noenimor
      #region Outlook Addressbook

      try
      {
         Outlook.Items       outlookItems;
         Outlook.Application outlookApplication;
         Outlook.MAPIFolder theMAPIFolder;
         outlookApplication = new Outlook.Application();
         theMAPIFolder = (Outlook.MAPIFolder)outlookApplication.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts);
         outlookItems = theMAPIFolder.Items;

         foreach(Outlook.ContactItem contact in outlookItems)
         {
            //Outlook.ContactItem contact = (Outlook.ContactItem)OutlookItems[i + 1];
            //string mail = contact.Email1Address     ;
            //string name = contact.FullNameAndCompany;
            if(contact.Email1Address.NotEmpty())
            {
               contactList             .Add(new ZXC.CdAndName_CommonStruct(contact.Email1Address, contact.FullNameAndCompany));
               ZXC.luiListaOutlookItems.Add(new VvLookUpItem              (contact.Email1Address, contact.FullNameAndCompany));
            }
         }
      }
      catch(System.Exception ex) 
      { 
         //ZXC.aim_emsg(MessageBoxIcon.Error, "Ne mogu isčitati adresar.\n\nZadajte email adresu ručno.\n\nGetContacts exception:\n\n{0}", ex.Message); return (contactList); 
      }

      #endregion Outlook Addressbook
#endif
      return contactList;
   }

   private string ReadSignature1()
   {
      string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\Signatures";
      string signature = string.Empty;
      string signatureFileExstension = "*.htm";

      DirectoryInfo diInfo = new DirectoryInfo(appDataDir);
      if(diInfo.Exists)
      {
         FileInfo[] fiSignature = diInfo.GetFiles(signatureFileExstension);

       //List<FileInfo> fiSignatureList = new List<FileInfo>(/*fiSignature*/);
       //
       //string wantedSignatureName = "zzz" + signatureFileExstension;
       //
       //FileInfo wantedSignatureFInfo = fiSignatureList.SingleOrDefault();

       if(fiSignature/*List*/.Length.IsPositive())
       {
          using(StreamReader sr = new StreamReader(fiSignature[0].FullName, System.Text.Encoding.Default))
          {
             signature = sr.ReadToEnd();
             if(!string.IsNullOrEmpty(signature))
             {
                string fileName = fiSignature[0].Name.Replace(fiSignature[0].Extension, string.Empty);
                signature = signature.Replace(fileName + "_files/", appDataDir + "/" + fileName + "_files/");
             }
          }
       }

      }
      return signature;
   }

   // !!! //
   public bool SendViaOutlook(string[] mailAddressList, bool isDisplay)
   {
      // 23.07.2025: gasim ovo da vise ne trebamo         
      // using Outlook = Microsoft.Office.Interop.Outlook;

      bool OK = true;

#if noenimore
      Outlook.Application outlookApplication;
      outlookApplication = new Outlook.Application();

    //Outlook.MailItem myOutlookMail = (Outlook.MailItem) outlookApplication.CreateItem(Outlook.OlItemType.olMailItem);
      Outlook.MailItem myOutlookMail =                    outlookApplication.CreateItem(Outlook.OlItemType.olMailItem);
      myOutlookMail.Subject    = MessageSubject;
      myOutlookMail.Body       = MessageBody   ;
      myOutlookMail.Importance = Outlook.OlImportance.olImportanceNormal;
    //myOutlookMail.HTMLBody   = MessageBody + myOutlookMail.HTMLBody;
      myOutlookMail.HTMLBody += ReadSignature1();

      foreach(string mailAddress in mailAddressList)
      {
         myOutlookMail.Recipients.Add(mailAddress);
      }

      if(MailAttachmentFileNameList != null && MailAttachmentFileNameList.Length.NotZero())
      {
         foreach(string mailAttachmentFileName in MailAttachmentFileNameList)
         {
            if(mailAttachmentFileName.NotEmpty()) myOutlookMail.Attachments.Add(mailAttachmentFileName);
         }
      }

      if(isDisplay) ((Outlook._MailItem)myOutlookMail).Display(false);
      else          ((Outlook._MailItem)myOutlookMail).Send();

      if(!isDisplay) ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Information, "Mail poslan.\n\nKopija je u Outlook-ovom Sent Items");
#endif
      return OK;
   }

   public static bool IsOutlookInstalled()
   {
      // 23.07.2025: gasim ovo da vise ne trebamo         
      // using Outlook = Microsoft.Office.Interop.Outlook;

      return false;

      //try
      //{
      //   Outlook.Application outlookApplication;
      //   outlookApplication = new Outlook.Application();
      //
      // //var officeType = Type.GetTypeFromProgID("Outlook.Application");
      // //if(officeType         == null)
      //   if(outlookApplication == null)
      //   {
      //      // Outlook is not installed.   
      //      return false;
      //   }
      //   else
      //   {
      //      // '14.0.0.7113' ... Ti je Outlook 2010 version
      //      // tu bi mozda trebalo provjeravati i verziju outlook-a 
      //      // ... ali .StartsWith() 
      //
      //     // case "7.0": sVersion = "95";   break;
      //     // case "8.0": sVersion = "97";   break;
      //     // case "9.0": sVersion = "2000"; break;
      //     // case "10.0":sVersion = "2002"; break;
      //     // case "11.0":sVersion = "2003"; break;
      //     // case "12.0":sVersion = "2007"; break;
      //     // case "14.0":sVersion = "2010"; break;
      //     //default: sVersion = "Too Old!"; break;
      //
      //      // Outlook is installed.      
      //      return true;
      //   }
      //}
      //catch(System.Exception ex)
      //{
      //   return false;
      //}
   }

   #endregion MS Outlook 2010

   private void Create_MailMessage_To(string[] mailAddressList)
   {
      if(EmailFromDisplayName.IsEmpty()) EmailFromDisplayName = EmailFromAddress;

      MailMessage.From = new MailAddress(EmailFromAddress, EmailFromDisplayName, ZXC.VvUTF8Encoding_noBOM);

      foreach(string mailAddress in mailAddressList)
      {
         if(mailAddress.NotEmpty()) MailMessage.To.Add(mailAddress);
      }
   }


   public bool SendMail_Normal(bool _isNotSilent, string mailTO_Address)
   {
      return SendMail_Normal(_isNotSilent, new string[] { mailTO_Address });
   }

   // !!! //
   public bool SendMail_Normal(bool _isNotSilent, string[] mailTO_AddressList)
   {
      // 14.08.2025: plus hosting inzistira na SSL/TLS 
      //ukoliko koristite port 587 tada je potrebno odabrati TLS kao "encryption method",
      //ukoliko koristite port 465 tada možete postaviti "encryption method" SSL.
      // dole fiksno stavljamo da je default port 587 te kada tu po novome fiksno upalimo EnableSSL 
      // zavrsit cemo na TLS (a 465 i SSL ode u timeout) 
      // ako ces ubuduce 465 onda koristi danas preko chatgpt dodan 'EmailHelper_GPT' 
      EnableSSL = true; // !!! 

      //CreateMailFromAndToAddresses(mailAddressList);

      isNotSilent = _isNotSilent;

      Cursor.Current = Cursors.WaitCursor;

    //using(MailMessage = new MailMessage(From, To))
      using(MailMessage = new MailMessage(        ))
      {
         Create_MailMessage_To(mailTO_AddressList);

         MailMessage.Subject            = MessageSubject;
         MailMessage.Body               = MessageBody   ;

         MailMessage.SubjectEncoding    = 
         MailMessage.BodyEncoding       = ZXC.VvUTF8Encoding_noBOM;

         MailMessage.IsBodyHtml = IsBodyHtml    ; // Can set to false, if you are sending pure text.

         if(MailAttachmentFileNameList != null && MailAttachmentFileNameList.Length.NotZero())
         {
            foreach(string mailAttachmentFileName in MailAttachmentFileNameList)
            {
               if(mailAttachmentFileName.NotEmpty()) MailMessage.Attachments.Add(new System.Net.Mail.Attachment(mailAttachmentFileName));
            }
         }

         if(IsCcToMySelf) MailMessage.CC.Add(new MailAddress(MailMessage.From.Address));

         if(IsMissingSomeMailData()) { mailSent = false; return false; }

         List<string> mailTO_stringList = new List<string>(mailTO_AddressList);

         // 31.10.2022: 
       //using(SmtpClient = new                 SmtpClient(MailHost, (PortNo.IsZero() ?  25 : PortNo))) // port 25 je default 
         using(SmtpClient = new System.Net.Mail.SmtpClient(MailHost, (PortNo.IsZero() ? 587 : PortNo))) // port 25 je default 
         {
            // !!! NOTA BENE !!!
            // 1. pozivanje SETtera od 'UseDefaultCredentials'  postavlja SmtpClient.Credentials na NULL!  
            // 2. pozivanje SETtera od 'SmtpClient.Credentials' postavlja UseDefaultCredentials  na false! 
            // ...tako da je JAKO BITAN RASPORED SETtanja
            // najbolje je uopce ne settati UseDefaultCredentials 

            // U biti, jos nijedamput, ni sa kojom kombinacijom nisam uspio dobiti da mi koristi moj konkretni credential 
            // ako ikako ide, koristi slanje mailova BEZ zadavanja passworda! 
            if(EmailFromUserName.IsEmpty()) EmailFromUserName = EmailFromAddress;
            if(EmailFromPasswd.NotEmpty ()) SmtpClient.Credentials = new NetworkCredential(/*EmailFromAddress*/EmailFromUserName, EmailFromPasswd); // ne treba ?! 

          //SmtpClient.UseDefaultCredentials = useDefaultCredentials; // false! 
            SmtpClient.EnableSsl             = EnableSSL            ; // false  (empirijski, ovo nema veze sa credentialsima) 
            SmtpClient.DeliveryMethod        = SmtpDeliveryMethod.Network;

            try
            {
               SmtpClient.Send(MailMessage); // normal 
             //SmtpClient.SendAndSaveMessageToIMAP(MailMessage, MailHost, 993/*PortNo*/, EmailFromUserName, EmailFromPasswd, "Sent"/*"Sent Items"*/); // normal 
               mailSent = true;
            }
            catch(System.Security.Authentication.AuthenticationException authEx)
            {
               mailSent = false;
             //ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "AuthenticationException!\n\r" + authEx.Message);
               ZXC.aim_emsg_List("AuthenticationException!" + authEx.Message, mailTO_stringList);
            }
            catch(System.Net.Mail.SmtpException smtpEx)
            {
               mailSent = false;
             //ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "SmtpException!\n\r" + smtpEx.Message);
               ZXC.aim_emsg_List("SmtpException!" + smtpEx.Message, mailTO_stringList);
            }
            catch(System.Exception ex)
            {
               mailSent = false;
             //ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "SendMail_Normal Exception!\n\r" + ex.Message);
               ZXC.aim_emsg_List("SendMail_Normal Exception!" + ex.Message, mailTO_stringList);
            }
            finally
            {
               MailMessage.Dispose();
               SmtpClient .Dispose();

               Cursor.Current = Cursors.Default;
            }

         } // using(SmtpClient smtpClient = new SmtpClient(MailHost/*, portNumber*/)) // port 25 je default 
      } // using(MailMessage mailMessage = new MailMessage(From, To))

      if(isNotSilent && mailSent) ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Information, "Mail poslan.");

      return mailSent;
   }

   public static bool gpt()
   {
      string smtpServer = ZXC.ViperMailHost      ;
      int smtpPort      = 465                    ;
      string smtpUser   = ZXC.SkyLabEmailAddress ;
      string smtpPass   = ZXC.SkyLabEmailPassword;

      string fromAddress = ZXC.SkyLabEmailAddress ;
      string toAddress   = "viper@zg.htnet.hr.com"; 
      string subject     = "Test Email"           ;
      string body        = "Hello! This is a test email sent via C# using SSL/TLS.";

      try
      {
         using(var client = new System.Net.Mail.SmtpClient(smtpServer, smtpPort))
         {
            client.EnableSsl             = true; // Enable SSL/TLS
            client.Credentials           = new NetworkCredential(smtpUser, smtpPass);
            client.DeliveryMethod        = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;

            // NOTE: SmtpClient supports implicit SSL only via ports like 465 in some .NET versions.
            // If issues arise, you might need MailKit (NuGet) for modern SSL support.

            using(var message = new MailMessage(fromAddress, toAddress, subject, body))
            {
               client.Send(message);
            }
         }

         Console.WriteLine("Email sent successfully!");
      }
      catch(Exception ex)
      {
         Console.WriteLine("Error sending email: " + ex.Message);
      }

      return true;
   }
   public bool SendMail_Async (bool _isNotSilent, string mailTO_Address)
   {
      return SendMail_Async(_isNotSilent, new string[] { mailTO_Address });
   }

   // !!! //
   public bool SendMail_Async (bool _isNotSilent, string[] mailTO_AddressList)
   {
      isNotSilent = _isNotSilent;

      MailMessage = new MailMessage();

      Create_MailMessage_To(mailTO_AddressList);

      MailMessage.Subject            = MessageSubject;
      MailMessage.Body               = MessageBody   ;

      MailMessage.SubjectEncoding    = 
      MailMessage.BodyEncoding       = ZXC.VvUTF8Encoding_noBOM;

      MailMessage.IsBodyHtml = IsBodyHtml; // Can set to false, if you are sending pure text.

      if(MailAttachmentFileNameList != null && MailAttachmentFileNameList.Length.NotZero())
      {
         foreach(string mailAttachmentFileName in MailAttachmentFileNameList)
         {
            if(mailAttachmentFileName.NotEmpty()) MailMessage.Attachments.Add(new System.Net.Mail.Attachment(mailAttachmentFileName));
         }
      }

      if(IsCcToMySelf) MailMessage.CC.Add(new MailAddress(MailMessage.From.Address));

      if(IsMissingSomeMailData()) { mailSent = false; return false; }

      SmtpClient = new System.Net.Mail.SmtpClient(MailHost, (PortNo.IsZero() ? 25 : PortNo));

     // !!! NOTA BENE !!!
     // 1. pozivanje SETtera od 'UseDefaultCredentials'  postavlja SmtpClient.Credentials na NULL!  
     // 2. pozivanje SETtera od 'SmtpClient.Credentials' postavlja UseDefaultCredentials  na false! 
     // ...tako da je JAKO BITAN RASPORED SETtanja
     // najbolje je uopce ne settati UseDefaultCredentials 

    //SmtpClient.UseDefaultCredentials = useDefaultCredentials; // false! 
      SmtpClient.EnableSsl = EnableSSL; // false  (empirijski, ovo nema veze sa credentialsima) 

     // U biti, jos nijedamput, ni sa kojom kombinacijom nisam uspio dobiti da mi koristi moj konkretni credential 
     // ako ikako ide, koristi slanje mailova BEZ zadavanja passworda! 
      if(EmailFromUserName.IsEmpty()) EmailFromUserName = EmailFromAddress;
      if(EmailFromPasswd.NotEmpty()) SmtpClient.Credentials = new NetworkCredential(/*EmailFromAddress*/EmailFromUserName, EmailFromPasswd); // ne treba ?! 

      SmtpClient.SendCompleted += new SendCompletedEventHandler(MaliMessage_SendCompletedCallback);

      string userState = "Some Fuse Info";

      try
      {
         SmtpClient.SendAsync(MailMessage, userState); // async  
         mailSent = true;
      }
      catch(System.Security.Authentication.AuthenticationException authEx)
      {
         mailSent = false;
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "AuthenticationException!\n\r" + authEx.Message);
      }
      catch(System.Exception ex)
      {
         mailSent = false;
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Exception!\n\r" + ex.Message);
      }

      return mailSent;
   }

   private void MaliMessage_SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
   {
      // Get the unique identifier for this asynchronous operation.
      String token = (string)e.UserState;

      MailMessage.Dispose();
      SmtpClient .Dispose();

      if(e.Cancelled)
      {
         if(isNotSilent) ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Information, "[{0}]\n\nSend canceled.", token);
      }
      if(e.Error != null)
      {
         if(isNotSilent) ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "[{0}]\n\n{1}", token, e.Error.ToString());
      }
      else
      {
         if(isNotSilent) ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Information, "Message sent async.");
      }
   }

   private bool IsMissingSomeMailData()
   {
      if(MailHost                 .IsEmpty()) { ZXC.aim_emsg(MessageBoxIcon.Error, "Mail poruka neće biti poslana.\n\nNedostaje podatak 'MailHost'"); return true; }
      if(EmailFromAddress         .IsEmpty()) { ZXC.aim_emsg(MessageBoxIcon.Error, "Mail poruka neće biti poslana.\n\nNedostaje podatak 'From'"    ); return true; }
      if(MailMessage.To.Count.IsZero() || MailMessage.To[0].Address.IsEmpty()) { ZXC.aim_emsg(MessageBoxIcon.Error, "Mail poruka neće biti poslana.\n\nNedostaje podatak 'To'"      ); return true; }
    //if(EmailTo                  .IsEmpty()) { ZXC.aim_emsg(MessageBoxIcon.Error, "Mail poruka neće biti poslana.\n\nNedostaje podatak 'To'"      ); return true; }
    //if(MessageSubject           .IsEmpty()) { ZXC.aim_emsg(MessageBoxIcon.Error, "Mail poruka neće biti poslana.\n\nNedostaje podatak 'Subject'" ); return true; }
    //if(MessageBody              .IsEmpty()) { ZXC.aim_emsg(MessageBoxIcon.Error, "Mail poruka neće biti poslana.\n\nNedostaje podatak 'Body'"    ); return true; }

      return false;
   }

   #endregion Methodz

#if NovijiVisualStudio
   void DisplayGlobalAddressListForStore()
   {
      Outlook.Folder currentFolder =
          Application.ActiveExplorer().CurrentFolder
          as Outlook.Folder;
      Outlook.Store currentStore = currentFolder.Store;
      if(currentStore.ExchangeStoreType !=
          Outlook.OlExchangeStoreType.olNotExchange)
      {
         Outlook.SelectNamesDialog snd =
             Application.Session.GetSelectNamesDialog();
         Outlook.AddressList addrList =
             GetGlobalAddressList(currentStore);
         if(addrList != null)
         {
            snd.InitialAddressList = addrList;
            snd.Display();
         }
      }
   }

   public Outlook.AddressList GetGlobalAddressList(Outlook.Store store)
   {
      string PR_EMSMDB_SECTION_UID =
          @"http://schemas.microsoft.com/mapi/proptag/0x3D150102";
      if(store == null)
      {
         throw new ArgumentNullException();
      }
      Outlook.PropertyAccessor oPAStore = store.PropertyAccessor;
      string storeUID = oPAStore.BinaryToString(
          oPAStore.GetProperty(PR_EMSMDB_SECTION_UID));
      foreach(Outlook.AddressList addrList
          in Application.Session.AddressLists)
      {
         Outlook.PropertyAccessor oPAAddrList =
             addrList.PropertyAccessor;
         string addrListUID = oPAAddrList.BinaryToString(
             oPAAddrList.GetProperty(PR_EMSMDB_SECTION_UID));
         // Return addrList if match on storeUID
         // and type is olExchangeGlobalAddressList.
         if(addrListUID == storeUID && addrList.AddressListType ==
             Outlook.OlAddressListType.olExchangeGlobalAddressList)
         {
            return addrList;
         }
      }
      return null;
   }

#endif

}

//  //MSDN SendAsync example: 
    //public class SimpleAsynchronousExample
    //{
    //    static bool mailSent = false;
    //    private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
    //    {
    //        // Get the unique identifier for this asynchronous operation.
    //         String token = (string) e.UserState;

    //        if (e.Cancelled)
    //        {
    //             Console.WriteLine("[{0}] Send canceled.", token);
    //        }
    //        if (e.Error != null)
    //        {
    //             Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
    //        } else
    //        {
    //            Console.WriteLine("Message sent.");
    //        }
    //        mailSent = true;
    //    }
    //    public static void Main(string[] args)
    //    {
    //        // Command line argument must the the SMTP host.
    //        SmtpClient client = new SmtpClient(args[0]);
    //        // Specify the e-mail sender.
    //        // Create a mailing address that includes a UTF8 character
    //        // in the display name.
    //        MailAddress from = new MailAddress("jane@contoso.com", 
    //           "Jane " + (char)0xD8+ " Clayton", 
    //        System.Text.Encoding.UTF8);
    //        // Set destinations for the e-mail message.
    //        MailAddress to = new MailAddress("ben@contoso.com");
    //        // Specify the message content.
    //        MailMessage message = new MailMessage(from, to);
    //        message.Body = "This is a test e-mail message sent by an application. ";
    //        // Include some non-ASCII characters in body and subject.
    //        string someArrows = new string(new char[] {'\u2190', '\u2191', '\u2192', '\u2193'});
    //        message.Body += Environment.NewLine + someArrows;
    //        message.BodyEncoding =  System.Text.Encoding.UTF8;
    //        message.Subject = "test message 1" + someArrows;
    //        message.SubjectEncoding = System.Text.Encoding.UTF8;
    //        // Set the method that is called back when the send operation ends.
    //        client.SendCompleted += new 
    //        SendCompletedEventHandler(SendCompletedCallback);
    //        // The userState can be any object that allows your callback 
    //        // method to identify this send operation.
    //        // For this example, the userToken is a string constant.
    //        string userState = "test message1";
    //        client.SendAsync(message, userState);
    //        Console.WriteLine("Sending message... press c to cancel mail. Press any other key to exit.");
    //        string answer = Console.ReadLine();
    //        // If the user canceled the send, and mail hasn't been sent yet,
    //        // then cancel the pending operation.
    //        if (answer.StartsWith("c") && mailSent == false)
    //        {
    //            client.SendAsyncCancel();
    //        }
    //        // Clean up.
    //        message.Dispose();
    //        Console.WriteLine("Goodbye.");
    //    }
    //}


//#if NeuspjeoPokusajSentItems

