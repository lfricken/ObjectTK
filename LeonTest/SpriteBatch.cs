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

        protected override void OnBeforeUse()
        {
            Trace.Assert(ScreenResolution != Vector2.Zero);
            var transform = Matrix4.CreateOrthographicOffCenter(0, ScreenResolution.X, ScreenResolution.Y, 0, 0, -1);

            {
                var loc = GL.GetUniformLocation(Handle, "transform");
                Trace.Assert(loc != -1, "\"transform\" was not found in the shader");
                GL.ProgramUniformMatrix4(Handle, loc, false, ref transform);
            }
        }
    }

    public struct SpriteData
    {
        public Vector2 TopLeft;
        public Vector2 BottomRight;
    }

    public struct SpriteVertex : IVertex<SpriteVertex, SpriteData>
    {
        public Vector2 position;
        public float other;

        public unsafe SpriteVertex GenVertex(SpriteData data, int offset)
        {
            return offset switch
            {
                0 => new SpriteVertex() { position = data.TopLeft, other = 0, },
                1 => new SpriteVertex() { position = new Vector2(data.BottomRight.X, data.TopLeft.Y), other = 0, },
                2 => new SpriteVertex() { position = data.BottomRight, other = 0, },
                3 => new SpriteVertex() { position = new Vector2(data.TopLeft.X, data.BottomRight.Y), other = 0, },
                _ => throw new Exception($"bad Vertex offset in {nameof(GenVertex)}"),
            };
        }
        public unsafe int GetSize()
        {
            return sizeof(SpriteVertex);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value")]
        public unsafe void SetVertAttributes(int programHandle)
        {
            var vertexStride = GetSize();
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
}
