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
            const string VertexShaderSource = @"
            #version 330
            in vec4 position;
            void main(void)
            {
                gl_Position = position;
            }
        ";

            // A simple fragment shader. Just a constant red color.
            const string FragmentShaderSource = @"
            #version 330
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
                0, 1, 3,   // first triangle
                3, 2, 1,   // second triangle
            };

            int VertexShader;
            int FragmentShader;
            int ShaderProgram;
            int vao;
            int vbo;
            int ebo;

            protected override void OnLoad(EventArgs e)
            {
                // shader
                {
                    // Load the source of the vertex shader and compile it.
                    VertexShader = GL.CreateShader(ShaderType.VertexShader);
                    GL.ShaderSource(VertexShader, VertexShaderSource);
                    GL.CompileShader(VertexShader);

                    // Load the source of the fragment shader and compile it.
                    FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
                    GL.ShaderSource(FragmentShader, FragmentShaderSource);
                    GL.CompileShader(FragmentShader);

                    // Create the shader program, attach the vertex and fragment shaders and link the program.
                    ShaderProgram = GL.CreateProgram();
                    GL.AttachShader(ShaderProgram, VertexShader);
                    GL.AttachShader(ShaderProgram, FragmentShader);
                    GL.LinkProgram(ShaderProgram);
                }

                vao = GL.GenVertexArray();
                vbo = GL.GenBuffer();
                ebo = GL.GenBuffer();

                // VAO
                GL.BindVertexArray(vao);

                // VBO
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(Points.Length * sizeof(float)), Points, BufferUsageHint.StaticDraw);

                // EBO
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
                GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(indices.Length * sizeof(int)), indices, BufferUsageHint.StaticDraw);


                var positionLocation = GL.GetAttribLocation(ShaderProgram, "position");
                GL.VertexAttribPointer(positionLocation, vertComponents, VertexAttribPointerType.Float, false, vboStride, 0);
                GL.EnableVertexAttribArray(positionLocation);

                // Set the clear color to blue
                GL.ClearColor(0.0f, 0.0f, 1.0f, 0.0f);

                base.OnLoad(e);



                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindVertexArray(0);
            }

            protected override void OnUnload(EventArgs e)
            {
                // Unbind all the resources by binding the targets to 0/null.
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindVertexArray(0);
                GL.UseProgram(0);

                // Delete all the resources.
                GL.DeleteBuffer(vbo);
                GL.DeleteVertexArray(vao);
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
                ErrorCode error;
                error = GL.GetError();

                //Clear the color buffer.
                GL.Clear(ClearBufferMask.ColorBufferBit);

                // Bind the VBO
                //GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);


                GL.UseProgram(ShaderProgram);
                GL.BindVertexArray(vao);

                GL.PolygonMode(MaterialFace.Back, PolygonMode.Line);
                GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);


                Context.SwapBuffers();

                // Swap the front/back buffers so what we just rendered to the back buffer is displayed in the window.
                base.OnRenderFrame(e);
            }
        }
    }
}
