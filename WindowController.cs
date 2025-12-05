using Raylib_cs;

namespace Potesara;

public class WindowController
{
    private readonly float targetAspect;
    private bool enableAspectLock = true;

    public WindowController(int baseWidth, int baseHeight)
    {
        targetAspect = (float)baseWidth / baseHeight;
    }

    /// <summary>
    /// アスペクト比固定をオン/オフ
    /// </summary>
    public void SetAspectLock(bool enable)
    {
        enableAspectLock = enable;
    }

    /// <summary>
    /// 毎フレーム呼び出し推奨（Update）
    /// </summary>
    public void Update()
    {
        if (!enableAspectLock) return;
        if (!Raylib.IsWindowResized()) return;

        int w = Raylib.GetScreenWidth();
        int h = Raylib.GetScreenHeight();

        float current = (float)w / h;

        if (Math.Abs(current - targetAspect) < 0.0001f) return;

        if (current > targetAspect)
        {
            h = (int)(w / targetAspect);
        }
        else
        {
            w = (int)(h * targetAspect);
        }

        Raylib.SetWindowSize(w, h);
    }
}
