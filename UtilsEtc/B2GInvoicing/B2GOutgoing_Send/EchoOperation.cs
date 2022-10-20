using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using B2GSendInvoicePKIClient.B2GSendInvoicePKIWebService;

namespace B2GSendInvoicePKIClient.Operations
{
    class EchoOperation
    {
        private const string B2G_ECHO_MESSAGE_TYPE = "9999";
        /// <summary>
        /// Metoda kreira EchoMsg
        /// </summary>
        /// <param name="buyerID">Element BuyerID u Header-u i podacima SOAP poruke</param>
        /// <param name="echoText">Element Echo u podacima SOAP poruke</param>
        /// <returns>EchoMsg</returns>
        public EchoMsg GetEchoMsg(string supplierID, string echoText)
        {
            EchoMsg message = new EchoMsg();

            //Header
            HeaderSupplierType header = new HeaderSupplierType();
            header.MessageID = Guid.NewGuid().ToString();
            header.SupplierID = supplierID;
            header.MessageType = B2G_ECHO_MESSAGE_TYPE;

            message.HeaderSupplier = header;

            //Data
            EchoMsgData data = new EchoMsgData();

            //Echo
            EchoMsgDataEchoData echoData = new EchoMsgDataEchoData();
            echoData.Echo = echoText;

            data.EchoData = echoData;

            message.Data = data;

            return message;
        }
    }
}
