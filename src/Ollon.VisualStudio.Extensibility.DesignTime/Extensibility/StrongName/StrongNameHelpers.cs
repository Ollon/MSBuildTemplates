using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;

namespace Ollon.VisualStudio.Extensibility.StrongName
{
    /// <summary>
    /// This was shamelessly ripped from microsoft.
    /// See https://referencesource.microsoft.com/#mscorlib/parent/parent/parent/parent/InternalApis/NDP_Common/inc/StrongNameHelpers.cs,47b03c4fa25f6f4f
    /// The methods here are designed to aid in transition from the v2 StrongName APIs on mscoree.dll to the
    /// v4 metahost APIs (which are in-proc SxS aware).
    /// </summary>
    public static class StrongNameHelpers
    {
        [ThreadStatic]
        private static int ts_LastStrongNameHR;
        [SecurityCritical]
        [ThreadStatic]
        private static IClrStrongName s_StrongName;
        private static IClrStrongName StrongName
        {
            [SecurityCritical]
            get
            {
                if (s_StrongName == null)
                {
                    s_StrongName = (IClrStrongName)RuntimeEnvironment.GetRuntimeInterfaceAsObject(
                        new Guid("B79B0ACD-F5CD-409b-B5A5-A16244610B92"),
                        new Guid("9FD93CCF-3280-4391-B3A9-96E1CDE77C8D"));
                }
                return s_StrongName;
            }
        }

        private static IClrStrongNameUsingIntPtr StrongNameUsingIntPtr
        {
            [SecurityCritical]
            get { return (IClrStrongNameUsingIntPtr)StrongName; }
        }

        [SecurityCritical]
        public static int StrongNameErrorInfo()
        {
            return ts_LastStrongNameHR;
        }

        [SecurityCritical]
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Microsoft.Runtime.Hosting.IClrStrongNameUsingIntPtr.StrongNameFreeBuffer(System.IntPtr)", Justification = "StrongNameFreeBuffer returns void but the new runtime wrappers return an HRESULT.")]
        public static void StrongNameFreeBuffer(IntPtr pbMemory)
        {
            StrongNameUsingIntPtr.StrongNameFreeBuffer(pbMemory);
        }

        [SecurityCritical]
        public static bool StrongNameGetPublicKey(string pwzKeyContainer, IntPtr pbKeyBlob, int cbKeyBlob, out IntPtr ppbPublicKeyBlob, out int pcbPublicKeyBlob)
        {
            int hr = StrongNameUsingIntPtr.StrongNameGetPublicKey(pwzKeyContainer, pbKeyBlob, cbKeyBlob, out ppbPublicKeyBlob, out pcbPublicKeyBlob);
            if (hr < 0)
            {
                ts_LastStrongNameHR = hr;
                ppbPublicKeyBlob = IntPtr.Zero;
                pcbPublicKeyBlob = 0;
                return false;
            }
            return true;
        }

        [SecurityCritical]
        public static bool StrongNameKeyDelete(string pwzKeyContainer)
        {
            int hr = StrongName.StrongNameKeyDelete(pwzKeyContainer);
            if (hr < 0)
            {
                ts_LastStrongNameHR = hr;
                return false;
            }
            return true;
        }

        [SecurityCritical]
        public static bool StrongNameKeyGen(string pwzKeyContainer, int dwFlags, out IntPtr ppbKeyBlob, out int pcbKeyBlob)
        {
            int hr = StrongName.StrongNameKeyGen(pwzKeyContainer, dwFlags, out ppbKeyBlob, out pcbKeyBlob);
            if (hr < 0)
            {
                ts_LastStrongNameHR = hr;
                ppbKeyBlob = IntPtr.Zero;
                pcbKeyBlob = 0;
                return false;
            }
            return true;
        }

        [SecurityCritical]
        public static bool StrongNameKeyInstall(string pwzKeyContainer, IntPtr pbKeyBlob, int cbKeyBlob)
        {
            int hr = StrongNameUsingIntPtr.StrongNameKeyInstall(pwzKeyContainer, pbKeyBlob, cbKeyBlob);
            if (hr < 0)
            {
                ts_LastStrongNameHR = hr;
                return false;
            }
            return true;
        }

        [SecurityCritical]
        public static bool StrongNameSignatureGeneration(string pwzFilePath, string pwzKeyContainer, IntPtr pbKeyBlob, int cbKeyBlob)
        {
            IntPtr ppbSignatureBlob = IntPtr.Zero;
            return StrongNameSignatureGeneration(pwzFilePath, pwzKeyContainer, pbKeyBlob, cbKeyBlob, ref ppbSignatureBlob, out int cbSignatureBlob);
        }

        [SecurityCritical]
        public static bool StrongNameSignatureGeneration(string pwzFilePath, string pwzKeyContainer, IntPtr pbKeyBlob, int cbKeyBlob, ref IntPtr ppbSignatureBlob, out int pcbSignatureBlob)
        {
            int hr = StrongNameUsingIntPtr.StrongNameSignatureGeneration(pwzFilePath, pwzKeyContainer, pbKeyBlob, cbKeyBlob, ppbSignatureBlob, out pcbSignatureBlob);
            if (hr < 0)
            {
                ts_LastStrongNameHR = hr;
                pcbSignatureBlob = 0;
                return false;
            }
            return true;
        }

        [SecurityCritical]
        public static bool StrongNameSignatureSize(IntPtr pbPublicKeyBlob, int cbPublicKeyBlob, out int pcbSize)
        {
            int hr = StrongNameUsingIntPtr.StrongNameSignatureSize(pbPublicKeyBlob, cbPublicKeyBlob, out pcbSize);
            if (hr < 0)
            {
                ts_LastStrongNameHR = hr;
                pcbSize = 0;
                return false;
            }
            return true;
        }

        [SecurityCritical]
        public static bool StrongNameSignatureVerification(string pwzFilePath, int dwInFlags, out int pdwOutFlags)
        {
            int hr = StrongName.StrongNameSignatureVerification(pwzFilePath, dwInFlags, out pdwOutFlags);
            if (hr < 0)
            {
                ts_LastStrongNameHR = hr;
                pdwOutFlags = 0;
                return false;
            }
            return true;
        }

        [SecurityCritical]
        public static bool StrongNameSignatureVerificationEx(string pwzFilePath, bool fForceVerification, out bool pfWasVerified)
        {
            int hr = StrongName.StrongNameSignatureVerificationEx(pwzFilePath, fForceVerification, out pfWasVerified);
            if (hr < 0)
            {
                ts_LastStrongNameHR = hr;
                pfWasVerified = false;
                return false;
            }
            return true;
        }

        [SecurityCritical]
        public static bool StrongNameTokenFromPublicKey(IntPtr pbPublicKeyBlob, int cbPublicKeyBlob, out IntPtr ppbStrongNameToken, out int pcbStrongNameToken)
        {
            int hr = StrongNameUsingIntPtr.StrongNameTokenFromPublicKey(pbPublicKeyBlob, cbPublicKeyBlob, out ppbStrongNameToken, out pcbStrongNameToken);
            if (hr < 0)
            {
                ts_LastStrongNameHR = hr;
                ppbStrongNameToken = IntPtr.Zero;
                pcbStrongNameToken = 0;
                return false;
            }
            return true;
        }

        [SecurityCritical]
        public static bool StrongNameSignatureSize(byte[] bPublicKeyBlob, int cbPublicKeyBlob, out int pcbSize)
        {
            int hr = StrongName.StrongNameSignatureSize(bPublicKeyBlob, cbPublicKeyBlob, out pcbSize);
            if (hr < 0)
            {
                ts_LastStrongNameHR = hr;
                pcbSize = 0;
                return false;
            }
            return true;
        }

        [SecurityCritical]
        public static bool StrongNameTokenFromPublicKey(byte[] bPublicKeyBlob, int cbPublicKeyBlob, out IntPtr ppbStrongNameToken, out int pcbStrongNameToken)
        {
            int hr = StrongName.StrongNameTokenFromPublicKey(bPublicKeyBlob, cbPublicKeyBlob, out ppbStrongNameToken, out pcbStrongNameToken);
            if (hr < 0)
            {
                ts_LastStrongNameHR = hr;
                ppbStrongNameToken = IntPtr.Zero;
                pcbStrongNameToken = 0;
                return false;
            }
            return true;
        }

        [SecurityCritical]
        public static bool StrongNameGetPublicKey(string pwzKeyContainer, byte[] bKeyBlob, int cbKeyBlob, out IntPtr ppbPublicKeyBlob, out int pcbPublicKeyBlob)
        {
            int hr = StrongName.StrongNameGetPublicKey(pwzKeyContainer, bKeyBlob, cbKeyBlob, out ppbPublicKeyBlob, out pcbPublicKeyBlob);
            if (hr < 0)
            {
                ts_LastStrongNameHR = hr;
                ppbPublicKeyBlob = IntPtr.Zero;
                pcbPublicKeyBlob = 0;
                return false;
            }
            return true;
        }

        [SecurityCritical]
        public static bool StrongNameKeyInstall(string pwzKeyContainer, byte[] bKeyBlob, int cbKeyBlob)
        {
            int hr = StrongName.StrongNameKeyInstall(pwzKeyContainer, bKeyBlob, cbKeyBlob);
            if (hr < 0)
            {
                ts_LastStrongNameHR = hr;
                return false;
            }
            return true;
        }

        [SecurityCritical]
        public static bool StrongNameSignatureGeneration(string pwzFilePath, string pwzKeyContainer, byte[] bKeyBlob, int cbKeyBlob)
        {
            IntPtr ppbSignatureBlob = IntPtr.Zero;
            return StrongNameSignatureGeneration(pwzFilePath, pwzKeyContainer, bKeyBlob, cbKeyBlob, ref ppbSignatureBlob, out int cbSignatureBlob);
        }

        [SecurityCritical]
        public static bool StrongNameSignatureGeneration(string pwzFilePath, string pwzKeyContainer, byte[] bKeyBlob, int cbKeyBlob, ref IntPtr ppbSignatureBlob, out int pcbSignatureBlob)
        {
            int hr = StrongName.StrongNameSignatureGeneration(pwzFilePath, pwzKeyContainer, bKeyBlob, cbKeyBlob, ppbSignatureBlob, out pcbSignatureBlob);
            if (hr < 0)
            {
                ts_LastStrongNameHR = hr;
                pcbSignatureBlob = 0;
                return false;
            }
            return true;
        }
    }
}
