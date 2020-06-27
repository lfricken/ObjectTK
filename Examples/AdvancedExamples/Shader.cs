using OpenTK.Graphics.OpenGL;
using System;
using System.Diagnostics;

namespace Examples.AdvancedExamples
{
    public class Shader : IDisposable
    {
        public int Handle { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shaderCode">
        /// Each line of the source code. They do not need to end in newlines. 
        /// The 0th line should be the version
        /// The 1st line should be a comment so we can insert a #define for the type
        /// </param>
        public Shader(string[] shaderCode, ShaderType type)
        {
            Handle = GL.CreateShader(type);

            // modify it
            Trace.Assert(shaderCode[0].Contains("#version"), "The first line in the shader should be the version"); // make sure the version is first
            Trace.Assert(shaderCode[1][0] == '/', "The second line in the shader should be a comment so we can replace it with a #define"); // make sure this is a comment


            shaderCode[1] = "#define " + Enum.GetName(typeof(ShaderType), type) + " 1";

            // compile it
            GL.ShaderSource(Handle, string.Join("\n", shaderCode));
            GL.CompileShader(Handle);

            // make sure it compiled
            GL.GetShader(Handle, ShaderParameter.CompileStatus, out int success);
            if (success != 1)
            {
                var log = GL.GetShaderInfoLog(Handle);
                throw new Exception(log);
            }
        }




        #region Lifecycle
        bool _isDisposed;
        public void Dispose()
        {
            if (!_isDisposed)
            {
                GL.DeleteShader(Handle);
            }
            _isDisposed = true;
        }
        ~Shader()
        {
            Debug.Assert(_isDisposed);
            Dispose();
        }
        #endregion
    }
}
