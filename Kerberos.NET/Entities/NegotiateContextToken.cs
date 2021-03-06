﻿using Kerberos.NET.Crypto;
using System;

namespace Kerberos.NET.Entities
{
    public sealed class NegotiateContextToken : ContextToken
    {
        public NegotiateContextToken(Asn1Element sequence, string mechType)
            : base(sequence)
        {
            if (MechType.NTLM == mechType)
            {
                NegotiationToken = new NegTokenInit() { MechToken = new MechToken().DecodeNtlm(sequence) };
            }
        }

        public NegTokenInit NegotiationToken;

        public NegTokenTarg? SubsequentContextToken;

        public override DecryptedData Decrypt(KeyTable keys)
        {
            if (NegotiationToken?.MechToken?.NtlmNegotiate != null)
            {
                throw new NotSupportedException("NTLM is not supported");
            }

            var token = NegotiationToken?.MechToken?.InnerContextToken;

            return Decrypt(token, keys);
        }

        protected override void ParseContextSpecific(Asn1Element element)
        {
            switch (element.ContextSpecificTag)
            {
                case 0:
                    NegotiationToken = new NegTokenInit().Decode(element[0]);
                    break;
                case 1:
                    SubsequentContextToken = new NegTokenTarg().Decode(element[0]);
                    break;
            }
        }
    }
}
