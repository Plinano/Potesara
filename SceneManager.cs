using Raylib_cs;
using System.Numerics;

namespace Potesara;

public class SceneManager
{
    private RenderTexture2D virtualScreen;

    public SceneManager()
    {
        Scenes = new List<Scene>();
        CurrentScene = null;

        // 仮想スクリーンサイズを固定
        virtualScreen = Raylib.LoadRenderTexture(1920, 1080);
    }

    /// <summary>
    /// 現在のシーンを切り替えます。
    /// </summary>
    /// <param name="scene">切り替えるシーン。</param>
    public void ChangeScene(Scene scene)
    {
        if (CurrentScene != null && CurrentScene != scene)
        {
            CurrentScene.Disable();
        }

        CurrentScene = scene;

        if (!Scenes.Contains(scene))
        {
            CurrentScene.Disable(); // 現在のシーンを無効化
        }

        CurrentScene.Enable(); // 新しいシーンを有効化
    }

    /// <summary>
    /// シーンを追加します（有効化はしません）。
    /// </summary>
    public void AddScene(Scene scene)
    {
        if (!Scenes.Contains(scene))
        {
            Scenes.Add(scene);
        }
    }

    /// <summary>
    /// シーンを削除します。
    /// </summary>
    public void RemoveScene(Scene scene)
    {
        if (Scenes.Contains(scene))
        {
            if (CurrentScene == scene)
            {
                CurrentScene = null; // 現在のシーンの場合はリセット
            }
            scene.Disable();
            Scenes.Remove(scene);
        }
    }

    /// <summary>
    /// すべてのシーンを削除します。
    /// </summary>
    public void DeleteAllScenes()
    {
        // 現在のシーンを無効化
        if (CurrentScene != null)
        {
            CurrentScene.Disable();
            CurrentScene = null;
        }

        // すべてのシーンを削除
        foreach (var scene in Scenes)
        {
            DeleteSceneRecursively(scene);
        }

        // シーンリストをクリア
        Scenes.Clear();
    }

    /// <summary>
    /// 再帰的にシーンを削除します。
    /// </summary>
    /// <param name="scene">削除するシーン。</param>
    private void DeleteSceneRecursively(Scene scene)
    {
        foreach (var child in scene.ChildScenes)
        {
            DeleteSceneRecursively(child);
        }

        scene.Disable(); // シーンを無効化
    }

    /// <summary>
    /// 仮想スクリーンに描画して、ウィンドウにスケーリングして表示
    /// </summary>
    public void Draw()
    {
        if (CurrentScene == null) return;

        // RenderTexture に描画開始
        Raylib.BeginTextureMode(virtualScreen);
        Raylib.ClearBackground(Color.Black);

        CurrentScene.Draw();

        Raylib.EndTextureMode();

        // フィルタリング設定
        Raylib.SetTextureFilter(virtualScreen.Texture, TextureFilter.Bilinear);

        float scaleX = (float)Raylib.GetScreenWidth() / virtualScreen.Texture.Width;
        float scaleY = (float)Raylib.GetScreenHeight() / virtualScreen.Texture.Height;

        Raylib.DrawTexturePro(
            virtualScreen.Texture,
            new Rectangle(0, 0, virtualScreen.Texture.Width, -virtualScreen.Texture.Height),
            new Rectangle(0, 0, virtualScreen.Texture.Width * scaleX, virtualScreen.Texture.Height * scaleY),
            new Vector2(0, 0),
            0,
            Color.White
        );
    }

    /// <summary>
    /// 更新します（現在のシーンのみ）。
    /// </summary>
    public void Update()
    {
        CurrentScene?.Update();
    }

    public List<Scene> Scenes { get; private set; }
    public Scene CurrentScene { get; private set; }

    public void Unload()
    {
        Raylib.UnloadRenderTexture(virtualScreen);
    }
}
