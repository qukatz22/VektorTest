using System;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Xml;
using System.Xml.Serialization;

public sealed class VvMailClient
{
   #region Propertiz, Fieldz, defaults 

   private MailMessage MailMessage { get; set; }
   private SmtpClient  SmtpClient  { get; set; }

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

      this.MessageBody = String.Format("{0}\n\rProjekt/Year: {1}\n\r{2}\n\r~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n\r", 
         ZXC.TheVvForm.Text, 
         ZXC.CURR_prjkt_rec.ToString() + " - " + ZXC.projectYear,

         (ZXC.TheVvForm.TheVvTabPage != null && ZXC.TheVvForm.TheVvUC != null) ? ZXC.TheVvForm.TheVvUC.ToString() : "");

      this.MessageBody += messageBody;

      return this.SendMail_Normal(isNotSilent, ZXC.VektorEmailAddress);
   }

   public void SetVvSupportMailData(string emailFromDisplayName_addition)
   {
      this.EmailFromPasswd      = ZXC.SkyLabEmailPassword;
      this.MailHost             = ZXC.ViperMailHost      ;
      this.EmailFromAddress     = 
      this.EmailFromUserName    = ZXC.SkyLabEmailAddress ;

      this.EmailFromDisplayName = ZXC.VvDeploymentSite + emailFromDisplayName_addition;
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
      bool OK = true;

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

      return OK;
   }

   public static bool IsOutlookInstalled()
   {
      try
      {
         Outlook.Application outlookApplication;
         outlookApplication = new Outlook.Application();

       //var officeType = Type.GetTypeFromProgID("Outlook.Application");
       //if(officeType         == null)
         if(outlookApplication == null)
         {
            // Outlook is not installed.   
            return false;
         }
         else
         {
            // '14.0.0.7113' ... Ti je Outlook 2010 version
            // tu bi mozda trebalo provjeravati i verziju outlook-a 
            // ... ali .StartsWith() 

           // case "7.0": sVersion = "95";   break;
           // case "8.0": sVersion = "97";   break;
           // case "9.0": sVersion = "2000"; break;
           // case "10.0":sVersion = "2002"; break;
           // case "11.0":sVersion = "2003"; break;
           // case "12.0":sVersion = "2007"; break;
           // case "14.0":sVersion = "2010"; break;
           //default: sVersion = "Too Old!"; break;

            // Outlook is installed.      
            return true;
         }
      }
      catch(System.Exception ex)
      {
         return false;
      }
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
       //using(SmtpClient = new SmtpClient(MailHost, (PortNo.IsZero() ?  25 : PortNo))) // port 25 je default 
         using(SmtpClient = new SmtpClient(MailHost, (PortNo.IsZero() ? 587 : PortNo))) // port 25 je default 
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

      SmtpClient = new SmtpClient(MailHost, (PortNo.IsZero() ? 25 : PortNo));

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

public static class SmtpClientExtensions
{
   static System.IO.StreamWriter sw = null;
   static System.Net.Sockets.TcpClient tcpc = null;
   static System.Net.Security.SslStream ssl = null;
   static string path;
   static int bytes = -1;
   static byte[] buffer;
   static System.Text.StringBuilder sb = new System.Text.StringBuilder();
   static byte[] dummy;

   /// <summary>
   /// Communication with server
   /// </summary>
   /// <param name="command">The command beeing sent</param>
   private static void SendCommandAndReceiveResponse(string command)
   {
      try
      {
         if(command != "")
         {
            if(tcpc.Connected)
            {
               dummy = System.Text.Encoding.ASCII.GetBytes(command);
               ssl.Write(dummy, 0, dummy.Length);
            }
            else
            {
               throw new System.ApplicationException("TCP CONNECTION DISCONNECTED");
            }
         }
         ssl.Flush();

         buffer = new byte[2048];
         bytes = ssl.Read(buffer, 0, 2048);
         sb.Append(System.Text.Encoding.ASCII.GetString(buffer));

         sw.WriteLine(sb.ToString());
         sb = new System.Text.StringBuilder();
      }
      catch(System.Exception ex)
      {
         throw new System.ApplicationException(ex.Message);
      }
   }

   /// <summary>
   /// Saving a mail message before beeing sent by the SMTP client
   /// </summary>
   /// <param name="self">The caller</param>
   /// <param name="imapServer">The address of the IMAP server</param>
   /// <param name="imapPort">The port of the IMAP server</param>
   /// <param name="userName">The username to log on to the IMAP server</param>
   /// <param name="password">The password to log on to the IMAP server</param>
   /// <param name="sentFolderName">The name of the folder where the message will be saved</param>
   /// <param name="mailMessage">The message being saved</param>
   public static void SendAndSaveMessageToIMAP(this System.Net.Mail.SmtpClient self, System.Net.Mail.MailMessage mailMessage, string imapServer, int imapPort, string userName, string password, string sentFolderName)
   {
      try
      {
         path = System.Environment.CurrentDirectory + "\\emailresponse.txt";

         if(System.IO.File.Exists(path))
            System.IO.File.Delete(path);

         sw = new System.IO.StreamWriter(System.IO.File.Create(path));

         tcpc = new System.Net.Sockets.TcpClient(imapServer, imapPort);

         ssl = new System.Net.Security.SslStream(tcpc.GetStream());
         ssl.AuthenticateAsClient(imapServer);
         SendCommandAndReceiveResponse("");

         SendCommandAndReceiveResponse(string.Format("$ LOGIN {1} {2}  {0}", System.Environment.NewLine, userName, password));

         using(var m = mailMessage.RawMessage())
         {
            m.Position = 0;
            var sr = new System.IO.StreamReader(m);
            var myStr = sr.ReadToEnd();
            SendCommandAndReceiveResponse(string.Format("$ APPEND {1} (\\Seen) {{{2}}}{0}", System.Environment.NewLine, sentFolderName, myStr.Length));
            SendCommandAndReceiveResponse(string.Format("{1}{0}", System.Environment.NewLine, myStr));
         }
         SendCommandAndReceiveResponse(string.Format("$ LOGOUT{0}", System.Environment.NewLine));
      }
      catch(System.Exception ex)
      {
       //System.Diagnostics.Debug.WriteLine("error: " + ex.Message);
         ZXC.aim_emsg(MessageBoxIcon.Error, "SendAndSaveMessageToIMAP (writ IMAP folder) error:\n\n" + ex.Message);
      }
      finally
      {
         if(sw != null)
         {
            sw.Close();
            sw.Dispose();
         }
         if(ssl != null)
         {
            ssl.Close();
            ssl.Dispose();
         }
         if(tcpc != null)
         {
            tcpc.Close();
         }
      }

      self.Send(mailMessage);
   }
}

public static class MailMessageExtensions
{
   private static readonly System.Reflection.BindingFlags Flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
   private static readonly System.Type MailWriter = typeof(System.Net.Mail.SmtpClient).Assembly.GetType("System.Net.Mail.MailWriter");
   private static readonly System.Reflection.ConstructorInfo MailWriterConstructor = MailWriter.GetConstructor(Flags, null, new[] { typeof(System.IO.Stream) }, null);
   private static readonly System.Reflection.MethodInfo CloseMethod = MailWriter.GetMethod("Close", Flags);
   private static readonly System.Reflection.MethodInfo SendMethod = typeof(System.Net.Mail.MailMessage).GetMethod("Send", Flags);

   /// <summary>
   /// A little hack to determine the number of parameters that we
   /// need to pass to the SaveMethod.
   /// </summary>
   private static readonly bool IsRunningInDotNetFourPointFive = SendMethod.GetParameters().Length == 3;

   /// <summary>
   /// The raw contents of this MailMessage as a MemoryStream.
   /// </summary>
   /// <param name="self">The caller.</param>
   /// <returns>A MemoryStream with the raw contents of this MailMessage.</returns>
   public static System.IO.MemoryStream RawMessage(this System.Net.Mail.MailMessage self)
   {
      var result = new System.IO.MemoryStream();
      var mailWriter = MailWriterConstructor.Invoke(new object[] { result });
      SendMethod.Invoke(self, Flags, null, IsRunningInDotNetFourPointFive ? new[] { mailWriter, true, true } : new[] { mailWriter, true }, null);
      result = new System.IO.MemoryStream(result.ToArray());
      CloseMethod.Invoke(mailWriter, Flags, null, new object[] { }, null);
      return result;
   }
}

public static class Vv_Http_Web_request
{
   #region SEND_eRacun_MER_WebService

   internal static VvMER_Json_SEND_Response_Data SEND_eRacun_MER_WebService(string xmlString, string fullPath_XML_FileName)
   {
      //string webAddress = @"http://demo.moj-eracun.hr/hr/";
      //string webAddress = @"https://demo.moj-eracun.hr/apis/v2/send";

      //string webAddress_SEND        = @"https://www.moj-eracun.hr/apis/v2/send"       ;
      string webAddress_QueryOutbox = @"https://www.moj-eracun.hr/apis/v2/queryOutbox";

      // Web adresa Vam je ispravna za demo okruženje: https://demo.moj-eracun.hr/apis/v2/send
      // Produkcijska adresa je : https://www.moj-eracun.hr/apis/v2/send

      // ID: 8633
      // Lozinka: T22zsEY
      // SoftwareID: Test-001


      //int    username   = 6072         ;
      //string password   = "buV733eX"   ;
      //int    username   = 8633         ;
      //string password   = "T22zsEY"    ;
      //string companyId  = "60042587515";
      //string companyId  = "04192765979";
      //string softwareId = "Test-001"   ;

      string webAddress_SEND = @"https://www.moj-eracun.hr/apis/v2/send"; // PRODUKCIJA 
    //int username   = ZXC.ValOrZero_Int(ZXC.CURR_prjkt_rec.SkyVvDomena); // PRODUKCIJA 
    //string password   = ZXC.CURR_prjkt_rec.SkyPasswordDecrypted       ; // PRODUKCIJA 
    //string companyId  = ZXC.CURR_prjkt_rec.Oib                        ; // PRODUKCIJA 
    //string companyBu  = ""                                            ; // PRODUKCIJA 
    //string softwareId = "Vektor-001"                                  ; // PRODUKCIJA 

//#if DEBUG
//         webAddress_SEND = @"https://demo.moj-eracun.hr/apis/v2/send";
//
//         username = 8633        ;
//         password   = "T22zsEY" ;
//         softwareId = "Test-001";
//
//         VvMER_Json_SEND_Request_Data json_SEND_Request_Data = new VvMER_Json_SEND_Request_Data(username, password, companyId, companyBu, softwareId, xmlString); // DEMO testiranje 
//#endif

      VvMER_Json_SEND_Request_Data json_SEND_Request_Data = new VvMER_Json_SEND_Request_Data(xmlString); // produkcija 

      string jsonString = JsonConvert.SerializeObject(json_SEND_Request_Data, Newtonsoft.Json.Formatting.Indented);

      VvMER_Json_SEND_Response_Data deserializedResponseData = Vv_Http_Web_request.Vv_PostJson_SEND_eRacun(webAddress_SEND, jsonString, fullPath_XML_FileName);

      return deserializedResponseData;
   }

   internal static VvMER_Json_Status_Data Get_eRacun_OneSingleSTATUS_MER_WebService(int electronicID)
   {
      string webAddress_QueryOutbox = @"https://www.moj-eracun.hr/apis/v2/queryOutbox"; // PRODUKCIJA 
    //int username   = ZXC.ValOrZero_Int(ZXC.CURR_prjkt_rec.SkyVvDomena)              ; // PRODUKCIJA 
    //string password   = ZXC.CURR_prjkt_rec.SkyPasswordDecrypted                     ; // PRODUKCIJA 
    //string companyId  = ZXC.CURR_prjkt_rec.Oib                                      ; // PRODUKCIJA 
    //string companyBu  = ""                                                          ; // PRODUKCIJA 
    //string softwareId = "Vektor-001"                                                ; // PRODUKCIJA 

//#if DEBUG
//         webAddress_SEND = @"https://demo.moj-eracun.hr/apis/v2/send";
//
//         username = 8633        ;
//         password   = "T22zsEY" ;
//         softwareId = "Test-001";
//
//         VvMER_Json_SEND_Request_Data json_SEND_Request_Data = new VvMER_Json_SEND_Request_Data(username, password, companyId, companyBu, softwareId, xmlString); // DEMO testiranje 
//#endif

      VvMER_Json_SEND_Request_Data json_SEND_Request_Data = new VvMER_Json_SEND_Request_Data(electronicID); // constructor za STATUS jednog racuna (electronicID-a) 

      JsonSerializerSettings settings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore };

      string jsonString = JsonConvert.SerializeObject(json_SEND_Request_Data, Newtonsoft.Json.Formatting.Indented, settings);

      VvMER_Json_Status_Data deserializedResponseData = Vv_Http_Web_request.Vv_PostJson_QueryOutbox_OneSingleStatus(webAddress_QueryOutbox, jsonString);

      return deserializedResponseData;
   }

   internal static List<VvMER_Json_Status_Data> Get_eRacun_STATUS_List_MER_WebService(DateTime dateOD, DateTime dateDO)
   {
      string webAddress_QueryOutbox = @"https://www.moj-eracun.hr/apis/v2/queryOutbox"; // PRODUKCIJA 
    //int username   = ZXC.ValOrZero_Int(ZXC.CURR_prjkt_rec.SkyVvDomena)              ; // PRODUKCIJA 
    //string password   = ZXC.CURR_prjkt_rec.SkyPasswordDecrypted                     ; // PRODUKCIJA 
    //string companyId  = ZXC.CURR_prjkt_rec.Oib                                      ; // PRODUKCIJA 
    //string companyBu  = ""                                                          ; // PRODUKCIJA 
    //string softwareId = "Vektor-001"                                                ; // PRODUKCIJA 

//#if DEBUG
//         webAddress_SEND = @"https://demo.moj-eracun.hr/apis/v2/send";
//
//         username = 8633        ;
//         password   = "T22zsEY" ;
//         softwareId = "Test-001";
//
//         VvMER_Json_SEND_Request_Data json_SEND_Request_Data = new VvMER_Json_SEND_Request_Data(username, password, companyId, companyBu, softwareId, xmlString); // DEMO testiranje 
//#endif

      VvMER_Json_SEND_Request_Data json_SEND_Request_Data = new VvMER_Json_SEND_Request_Data(dateOD, dateDO); // constructor za Listu STATUSa racuna (dateOD, dateDO) 

      JsonSerializerSettings settings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore };

      string jsonString = JsonConvert.SerializeObject(json_SEND_Request_Data, Newtonsoft.Json.Formatting.Indented, settings);

      return Vv_Http_Web_request.Vv_PostJson_QueryOutbox_StatusList(webAddress_QueryOutbox, jsonString);
   }

   #endregion SEND_eRacun_MER_WebService


   // https://stackoverflow.com/questions/36047410/c-sharp-json-post-using-httpwebrequest/36048030

   //https://www.moj-eracun.hr/hr/Manual/Stable/Api

   //public static void Post(/*string[] args*/) // was Main in some stackoverflow example
   //{
   //   try
   //   {
   //      string webAddr = "http://gurujsonrpc.appspot.com/guru";
   //
   //      HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
   //      httpWebRequest.ContentType = "application/json; charset=utf-8";
   //      httpWebRequest.Method = "POST";
   //
   //      using(StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
   //      {
   //         string jsonString = "{ \"method\" : \"guru.test\", \"params\" : [ \"Guru\" ], \"id\" : 123 }";
   //
   //         streamWriter.Write(jsonString);
   //         streamWriter.Flush();
   //      }
   //      HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
   //      using(var streamReader = new StreamReader(httpResponse.GetResponseStream()))
   //      {
   //         string responseText = streamReader.ReadToEnd();
   //       //Console.WriteLine(responseText);
   //         ZXC.aim_emsg(responseText);
   //
   //         //Now you have your response.
   //         //or false depending on information in the response     
   //      }
   //   }
   //   catch(WebException ex)
   //   {
   //      //Console.WriteLine(ex.Message);
   //      ZXC.aim_emsg(ex.Message);
   //   }
   //}


   //- Vv_PostJson treba napraviti u vise ver.
   //
   //   a one vracaju:
   //	- 1. VvMER_Json_SEND_Response_Data
   //	- 1. VvMER_Json_Status_Data
   //	- List<VvMER_Json_Status_Data>

   public static HttpWebResponse VvMER_SendRequest_GetResponse(string webAddress, string jsonString)
   {
      HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddress);
      httpWebRequest.ContentType = "application/json; charset=utf-8";
      httpWebRequest.Method = "POST";

      using(StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
      {
         streamWriter.Write(jsonString);
         streamWriter.Flush();
      }

      HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

      return httpResponse;
   }

   public static VvMER_Json_SEND_Response_Data Vv_PostJson_SEND_eRacun(string webAddress, string jsonString, string fullPath_eRacun_XML_FileName)
   {
      VvMER_Json_SEND_Response_Data deserializedResponseData = null;

      string fullPath_send_response_XML_FileName = fullPath_eRacun_XML_FileName.Replace(".xml", "_RES.xml");

      try
      {
         HttpWebResponse httpResponse = VvMER_SendRequest_GetResponse(webAddress, jsonString);

         using(StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
         {
            string responseJson = streamReader.ReadToEnd();

            if(responseJson.NotEmpty())
            {
               try
               {
                  deserializedResponseData = VvMER_Json_SEND_Response_Deserializator(responseJson);

                  if(deserializedResponseData.ElectronicId.IsZero()) throw new Exception("Zero ElectronicID in response JSON!");
                  if(deserializedResponseData == null              ) throw new Exception("Deserialized Response Data is null!");

                  if(fullPath_send_response_XML_FileName.NotEmpty())
                  {
                     deserializedResponseData.SaveToFile(fullPath_send_response_XML_FileName); // !!! Voila! 
                  }

               }
               catch(Exception ex2) // nije uspio Deserializirati, odi preko JsonTextReader 
               {
                  List<string> responseMsgList = new List<string>();

                  deserializedResponseData = new VvMER_Json_SEND_Response_Data { StatusName = "Err " };

                  int errMsgRowIdx = 0;

                  JsonTextReader reader = new JsonTextReader(new StringReader(responseJson));
                  while(reader.Read())
                  {
                     if(reader.Value != null)
                     {
                        errMsgRowIdx++;

                        if(errMsgRowIdx == 1) deserializedResponseData.Error_PropertyName  = reader.Value.ToString();
                        if(errMsgRowIdx == 3) deserializedResponseData.Error_PropertyValue = reader.Value.ToString();
                        if(errMsgRowIdx == 5) deserializedResponseData.Error_Message       = reader.Value.ToString();

                        if(errMsgRowIdx == 1 || errMsgRowIdx == 3 || errMsgRowIdx == 5)
                        {
                         //responseMsgList.Add(String.Format("{0}, Value: {1}", reader.TokenType, reader.Value          ));
                         //responseMsgList.Add(String.Format("{0}: {1}"       , reader.TokenType, reader.Value          ));
                           responseMsgList.Add(                                                   reader.Value.ToString());
                        }
                     }
                     else
                     {
                      //responseMsgList.Add(String.Format("{0}", reader.TokenType));
                     }
                  }

                  ZXC.aim_emsg_List("Response Messages from JsonTextReader", responseMsgList);

                  deserializedResponseData.StatusName += deserializedResponseData.Error_PropertyName;

                  if(fullPath_send_response_XML_FileName.NotEmpty())
                  {
                     deserializedResponseData.SaveToFile(fullPath_send_response_XML_FileName); // !!! Voila! 
                  }
               }

               //Now you have your response.
               //or false depending on information in the response     

            } // if(responseJson.NotEmpty())
            else
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "JSON response is empty!");
               deserializedResponseData = new VvMER_Json_SEND_Response_Data { StatusName = "JSON empty" };

               if(fullPath_send_response_XML_FileName.NotEmpty())
               {
                  deserializedResponseData.SaveToFile(fullPath_send_response_XML_FileName); // !!! Voila! 
               }
            }

         } // using(var streamReader = new StreamReader(httpResponse.GetResponseStream())) 

      } // try 

      catch(WebException ex)
      {
         ZXC.aim_emsg(ex.Message);
      }

      return deserializedResponseData;
   }

   public static VvMER_Json_Status_Data Vv_PostJson_QueryOutbox_OneSingleStatus(string webAddress, string jsonString)
   {
      VvMER_Json_Status_Data deserializedResponseData = null;

      try
      {       
         HttpWebResponse httpResponse = VvMER_SendRequest_GetResponse(webAddress, jsonString);
      
         using(StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
         {
            string responseJson = streamReader.ReadToEnd();
      
            if(responseJson.NotEmpty())
            {
               try
               {
//========================

// makni ovo odavdje. ovako ces raditi 'Vv_PostJson_Receive_OneSingleInvoice' .. kada dode na red. 

//      XmlSerializer ser = new XmlSerializer(typeof(EN16931.UBL.InvoiceType));
//      EN16931.UBL.InvoiceType invoice;
//      try
//      {
//         using (var reader = new StringReader(responseJson))
//         {
//            invoice = ser.Deserialize(reader) as EN16931.UBL.InvoiceType;
//         }
//
//      }
//      catch (Exception ex)
//      {
//         ZXC.aim_emsg_VvException(ex);
//      }
//
//========================
                  deserializedResponseData = VvMER_Json_SEND_OneSingleStatus_Deserializator(responseJson);
      
                  if(deserializedResponseData.ElectronicId.IsZero()) throw new Exception("Zero ElectronicID in response JSON!");
                  if(deserializedResponseData == null              ) throw new Exception("Deserialized Response Data is null!");
               }
               catch(Exception ex2) // nije uspio Deserializirati, odi preko JsonTextReader 
               {
                  List<string> responseMsgList = new List<string>();
      
                  deserializedResponseData = new VvMER_Json_Status_Data { StatusName = "Err " };
      
                  int errMsgRowIdx = 0;
      
                  JsonTextReader reader = new JsonTextReader(new StringReader(responseJson));
                  while(reader.Read())
                  {
                     if(reader.Value != null)
                     {
                        errMsgRowIdx++;
      
                        if(errMsgRowIdx == 1) deserializedResponseData.Error_PropertyName  = reader.Value.ToString();
                        if(errMsgRowIdx == 3) deserializedResponseData.Error_PropertyValue = reader.Value.ToString();
                        if(errMsgRowIdx == 5) deserializedResponseData.Error_Message       = reader.Value.ToString();
      
                        if(errMsgRowIdx == 1 || errMsgRowIdx == 3 || errMsgRowIdx == 5)
                        {
                         //responseMsgList.Add(String.Format("{0}, Value: {1}", reader.TokenType, reader.Value          ));
                         //responseMsgList.Add(String.Format("{0}: {1}"       , reader.TokenType, reader.Value          ));
                           responseMsgList.Add(                                                   reader.Value.ToString());
                        }
                     }
                     else
                     {
                      //responseMsgList.Add(String.Format("{0}", reader.TokenType));
                     }
                  }
      
                  ZXC.aim_emsg_List("Response Messages from JsonTextReader", responseMsgList);
      
                  deserializedResponseData.StatusName += deserializedResponseData.Error_PropertyName;
               }
      
               //Now you have your response.
               //or false depending on information in the response     
      
            } // if(responseJson.NotEmpty())
            else
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "JSON response is empty!");
               deserializedResponseData = new VvMER_Json_Status_Data { StatusName = "JSON empty" };
            }
      
         } // using(var streamReader = new StreamReader(httpResponse.GetResponseStream())) 
      
      } // try 
      
      catch(WebException ex)
      {
         ZXC.aim_emsg(ex.Message);
      }

      return deserializedResponseData;
   }

   public static List<VvMER_Json_Status_Data> Vv_PostJson_QueryOutbox_StatusList(string webAddress, string jsonString)
   {
      List<VvMER_Json_Status_Data> deserializedResponseDataList = null;

      try
      {       
         HttpWebResponse httpResponse = VvMER_SendRequest_GetResponse(webAddress, jsonString);
      
         using(StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
         {
            string responseJson = streamReader.ReadToEnd();
      
            if(responseJson.NotEmpty())
            {
               try
               {
                  deserializedResponseDataList = VvMER_Json_SEND_StatusList_Deserializator(responseJson);
      
                //if(deserializedResponseDataList.ElectronicId.IsZero()) throw new Exception("Zero ElectronicID in response JSON!");
                  if(deserializedResponseDataList == null              ) throw new Exception("Deserialized Response Data is null!");
               }
               catch(Exception ex2) // nije uspio Deserializirati, odi preko JsonTextReader 
               {
                  List<string> responseMsgList = new List<string>();
      
                  deserializedResponseDataList = new List<VvMER_Json_Status_Data> { new VvMER_Json_Status_Data { StatusName = "Err " } };
      
                  int errMsgRowIdx = 0;
                  int listIdx = 0;

                  JsonTextReader reader = new JsonTextReader(new StringReader(responseJson));
                  while(reader.Read())
                  {
                     if(reader.Value != null)
                     {
                        errMsgRowIdx++;
      
                        if(errMsgRowIdx == 1) deserializedResponseDataList[0].Error_PropertyName  = reader.Value.ToString();
                        if(errMsgRowIdx == 3) deserializedResponseDataList[0].Error_PropertyValue = reader.Value.ToString();
                        if(errMsgRowIdx == 5) deserializedResponseDataList[0].Error_Message       = reader.Value.ToString();
      
                        if(errMsgRowIdx == 1 || errMsgRowIdx == 3 || errMsgRowIdx == 5)
                        {
                         //responseMsgList.Add(String.Format("{0}, Value: {1}", reader.TokenType, reader.Value          ));
                         //responseMsgList.Add(String.Format("{0}: {1}"       , reader.TokenType, reader.Value          ));
                           responseMsgList.Add(                                                   reader.Value.ToString());
                        }
                     }
                     else
                     {
                      //responseMsgList.Add(String.Format("{0}", reader.TokenType));
                     }
                  }
      
                  ZXC.aim_emsg_List("Response Messages from JsonTextReader", responseMsgList);
      
                  deserializedResponseDataList[listIdx].StatusName += deserializedResponseDataList[listIdx].Error_PropertyName;
                  listIdx++;
               }
      
               //Now you have your response.
               //or false depending on information in the response     
      
            } // if(responseJson.NotEmpty())
            else
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "JSON response is empty!");
               deserializedResponseDataList[0] = new VvMER_Json_Status_Data { StatusName = "JSON empty" };
            }
      
         } // using(var streamReader = new StreamReader(httpResponse.GetResponseStream())) 
      
      } // try 
      
      catch(WebException ex)
      {
         ZXC.aim_emsg(ex.Message);
      }

      return deserializedResponseDataList;
   }

#if Vv_PostJson_OLD_First_Orig

   public static VvMER_Json_SEND_Response_Data Vv_PostJson_OLD_First_Orig(string webAddress, string jsonString, string fullPath_eRacun_XML_FileName)
   {
      bool isStatusOnly = fullPath_eRacun_XML_FileName.IsEmpty();

      VvMER_Json_SEND_Response_Data deserializedResponseData = null;

      string fullPath_send_response_XML_FileName = fullPath_eRacun_XML_FileName.Replace(".xml", "_RES.xml");

      try
      {
         List<string> responseMsgList = new List<string>();

         HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddress);
         httpWebRequest.ContentType = "application/json; charset=utf-8";
         httpWebRequest.Method = "POST";

         using(StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
         {
            streamWriter.Write(jsonString);
            streamWriter.Flush();
         }

         HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
         using(var streamReader = new StreamReader(httpResponse.GetResponseStream()))
         {
            string responseJson = streamReader.ReadToEnd();

            //ZXC.aim_emsg(responseJson);

            if(responseJson.NotEmpty())
            {
               try
               {
                  if(isStatusOnly) deserializedResponseData = VvMER_Json_SEND_Statuses_Deserializator(responseJson); // STATUS ONLY    
                  else             deserializedResponseData = VvMER_Json_SEND_Response_Deserializator(responseJson); // SENDamo eRacun 

                  if(deserializedResponseData.ElectronicId.IsZero()) throw new Exception("Zero ElectronicID in response JSON!");
                  if(deserializedResponseData == null              ) throw new Exception("Deserialized Response Data is null!");

                  if(fullPath_send_response_XML_FileName.NotEmpty())
                  {
                     deserializedResponseData.SaveToFile(fullPath_send_response_XML_FileName); // !!! Voila! 
                  }

               }
               catch(Exception ex)
               {
                  //ZXC.aim_emsg(MessageBoxIcon.Error, ex.Message);

                  try // probaj sada shvatiti iz Dictionary-a 
                  {
                     Dictionary<string, string> responseDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseJson);
                   //Dictionary<string, object> responseDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJson);
                     foreach(var item in responseDictionary)
                     {
                        responseMsgList.Add(item.Key + ": " + item.Value);
                     }
                     ZXC.aim_emsg_List("Response Messages from Dictionary", responseMsgList);

                     deserializedResponseData = new VvMER_Json_SEND_Response_Data { StatusName = "Err "};
                  }
                  catch(Exception ex2) // nije uspio parsirati Dictionary, odi preko JsonTextReader 
                  {
                     //ZXC.aim_emsg(MessageBoxIcon.Error, ex2.Message);

                     deserializedResponseData = new VvMER_Json_SEND_Response_Data { StatusName = "Err " };

                     int errMsgRowIdx = 0;

                     JsonTextReader reader = new JsonTextReader(new StringReader(responseJson));
                     while(reader.Read())
                     {
                        if(reader.Value != null)
                        {
                           errMsgRowIdx++;

                           if(errMsgRowIdx == 1) deserializedResponseData.Error_PropertyName  = reader.Value.ToString();
                           if(errMsgRowIdx == 3) deserializedResponseData.Error_PropertyValue = reader.Value.ToString();
                           if(errMsgRowIdx == 5) deserializedResponseData.Error_Message       = reader.Value.ToString();

                           if(errMsgRowIdx == 1 || errMsgRowIdx == 3 || errMsgRowIdx == 5)
                           {
                            //responseMsgList.Add(String.Format("{0}, Value: {1}", reader.TokenType, reader.Value          ));
                            //responseMsgList.Add(String.Format("{0}: {1}"       , reader.TokenType, reader.Value          ));
                              responseMsgList.Add(                                                   reader.Value.ToString());
                           }
                        }
                        else
                        {
                         //responseMsgList.Add(String.Format("{0}", reader.TokenType));
                        }
                     }

                     ZXC.aim_emsg_List("Response Messages from JsonTextReader", responseMsgList);

                     deserializedResponseData.StatusName += deserializedResponseData.Error_PropertyName;

                     if(fullPath_send_response_XML_FileName.NotEmpty())
                     {
                        deserializedResponseData.SaveToFile(fullPath_send_response_XML_FileName); // !!! Voila! 
                     }
                  }

               }

               //Now you have your response.
               //or false depending on information in the response     

            } // if(responseJson.NotEmpty())
            else
            {
               ZXC.aim_emsg(MessageBoxIcon.Error, "JSON response is empty!");
               deserializedResponseData = new VvMER_Json_SEND_Response_Data { StatusName = "JSON empty" };

               if(fullPath_send_response_XML_FileName.NotEmpty())
               {
                  deserializedResponseData.SaveToFile(fullPath_send_response_XML_FileName); // !!! Voila! 
               }
            }

         } // using(var streamReader = new StreamReader(httpResponse.GetResponseStream())) 

      } // try 

      catch(WebException ex)
      {
         ZXC.aim_emsg(ex.Message);
         //return ex.Message;
      }

      return deserializedResponseData;
   }

#endif

   public static VvMER_Json_SEND_Response_Data VvMER_Json_SEND_Response_Deserializator(string jsonString)
   {
      VvMER_Json_SEND_Response_Data deserializedResponseData = JsonConvert.DeserializeObject<VvMER_Json_SEND_Response_Data>(jsonString);

      return deserializedResponseData;
   }

   public static VvMER_Json_Status_Data       VvMER_Json_SEND_OneSingleStatus_Deserializator(string jsonString)
   {
      VvMER_Json_Status_Data deserializedResponseData = null;

      List<VvMER_Json_Status_Data> Statuses  = JsonConvert.DeserializeObject<List<VvMER_Json_Status_Data>>(jsonString);

      if(Statuses.NotEmpty())
      {
         deserializedResponseData = Statuses[0]/*.ThisSTATUS_as_VvMER_Json_SEND_Response_Data*/;
      }

      return deserializedResponseData;
   }

   public static List<VvMER_Json_Status_Data> VvMER_Json_SEND_StatusList_Deserializator(string jsonString)
   {
      return JsonConvert.DeserializeObject<List<VvMER_Json_Status_Data>>(jsonString);
   }

}

//#endif

public class VvMER_Json_SEND_Request_Data
{
   // "\"Username\" : "     + username   + " ,"   +
   // "\"Password\" : \""   + password   + "\" ," +
   // "\"CompanyId\" : \""  + companyId  + "\" ," +
   // "\"CompanyBu\" : \""  + companyBu  + "\" ," +
   // "\"SoftwareId\" : \"" + softwareId + "\" ," +
   // "\"File\" : \""       + xmlString  + "\" ," +

   public int    Username   { get; set; }
   public string Password   { get; set; }
   public string CompanyId  { get; set; }
   public string CompanyBu  { get; set; }
   public string SoftwareId { get; set; }
   public string File       { get; set; }

   // za testiranje, pa sa test parametrima 
   public VvMER_Json_SEND_Request_Data(int username, string password, string companyId, string companyBu, string softwareId, string xmlString)
   {
      this.Username   = username  ;
      this.Password   = password  ;
      this.CompanyId  = companyId ;
      this.CompanyBu  = companyBu ;
      this.SoftwareId = softwareId;
      this.File       =  xmlString;
   }

   public VvMER_Json_SEND_Request_Data(string xmlString) // za slanje jednog eRacuna 
   {
      this.Username   = ZXC.ValOrZero_Int(ZXC.CURR_prjkt_rec.SkyVvDomena);
      this.Password   = ZXC.CURR_prjkt_rec.SkyPasswordDecrypted          ;
      this.CompanyId  = ZXC.CURR_prjkt_rec.Oib                           ;
      this.CompanyBu  = ""                                               ;
      this.SoftwareId = "Vektor-001"                                     ;
      this.File       = xmlString                                        ;
   }

   // Query Inbox / Outbox Additions: 

   public int      ElectronicId { get; set; }
   public int      StatusId     { get; set; }
   public DateTime From         { get; set; } // DeteOD 
   public DateTime To           { get; set; } // DateDO 

   public VvMER_Json_SEND_Request_Data(int electronicId) // za jedan racun 
   {
      this.Username    = ZXC.ValOrZero_Int(ZXC.CURR_prjkt_rec.SkyVvDomena);
      this.Password    = ZXC.CURR_prjkt_rec.SkyPasswordDecrypted          ;
      this.CompanyId   = ZXC.CURR_prjkt_rec.Oib                           ;
      this.CompanyBu   = ""                                               ;
      this.SoftwareId  = "Vektor-001"                                     ;
      this.ElectronicId = electronicId                                    ;
   }

   public VvMER_Json_SEND_Request_Data(DateTime dateOD, DateTime dateDO) // za report 
   {
      this.Username    = ZXC.ValOrZero_Int(ZXC.CURR_prjkt_rec.SkyVvDomena);
      this.Password    = ZXC.CURR_prjkt_rec.SkyPasswordDecrypted          ;
      this.CompanyId   = ZXC.CURR_prjkt_rec.Oib                           ;
      this.CompanyBu   = ""                                               ;
      this.SoftwareId  = "Vektor-001"                                     ;
      this.From        = dateOD                                           ;
      this.To          = dateDO                                           ;
   }

}

public class VvMER_Json_SEND_Response_Data // Dis uan iz olso Serializable / Deserializable 
{
   // "ElectronicId": 394167,
   // "DocumentNr": "20156256",
   // "DocumentTypeId": 1,
   // "DocumentTypeName": "Račun",
   // "StatusId": 30,
   // "StatusName": "Sent",
   // "RecipientBusinessNumber": "99999999927",
   // "RecipientBusinessUnit": "",
   // "RecipientBusinessName": "Test Klising d.o.o.",
   // "Created": "2016-04-18T08:23:08.5879877+02:00",
   // "Sent": "2016-04-18T08:23:09.6730491+02:00",
   // "Modified": "2016-04-18T08:23:09.6840519+02:00",
   // "Delivered": null

   public int    ElectronicId            { get; set; }
   public string DocumentNr              { get; set; }
   public int    DocumentTypeId          { get; set; }
   public string DocumentTypeName        { get; set; }
   public int    StatusId                { get; set; }
   public string StatusName              { get; set; }
   public string RecipientBusinessNumber { get; set; }
   public string RecipientBusinessUnit   { get; set; }
   public string RecipientBusinessName   { get; set; }
   public string Created                 { get; set; }
   public string Sent                    { get; set; }
   public string Modified                { get; set; }
   public bool?  Delivered               { get; set; }

   public string Error_PropertyName  { get; set; }
   public string Error_PropertyValue { get; set; }
   public string Error_Message       { get; set; }

   #region Serialize/Deserialize

   private static System.Xml.Serialization.XmlSerializer serializer;
   private static System.Xml.Serialization.XmlSerializer Serializer
   {
      get
      {
         if((serializer == null))
         {
            serializer = new System.Xml.Serialization.XmlSerializer(typeof(VvMER_Json_SEND_Response_Data));
         }
         return serializer;
      }
   }

   /// <summary>
   /// Serializes current sObrazacURA object into an XML sObrazacURA
   /// </summary>
   /// <returns>string XML value</returns>
   public virtual string Serialize(System.Text.Encoding encoding)
   {
      System.IO.StreamReader streamReader = null;
      System.IO.MemoryStream memoryStream = null;
      string xmlString = "";

      try
      {
         System.Xml.XmlWriterSettings xmlWriterSettings = new System.Xml.XmlWriterSettings()
         {
            Encoding = encoding,
            Indent = true, // byQ 
            IndentChars = "   ", // byQ 
         };

         memoryStream = new System.IO.MemoryStream();

         System.Xml.XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
         Serializer.Serialize(xmlWriter, this);
         memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
         streamReader = new System.IO.StreamReader(memoryStream);
         xmlString = streamReader.ReadToEnd();

         return xmlString;
      }
      finally
      {
         if((streamReader != null))
         {
            streamReader.Dispose();
         }
         if((memoryStream != null))
         {
            memoryStream.Dispose();
         }
      }
   }

   public virtual string Serialize()
   {
      return Serialize(/*Encoding.UTF8*/ ZXC.VvUTF8Encoding_noBOM);
   }

   /// <summary>
   /// Deserializes workflow markup into an sObrazacURA object
   /// </summary>
   /// <param name="xml">string workflow markup to deserialize</param>
   /// <param name="obj">Output sObrazacURA object</param>
   /// <param name="exception">output Exception value if deserialize failed</param>
   /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
   public static bool Deserialize(string xml, out VvMER_Json_SEND_Response_Data obj, out System.Exception exception)
   {
      exception = null;
      obj = default(VvMER_Json_SEND_Response_Data);
      try
      {
         obj = Deserialize(xml);
         return true;
      }
      catch(System.Exception ex)
      {
         exception = ex;
         return false;
      }
   }

   public static bool Deserialize(string xml, out VvMER_Json_SEND_Response_Data obj)
   {
      System.Exception exception = null;
      return Deserialize(xml, out obj, out exception);
   }

   public static VvMER_Json_SEND_Response_Data Deserialize(string xml)
   {
      System.IO.StringReader stringReader = null;
      try
      {
         stringReader = new System.IO.StringReader(xml);
         return ((VvMER_Json_SEND_Response_Data)(Serializer.Deserialize(System.Xml.XmlReader.Create(stringReader))));
      }
      finally
      {
         if((stringReader != null))
         {
            stringReader.Dispose();
         }
      }
   }

   /// <summary>
   /// Serializes current sObrazacURA object into file
   /// </summary>
   /// <param name="fileName">full path of outupt xml file</param>
   /// <param name="exception">output Exception value if failed</param>
   /// <returns>true if can serialize and save into file; otherwise, false</returns>
   public virtual string SaveToFile(string fileName, System.Text.Encoding encoding, out System.Exception exception)
   {
      exception = null;
      try
      {
         return SaveToFile(fileName, encoding);
         //return true;
      }
      catch(System.Exception e)
      {
         exception = e;
         return "";
      }
   }

   public virtual string SaveToFile(string fileName, out System.Exception exception)
   {
      return SaveToFile(fileName, /*Encoding.UTF8*/ ZXC.VvUTF8Encoding_noBOM, out exception);
   }

   public virtual string SaveToFile(string fileName)
   {
      return SaveToFile(fileName, /*Encoding.UTF8*/ ZXC.VvUTF8Encoding_noBOM);
   }

   public virtual string SaveToFile(string fileName, System.Text.Encoding encoding)
   {
      string xmlString = "";

      System.IO.StreamWriter streamWriter = null;
      try
      {
         xmlString = Serialize(encoding);
         streamWriter = new System.IO.StreamWriter(fileName, false, /*Encoding.UTF8*/ ZXC.VvUTF8Encoding_noBOM);
         streamWriter.WriteLine(xmlString);
         streamWriter.Close();
      }
      finally
      {
         if((streamWriter != null))
         {
            streamWriter.Dispose();
         }
      }

      return xmlString;
   }

   public bool VvSaveToFile(string xmlString, string fileName, System.Text.Encoding encoding)
   {
      System.IO.StreamWriter streamWriter = null;

      bool isOK = true;
      try
      {
         streamWriter = new System.IO.StreamWriter(fileName, false, encoding /*Encoding.UTF8*/ /*ZXC.VvUTF8Encoding_noBOM*/);
         streamWriter.WriteLine(xmlString);
         streamWriter.Close();
      }
      catch(Exception)
      {
         isOK = false;
      }
      finally
      {
         if((streamWriter != null))
         {
            streamWriter.Dispose();
         }
      }

      return isOK;
   }

   /// <summary>
   /// Deserializes xml markup from file into an sObrazacURA object
   /// </summary>
   /// <param name="fileName">string xml file to load and deserialize</param>
   /// <param name="the_eRacun">Output sObrazacURA object</param>
   /// <param name="exception">output Exception value if deserialize failed</param>
   /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
   public static bool LoadFromFile(string fileName, System.Text.Encoding encoding, out VvMER_Json_SEND_Response_Data the_eRacun, out System.Exception exception)
   {
      exception = null;
      the_eRacun = default(VvMER_Json_SEND_Response_Data);
      try
      {
         the_eRacun = LoadFromFile(fileName, encoding);
         return true;
      }
      catch(System.Exception ex)
      {
         exception = ex;
         return false;
      }
   }

   public static bool LoadFromFile(string fileName, out VvMER_Json_SEND_Response_Data the_eRacun, out System.Exception exception)
   {
      return LoadFromFile(fileName, /*Encoding.UTF8*/ ZXC.VvUTF8Encoding_noBOM, out the_eRacun, out exception);
   }

   public static bool LoadFromFile(string fileName, out VvMER_Json_SEND_Response_Data the_eRacun)
   {
      System.Exception exception = null;
      return LoadFromFile(fileName, out the_eRacun, out exception);
   }

   //public static sObrazacURA LoadFromFile(string fileName)
   //{
   //   return LoadFromFile(fileName, /*Encoding.UTF8*/ ZXC.VvUTF8Encoding_noBOM);
   //}

   public static VvMER_Json_SEND_Response_Data LoadFromFile(string fileName, System.Text.Encoding encoding)
   {
      System.IO.FileStream file = null;
      System.IO.StreamReader sr = null;
      try
      {
         file = new System.IO.FileStream(fileName, FileMode.Open, FileAccess.Read);
         sr = new System.IO.StreamReader(file, encoding);
         string xmlString = sr.ReadToEnd();
         sr.Close();
         file.Close();
         return Deserialize(xmlString);
      }
      finally
      {
         if((file != null))
         {
            file.Dispose();
         }
         if((sr != null))
         {
            sr.Dispose();
         }
      }
   }

   #endregion Serialize/Deserialize

}

public class VvMER_Json_Status_Data
{
   public int    ElectronicId            { get; set; }
   public string DocumentNr              { get; set; }
   public int    DocumentTypeId          { get; set; }
   public string DocumentTypeName        { get; set; }
   public int    StatusId                { get; set; }
   public string StatusName              { get; set; }
   public string RecipientBusinessNumber { get; set; }
   public string RecipientBusinessUnit   { get; set; }
   public string RecipientBusinessName   { get; set; }
   public string Created                 { get; set; }
   public string Sent                    { get; set; }
   public string Updated                 { get; set; }
   public string Delivered               { get; set; }

   public string Error_PropertyName  { get; set; }
   public string Error_PropertyValue { get; set; }
   public string Error_Message       { get; set; }


   //public VvMER_Json_SEND_Response_Data ThisSTATUS_as_VvMER_Json_SEND_Response_Data
   //{
   //   get
   //   { 
   //      VvMER_Json_SEND_Response_Data vvMER_Json_SEND_Response_Data = new VvMER_Json_SEND_Response_Data();
   //
   //      vvMER_Json_SEND_Response_Data.ElectronicId            = this.ElectronicId           ;
   //      vvMER_Json_SEND_Response_Data.DocumentNr              = this.DocumentNr             ;
   //      vvMER_Json_SEND_Response_Data.DocumentTypeId          = this.DocumentTypeId         ;
   //      vvMER_Json_SEND_Response_Data.DocumentTypeName        = this.DocumentTypeName       ;
   //      vvMER_Json_SEND_Response_Data.StatusId                = this.StatusId               ;
   //      vvMER_Json_SEND_Response_Data.StatusName              = this.StatusName             ;
   //      vvMER_Json_SEND_Response_Data.RecipientBusinessNumber = this.RecipientBusinessNumber;
   //      vvMER_Json_SEND_Response_Data.RecipientBusinessUnit   = this.RecipientBusinessUnit  ;
   //      vvMER_Json_SEND_Response_Data.RecipientBusinessName   = this.RecipientBusinessName  ;
   //      vvMER_Json_SEND_Response_Data.Created                 = this.Created                ;
   //      vvMER_Json_SEND_Response_Data.Sent                    = this.Sent                   ;
   //      vvMER_Json_SEND_Response_Data.Modified                = this.Updated                ;
   //      vvMER_Json_SEND_Response_Data.Delivered               = this.Delivered.NotEmpty()   ;
   //
   //      return vvMER_Json_SEND_Response_Data;
   //   }
   //}
}
