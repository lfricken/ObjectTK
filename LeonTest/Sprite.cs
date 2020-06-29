using LeonTest;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Diagnostics;

namespace Examples.AdvancedExamples
{
    public class SpriteProgram : ShaderProgram
    {
        public Vector2 ScreenResolution = Vector2.Zero;
        public Matrix4 AdditionalTransform = Matrix4.Identity;
        Vector2 TextureResolution;
        public Texture Texture { get; protected set; }

        public void Make(Vector2 screenResolution, Vector2 textureRes, Texture texture)
        {
            Texture = texture;
            TextureResolution = textureRes;

            const string codePath = "../../../Sprite.glsl";
            ScreenResolution = screenResolution;

            Attach(ShaderType.VertexShader, codePath);
            Attach(ShaderType.FragmentShader, codePath);
            AssignTextureUnit("texture1", 1);
            Link();
        }


        protected override void OnBeforeUse()
        {

            {
                Trace.Assert(ScreenResolution != Vector2.Zero);
                var writeTransform = Matrix4.CreateOrthographicOffCenter(0, ScreenResolution.X, ScreenResolution.Y, 0, 0, -1);
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

            Texture.SetAs(TextureUnit.Texture0);
        }
    }

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

    public struct SpriteVertex : IVertex<SpriteVertex, SpriteData>
    {
        public Vector2 position;
        public Vector2 _texCoord;

        public unsafe SpriteVertex GenVertex(SpriteData data, int offset)
        {
            return offset switch
            {
                0 => new SpriteVertex() { position = data.WriteCoords.TopLeft(), _texCoord = data.ReadCoords.TopLeft(), },
                1 => new SpriteVertex() { position = data.WriteCoords.TopRight(), _texCoord = data.ReadCoords.TopRight(), },
                2 => new SpriteVertex() { position = data.WriteCoords.BotRight(), _texCoord = data.ReadCoords.BotRight(), },
                3 => new SpriteVertex() { position = data.WriteCoords.BotLeft(), _texCoord = data.ReadCoords.BotLeft(), },
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
                Trace.Assert(layoutLocation != -1, "\"position\" was not found in the shader");
                GL.VertexAttribPointer(layoutLocation, numAttribs, VertexAttribPointerType.Float, false, vertexStride, total);
                GL.EnableVertexAttribArray(layoutLocation);
                total += sizeof(Vector2);
            }

            { //_texCoord
                var numAttribs = 2;
                var layoutLocation = GL.GetAttribLocation(programHandle, nameof(_texCoord));
                Trace.Assert(layoutLocation != -1, "\"_texCoord\" was not found in the shader");
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
