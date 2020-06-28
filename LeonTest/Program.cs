using Examples.AdvancedExamples;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Diagnostics;

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

            // A simple vertex shader possible. Just passes through the position vector.
            const string VertexShaderSource =
            @"#version 440
//#define VertexShader 1
#if VertexShader
            uniform mat4 transform;

            in vec4 position;
            in float other;
            void main(void)
            {
                gl_Position = transform * position;
                gl_Position.x += 0.4 * other;
            }
#endif
        ";

            // A simple fragment shader. Just a constant red color.
            const string FragmentShaderSource =
            @"#version 440
//#define FragmentShader 1
#if FragmentShader
            out vec4 outputColor;
            void main(void)
            {
                outputColor = vec4(1.0, 0.0, 0.0, 1.0);
            }
#endif
        ";


            protected override unsafe void OnLoad(EventArgs e)
            {
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
