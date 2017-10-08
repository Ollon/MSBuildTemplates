using System;
using System.Runtime.InteropServices;

namespace Ollon.VisualStudio.Extensibility.StrongName
{
    public static class StrongNameKeyManager
    {
        public static StrongNameKeyInfo GenerateStrongNameKeyInfo()
        {
            StrongNameHelpers.StrongNameKeyGen(null, 0, out IntPtr ppbKeyBlob, out int pcbKeyBlob);
            byte[] strongNameKey = new byte[pcbKeyBlob];
            Marshal.Copy(ppbKeyBlob, strongNameKey, 0, pcbKeyBlob);

            StrongNameHelpers.StrongNameGetPublicKey(null, ppbKeyBlob, pcbKeyBlob, out IntPtr ppbPublicKeyBlob, out int pcbPublicKeyBlob);
            byte[] publicKey = new byte[pcbPublicKeyBlob];
            Marshal.Copy(ppbPublicKeyBlob, publicKey, 0, pcbPublicKeyBlob);

            StrongNameHelpers.StrongNameTokenFromPublicKey(ppbPublicKeyBlob, pcbPublicKeyBlob, out IntPtr ppbStrongNameToken, out int pcbStrongNameToken);
            byte[] publicKeyToken = new byte[pcbStrongNameToken];
            Marshal.Copy(ppbStrongNameToken, publicKeyToken, 0, pcbStrongNameToken);

            return new StrongNameKeyInfo(strongNameKey, publicKey, publicKeyToken);
        }
    }
}
