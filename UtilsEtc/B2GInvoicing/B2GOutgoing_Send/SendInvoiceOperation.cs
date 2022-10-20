using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B2GSendInvoicePKIClient.B2GSendInvoicePKIWebService;

namespace B2GSendInvoicePKIClient.Operations
{
    class SendInvoiceOperation
    {
        private const string B2G_SEND_INVOICE_MESSAGE_TYPE = "9001";

        public SendB2GOutgoingInvoiceMsg GetSendInvoiceMsg(string supplierID, string buyerID, string supplierInvoiceID, byte[] signedInvoiceXML)
        {
            SendB2GOutgoingInvoiceMsg message = new SendB2GOutgoingInvoiceMsg();

            //Header
            HeaderSupplierType header = new HeaderSupplierType();
            header.MessageID = Guid.NewGuid().ToString();
            header.SupplierID = supplierID;
            header.MessageType = B2G_SEND_INVOICE_MESSAGE_TYPE;

            message.HeaderSupplier = header;

            //Data
            SendB2GOutgoingInvoiceMsgData data = new SendB2GOutgoingInvoiceMsgData();

            //B2GOutgoingInvoiceEnvelope
            SendB2GOutgoingInvoiceMsgDataB2GOutgoingInvoiceEnvelope b2gOutgoingInvoiceEnvelope = new SendB2GOutgoingInvoiceMsgDataB2GOutgoingInvoiceEnvelope();
            
            b2gOutgoingInvoiceEnvelope.BuyerID = buyerID;
            b2gOutgoingInvoiceEnvelope.XMLStandard = SendB2GOutgoingInvoiceMsgDataB2GOutgoingInvoiceEnvelopeXMLStandard.UBL;
            b2gOutgoingInvoiceEnvelope.SpecificationIdentifier = SendB2GOutgoingInvoiceMsgDataB2GOutgoingInvoiceEnvelopeSpecificationIdentifier.urnceneuen169312017;
            b2gOutgoingInvoiceEnvelope.SupplierInvoiceID = supplierInvoiceID;

            //InvoiceEnvelope
            b2gOutgoingInvoiceEnvelope.ItemElementName = ItemChoiceType.InvoiceEnvelope;
            b2gOutgoingInvoiceEnvelope.Item = signedInvoiceXML;

            data.B2GOutgoingInvoiceEnvelope = b2gOutgoingInvoiceEnvelope;

            message.Data = data;

            return message;
        }
    }
}
