using OpenTK;
using System;
using System.Diagnostics;

namespace Examples.AdvancedExamples
{
    public class QuadBatch : IDisposable
    {
        #region Lifecycle
        public QuadBatch(ShaderProgram shader, int maxQuads)
        {

        }
        public static Matrix4 GenerateSpriteTransform(Vector2 vp)
        {
            return Matrix4.CreateOrthographicOffCenter(0, vp.X, vp.Y, 0, 0, -1);
        }
        #endregion



        #region Lifecycle
        bool _isDisposed;
        public void Dispose()
        {
            if (!_isDisposed)
            {
                //dispose
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
