using NAudio.Wave;
using System;

namespace Tetris_Game
{
    public class AudioManager : IDisposable
    {
        private readonly IWavePlayer audioPlayer;
        public readonly AudioFileReader audioReader;

        public AudioManager(string filePath)
        {
            audioPlayer = new WaveOutEvent();
            audioReader = new AudioFileReader(filePath);
            audioPlayer.Init(audioReader);
        }

        public void Play()
        {
            audioReader.Position = 0; 
            audioPlayer.Play();
        }

        public void Continue()
        {
            audioPlayer.Play();
        }

        public void Stop()
        {
            audioPlayer.Stop();
        }

        public void SetVolume(float volume)
        {
            audioPlayer.Volume = volume;
        }

        public PlaybackState GetPlaybackState()
        {
            return audioPlayer.PlaybackState;
        }

        public void Pause()
        {
            if (audioPlayer.PlaybackState == PlaybackState.Playing)
            {
                audioPlayer.Pause();
            }
        }

        public void Resume()
        {
            if (audioPlayer.PlaybackState == PlaybackState.Paused)
            {
                audioPlayer.Play();
            }
        }

        public void Reset()
        {
            audioReader.Position = 0;
        }

        public void Dispose()
        {
            audioPlayer.Dispose();
            audioReader.Dispose();
        }
    }
}
