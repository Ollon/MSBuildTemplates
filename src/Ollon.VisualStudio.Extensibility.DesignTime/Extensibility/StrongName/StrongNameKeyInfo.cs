namespace Ollon.VisualStudio.Extensibility.StrongName
{
    public struct StrongNameKeyInfo
    {
        public StrongNameKeyInfo(byte[] rawBytes, byte[] publicKey, byte[] publicKeyToken) : this()
        {
            RawBytes = rawBytes;
            RawPublicKey = publicKey;
            RawPublicKeyToken = publicKeyToken;
        }

        public string PublicKey => Hex.EncodeHexString(RawPublicKey);

        public string PublicKeyToken => Hex.EncodeHexString(RawPublicKeyToken).ToLower();

        public byte[] RawBytes { get; }
        public byte[] RawPublicKey { get; }
        public byte[] RawPublicKeyToken { get; }
    }
}