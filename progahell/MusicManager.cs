using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaPlayer = System.Windows.Media.MediaPlayer;

namespace progahell
{
    public class MusicManager
    {
        private readonly MediaPlayer _player = new();
        private string _currentTrack = "";

        private static readonly Dictionary<string, string> TrackPaths = new(StringComparer.OrdinalIgnoreCase)
        {
            { "main_theme", "Audio/MioMao.mp3" },
            { "room_theme", "Audio/Shop.mp3" },
            { "discord_theme", "Audio/DiscordHalloween2022.mp3" },
            { "saddness_theme", "Audio/AnimationVsMath.mp3" },
            { "scary_theme", "Audio/смешарики.mp3" },
            { "best_theme", "Audio/Asgore.mp3" },
            { "good_theme", "Audio/Sans.mp3" },
            { "bad_theme", "Audio/FallenDown.mp3" },
            { "secret_theme", "Audio/Ooo.mp3" },

        };

        public void PlayTrack(string trackName)
        {
            if (string.IsNullOrEmpty(trackName))
            {
                Stop();
                return;
            }

            if (_currentTrack == trackName)
                return;

            // Остановить текущий трек ПЕРЕД загрузкой нового
            _player.Stop(); // ← ЭТО КЛЮЧЕВО!

            _currentTrack = trackName;

            if (TrackPaths.TryGetValue(trackName, out string relativePath))
            {
                try
                {
                    string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
                    if (File.Exists(fullPath))
                    {
                        _player.MediaEnded -= OnMediaEnded;
                        _player.Open(new Uri(fullPath));
                        _player.Volume = 0.3;
                        _player.MediaEnded += OnMediaEnded;
                        _player.Play();
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Трек не найден: {fullPath}");
                        _currentTrack = ""; // сбросить, так как трек не найден
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка воспроизведения: {ex.Message}");
                    _currentTrack = "";
                }
            }
            else
            {
                // Трек не найден в словаре
                _currentTrack = "";
            }
        }

        private void OnMediaEnded(object sender, EventArgs e)
        {
            _player.Position = TimeSpan.Zero;
            _player.Play();
        }

        public void Stop()
        {
            _player.Stop();
            _currentTrack = "";
        }

        public void SetVolume(double volume)
        {
            _player.Volume = Math.Max(0, Math.Min(1, volume));
        }
    }
}
