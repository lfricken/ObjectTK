﻿#region License
// DerpGL License
// Copyright (C) 2013-2014 J.C.Bernack
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
#endregion
using OpenTK.Graphics.OpenGL;

namespace DerpGL.Textures
{
    /// <summary>
    /// Represents a 2D texture.<br/>
    /// Images in this texture all are 2-dimensional. They have width and height, but no depth.
    /// </summary>
    public sealed class Texture2D
        : Texture
    {
        public override TextureTarget TextureTarget { get { return TextureTarget.Texture2D; } }

        /// <summary>
        /// The width of the texture.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// The height of the texture.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Allocates immutable texture storage with the given parameters.<br/>
        /// A value of zero for the number of mipmap levels will default to the maximum number of levels possible for the given bitmaps width and height.
        /// </summary>
        /// <param name="internalFormat">The internal format to allocate.</param>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        /// <param name="levels">The number of mipmap levels.</param>
        public Texture2D(SizedInternalFormat internalFormat, int width, int height, int levels = 0)
            : base(internalFormat, GetLevels(levels, width, height))
        {
            Width = width;
            Height = height;
            GL.BindTexture(TextureTarget, Handle);
            GL.TexStorage2D((TextureTarget2d)TextureTarget, Levels, InternalFormat, Width, Height);
            CheckError();
        }

        /// <summary>
        /// Internal constructor used by <see cref="TextureFactory"/> to wrap a Texture2D instance around an already existing texture.
        /// </summary>
        internal Texture2D(int textureHandle, SizedInternalFormat internalFormat, int width, int height, int levels)
            : base(textureHandle, internalFormat, levels)
        {
            Width = width;
            Height = height;
        }
    }
}