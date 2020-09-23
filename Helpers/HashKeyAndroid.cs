using System;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Auth.Api.Phone;
using Android.Util;
using ExpressBase.Mobile.Droid.Helpers;
using ExpressBase.Mobile.Helpers;
using Java.Security;
using Java.Util;

[assembly: Xamarin.Forms.Dependency(typeof(HashKeyAndroid))]

namespace ExpressBase.Mobile.Droid.Helpers
{
    public class HashKeyAndroid : IHashService
    {
        public string GenerateHashKey()
        {
            return new AppHashKeyHelper().GetAppHashKey(Application.Context);
        }

        public void StartSMSRetrieverReceiver()
        {
            SmsRetrieverClient client = SmsRetriever.GetClient(Application.Context);
            client.StartSmsRetriever();
        }
    }

    public class AppHashKeyHelper
    {
        private static string HASH_TYPE = "SHA-256";
        private static int NUM_HASHED_BYTES = 9;
        private static int NUM_BASE64_CHAR = 11;

        private static string GetPackageSignature(Context context)
        {
            PackageManager packageManager = context.PackageManager;
            var packageInfo = packageManager.GetPackageInfo(context.PackageName, PackageInfoFlags.Signatures);

            if (packageInfo.SigningInfo.HasMultipleSigners)
                return packageInfo.SigningInfo.GetApkContentsSigners().First().ToCharsString();
            else
                return packageInfo.SigningInfo.GetSigningCertificateHistory().First().ToCharsString();
        }

        public string GetAppHashKey(Context context)
        {
            string keystoreHexSignature = GetPackageSignature(context);

            String appInfo = context.PackageName + " " + keystoreHexSignature;
            try
            {
                MessageDigest messageDigest = MessageDigest.GetInstance(HASH_TYPE);
                messageDigest.Update(Encoding.UTF8.GetBytes(appInfo));
                byte[] hashSignature = messageDigest.Digest();

                hashSignature = Arrays.CopyOfRange(hashSignature, 0, NUM_HASHED_BYTES);
                String base64Hash = global::Android.Util.Base64.EncodeToString(hashSignature, Base64Flags.NoPadding | Base64Flags.NoWrap);
                base64Hash = base64Hash.Substring(0, NUM_BASE64_CHAR);

                return base64Hash;
            }
            catch (NoSuchAlgorithmException e)
            {
                return null;
            }
        }
    }
}