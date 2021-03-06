﻿using Kerberos.NET.Crypto;
using System;
using System.Collections.Generic;

namespace Kerberos.NET.Entities
{
    public class Authenticator
    {
        public Authenticator Decode(Asn1Element asn1Element)
        {
            Asn1Element childNode = asn1Element[0];

            for (var i = 0; i < childNode.Count; i++)
            {
                var node = childNode[i];

                switch (node.ContextSpecificTag)
                {
                    case 0:
                        VersionNumber = node[0].AsLong();
                        break;
                    case 1:
                        Realm = node[0].AsString();
                        break;
                    case 2:
                        CName = new PrincipalName().Decode(node[0], Realm);
                        break;
                    case 3:
                        Checksum = node[0].Value;
                        break;
                    case 4:
                        CuSec = node[0].AsLong();
                        break;
                    case 5:
                        CTime = node[0].AsDateTimeOffset();
                        break;
                    case 6:
                        SubSessionKey = new EncryptionKey().Decode(node[0]);
                        break;
                    case 7:
                        SequenceNumber = node[0].AsLong();
                        break;
                    case 8:
                        var parent = node[0];

                        for (var p = 0; p < parent.Count; p++)
                        {
                            var azElements = AuthorizationDataElement.ParseElements(parent[p]);
                            
                            Authorizations.AddRange(azElements);
                        }
                        break;
                }
            }

            return this;
        }

        public long VersionNumber;

        public string Realm;

        public PrincipalName CName;

        public byte[] Checksum;

        public long CuSec;

        public DateTimeOffset CTime;

        [Obsolete]
        public byte[] Subkey { get { return SubSessionKey?.RawKey; } }

        public EncryptionKey SubSessionKey;

        public long SequenceNumber;

        private List<AuthorizationData> authorizations;

        public List<AuthorizationData> Authorizations { get { return authorizations ?? (authorizations = new List<AuthorizationData>()); } }

        public override string ToString()
        {
            return $"Version: {VersionNumber} | Realm: {Realm} | Sequence: {SequenceNumber}";
        }
    }
}
