using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ExpressBase.Mobile.Droid.Helpers;
using ExpressBase.Mobile.Enums;
using ExpressBase.Mobile.Models;
using static Android.Provider.Settings;

[assembly: Xamarin.Forms.Dependency(typeof(NativeHelper))]
[assembly: Xamarin.Forms.Dependency(typeof(ToastMessage))]
namespace ExpressBase.Mobile.Droid.Helpers
{
    public class NativeHelper : INativeHelper
    {
        public NativeHelper() { }

        private string _devoiceid;

        public string NativeRoot => Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;

        public string DeviceId
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_devoiceid))
                    return _devoiceid;

                _devoiceid = Android.OS.Build.Serial;
                if (string.IsNullOrWhiteSpace(_devoiceid) || _devoiceid == Build.Unknown || _devoiceid == "0")
                {
                    try
                    {
                        var context = Android.App.Application.Context;
                        _devoiceid = Secure.GetString(context.ContentResolver, Secure.AndroidId);
                    }
                    catch (Exception ex)
                    {
                        Android.Util.Log.Warn("DeviceInfo", "Unable to get id: " + ex.ToString());
                    }
                }

                return _devoiceid;
            }
        }

        public string AppVersion
        {
            get
            {
                var context = Android.App.Application.Context;
                var info = context.PackageManager.GetPackageInfo(context.PackageName, 0);

                return $"{info.VersionName}.{info.VersionCode.ToString()}";
            }
        }

        public void CloseApp()
        {
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }

        public bool DirectoryOrFileExist(string Path, SysContentType Type)
        {
            try
            {
                var pathToNewFolder = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + $"/{Path}";
                if (Type == SysContentType.File)
                    return File.Exists(pathToNewFolder);
                else if (Type == SysContentType.Directory)
                    return Directory.Exists(pathToNewFolder);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        public string CreateDirectoryOrFile(string DirectoryPath, SysContentType Type)
        {
            try
            {
                string pathToNewFolder = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + $"/{DirectoryPath}";
                if (Type == SysContentType.Directory)
                    Directory.CreateDirectory(pathToNewFolder);
                else
                    File.Create(pathToNewFolder);

                return pathToNewFolder;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public byte[] GetPhoto(string url)
        {
            try
            {
                string path = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + $"/{url}";
                return File.ReadAllBytes(path);
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
            }
            return null;
        }

        public string[] GetFiles(string Url, string Pattern)
        {
            string path = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + $"/{Url}";
            return Directory.GetFiles(path, Pattern);
        }

        public string GetBaseURl()
        {
            return "file:///android_asset/";
        }

        public void WriteLogs(string message, LogTypes logType)
        {
            try
            {
                string sid = Settings.SolutionId.ToUpper();

                string path = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + $"/ExpressBase/{sid}/logs.txt";

                // Create a string array with the additional lines of text
                string[] lines = {
                    $"CREATED ON { DateTime.UtcNow }",
                    $"{logType.ToString()} : {message}"
                };

                File.AppendAllLines(path, lines);
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
            }
        }
    }

    public class ToastMessage : IToast
    {
        public void Show(string message)
        {
            Android.Widget.Toast.MakeText(Android.App.Application.Context, message, ToastLength.Long).Show();
        }
    }
}