using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Examples.AdvancedExamples
{
    public class QuadHandle
    {
        public int index;
    }

    public interface IVertex
    {
        public unsafe int GetSize();
        public unsafe void SetVertAttributes(int programHandle);
    }

    public abstract class QuadBatch<TQuadData, TVertex> : IDisposable where TVertex : struct, IVertex
    {
        #region Lifecycle
        List<QuadHandle> handlesToUpdate = new List<QuadHandle>();
        List<QuadHandle> Handles = new List<QuadHandle>();
        List<TQuadData> Data = new List<TQuadData>();


        public ShaderProgram Shader;
        int vao;
        int vbo; readonly TVertex[] vArray;
        int ebo; readonly int[] iArray;


        int numQuads = 0;

        const int vPerQuad = 4;
        const int iPerQuad = 6;
        public QuadHandle AddQuad(TQuadData data)
        {
            var newHandle = new QuadHandle() { index = Handles.Count };

            Handles.Add(newHandle);
            Data.Add(data);
            UpdateQuad(newHandle);

            return newHandle;
        }

        protected abstract TVertex GenVertex(TQuadData data, int offset);

        public void UpdateQuad(QuadHandle _handle)
        {
            var handle = _handle.index;
            var data = Data[handle];
            Trace.Assert(handle < Handles.Count);

            {
                int[] verts = new int[4];
                // vertexes
                int vOffset = vPerQuad * handle;
                for (int i = 0; i < 4; ++i)
                {
                    var vIndex = i + vOffset;
                    verts[i] = vIndex;
                    vArray[vIndex] = GenVertex(data, i);
                }

                // indexes to those vertexes
                // see the index diagram at https://github.com/MonoGame/MonoGame/blob/develop/MonoGame.Framework/Graphics/SpriteBatcher.cs
                // except we flipped vertex 2 and 3
                int iOffset = iPerQuad * handle;
                iArray[iOffset + 0] = verts[0];
                iArray[iOffset + 1] = verts[1];
                iArray[iOffset + 2] = verts[2];

                iArray[iOffset + 3] = verts[0];
                iArray[iOffset + 4] = verts[2];
                iArray[iOffset + 5] = verts[3];
            }

            handlesToUpdate.Add(_handle);
        }
        public void Draw()
        {
            GL.BindVertexArray(vao);

            Shader.Use();
            // uses the EBO implicitly (actually it's in the name silly!)
            GL.DrawElements(PrimitiveType.Triangles, numQuads * iPerQuad, DrawElementsType.UnsignedInt, 0);
        }
        public unsafe QuadBatch(ShaderProgram shader, int maxQuads)
        {
            var maxTriangles = maxQuads * 2;
            vArray = new TVertex[maxQuads * vPerQuad];
            iArray = new int[maxQuads * iPerQuad];

            Shader = shader;
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            ebo = GL.GenBuffer();
            new TVertex().SetVertAttributes(Shader.Handle);
        }


        public void UpdateGpu()
        {
            UpdateAllGpuBuffers();
        }

        private void UpdateAllGpuBuffers()
        {
            // VAO
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);

            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(vArray.Length * new TVertex().GetSize()), vArray, BufferUsageHint.StaticDraw);
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(iArray.Length * sizeof(int)), iArray, BufferUsageHint.StaticDraw);


            // unbind for sanity so we don't accidentally write later
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
        #endregion



        #region Lifecycle
        bool _isDisposed;
        public void Dispose()
        {
            if (!_isDisposed)
            {
                GL.BindVertexArray(0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
                GL.UseProgram(0);

                Shader.Dispose();
                GL.DeleteVertexArray(vao);
                GL.DeleteBuffer(vbo);
                GL.DeleteBuffer(ebo);
            }
            _isDisposed = true;
        }
        ~QuadBatch()
        {
            Debug.Assert(_isDisposed);
            Dispose();
        }
        #endregion
    }
}
