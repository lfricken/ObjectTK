using LeonTest;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Diagnostics;

namespace Examples.AdvancedExamples
{
    /// <summary>
    /// 
    /// </summary>
    public class SpriteProgram : ShaderProgram
    {
        public Vector2 ScreenResolution = Vector2.Zero;
        public Matrix4 AdditionalTransform = Matrix4.Identity;
        Vector2 TextureResolution;
        public Texture Texture0 { get; protected set; }
        public Texture Texture1 { get; protected set; }

        public void Make(Vector2 screenResolution, Vector2 textureRes, Texture texture0, Texture texture1)
        {
            Texture0 = texture0;
            Texture1 = texture1;
            TextureResolution = textureRes;

            const string codePath = "../../../Sprite.glsl";
            ScreenResolution = screenResolution;

            Attach(ShaderType.VertexShader, codePath);
            Attach(ShaderType.FragmentShader, codePath);
            Link();

            AssignTextureUnit(nameof(texture0), 0);
            AssignTextureUnit(nameof(texture1), 1);
        }


        protected override void OnBeforeUse()
        {

            {
                Trace.Assert(ScreenResolution != Vector2.Zero);
                var writeTransform = Matrix4.CreateOrthographicOffCenter(0, ScreenResolution.X, ScreenResolution.Y, 0, 0, -1);
                writeTransform = AdditionalTransform * writeTransform;
                var loc = GL.GetUniformLocation(Handle, "writeTransform");
                Trace.Assert(loc != -1, "\"writeTransform\" was not found in the shader");
                GL.ProgramUniformMatrix4(Handle, loc, false, ref writeTransform);
            }
            {
                var readTransform = new Vector2(1f / TextureResolution.X, 1f / TextureResolution.Y);
                var loc = GL.GetUniformLocation(Handle, "readTransform");
                Trace.Assert(loc != -1, "\"readTransform\" was not found in the shader");
                GL.ProgramUniform2(Handle, loc, ref readTransform);
            }


            Texture0.SetAs(TextureUnit.Texture0);
            Texture1.SetAs(TextureUnit.Texture1);
        }
    }



    /// <summary>
    /// 
    /// </summary>
    public struct SpriteData
    {
        public SpriteData(Rectangle write, Rectangle read)
        {
            WriteCoords = write;
            ReadCoords = read;
        }

        public Rectangle WriteCoords;
        public Rectangle ReadCoords;
    }



    /// <summary>
    /// 
    /// </summary>
    public struct SpriteVertex : IVertex<SpriteVertex, SpriteData>
    {
        public Vector2 position;
        public Vector2 texCoord;

        public unsafe SpriteVertex GenVertex(SpriteData data, int offset)
        {
            return offset switch
            {
                0 => new SpriteVertex() { position = data.WriteCoords.TopLeft(), texCoord = data.ReadCoords.TopLeft(), },
                1 => new SpriteVertex() { position = data.WriteCoords.TopRight(), texCoord = data.ReadCoords.TopRight(), },
                2 => new SpriteVertex() { position = data.WriteCoords.BotRight(), texCoord = data.ReadCoords.BotRight(), },
                3 => new SpriteVertex() { position = data.WriteCoords.BotLeft(), texCoord = data.ReadCoords.BotLeft(), },
                _ => throw new Exception($"bad Vertex offset in {nameof(GenVertex)}"),
            };
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value")]
        public unsafe void SetVertAttributes(int programHandle)
        {
            var vertexStride = GetSize();
            int total = 0;
            {
                var numAttribs = 2;
                var layoutLocation = GL.GetAttribLocation(programHandle, nameof(position));
                Trace.Assert(layoutLocation != -1, $"\"{nameof(position)}\" was not found in the shader");
                GL.VertexAttribPointer(layoutLocation, numAttribs, VertexAttribPointerType.Float, false, vertexStride, total);
                GL.EnableVertexAttribArray(layoutLocation);
                total += sizeof(Vector2);
            }

            { //_texCoord
                var numAttribs = 2;
                var layoutLocation = GL.GetAttribLocation(programHandle, nameof(texCoord));
                Trace.Assert(layoutLocation != -1, $"\"{nameof(texCoord)}\" was not found in the shader");
                GL.VertexAttribPointer(layoutLocation, numAttribs, VertexAttribPointerType.Float, false, vertexStride, total);
                GL.EnableVertexAttribArray(layoutLocation);
                total += sizeof(Vector2);
            }
        }
        public unsafe int GetSize()
        {
            return sizeof(SpriteVertex);
        }
    }
}
