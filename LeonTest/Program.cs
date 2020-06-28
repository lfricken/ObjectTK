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
            @"#version 330
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
            @"#version 330
// type
            out vec4 outputColor;
            void main(void)
            {
                outputColor = vec4(1.0, 0.0, 0.0, 1.0);
            }
        ";

            SpriteBatch batch;

            // vArray of a triangle in normalized device coordinates.
            readonly IVertex[] vArray = new IVertex[]
            {
                 new SpriteVertex() { position = new Vector2(0, 0), other = 0 },  // top right
                 new SpriteVertex() { position = new Vector2(100, 0), other = 0 },  // top right
                 new SpriteVertex() { position = new Vector2(100, 100), other = 0 },  // top right
                 new SpriteVertex() { position = new Vector2(0, 100), other = 0.1f },  // top right
            };

            protected override unsafe void OnLoad(EventArgs e)
            {
                {
                    var shader = new SpriteProgram(new Vector2(800, 800));
                    shader.Attach(ShaderType.VertexShader, VertexShaderSource.Split("\r\n"));
                    shader.Attach(ShaderType.FragmentShader, FragmentShaderSource.Split("\r\n"));
                    shader.Link();

                    batch = new SpriteBatch(shader, 1);

                    batch.AddQuad(new SpriteData() { TopLeft = new Vector2(0, 0), BottomRight = new Vector2(100, 100), });

                    batch.UpdateGpu();
                }


                // random final setup
                {
                    GL.PolygonMode(MaterialFace.Back, PolygonMode.Line); // make draw with lines if drawing backs
                    GL.ClearColor(0.0f, 0.0f, 1.0f, 0.0f); // set clear color
                }

                base.OnLoad(e);
            }

            protected override void OnUnload(EventArgs e)
            {
                batch.Dispose();
                base.OnUnload(e);
            }

            protected override void OnResize(EventArgs e)
            {
                // Resize the viewport to match the window size.
                GL.Viewport(0, 0, Width, Height);
                base.OnResize(e);
            }

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
