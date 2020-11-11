using System;
using System.IO;
using System.Threading.Tasks;
using Android.Media;
using ExpressBase.Mobile.Droid.Helpers;
using ExpressBase.Mobile.Helpers;

[assembly: Xamarin.Forms.Dependency(typeof(EbAudioHelper))]

namespace ExpressBase.Mobile.Droid.Helpers
{
    public class EbAudioHelper : IEbAudioHelper
    {
        readonly string filePath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/ebaudio_temp";

        public bool StopRecordingAfterTimeout { get; set; }

        public TimeSpan TotalAudioTimeout { get; set; }

        MediaRecorder recorder;

        MediaPlayer player;

        public Task<string> StartRecording()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                recorder = new MediaRecorder();
                recorder.SetAudioSource(AudioSource.Mic);
                recorder.SetOutputFormat(OutputFormat.Mpeg4);
                recorder.SetAudioEncoder(AudioEncoder.AmrNb);
                recorder.SetOutputFile(filePath);
                recorder.Prepare();
                recorder.Start();
            }
            catch (Exception ex)
            {
                EbLog.Error(ex.Message);
            }
            return Task.FromResult(filePath);
        }

        public Task<byte[]> StopRecording()
        {
            if (recorder == null) return null;

            byte[] note = null;
            try
            {
                recorder.Stop();
                recorder.Release();

                if (File.Exists(filePath))
                {
                    note = File.ReadAllBytes(filePath);
                }
            }
            catch (Exception ex)
            {
                EbLog.Error(ex.Message);
            }
            return Task.FromResult(note);
        }

        public Task StartPlaying(byte[] audioFile)
        {
            if (audioFile == null) 
                return Task.FromResult(false);
            try
            {
                player = new MediaPlayer();

                player.Prepared += (sender, e) =>
                {
                    player.Start();
                };
                player.SetDataSource(new StreamMediaDataSource(new MemoryStream(audioFile)));
                player.Prepare();
            }
            catch (IOException ex)
            {
                EbLog.Error("There was an error trying to start the MediaPlayer!");
                EbLog.Error(ex.Message);
            }
            return Task.FromResult(false);
        }

        public void StopPlaying()
        {
            if (player == null)
                return;
            player.Stop();
            player.Dispose();
            player = null;
        }
    }

    public class StreamMediaDataSource : MediaDataSource
    {
        System.IO.Stream data;

        public StreamMediaDataSource(System.IO.Stream Data)
        {
            data = Data;
        }

        public override long Size
        {
            get
            {
                return data.Length;
            }
        }

        public override int ReadAt(long position, byte[] buffer, int offset, int size)
        {
            data.Seek(position, System.IO.SeekOrigin.Begin);
            return data.Read(buffer, offset, size);
        }

        public override void Close()
        {
            if (data != null)
            {
                data.Dispose();
                data = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (data != null)
            {
                data.Dispose();
                data = null;
            }
        }
    }
}