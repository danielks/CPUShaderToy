using CPUShaderToy;
using Raylib_cs;
using System.Diagnostics;
using System.Numerics;

float iTime = 0f;
Stopwatch timer = Stopwatch.StartNew();
Resolution iResolution = new Resolution();
long frameCount = 0;
BaseShader shader = new RaymarchingShader(iResolution);
//BaseShader shader = new IntroShader(iResolution);

Raylib.InitWindow(iResolution.x, iResolution.y, "CPU Shader Toy");
//Raylib.ToggleFullscreen();

int renderWidth = Raylib.GetRenderWidth();
int renderHeight = Raylib.GetRenderHeight();

Byte[] buffer = new Byte[Raylib.GetRenderWidth() * Raylib.GetRenderHeight() * 4];

Stopwatch sw = new Stopwatch();
double frameTime = sw.Elapsed.TotalMilliseconds;

while (!Raylib.WindowShouldClose())
{
    sw.Restart();

    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.White);
        
    iTime = (float)timer.Elapsed.TotalSeconds;    
    
    for (int x = 0; x < renderWidth; x++)
    //Parallel.For(0, renderWidth, new ParallelOptions { MaxDegreeOfParallelism = 6 }, x =>
    {
        for (int ty = 0; ty < renderHeight; ty++)
        //Parallel.For(0, renderHeight, ty =>
        {
            //o y no Raylib comeca de cima pra baixo. no GLSL comeca de baixo pra cima. entao trocamos aqui.

            int y = Math.Abs(ty - renderHeight);

            Vector4 color;

            Vector2 pos = new Vector2(x, y);            
                        
            shader.iTime = iTime;
            shader.iMouse.X = Raylib.GetMouseX();
            shader.iMouse.Y = Raylib.GetMouseY();
            shader.mainImage(out color, pos);

            Color c = Util.ColorFromVector4(color);


            //mas na hora de renderizar, usamos o ty
            pos.Y = ty;            

            int idx = (ty * renderWidth + x) * 4;

            buffer[idx]  = c.R;
            buffer[idx + 1] = c.G;
            buffer[idx + 2] = c.B;
            buffer[idx + 3] = c.A;
        }
        //});
     }
    //});    

    Image i2 = new Image
    {
        Format = PixelFormat.UncompressedR8G8B8A8,
        Width = renderWidth,
        Height = renderHeight,
        Mipmaps = 1
    };
    Raylib_cs.Texture2D t2 = Raylib.LoadTextureFromImage(i2);

    unsafe
    {
        fixed (byte* bPtr = &buffer[0])
        {
            Raylib.UpdateTexture(t2, bPtr);
            Raylib.DrawTexture(t2, 0, 0, Color.White);
        }
    }

    Raylib.EndDrawing();

    sw.Stop();
    frameTime = sw.Elapsed.TotalMilliseconds;

    if (frameCount % 50 == 0) //a cada 50 frames mostra o ultimo frametime
    {
        Raylib.SetWindowTitle(string.Format("{0}ms. FPS: {1}", frameTime, 1000.0f / frameTime));
    }

    frameCount++;
}

Raylib.CloseWindow();