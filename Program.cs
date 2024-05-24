using CPUShaderToy;
using Raylib_cs;
using System.Diagnostics;
using System.Numerics;

float iTime = 0f;
Stopwatch timer = Stopwatch.StartNew();
Resolution iResolution = new Resolution();
long frameCount = 0;
BaseShader shader = new RaymarchingShader(iResolution);

Object syncObj = new object();

Raylib.InitWindow(iResolution.x, iResolution.y, "CPU Shader Toy");
//Raylib.ToggleFullscreen();

int renderWidth = Raylib.GetRenderWidth();
int renderHeight = Raylib.GetRenderHeight();

Color[,] buffer = new Color[Raylib.GetRenderWidth(), Raylib.GetRenderHeight()];
Byte[] buffer2 = new byte[Raylib.GetRenderWidth() * Raylib.GetRenderHeight() * 4];

byte DeNormalize(float v)
{
    if (v < 0) { return 0; }

    if (v > 1.0f) { v = 1; }

    byte max = Byte.MaxValue;

    return Convert.ToByte(v * max);
}

Stopwatch sw = new Stopwatch();
double frameTime = sw.Elapsed.TotalMilliseconds;


FractalPyramidShader fs = new FractalPyramidShader(iResolution);

while (!Raylib.WindowShouldClose())
{
    sw.Restart();

    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.White);


    //iTime = timer.ElapsedMilliseconds;
    iTime = (float)timer.Elapsed.TotalSeconds;    
    fs.iTime = iTime;
    

    //for (int x = 0; x < renderWidth; x++)
    Parallel.For(0, renderWidth, new ParallelOptions { MaxDegreeOfParallelism = 6 }, x =>
    {
        //for (int ty = 0; ty < renderHeight; ty++)
        Parallel.For(0, renderHeight, ty =>
        {
            //o y no Raylib comeca de cima pra baixo. no GLSL comeca de baixo pra cima. entao trocamos aqui.

            int y = Math.Abs(ty - renderHeight);

            Vector4 color;

            Vector2 pos = new Vector2(x, y);

            //mainImage(out color, pos);

            //shader 2

            
            
            //fs.mainImage(out color, pos);

            //
            shader.iTime = iTime;
            shader.iMouse.X = Raylib.GetMouseX();
            shader.iMouse.Y = Raylib.GetMouseY();
            shader.mainImage(out color, pos);
            

            Color c = new Color(
                    DeNormalize(color.X),
                    DeNormalize(color.Y),
                    DeNormalize(color.Z),
                    DeNormalize(color.W));


            //mas na hora de renderizar, usamos o ty
            pos.Y = ty;

            buffer[x, ty] = c;

            int idx = (ty * renderWidth + x) * 4;

            buffer2[idx]  = c.R;
            buffer2[idx + 1] = c.G;
            buffer2[idx + 2] = c.B;
            buffer2[idx + 3] = c.A;
            //Raylib.DrawPixelV(pos, c);











        //}
        });
     //}
    });

    

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
        fixed (byte* bPtr = &buffer2[0])
        {
            Raylib.UpdateTexture(t2, bPtr);
            Raylib.DrawTexture(t2, 0, 0, Color.White);
        }
    }
    

    

    /*
    for (int x = 0; x < renderWidth; x++)
    {
        for (int y = 0; y < renderHeight; y++)
        {
            Raylib.DrawPixel(x, y, buffer[x, y]);

            
        }
    }*/
     


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


//funcao para o o shader especifico.
Vector3 palette(float t)
{
    Vector3 a = new Vector3(0.5f, 0.5f, 0.5f);
    Vector3 b = new Vector3(0.5f, 0.5f, 0.5f);
    Vector3 c = new Vector3(1f, 1f, 1f);
    Vector3 d = new Vector3(0.263f, 0.416f, 0.557f);

    var res = c * t + d;
    var res2 = 6.28318f * res;

    Vector3 resfinal = new Vector3(
        (float)(a.X + b.X * (float)Math.Cos((float)res2.X)),
        (a.Y + b.Y * (float)Math.Cos((float)res2.Y)),
        (a.Z + b.Z * (float)Math.Cos((float)res2.Z))
        );

    return resfinal;
}

void mainImage(out Vector4 fragColor, Vector2 fragCoord)
{
    Vector2 uv = fragCoord / iResolution.xy;
    uv.X = uv.X - 0.5f;
    uv.Y = uv.Y - 0.5f;
    uv.X = uv.X * 2;
    uv.Y = uv.Y * 2;
    uv.X *= (iResolution.x / iResolution.y);

    Vector2 uv0 = uv;

    Vector3 finalColor = new Vector3(0.0f, 0.0f, 0.0f);

    for (int i = 0; i < 4; i++)
    { 

        uv *= 1.5f;
        uv.X = Util.Fract(uv.X);
        uv.Y = Util.Fract(uv.Y);
        uv.X = uv.X - 0.5f;
        uv.Y = uv.Y - 0.5f;

        float d = uv.Length() * (float)Math.Exp((double)-uv0.Length());

        Vector3 col = palette(uv0.Length() + i * 0.4f + iTime * 0.4f);

        d = (float)Math.Sin(d*8.0f + iTime) / 8f;
        d = Math.Abs(d);

        d = (float)Math.Pow(0.01 / (double)d, 1.2);
        //d = 0.02f / d;    

        finalColor += col * d;
    }

    fragColor = new Vector4(finalColor.X, finalColor.Y, finalColor.Z, 1);
}


