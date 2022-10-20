using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Security;
using System.IdentityModel.Tokens;

namespace B2GSendInvoicePKIClient.WSSecurity
{
    class B2GSecurityAlgorithmSuite : SecurityAlgorithmSuite
    {
        public override string DefaultAsymmetricKeyWrapAlgorithm
        {
            get { return SecurityAlgorithmSuite.Basic256Sha256Rsa15.DefaultAsymmetricKeyWrapAlgorithm; }
        }

        public override string DefaultAsymmetricSignatureAlgorithm
        {
            get { return SecurityAlgorithms.RsaSha1Signature; }
        }

        public override string DefaultCanonicalizationAlgorithm
        {
            get { return SecurityAlgorithmSuite.Basic256Sha256Rsa15.DefaultCanonicalizationAlgorithm; }
        }

        public override string DefaultDigestAlgorithm
        {
            get { return SecurityAlgorithmSuite.Basic256Rsa15.DefaultDigestAlgorithm; }
        }

        public override string DefaultEncryptionAlgorithm
        {
            get { return SecurityAlgorithmSuite.Basic256Sha256Rsa15.DefaultEncryptionAlgorithm; }
        }

        public override int DefaultEncryptionKeyDerivationLength
        {
            get { return SecurityAlgorithmSuite.Basic256Sha256Rsa15.DefaultEncryptionKeyDerivationLength; }
        }

        public override string DefaultSymmetricKeyWrapAlgorithm
        {
            get { return SecurityAlgorithmSuite.Basic256Sha256Rsa15.DefaultSymmetricKeyWrapAlgorithm; }
        }

        public override string DefaultSymmetricSignatureAlgorithm
        {
            get { return SecurityAlgorithmSuite.Basic256Sha256Rsa15.DefaultSymmetricSignatureAlgorithm; }
        }

        public override int DefaultSignatureKeyDerivationLength
        {
            get { return SecurityAlgorithmSuite.Basic256Sha256Rsa15.DefaultSignatureKeyDerivationLength; }
        }

        public override int DefaultSymmetricKeyLength
        {
            get { return SecurityAlgorithmSuite.Basic256Sha256Rsa15.DefaultSymmetricKeyLength; }
        }


        public override bool IsAsymmetricKeyLengthSupported(int i)
        {
            return SecurityAlgorithmSuite.Basic256Sha256Rsa15.IsAsymmetricKeyLengthSupported(i);
        }

        public override bool IsSymmetricKeyLengthSupported(int i)
        {
            return SecurityAlgorithmSuite.Basic256Sha256Rsa15.IsSymmetricKeyLengthSupported(i);
        }
    }
}
