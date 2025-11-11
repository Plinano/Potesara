using System;
using System.Collections.Generic;
using Un4seen.Bass;
using Un4seen.BassWasapi;

namespace Potesara
{
    public static class AudioManager
    {
        private static readonly Dictionary<string, Music> musics = new();
        private static readonly Dictionary<string, Sound> sounds = new();

        public static void RegisterMusic(string name, string filePath, bool loop = true, float volume = 1.0f)
        {
            if (!musics.ContainsKey(name))
            {
                try
                {
                    var music = new Music(filePath, loop, volume);
                    musics[name] = music;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Music Load Error] {filePath} : {ex.Message} (BASS error {Bass.BASS_ErrorGetCode()})");
                }
            }
        }

        public static Music? GetMusic(string name)
        {
            musics.TryGetValue(name, out var music);
            return music;
        }

        public static void PlayMusic(string name)
        {
            if (musics.TryGetValue(name, out var music))
            {
                music.Play();
            }
        }

        public static void StopMusic(string name)
        {
            if (musics.TryGetValue(name, out var music))
            {
                music.Stop();
            }
        }

        public static void StopAllMusic()
        {
            foreach (var music in musics.Values)
            {
                music.Stop();
            }
        }

        public static void UpdateMusic()
        {
            foreach (var music in musics.Values)
            {
                music.Update();
            }
        }

        public static void RegisterSound(string name, string filePath, float volume = 1.0f)
        {
            if (!sounds.ContainsKey(name))
            {
                try
                {
                    var sound = new Sound(filePath, volume);
                    sounds[name] = sound;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Sound Load Error] {filePath} : {ex.Message} (BASS error {Bass.BASS_ErrorGetCode()})");
                }
            }
        }

        public static Sound? GetSound(string name)
        {
            sounds.TryGetValue(name, out var sound);
            return sound;
        }

        public static void PlaySound(string name)
        {
            if (sounds.TryGetValue(name, out var sound))
            {
                sound.Play();
            }
        }

        public static void StopSound(string name)
        {
            if (sounds.TryGetValue(name, out var sound))
            {
                sound.Stop();
            }
        }

        // ===========================
        // 🔄 破棄
        // ===========================
        public static void UnloadAll()
        {
            foreach (var music in musics.Values)
            {
                try { music.Dispose(); } catch { }
            }
            foreach (var sound in sounds.Values)
            {
                try { sound.Dispose(); } catch { }
            }

            musics.Clear();
            sounds.Clear();
        }
    }
}
