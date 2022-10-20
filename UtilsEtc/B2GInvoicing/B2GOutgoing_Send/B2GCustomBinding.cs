using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace B2GSendInvoicePKIClient.WSSecurity
{
    class B2GCustomBinding
    {
        public CustomBinding GetB2GCustomBinding()
        {
            CustomBinding binding = new CustomBinding();
            binding.Name = "B2GCustomBinding";

            AsymmetricSecurityBindingElement sbe = (AsymmetricSecurityBindingElement)SecurityBindingElement.CreateMutualCertificateDuplexBindingElement(MessageSecurityVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10);
            sbe.AllowSerializedSigningTokenOnReply = true;
            sbe.SecurityHeaderLayout = SecurityHeaderLayout.Lax;
            sbe.IncludeTimestamp = true;
            sbe.DefaultAlgorithmSuite = new B2GSecurityAlgorithmSuite();
            sbe.EnableUnsecuredResponse = false;

            binding.Elements.Add(sbe);

            binding.Elements.Add(new TextMessageEncodingBindingElement(MessageVersion.Soap11, System.Text.Encoding.UTF8));

            HttpsTransportBindingElement htbe = new HttpsTransportBindingElement();
            htbe.RequireClientCertificate = true;

            binding.Elements.Add(htbe);

            return binding;
        }
    }
}
