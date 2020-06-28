using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace Examples.AdvancedExamples
{

    public class SpriteBatch : QuadBatch<SpriteData, SpriteVertex>
    {
        public SpriteBatch(SpriteProgram shader, int maxQuads) : base(shader, maxQuads)
        {

        }

        protected override SpriteVertex GenVertex(SpriteData data, int offset)
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
    }
    public struct SpriteData
    {
        public Vector2 TopLeft;
        public Vector2 BottomRight;
    }

    public struct SpriteVertex : IVertex
    {
        public Vector2 position;
        public float other;

        public unsafe int GetSize()
        {
            return sizeof(SpriteVertex);
        }
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
