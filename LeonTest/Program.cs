using Examples.AdvancedExamples;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace LeonTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Game game = new Game())
            {
                game.Run(30.0);
            }
        }
        class Game : GameWindow
        {
            public Game() : base(800, 800) { }

            // A simple vertex shader possible. Just passes through the position vector.
            const string VertexShaderSource =
            @"#version 440
// type
            uniform mat4 transform;

            in vec4 position;
            in float other;
            void main(void)
            {
                gl_Position = transform * position;
                gl_Position.x += 0.4 * other;
            }
        ";

            // A simple fragment shader. Just a constant red color.
            const string FragmentShaderSource =
            @"#version 440
// type
            out vec4 outputColor;
            void main(void)
            {
                outputColor = vec4(1.0, 0.0, 0.0, 1.0);
            }
        ";


            protected override unsafe void OnLoad(EventArgs e)
            {
                {
                    batch = new QuadBatch<SpriteProgram, SpriteData, SpriteVertex>(4);
                    batch.Shader.Attach(ShaderType.VertexShader, VertexShaderSource.Split("\r\n"));
                    batch.Shader.Attach(ShaderType.FragmentShader, FragmentShaderSource.Split("\r\n"));
                    batch.Shader.Link();
                    batch.Shader.ScreenResolution = new Vector2(800, 800);

                    batch.AddQuad(new SpriteData() { TopLeft = new Vector2(0, 0), BottomRight = new Vector2(200, 200), });
                    batch.AddQuad(new SpriteData() { TopLeft = new Vector2(300, 300), BottomRight = new Vector2(350, 350), });
                    batch.UpdateGpu();
                }


                // random final setup
                {
                    GL.FrontFace(FrontFaceDirection.Cw);
                    GL.PolygonMode(MaterialFace.Back, PolygonMode.Line); // make draw with lines if drawing backs
                    GL.ClearColor(0.0f, 0.0f, 1.0f, 0.0f); // set clear color
                }

                base.OnLoad(e);
            }

            QuadBatch<SpriteProgram, SpriteData, SpriteVertex> batch;
            protected override void OnUnload(EventArgs e)
            {
                batch.Dispose();
                base.OnUnload(e);
            }

            protected override void OnResize(EventArgs e) { GL.Viewport(0, 0, Width, Height); base.OnResize(e); }

            protected override void OnRenderFrame(FrameEventArgs e)
            {
                GL.Clear(ClearBufferMask.ColorBufferBit);

                batch.Draw();
                Context.SwapBuffers();

                // Swap the front/back buffers so what we just rendered to the back buffer is displayed in the window.
                base.OnRenderFrame(e);
            }
        }
    }
}
