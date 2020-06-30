using LeonTest;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Diagnostics;

namespace Examples.AdvancedExamples
{

    public struct Cell
    {
        public float CleanAir;
        public float DirtyAir;
    }



    /// <summary>
    /// 
    /// </summary>
    public class ComputeProgram : ShaderProgram
    {
        int ssbo0;
        int ssbo1;

        int sideLength;
        int groups;

        public unsafe void Make(int _workPerGroup, int _groups)
        {
            groups = _groups;
            sideLength = _workPerGroup * _groups;
            const string codePath = "../../../ComputeProgram.glsl";
            Attach(ShaderType.ComputeShader, codePath);
            Link();

            var bufferSizeInBytes = sideLength * sizeof(Cell);
            ssbo0 = GL.GenBuffer();
            ssbo1 = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, ssbo0);
            GL.BufferData(BufferTarget.ShaderStorageBuffer, new IntPtr(bufferSizeInBytes), new IntPtr(0), BufferUsageHint.StaticDraw);


            var cellsa = GL.MapBufferRange(BufferTarget.ShaderStorageBuffer, new IntPtr(0), bufferSizeInBytes, BufferAccessMask.MapWriteBit | BufferAccessMask.MapInvalidateBufferBit);
            var cells = (Cell*)cellsa.ToPointer();
            for (int i = 0; i < sideLength; ++i)
            {
                cells[i].CleanAir = 1f;
                cells[i].DirtyAir = 0.5f;
            }
            GL.UnmapBuffer(BufferTarget.ShaderStorageBuffer);
        }


        public override void Use()
        {
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 4, ssbo0);

            GL.UseProgram(Handle);
            GL.DispatchCompute(groups, 1, 1);
            GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);
        }
    }
}
