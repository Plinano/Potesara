using System.Collections.Generic;
using Raylib_cs;

namespace Potesara;

public static class TextureManager
{
    private static readonly Dictionary<string, Texture> textures = new();

    /// <summary>
    /// テクスチャを登録します。
    /// </summary>
    /// <param name="key">登録名。</param>
    /// <param name="path">画像ファイルのパス。</param>
    public static void Register(string key, string path)
    {
        // すでに登録されている場合は削除して再登録
        if (textures.ContainsKey(key))
        {
            textures[key].Dispose();
            textures.Remove(key);
        }

        var tex = new Texture(path);
        if (tex.IsEnable)
        {
            // Bilinear フィルタを設定
            Raylib.SetTextureFilter(tex.RayTexture, TextureFilter.Bilinear);

            textures[key] = tex;
        }
        else
        {
            System.Console.WriteLine($"[TextureManager] Failed to load texture: {path}");
        }
    }

    /// <summary>
    /// 登録済みのテクスチャを取得します。
    /// </summary>
    public static Texture? Get(string key)
    {
        if (textures.TryGetValue(key, out var tex))
        {
            return tex;
        }
        else
        {
            System.Console.WriteLine($"[TextureManager] Texture not found: {key}");
            return null;
        }
    }

    /// <summary>
    /// テクスチャを削除します。
    /// </summary>
    public static void Unregister(string key)
    {
        if (textures.TryGetValue(key, out var tex))
        {
            tex.Dispose();
            textures.Remove(key);
        }
    }

    /// <summary>
    /// すべてのテクスチャを解放します。
    /// </summary>
    public static void UnregisterAll()
    {
        foreach (var tex in textures.Values)
        {
            tex.Dispose();
        }
        textures.Clear();
    }
}
