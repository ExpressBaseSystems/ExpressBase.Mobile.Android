using System;
using System.IO;
using System.Threading.Tasks;
using Android.Media;
using ExpressBase.Mobile.Droid.Helpers;
using ExpressBase.Mobile.Helpers;
using ExpressBase.Mobile.Views.Base;
using Xamarin.Forms;

[assembly: Dependency(typeof(EbAudioHelper))]

namespace ExpressBase.Mobile.Droid.Helpers
{
    public class EbAudioHelper : IEbAudioHelper
    {
        readonly string filePath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/ebaudio_temp";

        public double MaximumDuration { get; set; }

        public event EbEventHandler OnRecordingCompleted;

        MediaRecorder recorder;

        System.Timers.Timer timer;

        public Task StartRecording()
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
                recorder.SetAudioEncoder(AudioEncoder.Aac);
                recorder.SetOutputFile(filePath);
                recorder.Prepare();
                recorder.Start();

                WatchDuration();
            }
            catch (Exception ex)
            {
                EbLog.Error(ex.Message);
            }
            return Task.FromResult(false);
        }

        public void StopRecording()
        {
            if (recorder == null) return;
            try
            {
                recorder.Stop();
                recorder.Reset();
                recorder.Release();

                if (timer != null && timer.Enabled) timer.Stop();

                if (File.Exists(filePath))
                {
                    byte[] note = File.ReadAllBytes(filePath);
                    OnRecordingCompleted?.Invoke(note, null);
                }
            }
            catch (Exception ex)
            {
                EbLog.Error(ex.Message);
            }
        }

        private void WatchDuration()
        {
            timer = new System.Timers.Timer(MaximumDuration);
            timer.Elapsed += OnMaximumDurationReached;
            timer.Start();
        }

        private void OnMaximumDurationReached(object sender, System.Timers.ElapsedEventArgs e)
        {
            timer.Stop();
            timer = null;
            Device.BeginInvokeOnMainThread(() => StopRecording());
        }
    }
}