using Examples.AdvancedExamples;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Image = SixLabors.ImageSharp.Image;

namespace LeonTest
{
    class Program
    {
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
                //Load the image
                Image<Rgba32> image = Image.Load<Rgba32>("green.png");

                //ImageSharp loads from the top-left pixel, whereas OpenGL loads from the bottom-left, causing the texture to be flipped vertically.
                //This will correct that, making the texture display properly.
                image.Mutate(x => x.Flip(FlipMode.Vertical));

                //Get an array of the pixels, in ImageSharp's internal format.
                image.TryGetSinglePixelSpan(out var span);
                Rgba32[] pixels = span.ToArray();

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }


            protected override unsafe void OnLoad(EventArgs e)
            {
                LoadTex();

                {
                    batch = new QuadBatch<SpriteProgram, SpriteData, SpriteVertex>(4);
                    batch.Shader.Make(new Vector2(800, 800));

                    batch.AddQuad(new SpriteData() { TopLeft = new Vector2(0, 0), BottomRight = new Vector2(200, 200), });
                    batch.AddQuad(new SpriteData() { TopLeft = new Vector2(300, 300), BottomRight = new Vector2(350, 350), });
                    batch.UpdateGpu();
                }
                {
                    batch2 = new QuadBatch<SpriteProgram, SpriteData, SpriteVertex>(4);
                    batch2.Shader.Make(new Vector2(800, 800));

                    batch2.AddQuad(new SpriteData() { TopLeft = new Vector2(400, 0), BottomRight = new Vector2(500, 200), });
                    batch2.AddQuad(new SpriteData() { TopLeft = new Vector2(0, 500), BottomRight = new Vector2(100, 600), });
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
