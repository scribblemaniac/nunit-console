﻿// ***********************************************************************
// Copyright (c) 2021 NUnit Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************
namespace NUnit.Engine
{
    using System.Collections.Generic;

    /// <summary>
    /// Extension methods to get some Linq-feeling in .NET 2.0 projects.
    /// </summary>
    internal static class IEnumerableExtensions
    {
        /// <summary>
        /// Gets a value indicating whether the enumeration contains at least one item or not.
        /// </summary>
        /// <typeparam name="T">The type of objects contained in the enumeration.</typeparam>
        /// <param name="enumeration">The enumeration.</param>
        /// <returns><see langword="true"/> if <paramref name="enumeration"/> contains at least one item; otherwhise, <see langword="false"/>.</returns>
        public static bool Any<T>(this IEnumerable<T> enumeration)
        {
            foreach (var _ in enumeration)
            {
                return true;
            }

            return false;
        }
    }
}
