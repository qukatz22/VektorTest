using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B2GSendInvoicePKIClient.B2GSendInvoicePKIWebService;
using B2GSendInvoicePKIClient.WSClient;
using B2GSendInvoicePKIClient.Operations;
using B2GSendInvoicePKIClient.XML;

namespace B2GSendInvoicePKIClient
{
    class SendInvoiceClient
    {
        static void Main(string[] args)
      //static void callSendInvoice()
        {
            //TODO: adrese, certovi,... iz app.config ili po želji...
            string endpointAddress           =  "https://prez.fina.hr/SendB2GOutgoingInvoicePKIWebService/services/SendB2GOutgoingInvoicePKIWebService";
            string dnsIdentity               =  "e-invoice";
          //string serviceCertificatePath    =  "putanja/do/e-invoice_DEMO.cer";
          //string clientCertificatePath     =  "putanja/do/privatnikljuc.p12" ;
          //string clientCertificatePassword =  "***password od p12***"        ;
            string serviceCertificatePath    = @"D:\000_XSD_Qtmp\ByJuraByQ\B2GSendInvoicePKIClient\certs\demo\e-invoice_DEMO.cer";
            string clientCertificatePath     = @"D:\000_XSD_Qtmp\ByJuraByQ\B2GSendInvoicePKIClient\certs\demo\VEKTOR.p12"        ;
            string clientCertificatePassword =  "1q1q1Q";

            string supplierID = "9934:60042587515";
            string buyerID    = "9934:65119154523";

            try
            {
               //pozovi kreiranje XML-a (s potpisom)
               //TODO: ovo je samo primjer
               CreateInvoiceXML xml              = new CreateInvoiceXML();
               byte[]           signedInvoiceXML = xml.getSignedInvoice();
               
               B2GGenericClient genericClient = new B2GGenericClient();
               eRacunB2GPortTypeClient client = 
               genericClient.GetB2GClient(endpointAddress, dnsIdentity, serviceCertificatePath, clientCertificatePath, clientCertificatePassword);

               SendInvoiceOperation sendInvoiceOperation = new SendInvoiceOperation();
               SendB2GOutgoingInvoiceMsg message = sendInvoiceOperation.GetSendInvoiceMsg(supplierID, buyerID, "broj-racuna-0001-NET", signedInvoiceXML);

               SendB2GOutgoingInvoiceAckMsg ackMessage = client.sendB2GOutgoingInvoice(message);

               Console.Out.WriteLine("MeesageID: \t"     + ackMessage.MessageAck.MessageID    );
               Console.Out.WriteLine("MeesageAckID: \t"  + ackMessage.MessageAck.MessageAckID );
               Console.Out.WriteLine("AckStatus: \t"     + ackMessage.MessageAck.AckStatus    );
               Console.Out.WriteLine("AckStatusCode: \t" + ackMessage.MessageAck.AckStatusCode);
               Console.Out.WriteLine("AckStatusText: \t" + ackMessage.MessageAck.AckStatusText);
               if (ackMessage.MessageAck.AckStatus.Equals(B2GSendInvoicePKIWebService.AckStatusType.ACCEPTED))
               {
                  SendB2GOutgoingInvoiceAckMsgB2GOutgoingInvoiceEnvelopeB2GOutgoingInvoiceProcessing processing = ackMessage.B2GOutgoingInvoiceEnvelope.B2GOutgoingInvoiceProcessing;
                  //ako je ispravan račun
                  if (processing.Item is SendB2GOutgoingInvoiceAckMsgB2GOutgoingInvoiceEnvelopeB2GOutgoingInvoiceProcessingCorrectB2GOutgoingInvoice)
                  {
                     SendB2GOutgoingInvoiceAckMsgB2GOutgoingInvoiceEnvelopeB2GOutgoingInvoiceProcessingCorrectB2GOutgoingInvoice 
                     correctOutgoingInvoice = (SendB2GOutgoingInvoiceAckMsgB2GOutgoingInvoiceEnvelopeB2GOutgoingInvoiceProcessingCorrectB2GOutgoingInvoice)processing.Item;
                     Console.Out.WriteLine("\nISPRAVAN RAČUN!!!");
                     Console.Out.WriteLine("SupplierInvoiceID: \t" + correctOutgoingInvoice.SupplierInvoiceID);
                     Console.Out.WriteLine("InvoiceID: \t\t" + correctOutgoingInvoice.InvoiceID);
                     Console.Out.WriteLine("InvoiceTimestamp: \t" + correctOutgoingInvoice.InvoiceTimestamp);
                  }
                  //ako je neispravan račun
                  if (processing.Item is SendB2GOutgoingInvoiceAckMsgB2GOutgoingInvoiceEnvelopeB2GOutgoingInvoiceProcessingIncorrectB2GOutgoingInvoice)
                  {
                    SendB2GOutgoingInvoiceAckMsgB2GOutgoingInvoiceEnvelopeB2GOutgoingInvoiceProcessingIncorrectB2GOutgoingInvoice 
                    incorrectOutgoingInvoice = (SendB2GOutgoingInvoiceAckMsgB2GOutgoingInvoiceEnvelopeB2GOutgoingInvoiceProcessingIncorrectB2GOutgoingInvoice)processing.Item;
                    Console.Out.WriteLine("\nNEISPRAVAN RAČUN!!!"                                             );
                    Console.Out.WriteLine("SupplierInvoiceID: \t" + incorrectOutgoingInvoice.SupplierInvoiceID);
                    Console.Out.WriteLine("ErrorCode: \t\t"       + incorrectOutgoingInvoice.ErrorCode        );
                    Console.Out.WriteLine("ErrorMessage: \t"      + incorrectOutgoingInvoice.ErrorMessage     );
                  }
               }


               Console.ReadLine();
            }
            catch (Exception e)
            {
               Console.Out.WriteLine("Greska: " + e.Message);
               Console.Out.WriteLine("Inner: " + e.InnerException);
               Console.ReadLine();
            }
        }
    }
}
