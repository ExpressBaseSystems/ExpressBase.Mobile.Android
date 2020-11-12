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

        public event EbEventHandler OnPlayerCompleted;

        MediaRecorder recorder;
        System.Timers.Timer timer;
        MediaPlayer player;

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
                //recorder.SetMaxDuration(MaximumDuration);
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

        public Task<int> StartPlaying(byte[] audioFile, Button playButton)
        {
            if (audioFile == null)
                return Task.FromResult(0);
            try
            {
                player = new MediaPlayer();
                player.Completion += (sender, e) => OnPlayerCompleted?.Invoke(playButton, null);
                player.SetDataSource($"data:audio;base64,{Convert.ToBase64String(audioFile)}");
                player.Prepare();
                player.Start();
            }
            catch (IOException ex)
            {
                EbLog.Error("There was an error trying to start the MediaPlayer!");
                EbLog.Error(ex.Message);
            }
            return Task.FromResult(player.Duration);
        }

        public void StopPlaying()
        {
            if (player == null)
                return;
            player.Stop();
            player = null;
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

        public int GetPlayerPosition()
        {
            if (player != null)
                return player.CurrentPosition;
            return 0;
        }
    }
}