﻿using System;
using System.IO;
using Android.OS;
using ExpressBase.Mobile.Droid.Helpers;
using ExpressBase.Mobile.Enums;
using ExpressBase.Mobile.Helpers;
using static Android.Provider.Settings;

[assembly: Xamarin.Forms.Dependency(typeof(NativeHelper))]

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
                return info.VersionName;
            }
        }

        public void Close()
        {
            try { Process.KillProcess(Process.MyPid()); } catch { }
        }

        public bool Exist(string Path, SysContentType Type)
        {
            try
            {
                string pathToNewFolder = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + $"/{Path}";

                if (Type == SysContentType.File)
                    return File.Exists(pathToNewFolder);
                else
                    return Directory.Exists(pathToNewFolder);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        public string Create(string DirectoryPath, SysContentType Type)
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

        public byte[] GetFile(string url)
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

        public string GetAssetsURl()
        {
            return "file:///android_asset/";
        }

        public void WriteLogs(string message, LogTypes logType)
        {
            try
            {
                string sid = App.Settings.Sid.ToUpper();
                string root = App.Settings.AppDirectory;

                string path = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + $"/{root}/{sid}/logs.txt";

                // Create a string array with the additional lines of text
                string[] lines = {
                    $"{ DateTime.UtcNow } | {logType} | {message}"
                };

                File.AppendAllLines(path, lines);
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
            }
        }
    }
}