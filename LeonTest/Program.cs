using Examples.AdvancedExamples;
using OpenTK;
using OpenTK.Graphics.OpenGL;
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


            public interface IVertex
            {
                public unsafe void SetVertAttributes(int programHandle);
            }


            public struct Vertex : IVertex
            {
                public Vector2 position;
                public float other;

                public unsafe void SetVertAttributes(int programHandle)
                {
                    var vertexStride = sizeof(Vertex);
                    int total = 0;
                    {
                        var numAttribs = 2;
                        var layoutLocation = GL.GetAttribLocation(programHandle, nameof(position));
                        GL.VertexAttribPointer(layoutLocation, numAttribs, VertexAttribPointerType.Float, false, vertexStride, total);
                        GL.EnableVertexAttribArray(layoutLocation);
                        total += sizeof(Vector2);
                    }

                    {
                        var numAttribs = 1;
                        var layoutLocation = GL.GetAttribLocation(programHandle, nameof(other));
                        GL.VertexAttribPointer(layoutLocation, numAttribs, VertexAttribPointerType.Float, false, vertexStride, total);
                        GL.EnableVertexAttribArray(layoutLocation);
                        total += sizeof(float);
                    }
                }
            }



            // vArray of a triangle in normalized device coordinates.
            readonly Vertex[] vArray = new Vertex[]
            {
                 new Vertex() { position = new Vector2(0, 0), other = 0 },  // top right
                 new Vertex() { position = new Vector2(100, 0), other = 0 },  // top right
                 new Vertex() { position = new Vector2(100, 100), other = 0 },  // top right
                 new Vertex() { position = new Vector2(0, 100), other = 0.1f },  // top right
            };

            // note that we start from 0!
            readonly int[] indices = new int[]
            {
                0, 1, 2,   // first triangle
                0, 2, 3,
            };

            ShaderProgram ShaderProgram;
            int vao;
            int vbo;
            int ebo;
            Matrix4 transform = QuadBatch.GenerateSpriteTransform(new Vector2(800, 800));

            protected override unsafe void OnLoad(EventArgs e)
            {
                // shaders
                {
                    var shader = new ShaderProgram();
                    shader.Attach(ShaderType.VertexShader, VertexShaderSource.Split("\r\n"));
                    shader.Attach(ShaderType.FragmentShader, FragmentShaderSource.Split("\r\n"));
                    shader.Link();

                    ShaderProgram = shader;
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

                    GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(vArray.Length * sizeof(Vertex)), vArray, BufferUsageHint.StaticDraw);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(indices.Length * sizeof(int)), indices, BufferUsageHint.StaticDraw);

                    new Vertex().SetVertAttributes(ShaderProgram.Handle);

                    {
                        var loc = GL.GetUniformLocation(ShaderProgram.Handle, "transform");
                        Trace.Assert(loc != -1);
                        GL.ProgramUniformMatrix4(ShaderProgram.Handle, loc, false, ref transform);
                    }

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

                GL.BindVertexArray(vao);

                ShaderProgram.Use();
                // uses the EBO implicitly (actually it's in the name silly!)
                GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

                Context.SwapBuffers();

                // Swap the front/back buffers so what we just rendered to the back buffer is displayed in the window.
                base.OnRenderFrame(e);
            }
        }
    }
}
