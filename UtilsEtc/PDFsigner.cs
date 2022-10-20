using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using iTextSharp.text.pdf;
using System.Security.Cryptography.X509Certificates;
using System.Collections;
//using Org.Eurekaa.PDF.iSafePDF.Properties;
using System.IO;
//using Org.Eurekaa.PDF.iSafePDF.Lib;
using iTextSharp.text.pdf.security;
using System.Security;
using System.Security.Cryptography;

public static class VvPDFsigner
{
#if iSafePDF_ORIG
   #region iSafePDF ORIG

       private PDFEncryption PDFEnc = new PDFEncryption();
        private PickBox pb = new PickBox();
        private PdfReader reader = null;
        X509Certificate2 certificateData = null;

        private void ProcessBtn_Click(object sender, EventArgs e)
        {
          debug("*****Started (document = " + inputBox.Text + " => "+outputBox.Text+") ");
          debug("Checking certificate ...");

          //button1_Click(sender, e); //Sign from SmartCard 
          //button2_Click(sender, e); //Sign with certificate selection in the windows certificate store 
          //button3_Click(sender, e); //Sign from certificate in a pfx or a p12 file 

          button2_Click(sender, e); //Sign with certificate selection in the windows certificate store 

          //Cert myCert=null;
          //try
          //{
          //    string tsaUrl = String.IsNullOrEmpty(TSAUrlTextBox.Text) ? null : TSAUrlTextBox.Text;
          //    if (certificateData != null)
          //    {
          //        //X509Certificate2 cert = certsListBox.SelectedItem as X509Certificate2;
          //        byte[] bytes = certificateData.Export(X509ContentType.Pfx, certificatePwdBox.Text);
          //        myCert = new Cert(bytes, certificatePwdBox.Text, tsaUrl, tsaLogin.Text, tsaPwd.Text);
          //    }
          //    else
          //    {
          //       myCert = new Cert(certificateTextBox.Text, certificatePwdBox.Text, tsaUrl, tsaLogin.Text, tsaPwd.Text);
          //    }
          //
          //    debug("Certificate OK");                
          //}
          //catch (Exception ex)
          //{                
          //    debug("Warning : No valid certificate found, please make sure you entered a valid certificate file and password");
          //    //debug("Exception : " + ex.ToString());
          //    debug(" ==> Continue ... the document will not be signed !");
          //    //return;
          //}
          //
          //debug("Checking encryption options ...");
          //PDFEnc.UserPwd = encUserPwd.Text;
          //PDFEnc.OwnerPwd = encOwnerPwd.Text;
          //
          //
          //debug("Creating new MetaData object... ");
          //
          ////Adding Meta Datas
          //MetaData MyMD = new MetaData();
          //MyMD.Author = authorBox.Text;
          //MyMD.Title = titleBox.Text;
          //MyMD.Subject = subjectBox.Text;
          //MyMD.Keywords = kwBox.Text;
          //MyMD.Creator = creatorBox.Text;
          //MyMD.Producer = prodBox.Text;
          //
          //
          //debug("Processing document ... ");
          //PDFSigner pdfs = new PDFSigner(inputBox.Text, outputBox.Text, myCert, MyMD);
          //PDFSignatureAP sigAp = new PDFSignatureAP();
          //sigAp.SigReason = Reasontext.Text;
          //sigAp.SigContact = Contacttext.Text;
          //sigAp.SigLocation = Locationtext.Text;
          //sigAp.Visible = SigVisible.Checked;
          //sigAp.Multi = multiSigChkBx.Checked;
          //sigAp.Page = Convert.ToInt32(numberOfPagesUpDown.Value);
          //sigAp.CustomText = custSigText.Text;
          //
          //if (sigImgBox.Image != null)
          //{
          //    MemoryStream ms = new MemoryStream();
          //    sigImgBox.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
          //    sigAp.RawData = ms.ToArray();
          //    ms.Close();
          //}
          //
          //sigAp.SigX = (float)sigPosX.Value;
          //sigAp.SigY = (float)sigPosY.Value;
          //sigAp.SigW = (float)sigWidth.Value;
          //sigAp.SigH = (float)sigHeight.Value;
          //
          //pdfs.Sign(sigAp, encryptChkBx.Checked, PDFEnc);
          
            debug("Done :)");
            
            MessageBox.Show("The document has been succesfully processed", "iSafePDF :: Signature done", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        public Form1()
        {
            InitializeComponent();
            pb.WireControl(sigPicture);
            


            
        }


        private void debug(string txt)
        {
            DebugBox.AppendText(txt + System.Environment.NewLine);
        }



        private void button4_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFile;
            openFile = new System.Windows.Forms.OpenFileDialog();
            openFile.Filter = "PDF files *.pdf|*.pdf";
            openFile.Title = "Select a file";
            if (openFile.ShowDialog() != DialogResult.OK)
                return;

            inputBox.Text = openFile.FileName;

            
            
            try
            {
                reader = new PdfReader(inputBox.Text);

            }
            catch
            {
                PwdDialog dlg = new PwdDialog();
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string pwd = (dlg.Controls["pwdTextBox"] as TextBox).Text;
                    reader = new PdfReader(inputBox.Text, Tools.StrToByteArray(pwd));
                }
                else
                {
                    inputBox.Text = "";
                    return;
                }
            }
            MetaData md = new MetaData();
            md.Info = reader.Info;

            authorBox.Text = md.Author;
            titleBox.Text = md.Title;
            subjectBox.Text = md.Subject;
            kwBox.Text = md.Keywords;
            creatorBox.Text = md.Creator;
            prodBox.Text = md.Producer;


            numberOfPagesUpDown.Maximum = reader.NumberOfPages;
            numberOfPagesUpDown.Minimum = numberOfPagesUpDown.Value = 1;
            numberOfPagesUpDown_ValueChanged(numberOfPagesUpDown, null);

            sigPicture.Left = 0;
            sigPicture.Top = sigPicture.Parent.Height - sigPicture.Height;
            sigPicture_Move(sigPicture, null);
            
            sigPicture.Width = 50;
            sigPicture.Height = 20;
            sigPicture_Resize(sigPicture, null);
            
            
        }



        private void button5_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.SaveFileDialog saveFile;
            saveFile = new System.Windows.Forms.SaveFileDialog();            
            saveFile.Filter = "PDF files (*.pdf)|*.pdf";
            saveFile.Title = "Save PDF File";
            if (saveFile.ShowDialog() != DialogResult.OK)
                return;
            outputBox.Text = saveFile.FileName;

        }



        private void Form1_Load(object sender, EventArgs e)
        {
            PDFEnc.Permissions.Add(permissionType.Assembly);
            PDFEnc.Permissions.Add(permissionType.Copy);
            PDFEnc.Permissions.Add(permissionType.DegradedPrinting);
            PDFEnc.Permissions.Add(permissionType.FillIn);
            PDFEnc.Permissions.Add(permissionType.ModifyAnnotation);
            PDFEnc.Permissions.Add(permissionType.ModifyContent);
            PDFEnc.Permissions.Add(permissionType.Printing);
            PDFEnc.Permissions.Add(permissionType.ScreenReaders);

            webBrowser1.Navigate("about:blank");
            if (webBrowser1.Document != null)
            {
                webBrowser1.Document.Write(string.Empty);
            }
            webBrowser1.DocumentText = Resources.DonateBtn;           
        }


        private void tsaCbx_CheckedChanged(object sender, EventArgs e)
        {
            TSAUrlTextBox.Enabled = (sender as CheckBox).Checked;
            tsaLogin.Enabled = (sender as CheckBox).Checked;
            tsaPwd.Enabled = (sender as CheckBox).Checked;
            TSALbl1.Enabled = (sender as CheckBox).Checked;
            TSALbl2.Enabled = (sender as CheckBox).Checked;
            TSALbl3.Enabled = (sender as CheckBox).Checked;
            
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void encryptChkBx_CheckedChanged(object sender, EventArgs e)
        {
            EncryptionGrp.Enabled = (sender as CheckBox).Checked;
            ProtectionGrp.Enabled = (sender as CheckBox).Checked;
            if ((sender as CheckBox).Checked)
                multiSigChkBx.Checked = false;
        }

        private void encAssemblyChk_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked) PDFEnc.Permissions.Add(permissionType.Assembly);
            else PDFEnc.Permissions.Remove(permissionType.Assembly);
        }

        private void encCopyChk_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked) PDFEnc.Permissions.Add(permissionType.Copy);
            else PDFEnc.Permissions.Remove(permissionType.Copy);

        }

        private void encDegPrintChk_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked) PDFEnc.Permissions.Add(permissionType.DegradedPrinting);
            else PDFEnc.Permissions.Remove(permissionType.DegradedPrinting);

        }

        private void encFillInChk_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked) PDFEnc.Permissions.Add(permissionType.FillIn);
            else PDFEnc.Permissions.Remove(permissionType.FillIn);

        }

        private void encModAnnotChk_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked) PDFEnc.Permissions.Add(permissionType.ModifyAnnotation);
            else PDFEnc.Permissions.Remove(permissionType.ModifyAnnotation);

        }

        private void encModContChk_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked) PDFEnc.Permissions.Add(permissionType.ModifyContent);
            else PDFEnc.Permissions.Remove(permissionType.ModifyContent);

        }

        private void encPrintChk_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked) PDFEnc.Permissions.Add(permissionType.Printing);
            else PDFEnc.Permissions.Remove(permissionType.Printing);

        }

        private void encScrRdChk_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked) PDFEnc.Permissions.Add(permissionType.ScreenReaders);
            else PDFEnc.Permissions.Remove(permissionType.ScreenReaders);

        }



        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
        }

        private void tabControl1_Deselected(object sender, TabControlEventArgs e)
        {
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ProcessBtn.Visible = !tabControl1.SelectedTab.Equals(aboutTab);
        }

        private void VisitProjectHome(object sender, LinkLabelLinkClickedEventArgs e)
        {

            //LinkLabel lnk = sender as LinkLabel;
            System.Diagnostics.Process.Start("IExplore", "http://isafepdf.eurekaa.org");
            
        }



        private void certTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void browseBtn_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFile;
            openFile = new System.Windows.Forms.OpenFileDialog();
            openFile.Filter = "*.jpg|*.gif|*.bmp|*.png";
            openFile.Title = "Select a file";
            if (openFile.ShowDialog() != DialogResult.OK)
                return;

            sigPicture.Image = sigImgBox.Image = new Bitmap(openFile.FileName);
        }




        private void numberOfPagesUpDown_ValueChanged(object sender, EventArgs e)
        {

            
            iTextSharp.text.Rectangle rect = reader.GetPageSize(Convert.ToInt32(numberOfPagesUpDown.Value));

            pagePreviewPanel.Top = 0;

            if (rect.Width > rect.Height)
            {
                pagePreviewPanel.Width = pagePreviewPanel.Parent.Width;
                pagePreviewPanel.Height = Convert.ToInt32((pagePreviewPanel.Width * rect.Height) / rect.Width);
            }
            else
            {
                pagePreviewPanel.Height = pagePreviewPanel.Parent.Height;
                pagePreviewPanel.Width = Convert.ToInt32((pagePreviewPanel.Height * rect.Width) / rect.Height);                
            }
            pagePreviewPanel.Left = (pagePreviewPanel.Parent.Width - pagePreviewPanel.Width) / 2;
            pagePreviewPanel.Top = (pagePreviewPanel.Parent.Height - pagePreviewPanel.Height) / 2;


            sigPosX.Maximum = Convert.ToInt32(rect.Width);
            sigPosY.Maximum = Convert.ToInt32(rect.Height);

            sigWidth.Maximum = Convert.ToInt32(rect.Width);
            sigHeight.Maximum = Convert.ToInt32(rect.Height);
            
        }

        private void sigPicture_Move(object sender, EventArgs e)
        {
            iTextSharp.text.Rectangle rect = reader.GetPageSize(Convert.ToInt32(numberOfPagesUpDown.Value));
            


            decimal X = Convert.ToInt32( (rect.Width * sigPicture.Left) / pagePreviewPanel.Width ); 

            decimal Y = sigPicture.Parent.Height - sigPicture.Top - sigPicture.Height;
            Y =  Convert.ToInt32( (rect.Height * (float)Y) / pagePreviewPanel.Height );

            if (X > sigPosX.Maximum) X = sigPosX.Maximum;
            if (X < sigPosX.Minimum) X = sigPosX.Minimum;
            if (Y > sigPosY.Maximum) Y = sigPosY.Maximum;
            if (Y < sigPosY.Minimum) Y = sigPosY.Minimum;

            sigPosX.Value = X;
            sigPosY.Value = Y;
        }

        private void clearBtn_Click(object sender, EventArgs e)
        {
            sigPicture.Image = sigImgBox.Image = null;
        }

        private void sigPicture_Resize(object sender, EventArgs e)
        {
            iTextSharp.text.Rectangle rect = reader.GetPageSize(Convert.ToInt32(numberOfPagesUpDown.Value));

            decimal W = Convert.ToInt32((rect.Width * sigPicture.Width) / pagePreviewPanel.Width);

            decimal H = Convert.ToInt32((rect.Height * sigPicture.Height) / pagePreviewPanel.Height);
            
            if (W > sigWidth.Maximum) W = sigWidth.Maximum;
            if (W < sigWidth.Minimum) W = sigWidth.Minimum;
            if (H > sigHeight.Maximum) H = sigHeight.Maximum;
            if (H < sigHeight.Minimum) H = sigHeight.Minimum;

            sigWidth.Value = W;
            sigHeight.Value = H;
        }

        private void SigVisible_CheckedChanged(object sender, EventArgs e)
        {
            sigPanel1.Visible = sigPanel2.Visible = sigPanel1.Enabled = sigPanel2.Enabled = SigVisible.Checked;
        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void selectSertBtn_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(inputBox.Text) || String.IsNullOrEmpty(outputBox.Text))
            {
                MessageBox.Show("Please go to the 'Document tab' and select a source and a target file first", "iSafePDF :: Action required", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }

            certificateData = null;
            sigPanel.Visible = false;
            certificateTextBox.Text = String.Empty;

            CertificateDialog dlg = new CertificateDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                ListBox certs = (dlg.Controls["certsListBox"] as ListBox);
                if (certs.SelectedItem != null)
                {
                    certificateData = certs.SelectedItem as X509Certificate2;
                    certificateTextBox.Text = certs.SelectedItem.ToString();
                }
                else
                {
                    certificateTextBox.Text = (dlg.Controls["certTextBox"] as TextBox).Text;
                }
                certificatePwdBox.Text = (dlg.Controls["passwordBox"] as TextBox).Text;

                sigPanel.Visible = !String.IsNullOrEmpty(certificateTextBox.Text);
            }

        }

   #endregion iSafePDF ORIG
#endif

   #region StackOverflowSample.cs

   //button1_Click(sender, e); //Sign from SmartCard 
   //button2_Click(sender, e); //Sign with certificate selection in the windows certificate store 
   //button3_Click(sender, e); //Sign from certificate in a pfx or a p12 file 

   private static string tmpFilePath = "";

 //private void button2_Click(object sender, EventArgs e)
   public static bool SignPDFwithCertificateFromWindowsCertificateStore(string fullPathSourcePdfFileName, string fullPathSignedPdfFileName, bool isVisible)
   {
      bool success = false;

      #region some debug stuff
         //byQ: start
      //try
      //{
      //   store = new X509Store(/*"MY"*/StoreName.My, StoreLocation.CurrentUser);
      //   store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
      //   X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates;
      //   X509Certificate2Collection fcollection = (X509Certificate2Collection)collection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
      //   X509Certificate2Collection scollection = X509Certificate2UI.SelectFromCollection(fcollection, "Test Certificate Select", "Select a certificate from the following list to get information on that certificate", X509SelectionFlag.SingleSelection);
      //   debug(String.Format("Number of certificates: {0}{1}", scollection.Count, Environment.NewLine));
      //   foreach(X509Certificate2 x509 in scollection)
      //   {
      //      byte[] rawdata = x509.RawData;
      //      
      //      debug(String.Format("Content Type: {0}{1}"         , X509Certificate2.GetCertContentType(rawdata),    Environment.NewLine));
      //      debug(String.Format("Friendly Name: {0}{1}"        , x509.FriendlyName,                               Environment.NewLine));
      //      debug(String.Format("Certificate Verified?: {0}{1}", x509.Verify(),                                   Environment.NewLine));
      //      debug(String.Format("Simple Name: {0}{1}"          , x509.GetNameInfo(X509NameType.SimpleName, true), Environment.NewLine));
      //      debug(String.Format("Signature Algorithm: {0}{1}"  , x509.SignatureAlgorithm.FriendlyName,            Environment.NewLine));
      //      debug(String.Format("Private Key: {0}{1}"          , x509.PrivateKey.ToXmlString(false),              Environment.NewLine));
      //      debug(String.Format("Public Key: {0}{1}"           , x509.PublicKey.Key.ToXmlString(false),           Environment.NewLine));
      //      debug(String.Format("Certificate Archived?: {0}{1}", x509.Archived,                                   Environment.NewLine));
      //      debug(String.Format("Length of Raw Data: {0}{1}"   , x509.RawData.Length,                             Environment.NewLine));
      //      X509Certificate2UI.DisplayCertificate(x509);
      //      x509.Reset();
      //   }
      //   //store.Close();
      //}
      //catch(CryptographicException)
      //{
      //   Console.WriteLine("Information could not be written out for this certificate.");
      //}
         //byQ: end
      #endregion some debug stuff

      //Sign with certificate selection in the windows certificate store
      X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser); // !!! 
      
      store.Open(OpenFlags.ReadOnly);

      X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates;

      #region Certificate FILTERS

      X509FindType findType;
      object       findValue;
      bool validOnly = false;

      // FILTER 1 
      findType  = X509FindType.FindByTimeValid;
      findValue = DateTime.Now;
      X509Certificate2Collection fcollection1 = (X509Certificate2Collection)collection.Find(findType, findValue, validOnly);

      // FILTER 2 
      findType = X509FindType.FindByIssuerName;
      findValue = "ZABA CA";
      X509Certificate2Collection fcollection2 = (X509Certificate2Collection)fcollection1.Find(findType, findValue, validOnly);

      // FILTER 3 
      findType = X509FindType.FindByKeyUsage;
      findValue = X509KeyUsageFlags.DigitalSignature;
      X509Certificate2Collection fcollection3 = (X509Certificate2Collection)fcollection2.Find(findType, findValue, validOnly);

      #endregion Certificate FILTERS

      string title   = "Odabir potpisnog certifikata";
      string message = "Odaberite s liste certifikat kojim želite digitalno potisati PDF datoteku.";
      X509Certificate2Collection scollection = X509Certificate2UI.SelectFromCollection(fcollection1/*3*/, title, message, X509SelectionFlag.SingleSelection);

      X509Certificate2 cert = null;

      if(scollection.Count > 0)
      {
         cert = scollection[0];
      }
      else
      {
         ZXC.aim_emsg(MessageBoxIcon.Error, "Akcija prekinuta\n\nili\n\ncertifikat nije pronađen.");
         store.Close();
         return false;
      }

      try
      {
         success = SignWithThisCert(cert, fullPathSourcePdfFileName, fullPathSignedPdfFileName, isVisible); // VOILA 
      }
      catch(Exception ex)
      {
         ZXC.aim_emsg(MessageBoxIcon.Warning, ex.Message);
         success = false;
      }

      store.Close();

      return success;
   }

   private static bool SignWithThisCert(X509Certificate2 theCertificate, string fullPathSourcePdfFileName, string fullPathSignedPdfFileName, bool isVisible)
   {
      bool success = false;

      PdfReader pdfReader = null;
      PdfStamper pdfStamper = null;
      FileStream signedPdf = null;
      try
      {
         //string SourcePdfFileName = textBox1.Text;
         //string DestPdfFileName = fullPathSourcePdfFileName + "-SGN.pdf";
         Org.BouncyCastle.X509.X509CertificateParser cp = new Org.BouncyCastle.X509.X509CertificateParser();
         Org.BouncyCastle.X509.X509Certificate[] chain = new Org.BouncyCastle.X509.X509Certificate[] { cp.ReadCertificate(theCertificate.RawData) };
         IExternalSignature externalSignature = new X509Certificate2Signature(theCertificate, "SHA-1");
         pdfReader = new PdfReader(fullPathSourcePdfFileName);
         try { signedPdf = new FileStream(fullPathSignedPdfFileName, FileMode.Create); } //the output pdf file
         catch(Exception ex) { ZXC.aim_emsg(MessageBoxIcon.Error, ex.Message); return false; }

         pdfStamper = PdfStamper.CreateSignature(pdfReader, signedPdf, '\0');
         
         PdfSignatureAppearance signatureAppearance = pdfStamper.SignatureAppearance;
         //here set signatureAppearance at your will
       //signatureAppearance.Reason = "Vektor digitalni potpis";
       //signatureAppearance.Location = "HRVATSKA";
       //signatureAppearance.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.DESCRIPTION;

         // q override of signatureAppearance ___ START ___

         if(isVisible)
         {
            // public Rectangle(float llx, float lly, float urx, float ury);
            float llx = 350;
            float lly = 55;
            float urx = llx + 240;
            float ury = lly + 75;
            iTextSharp.text.Rectangle pageRect = new iTextSharp.text.Rectangle(llx, lly, urx, ury);
            signatureAppearance.SetVisibleSignature(pageRect, 1, null);

       //   iTextSharp.text.Image sigGraph = iTextSharp.text.Image.GetInstance(@"E:\0_DOWNLOAD\Code Project Stuff\PDF3\ZIG2.png");
            byte[] sigImage = ZXC.CURR_prjkt_rec.TheLogo2;

            if(sigImage != null)
            {
               iTextSharp.text.Image sigGraph = iTextSharp.text.Image.GetInstance(ZXC.CURR_prjkt_rec.TheLogo2);
               //sigGraph.ScaleToFit(pageRect);
               signatureAppearance.SignatureGraphic = sigGraph;
               signatureAppearance.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.GRAPHIC_AND_DESCRIPTION;
            }
            else
            {
               signatureAppearance.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.DESCRIPTION;
            }

            //signatureAppearance.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.DESCRIPTION;
            //signatureAppearance.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.GRAPHIC ;
            //signatureAppearance.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.GRAPHIC_AND_DESCRIPTION;
            //signatureAppearance.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.NAME_AND_DESCRIPTION;
            // q override of signatureAppearance ___  END  ___
         }
         else
         {
            signatureAppearance.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.DESCRIPTION;
         }

         MakeSignature.SignDetached(signatureAppearance, externalSignature, chain, null, null, null, 0, CryptoStandard.CMS);
         //MakeSignature.SignDetached(signatureAppearance, externalSignature, chain, null, null, null, 0, CryptoStandard.CADES);
         ZXC.aim_emsg(MessageBoxIcon.Information, "Potpisana datoteka:\n\n{0}\n\nCertifikat:\n\n{1}", fullPathSignedPdfFileName, theCertificate.IssuerName.Name);
         success = true;
      }

      finally
      {
         pdfReader.Close();
         pdfStamper.Close();
         signedPdf.Close();
      }

      return success;
   }

   private static SecureString GetSecurePin(string PinCode)
   {
      SecureString pwd = new SecureString();
      foreach(var c in PinCode.ToCharArray()) pwd.AppendChar(c);
      return pwd;
   }

   private static void button1_Click(object sender, EventArgs e)
   {
      //Sign from SmartCard
      //note : ProviderName and KeyContainerName can be found with the dos command : CertUtil -ScInfo
      string ProviderName     = /*textBox2.Text*/"ActivClient Cryptographic Service Provider";
      string KeyContainerName = /*textBox3.Text*/"ADE569B0-7DEE-4008-8859-9B4DB5451263";
      string PinCode          = /*textBox4.Text*/"";
      if(PinCode != "")
      {
         //if pin code is set then no windows form will popup to ask it
         SecureString pwd = GetSecurePin(PinCode);
         CspParameters csp = new CspParameters(1,
                                                ProviderName,
                                                KeyContainerName,
                                                new System.Security.AccessControl.CryptoKeySecurity(),
                                                pwd);
         try
         {
            RSACryptoServiceProvider rsaCsp = new RSACryptoServiceProvider(csp);
            // the pin code will be cached for next access to the smart card
         }
         catch(Exception ex)
         {
            MessageBox.Show("Crypto error: " + ex.Message);
            return;
         }
      }
      X509Store store = new X509Store(StoreLocation.CurrentUser);
      store.Open(OpenFlags.ReadOnly);
      X509Certificate2 cert = null;
      if((ProviderName == "") || (KeyContainerName == ""))
      {
         MessageBox.Show("You must set Provider Name and Key Container Name");
         return;
      }
      foreach(X509Certificate2 cert2 in store.Certificates)
      {
         if(cert2.HasPrivateKey)
         {
            RSACryptoServiceProvider rsa = null;
            try { rsa = (RSACryptoServiceProvider)cert2.PrivateKey; }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
            if(rsa == null) continue; // not smart card cert again
            if(rsa.CspKeyContainerInfo.HardwareDevice) // sure - smartcard
            {
               if((rsa.CspKeyContainerInfo.KeyContainerName == KeyContainerName) && (rsa.CspKeyContainerInfo.ProviderName == ProviderName))
               {
                  //we find it
                  cert = cert2;
                  break;
               }
            }
         }
      }
      if(cert == null)
      {
         MessageBox.Show("Certificate not found");
         return;
      }
      SignWithThisCert(cert, tmpFilePath, tmpFilePath, false);
   }

   private static void button3_Click(object sender, EventArgs e)
   {
      //Sign from certificate in a pfx or a p12 file
    //string PfxFileName = textBox5.Text;
    //string PfxPassword = textBox6.Text;
      string PfxFileName = "daj tu neki certFile fileNmae";
      string PfxPassword = ZXC.CURR_prjkt_rec.ESgnCertPasswdDecrypted;

      X509Certificate2 cert = new X509Certificate2(PfxFileName, PfxPassword);
      SignWithThisCert(cert, tmpFilePath, tmpFilePath, false);
   }

   #endregion StackOverflowSample.cs

}