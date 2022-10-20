using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B2GSendInvoicePKIClient.B2GSendInvoicePKIWebService;
using B2GSendInvoicePKIClient.WSClient;
using B2GSendInvoicePKIClient.Operations;

namespace B2GSendInvoicePKIClient
{
    class EchoClient
    {
      //static void Main(string[] args)
        static void callEcho()
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

            
            try
            {
               B2GGenericClient genericClient = new B2GGenericClient();
               eRacunB2GPortTypeClient client = 
               genericClient.GetB2GClient(endpointAddress, dnsIdentity, serviceCertificatePath, clientCertificatePath, clientCertificatePassword);

               EchoOperation echoOperation = new EchoOperation();
               EchoMsg message             = echoOperation.GetEchoMsg(supplierID, "Hello world");

               EchoAckMsg ackMessage = client.echo(message);

               Console.Out.WriteLine("MeesageID: \t"     + ackMessage.MessageAck.MessageID    );
               Console.Out.WriteLine("MeesageAckID: \t"  + ackMessage.MessageAck.MessageAckID );
               Console.Out.WriteLine("AckStatus: \t"     + ackMessage.MessageAck.AckStatus    );
               Console.Out.WriteLine("AckStatusCode: \t" + ackMessage.MessageAck.AckStatusCode);
               Console.Out.WriteLine("AckStatusText: \t" + ackMessage.MessageAck.AckStatusText);
               Console.Out.WriteLine("Echo: \t"          + ackMessage.EchoData.Echo           );
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
