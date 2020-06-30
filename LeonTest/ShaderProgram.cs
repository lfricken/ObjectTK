using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Examples.AdvancedExamples
{
    public class ShaderProgram : IDisposable
    {
        #region Lifecycle
        public readonly int Handle;
        readonly List<int> shaders = new List<int>();
        public ShaderProgram()
        {
            Handle = GL.CreateProgram();
        }
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
        bool _isDisposed;
        ~ShaderProgram()
        {
            Trace.Assert(_isDisposed);
            Dispose();
        }
        #endregion



        #region API
        protected void AssignTextureUnit(string textureName, int textureUnit)
        {
            int location = GL.GetUniformLocation(Handle, textureName);
            Trace.Assert(location != -1);
            GL.ProgramUniform1(Handle, location, textureUnit);
        }
        protected int GetLocation(string name)
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
        protected void Attach(ShaderType type, string fullPathToCode)
        {
            var code = File.ReadAllLines(fullPathToCode);
            var shader = CreateShader(type, code);
            shaders.Add(shader);

            // attach it to the program
            GL.AttachShader(Handle, shader);
        }
        /// <summary>
        /// Cant get name of enum because of overlapping names
        /// </summary>
        static string GetShaderName(ShaderType type)
        {
            return type switch
            {
                ShaderType.VertexShader => nameof(ShaderType.VertexShader),
                ShaderType.ComputeShader => nameof(ShaderType.ComputeShader),
                ShaderType.FragmentShader => nameof(ShaderType.FragmentShader),
                _ => nameof(ShaderType.FragmentShader),
            };
        }
        static int CreateShader(ShaderType type, string[] shaderCode)
        {
            var shader = GL.CreateShader(type);

            // modify it
            Trace.Assert(shaderCode[0].Contains("#version"), "The first line in the shader should be the version"); // make sure the version is first
            Trace.Assert(shaderCode[1][0] == '/', "The second line in the shader should be a comment so we can replace it with a #define"); // make sure this is a comment
            shaderCode[1] = "#define " + "TypeIs_" + GetShaderName(type) + " 1";

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
        protected void Link()
        {
            GL.LinkProgram(Handle);

            foreach (var shader in shaders)
            {
                GL.DeleteShader(shader);
            }
            shaders.Clear();
        }
        public virtual void Use()
        {
            OnBeforeUse();
            GL.UseProgram(Handle);
        }
        protected virtual void OnBeforeUse() { }
        #endregion
    }
}
