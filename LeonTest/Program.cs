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

            public Game() : base(800, 800)
            {

            }

            // A simple vertex shader possible. Just passes through the position vector.
            const string VertexShaderSource =
            @"#version 330
// type


            in vec4 position;

            uniform float CenterMass;
            void main(void)
            {
                gl_Position = position;
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


            static int vertComponents = 3;//number of components per generic vertex attribute
            static int SizeOfPoint = vertComponents * sizeof(float);
            static int vboStride = SizeOfPoint; // + other objects

            // Points of a triangle in normalized device coordinates.
            readonly float[] Points = new float[] {
                 0.1f,  0.1f, 0.0f,  // top right
                 0.1f, -0.1f, 0.0f,  // bottom right
                -0.1f, -0.1f, 0.0f,  // bottom left
                -0.1f,  0.1f, 0.0f   // top left 
            };

            readonly int[] indices = new int[] {  // note that we start from 0!
                0, 1, 2,   // first triangle
                0, 2, 3,   // second triangle
            };

            int VertexShader;
            int FragmentShader;
            int ShaderProgram;
            int vao;
            int vbo;
            int ebo;

            protected override void OnLoad(EventArgs e)
            {
                // shaders
                {
                    var shader = new ShaderProgram();
                    shader.Attach(ShaderType.VertexShader, VertexShaderSource.Split("\r\n"));
                    shader.Attach(ShaderType.FragmentShader, FragmentShaderSource.Split("\r\n"));
                    shader.Link();

                    ShaderProgram = shader.Handle;
                }


                // buffers
                {
                    vao = GL.GenVertexArray();
                    vbo = GL.GenBuffer();
                    ebo = GL.GenBuffer();

                    // VAO
                    GL.BindVertexArray(vao);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);

                    GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(Points.Length * sizeof(float)), Points, BufferUsageHint.StaticDraw);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(indices.Length * sizeof(int)), indices, BufferUsageHint.StaticDraw);

                    var positionLocation = GL.GetAttribLocation(ShaderProgram, "position");
                    GL.VertexAttribPointer(positionLocation, vertComponents, VertexAttribPointerType.Float, false, vboStride, 0);
                    GL.EnableVertexAttribArray(positionLocation);

                    // unbind for sanity so we don't accidentally write later
                    GL.BindVertexArray(0);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
                }


                // random examples
                //{
                //    int count;
                //    // read shader variables
                //    GL.GetProgramInterface(ShaderProgram, ProgramInterface.ProgramInput, ProgramInterfaceParameter.ActiveResources, out count);
                //    for (int i = 0; i < count; ++i)
                //    {
                //        GL.GetActiveAttrib(ShaderProgram, 0, 256, out int len, out var size, out var type, out string name);
                //    }

                //    GL.GetProgramInterface(ShaderProgram, ProgramInterface.Uniform, ProgramInterfaceParameter.ActiveResources, out count);
                //    for (int i = 0; i < count; ++i)
                //    {
                //        GL.GetActiveAttrib(ShaderProgram, 0, 256, out int len, out var size, out var type, out string name);
                //    }

                //    GL.GetProgramInterface(ShaderProgram, ProgramInterface.ShaderStorageBlock, ProgramInterfaceParameter.ActiveResources, out count);
                //    for (int i = 0; i < count; ++i)
                //    {
                //        GL.GetActiveAttrib(ShaderProgram, 0, 256, out int len, out var size, out var type, out string name);
                //    }
                //    GL.GetProgramInterface(ShaderProgram, ProgramInterface.UniformBlock, ProgramInterfaceParameter.ActiveResources, out count);
                //    for (int i = 0; i < count; ++i)
                //    {
                //        GL.GetActiveAttrib(ShaderProgram, 0, 256, out int len, out var size, out var type, out string name);
                //    }
                //}



                // random final setup
                {
                    GL.PolygonMode(MaterialFace.Back, PolygonMode.Line); // make draw with lines if drawing backs
                    GL.ClearColor(0.0f, 0.0f, 1.0f, 0.0f); // set clear color
                }

                base.OnLoad(e);
            }

            protected override void OnUnload(EventArgs e)
            {
                // Unbind all the resources by binding the targets to 0/null.
                GL.BindVertexArray(0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
                GL.UseProgram(0);

                // Delete all the resources.
                GL.DeleteVertexArray(vao);
                GL.DeleteBuffer(vbo);
                GL.DeleteBuffer(ebo);
                GL.DeleteProgram(ShaderProgram);
                GL.DeleteShader(FragmentShader);
                GL.DeleteShader(VertexShader);

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

                GL.UseProgram(ShaderProgram);
                GL.BindVertexArray(vao);

                // uses the EBO implicitly (actually it's in the name silly!)
                GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

                Context.SwapBuffers();

                // Swap the front/back buffers so what we just rendered to the back buffer is displayed in the window.
                base.OnRenderFrame(e);
            }
        }
    }
}
