// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

#if VSExtension
namespace VSExtension
{
#endif
    class ProductInfo
    {
        public const string VERSION_PLACEHOLDER = "1.0.8784.0";
        public const string FILEVERSION_PLACEHOLDER = "1.0.8784.0";
        public const string PRODUCTNAME_PLACEHOLDER = "Graph Engine";
        public const string COMPANY_PLACEHOLDER = "Microsoft Corporation";
        public const string COPYRIGHT_PLACEHOLDER = "© Microsoft Corporation.  All rights reserved.";
        public const string TRADEMARK_PLACEHOLDER = "";
        public const string CULTURE_PLACEHOLDER = "";
    }

    class TrinityPublicKey
    {
        public const string Key4GeneratedCode =
            ", PublicKey=00240000048000009400000006020000002400005253413100040000010001006b575c58c683f1d104e387196723d95eeccfe0b540848fe2a992739d360046f8e9c726f6b39c928baf11b5a869bb05447d76a924261e593bc2e5e01bd280b6784d69d4c99495bfcab76fb4cc3b7d1eb7846a63cefe7e9034abc7de16fe17536df082d0de437917fc66809a9ec6967022d37c3a736b64a3f938d6603fa459e8bf";

        public const string Value = Key4GeneratedCode;

#if _PUBLIC_RELEASE_
    public const string MSPublicKey =
        ", PublicKey=0024000004800000940000000602000000240000525341310004000001000100b5fc90e7027f67871e773a8fde8938c81dd402ba65b9201d60593e96c492651e889cc13f1415ebb53fac1131ae0bd333c5ee6021672d9718ea31a8aebd0da0072f25d87dba6fc90ffd598ed4da35e44c398c454307e8e33b8426143daec9f596836f97c8f74750e5975c64e2189f45def46b2a2b1247adc3652bf5c308055da9";
#else
        public const string MSPublicKey = Key4GeneratedCode;
#endif
    }
#if VSExtension
}
#endif
