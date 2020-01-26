using Discord.Audio;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;

namespace ubad.Services
{
    public class AudioService
    {
        public async Task SendAsync(IAudioClient client, string path)
        {
            // Create FFmpeg using the previous example
            using var ffmpeg = CreateStream(path);
            using var output = ffmpeg.StandardOutput.BaseStream;
            using var discord = client.CreatePCMStream(AudioApplication.Mixed);
            try
            {
                await output.CopyToAsync(discord);
            }
            finally
            {
                await discord.FlushAsync();
            }
        }

        internal async Task DownloadSong(string url, string outName = null)
        {
            var ytdl = new YoutubeDL();
            ytdl.OutputFolder = Environment.GetEnvironmentVariable("musicDirectory");
            ytdl.OutputFileTemplate = outName;
            await ytdl.RunAudioDownload(url, AudioConversionFormat.Mp3);

            //using var downloaded = DownloadYoutubeVideo(url);
            //await Task.Delay(1);
        }

        //private Process DownloadYoutubeVideo(string path)
        //{
        //    return Process.Start(new ProcessStartInfo
        //    {
        //        WorkingDirectory = Environment.GetEnvironmentVariable("musicDirectory"),
        //        FileName = " youtube-dl",
        //        Arguments = $"-x --audio-format mp3 {path} ",
        //        UseShellExecute = false,
        //        RedirectStandardOutput = true,
        //     }); ;
        //}

        private Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });
        }
    }
}