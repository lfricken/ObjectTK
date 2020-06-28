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

    public interface IVertex<TVertex, TQuadData>
    {
        public unsafe TVertex GenVertex(TQuadData data, int offset);
        public unsafe int GetSize();
        public unsafe void SetVertAttributes(int programHandle);
    }

    public class QuadBatch<TShader, TQuadData, TVertex> : IDisposable where TVertex : struct, IVertex<TVertex, TQuadData> where TShader : ShaderProgram, new()
    {
        #region Lifecycle
        readonly List<QuadHandle> handlesToUpdate = new List<QuadHandle>();
        readonly List<QuadHandle> Handles = new List<QuadHandle>();
        readonly List<TQuadData> Data = new List<TQuadData>();
        readonly int MaxQuads;
        int NumQuads { get { return Handles.Count; } }


        public TShader Shader;
        readonly int vao;
        readonly int vbo; readonly TVertex[] vArray;
        readonly int ebo; readonly int[] iArray;


        const int vPerQuad = 4;
        const int iPerQuad = 6;
        public QuadBatch(int maxQuads)
        {
            MaxQuads = maxQuads;
            vArray = new TVertex[MaxQuads * vPerQuad];
            iArray = new int[MaxQuads * iPerQuad];

            Shader = new TShader();
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            ebo = GL.GenBuffer();
        }
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



        #region API
        public QuadHandle AddQuad(TQuadData data)
        {
            var newHandle = new QuadHandle() { index = Handles.Count };

            Handles.Add(newHandle);
            Trace.Assert(Handles.Count <= MaxQuads, "Too many quads added");


            Data.Add(data);
            UpdateQuad(newHandle);

            return newHandle;
        }
        public void RemoveQuad(QuadHandle toHandle)
        {
            var lastIndex = Handles.Count - 1;
            var fromHandle = Handles[lastIndex];

            // if they are equal, we don't need to do anything
            if (toHandle.index != Handles.Count - 1)
            {
                // copy last into another position
                Data[toHandle.index] = Data[lastIndex];
                Handles[toHandle.index] = Handles[lastIndex];

                // update the handle
                fromHandle.index = toHandle.index;
                UpdateQuad(fromHandle);
            }

            // remove the data for the last element
            Data.RemoveAt(lastIndex);
            Handles.RemoveAt(lastIndex);
        }
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
                    vArray[vIndex] = new TVertex().GenVertex(data, i);
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
        public void UpdateGpu()
        {
            const int min = 0;
            const int max = 0; // TODO
            if (handlesToUpdate.Count < min)
                return;

            if (handlesToUpdate.Count > max)
            {
                UpdateAllGpuBuffers();
            }
            else // between min and max
            {
                // TODO
                //foreach (var _handle in handlesToUpdate)
                //{
                //    var handle = _handle.index;

                //    // vertexes
                //    int startWritePosition = handle * vertSizeInBytes;
                //    int dataToSendReadStart = 0;
                //    int numDataToSend = vertsPerObject;

                //    // TODO the below could maybe could be optimized
                //    var dataToSend = Vertexes.Skip(handle).Take(numDataToSend).ToArray();
                //    VBuffer.SetData(startWritePosition, dataToSend, dataToSendReadStart, numDataToSend, vertSizeInBytes);

                //    // indexes shouldn't change
                //}
            }
        }
        void UpdateAllGpuBuffers()
        {
            // VAO
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            var size = new IntPtr(vArray.Length * new TVertex().GetSize());
            GL.BufferData(BufferTarget.ArrayBuffer, size, vArray, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(iArray.Length * sizeof(int)), iArray, BufferUsageHint.StaticDraw);

            new TVertex().SetVertAttributes(Shader.Handle);


            // unbind for sanity so we don't accidentally write later
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
        public void Draw()
        {
            GL.BindVertexArray(vao);

            Shader.Use();
            // uses the EBO implicitly (actually it's in the name silly!)
            GL.DrawElements(PrimitiveType.Triangles, NumQuads * iPerQuad, DrawElementsType.UnsignedInt, 0);
        }
        #endregion
    }
}
