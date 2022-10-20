using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using B2GSendInvoicePKIClient.B2GSendInvoicePKIWebService;
using B2GSendInvoicePKIClient.WSSecurity;

namespace B2GSendInvoicePKIClient.WSClient
{
   class B2GGenericClient
   {
      /// <summary>
      /// 
      /// </summary>
      /// <param name="endpointUrl">endpoint web servisa</param>
      /// <param name="dnsIdentity">Subject certifikata na servisnoj starni (eInvoice)</param>
      /// <param name="clientCertificatePath">Putanja do .p12 (.pfx) datoteke</param>
      /// <param name="clientCertificatePassword">Password .p12 (.pfx) datoteke</param>
      /// <param name="serviceCertificatePath">Putanja do javnog ključa certifikata na servisnoj strani</param>
      /// <returns>eRacunB2GPortTypeClient</returns>
      public eRacunB2GPortTypeClient GetB2GClient(string endpointUrl, string dnsIdentity, string serviceCertificatePath, string clientCertificatePath, string clientCertificatePassword)
      {
          //Web service endpoint with DNS identity (Subject from certificate)
          EndpointAddress endpointAddress = new EndpointAddress(new Uri(endpointUrl), EndpointIdentity.CreateDnsIdentity(dnsIdentity));

          //CustomBinding for eBox web service with security setup
          B2GCustomBinding b2gCustomBinding = new B2GCustomBinding();
          CustomBinding binding = b2gCustomBinding.GetB2GCustomBinding();

          //Web service client
          eRacunB2GPortTypeClient client = new eRacunB2GPortTypeClient(binding, endpointAddress);
          client.Endpoint.Contract.ProtectionLevel = ProtectionLevel.Sign;

          //client certificate (private key for signature)
          X509Certificate2 clientCertificate = new X509Certificate2(clientCertificatePath, clientCertificatePassword);
          client.ClientCredentials.ClientCertificate.Certificate = clientCertificate;


          //service certificate (public key for signature validation)
          X509Certificate2 serviceCertificate = new X509Certificate2(serviceCertificatePath);
          client.ClientCredentials.ServiceCertificate.DefaultCertificate = serviceCertificate;
          //*****************
          //https://stackoverflow.com/questions/48618685/soap-security-negotiation-failed-after-windows-security-update-kb4056892
          //*****************
          client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;

          return client;
      }

      /// <summary>
      /// byQ
      /// </summary>
      /// <param name="endpointUrl"></param>
      /// <param name="dnsIdentity"></param>
      /// <param name="serviceCertificatePath"></param>
      /// <param name="clientCertificatePath"></param>
      /// <param name="clientCertificatePassword"></param>
      /// <returns></returns>
      public eRacunB2GPortTypeClient GetB2GClient(string endpointUrl, string dnsIdentity, string serviceCertificatePath, X509Certificate2 clientCertificate)
      {
          //Web service endpoint with DNS identity (Subject from certificate)
          EndpointAddress endpointAddress = new EndpointAddress(new Uri(endpointUrl), EndpointIdentity.CreateDnsIdentity(dnsIdentity));

          //CustomBinding for eBox web service with security setup
          B2GCustomBinding b2gCustomBinding = new B2GCustomBinding();
          CustomBinding binding = b2gCustomBinding.GetB2GCustomBinding();

          //Web service client
          eRacunB2GPortTypeClient client = new eRacunB2GPortTypeClient(binding, endpointAddress);
          client.Endpoint.Contract.ProtectionLevel = ProtectionLevel.Sign;

          //client certificate (private key for signature)
          //X509Certificate2 clientCertificate = new X509Certificate2(clientCertificatePath, clientCertificatePassword); byQ 
          client.ClientCredentials.ClientCertificate.Certificate = clientCertificate;


          //service certificate (public key for signature validation)
          X509Certificate2 serviceCertificate = new X509Certificate2(serviceCertificatePath);
          client.ClientCredentials.ServiceCertificate.DefaultCertificate = serviceCertificate;
          //*****************
          //https://stackoverflow.com/questions/48618685/soap-security-negotiation-failed-after-windows-security-update-kb4056892
          //*****************
          client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;

          return client;
      }

   }  
}
