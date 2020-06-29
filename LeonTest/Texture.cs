using OpenTK.Graphics.OpenGL;
using SixLabors.ImageSharp.Processing;
using System;
using System.Diagnostics;

namespace Examples.AdvancedExamples
{
    public class Texture : IDisposable
    {
        #region Lifecycle
        public int Handle { get; protected set; }

        public Texture(string relativePath)
        {
            Handle = GL.GenTexture();

            Load(relativePath);
        }
        public void Dispose()
        {
            if (!_isDisposed)
            {
                GL.DeleteTexture(Handle);
            }
            _isDisposed = true;
        }
        bool _isDisposed;
        ~Texture()
        {
            Debug.Assert(_isDisposed);
            Dispose();
        }
        #endregion



        public void SetAs(TextureUnit unit)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }

        private void Load(string relativePath)
        {
            GL.BindTexture(TextureTarget.Texture2D, Handle);
            //Load the image
            SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32> image = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(relativePath);

            //ImageSharp loads from the top-left pixel, whereas OpenGL loads from the bottom-left, causing the texture to be flipped vertically.
            //This will correct that, making the texture display properly.
            image.Mutate(x => x.Flip(FlipMode.Vertical));

            //Get an array of the pixels, in ImageSharp's internal format.
            image.TryGetSinglePixelSpan(out var span);
            SixLabors.ImageSharp.PixelFormats.Rgba32[] pixels = span.ToArray();

            //put this data into the active texture
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }
    }
}
