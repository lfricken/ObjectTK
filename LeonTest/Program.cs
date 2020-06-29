using Examples.AdvancedExamples;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using SixLabors.ImageSharp.Processing;
using System;
using System.Diagnostics;
using Image = SixLabors.ImageSharp.Image;

namespace LeonTest
{
    public static class Program
    {
        public static Vector2 TopLeft(this Rectangle rect)
        {
            return new Vector2(rect.Left, rect.Top);
        }
        public static Vector2 TopRight(this Rectangle rect)
        {
            return new Vector2(rect.Right, rect.Top);
        }
        public static Vector2 BotRight(this Rectangle rect)
        {
            return new Vector2(rect.Right, rect.Bottom);
        }
        public static Vector2 BotLeft(this Rectangle rect)
        {
            return new Vector2(rect.Left, rect.Bottom);
        }

        static void Main(string[] args)
        {
            using (Game game = new Game())
            {
                game.Run();
            }
        }
        class Game : GameWindow
        {
            public Game() : base(800, 800) { }

            public void LoadTex()
            {

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            }


            protected override unsafe void OnLoad(EventArgs e)
            {
                LoadTex();

                var green = new Texture("green.png");
                var red = new Texture("red.png");

                {
                    batch = new QuadBatch<SpriteProgram, SpriteData, SpriteVertex>(4);
                    batch.Shader.Make(new Vector2(800, 800), new Vector2(338, 338), green);

                    batch.AddQuad(new SpriteData(new Rectangle(new Point(0, 0), new Size(200, 200)), new Rectangle(new Point(0, 0), new Size(200, 200))));
                    batch.AddQuad(new SpriteData(new Rectangle(new Point(300, 0), new Size(200, 200)), new Rectangle(new Point(100, 100), new Size(200, 200))));
                    batch.UpdateGpu();
                }
                {
                    batch2 = new QuadBatch<SpriteProgram, SpriteData, SpriteVertex>(4);
                    batch2.Shader.Make(new Vector2(800, 800), new Vector2(338, 338), red);

                    batch2.AddQuad(new SpriteData(new Rectangle(new Point(0, 300), new Size(200, 200)), new Rectangle(new Point(0, 0), new Size(200, 200))));
                    batch2.AddQuad(new SpriteData(new Rectangle(new Point(300, 300), new Size(200, 200)), new Rectangle(new Point(100, 100), new Size(200, 200))));
                    batch2.UpdateGpu();
                }

                watch.Start();
                // random final setup
                {
                    //var x = GLFW.SetMouseButtonCallback;

                    VSync = VSyncMode.Adaptive;
                    GL.FrontFace(FrontFaceDirection.Cw);
                    GL.PolygonMode(MaterialFace.Back, PolygonMode.Line); // make draw with lines if drawing backs
                    //GL.ClearColor(0.0f, 0.0f, 1.0f, 0.0f); // set clear color
                }
                int total = 0;
                for (int currentNumber = 1; currentNumber < 1000; currentNumber++)
                {
                    int iMod3 = currentNumber % 3;
                    int iMod5 = currentNumber % 5;
                    bool isDivisibleBy3 = iMod3 == 0;
                    bool isDivisibleBy5 = iMod5 == 0;
                    if (isDivisibleBy3 || isDivisibleBy5)
                    {
                        total += currentNumber;
                    }
                }
                base.OnLoad(e);
            }

            QuadBatch<SpriteProgram, SpriteData, SpriteVertex> batch;
            QuadBatch<SpriteProgram, SpriteData, SpriteVertex> batch2;
            protected override void OnUnload(EventArgs e)
            {
                batch.Dispose();
                batch2.Dispose();
                base.OnUnload(e);
            }

            protected override void OnResize(EventArgs e) { GL.Viewport(0, 0, Width, Height); base.OnResize(e); }

            Stopwatch watch = new Stopwatch();
            int frames = 0;
            protected override void OnRenderFrame(FrameEventArgs e)
            {
                var total = watch.Elapsed.TotalSeconds;
                watch.Restart();
                watch.Start();
                frames++;
                if (frames > 30)
                {
                    frames = 0;
                    Console.WriteLine($"FPS: {Math.Round(1d / total, 0)}");
                }

                //GL.Clear(ClearBufferMask.ColorBufferBit);

                batch2.Draw();
                batch.Draw();
                Context.SwapBuffers();

                // Swap the front/back buffers so what we just rendered to the back buffer is displayed in the window.
                base.OnRenderFrame(e);
            }

            protected override void OnMouseDown(MouseButtonEventArgs e)
            {
                base.OnMouseDown(e);
            }
        }
    }
}
