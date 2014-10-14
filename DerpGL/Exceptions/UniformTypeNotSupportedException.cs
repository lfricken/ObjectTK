#region License
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
using System;

namespace DerpGL.Exceptions
{
    internal class UniformTypeNotSupportedException
        : DerpGLException
    {
        public readonly Type UniformType;
        
        public UniformTypeNotSupportedException(Type uniformType)
            : base(string.Format("Uniforms of type {0} are not supported", uniformType.Name))
        {
            UniformType = uniformType;
        }
    }
}