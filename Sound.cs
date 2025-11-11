using Raylib_cs;
using Un4seen.Bass;
using Un4seen.BassWasapi;
using System;
using System.Collections.Generic;

namespace Potesara;

/// <summary>
/// 非ストリーミング再生用のクラス (BassAudio)。
/// 音ゲー向けに同じサウンドを複数同時再生可能。
/// </summary>
public class Sound : IDisposable
{
    private int sample;
    private List<int> channels;      // 複数チャンネルを管理
    private int maxChannels;
    private bool isLoaded = false;

    public Sound(string fileName, float volume = 1.0f, int maxChannels = 8)
    {
        this.maxChannels = maxChannels;

        if (!Raylib.IsFileExtension(fileName, ".wav") &&
            !Raylib.IsFileExtension(fileName, ".mp3") &&
            !Raylib.IsFileExtension(fileName, ".ogg"))
        {
            throw new Exception($"対応していない音声ファイル形式です: {fileName}");
        }

        // 複数同時再生可能にサンプル作成
        sample = Bass.BASS_SampleLoad(fileName, 0, 0, maxChannels,
            BASSFlag.BASS_SAMPLE_SOFTWARE | BASSFlag.BASS_SAMPLE_FLOAT);

        if (sample == 0)
        {
            var err = Bass.BASS_ErrorGetCode();
            throw new Exception($"音声サンプルの作成失敗: {fileName} (BASS error {err})");
        }

        channels = new List<int>();
        for (int i = 0; i < maxChannels; i++)
        {
            channels.Add(0); // 初期は空
        }

        isLoaded = true;
        FileName = fileName;
        Volume = volume;
    }

    /// <summary>
    /// サウンドを再生します（空いているチャンネルで）。
    /// </summary>
    public void Play()
    {
        if (!isLoaded) return;

        int ch = Bass.BASS_SampleGetChannel(sample, BASSFlag.BASS_SAMPLE_OVER_POS); // ✅ 空きチャンネル取得
        if (ch != 0)
        {
            Bass.BASS_ChannelSetAttribute(ch, BASSAttribute.BASS_ATTRIB_VOL, Volume);
            Bass.BASS_ChannelPlay(ch, false);
        }
    }

    /// <summary>
    /// サウンドを停止します（全チャンネル）。
    /// </summary>
    public void Stop()
    {
        if (!isLoaded) return;

        foreach (var ch in channels)
        {
            if (ch != 0) Bass.BASS_ChannelStop(ch);
        }
    }

    /// <summary>
    /// サウンドの再生中判定（どれかのチャンネル）。
    /// </summary>
    public bool IsPlaying()
    {
        if (!isLoaded) return false;

        foreach (var ch in channels)
        {
            if (ch != 0 && Bass.BASS_ChannelIsActive(ch) == BASSActive.BASS_ACTIVE_PLAYING)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 音量を設定します。
    /// </summary>
    public void SetVolume(float volume)
    {
        Volume = Math.Clamp(volume, 0.0f, 1.0f);
    }

    /// <summary>
    /// ピッチを設定します（全チャンネル）。
    /// </summary>
    public void SetPitch(float pitch)
    {
        if (!isLoaded) return;

        foreach (var ch in channels)
        {
            if (ch != 0)
                Bass.BASS_ChannelSetAttribute(ch, BASSAttribute.BASS_ATTRIB_TEMPO_PITCH, pitch);
        }
    }

    /// <summary>
    /// パンを設定します（全チャンネル）。
    /// </summary>
    public void SetPan(float pan)
    {
        if (!isLoaded) return;

        foreach (var ch in channels)
        {
            if (ch != 0)
                Bass.BASS_ChannelSetAttribute(ch, BASSAttribute.BASS_ATTRIB_PAN, Math.Clamp(pan, -1.0f, 1.0f));
        }
    }

    public void Dispose()
    {
        if (!isLoaded) return;

        Bass.BASS_SampleFree(sample);
        isLoaded = false;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// サウンドの音量
    /// </summary>
    public float Volume { get; private set; }

    /// <summary>
    /// ファイルパス
    /// </summary>
    public string FileName { get; private set; }
}
