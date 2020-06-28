using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;

namespace Examples.AdvancedExamples
{
    public class SpriteProgram : ShaderProgram
    {
        public SpriteProgram(Vector2 res)
        {
            Resolution = res;
        }

        public Vector2 Resolution;
        public Matrix4 AdditionalTransform = Matrix4.Identity;

        protected override void OnBeforeUse()
        {
            var transform = AdditionalTransform * Matrix4.CreateOrthographicOffCenter(0, Resolution.X, Resolution.Y, 0, 0, -1);

            {
                var loc = GL.GetUniformLocation(Handle, "transform");
                Trace.Assert(loc != -1);
                GL.ProgramUniformMatrix4(Handle, loc, false, ref transform);
            }
        }
    }
}
