using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.OS;
using ExpressBase.Mobile.Droid.Helpers;
using ExpressBase.Mobile.Enums;
using ExpressBase.Mobile.Helpers;
using Xamarin.Essentials;
using static Android.Provider.Settings;

[assembly: Xamarin.Forms.Dependency(typeof(NativeHelper))]

namespace ExpressBase.Mobile.Droid.Helpers
{
    public class NativeHelper : INativeHelper
    {
        public NativeHelper() { }

        private string _devoiceid;

        public string NativeRoot => FileSystem.AppDataDirectory;
        //public string NativeRoot => Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;

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
                string pathToNewFolder = NativeRoot + $"/{Path}";

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
                string pathToNewFolder = NativeRoot + $"/{DirectoryPath}";

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

        public string Delete(string DirectoryPath, SysContentType Type)
        {
            try
            {
                string pathToNewFolder = NativeRoot + $"/{DirectoryPath}";

                if (Type == SysContentType.Directory)
                    Directory.Delete(pathToNewFolder, true);
                else
                    File.Delete(pathToNewFolder);

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
                string path = NativeRoot + $"/{url}";
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
            string path = NativeRoot + $"/{Url}";
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

                string path = NativeRoot + $"/{sid}/logs.txt";

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

        public void BackupLogFiles()
        {
            try
            {
                const int SizeInKB = 1024;//1MB
                const int PersistDays = 30;

                string sid = App.Settings.Sid.ToUpper();

                string fileDir = NativeRoot + $"/{sid}";

                long length = new FileInfo(fileDir + "/logs.txt").Length;

                if (length > SizeInKB * 1024)
                {
                    string[] filePaths = Directory.GetFiles(fileDir, "logs_*.txt");

                    for (int i = 0; i < filePaths.Length; i++)
                    {
                        DateTime date = DateTime.MinValue;
                        try
                        {
                            string datePart = Regex.Match(Path.GetFileName(filePaths[i]), @"\d+").Value;
                            date = DateTime.ParseExact(datePart, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                        }
                        catch (Exception ex)
                        {

                        }

                        if (DateTime.UtcNow > date.AddDays(PersistDays))
                            File.Delete(filePaths[i]);
                    }

                    string newPath = NativeRoot + $"/{sid}/logs_{(DateTime.UtcNow.ToString("yyyyMMddHHmmss"))}.txt";

                    File.Move(fileDir + "/logs.txt", newPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void AddBackupLogFiles(List<EmailAttachment> Attachments)
        {
            try
            {
                string sid = App.Settings.Sid.ToUpper();
                string fileDir = NativeRoot + $"/{sid}";
                string[] filePaths = Directory.GetFiles(fileDir, "logs_*.txt");
                for (int i = 0; i < filePaths.Length; i++)
                {
                    Attachments.Add(new EmailAttachment(filePaths[i]));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("AddBackupLogFiles: " + ex.Message);
            }
        }

        public async Task<bool> RequestGalleryPermissionAsync()
        {
            if (DeviceInfo.Platform != DevicePlatform.Android)
                return true;

            if (DeviceInfo.Version.Major >= 13)
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Media>();
                if (status != PermissionStatus.Granted)
                    status = await Permissions.RequestAsync<Permissions.Media>();

                return status == PermissionStatus.Granted;
            }
            else
            {
                var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
                if (status != PermissionStatus.Granted)
                    status = await Permissions.RequestAsync<Permissions.StorageRead>();

                return status == PermissionStatus.Granted;
            }
        }

        public async Task<bool> RequestCameraPermissionAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
                status = await Permissions.RequestAsync<Permissions.Camera>();

            return status == PermissionStatus.Granted;
        }

    }
}