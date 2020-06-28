using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Examples.AdvancedExamples
{
    public class ShaderProgram : IDisposable
    {
        public readonly int Handle;
        private List<int> shaders = new List<int>();

        public ShaderProgram()
        {
            Handle = GL.CreateProgram();
        }

        public int GetLocation(string name)
        {
            return GL.GetAttribLocation(Handle, name);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="shaderCode">
        /// Each line of the source code. They should not have newlines. 
        /// The 0th line should be the version
        /// The 1st line should be a comment so we can insert a #define for the type
        /// </param>
        public void Attach(ShaderType type, string[] shaderCode)
        {
            var shader = CreateShader(type, shaderCode);
            shaders.Add(shader);

            // attach it to the program
            GL.AttachShader(Handle, shader);
        }


        private static int CreateShader(ShaderType type, string[] shaderCode)
        {
            var shader = GL.CreateShader(type);

            // modify it
            Trace.Assert(shaderCode[0].Contains("#version"), "The first line in the shader should be the version"); // make sure the version is first
            Trace.Assert(shaderCode[1][0] == '/', "The second line in the shader should be a comment so we can replace it with a #define"); // make sure this is a comment
            shaderCode[1] = "#define " + Enum.GetName(typeof(ShaderType), type) + " 1";

            // compile it
            GL.ShaderSource(shader, string.Join("\n", shaderCode));
            GL.CompileShader(shader);

            // make sure it compiled
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success != 1)
            {
                var log = GL.GetShaderInfoLog(shader);
                throw new Exception(log);
            }

            return shader;
        }

        public void Link()
        {
            GL.LinkProgram(Handle);

            foreach (var shader in shaders)
            {
                GL.DeleteShader(shader);
            }
            shaders.Clear();
        }


        protected virtual void OnBeforeUse()
        {

        }
        public virtual void Use()
        {
            OnBeforeUse();
            GL.UseProgram(Handle);
        }



        #region Lifecycle
        bool _isDisposed;
        public void Dispose()
        {
            if (!_isDisposed)
            {
                GL.DeleteProgram(Handle);

                foreach (var shader in shaders)
                {
                    GL.DeleteShader(shader);
                }
            }
            _isDisposed = true;
        }
        ~ShaderProgram()
        {
            Trace.Assert(_isDisposed);
            Dispose();
        }
        #endregion
    }
}
