using System.Text.Json;
using Raylib_cs;

namespace Potesara;

public class Animation : IDisposable
{
    private readonly List<Texture> frames = new();
    private int currentFrame;
    private float timer;

    public bool Loop { get; private set; } = true;
    public bool Playing { get; private set; } = true;
    public float FrameTime { get; private set; }
    public float Opacity { get; set; } = 1.0f;

    public int CurrentFrame => currentFrame;
    public int FrameCount => frames.Count;

    public Animation(string folderPath)
    {
        // ---------- 設定読み込み ----------
        int fps = 60;

        string jsonPath = Path.Combine(folderPath, "anim.json");
        if (File.Exists(jsonPath))
        {
            using var doc = JsonDocument.Parse(File.ReadAllText(jsonPath));
            var root = doc.RootElement;

            if (root.TryGetProperty("Loop", out var loop))
                Loop = loop.GetBoolean();

            if (root.TryGetProperty("FPS", out var fpsProp))
                fps = fpsProp.GetInt32();
        }

        FrameTime = 1.0f / fps;

        // ---------- フレーム読み込み ----------
        int index = 0;
        while (true)
        {
            string path = Path.Combine(folderPath, $"{index}.png");
            if (!File.Exists(path))
                break;

            frames.Add(new Texture(path));
            index++;
        }

        if (frames.Count == 0)
            throw new Exception($"Animation: no frames in {folderPath}");
    }

    public void Update(float deltaTime)
    {
        if (!Playing || frames.Count == 0)
            return;

        timer += deltaTime;

        while (timer >= FrameTime)
        {
            timer -= FrameTime;
            currentFrame++;

            if (currentFrame >= frames.Count)
            {
                if (Loop)
                {
                    currentFrame = 0;
                }
                else
                {
                    currentFrame = frames.Count - 1;
                    Playing = false;
                    break;
                }
            }
        }
    }

    public void Draw(
        ReferencePoint? referencePoint,
        float x, float y,
        Rectangle? sourceRect = null,
        Color? color = null,
        float? rotation = null,
        System.Numerics.Vector2? scale = null,
        System.Numerics.Vector2? drawOrigin = null,
        bool reverseX = false,
        bool reverseY = false)
    {
        if (frames.Count == 0)
            return;

        Color drawColor;
        if (color.HasValue)
        {
            drawColor = color.Value;
        }
        else
        {
            int a = (int)(255 * Math.Clamp(Opacity, 0f, 1f));
            drawColor = new Color(255, 255, 255, a);
        }

        frames[currentFrame].Draw(
            referencePoint,
            x, y,
            sourceRect,
            drawColor,
            rotation,
            scale,
            drawOrigin,
            reverseX,
            reverseY
        );
    }

    public void Reset()
    {
        currentFrame = 0;
        timer = 0f;
        Playing = true;
    }

    public void Play()
    {
        Playing = true;
    }

    public void Stop()
    {
        Playing = false;
    }

    public void Dispose()
    {
        foreach (var tex in frames)
            tex.Dispose();

        frames.Clear();
    }
}
