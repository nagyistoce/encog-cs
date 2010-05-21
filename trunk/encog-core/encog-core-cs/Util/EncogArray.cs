// Encog(tm) Artificial Intelligence Framework v2.3
// .Net Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-java/
// 
// Copyright 2008-2010 by Heaton Research Inc.
// 
// Released under the LGPL.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
// 
// Encog and Heaton Research are Trademarks of Heaton Research, Inc.
// For information on Heaton Research trademarks, visit:
// 
// http://www.heatonresearch.com/copyright.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Encog.Util
{
    /// <summary>
    /// Simple array utilities for Encog.
    /// </summary>
    public class EncogArray
    {
        /// <summary>
        /// Completely copy one array into another. 
        /// </summary>
        /// <param name="src">Source array.</param>
        /// <param name="dst">Destination array.</param>
        public static void ArrayCopy(double[] src, double[] dst)
        {
            Array.Copy(src, dst, src.Length);
        }

        /// <summary>
        /// Calculate the product of two vectors (a scalar value).
        /// </summary>
        /// <param name="a">First vector to multiply.</param>
        /// <param name="b">Second vector to multiply.</param>
        /// <returns>The product of the two vectors (a scalar value).</returns>
        public static double VectorProduct(double[] a, double[] b)
        {
            int length = a.Length;
            double value = 0;

            for (int i = 0; i < length; ++i)
                value += a[i] * b[i];

            return value;
        }

        /// <summary>
        /// Allocate a 2D array of doubles.
        /// </summary>
        /// <param name="rows">The number of rows.</param>
        /// <param name="cols">The number of columns.</param>
        /// <returns>The array.</returns>
        public static double[][] AllocateDouble2D(int rows, int cols)
        {
            double[][] result = new double[rows][];
            for (int i = 0; i < rows; i++)
            {
                result[i] = new double[cols];
            }
            return result;
        }

        /// <summary>
        /// Allocate a 2D array of bools.
        /// </summary>
        /// <param name="rows">The number of rows.</param>
        /// <param name="cols">The number of columns.</param>
        /// <returns>The array.</returns>
        public static bool[][] AllocateBool2D(int rows, int cols)
        {
            bool[][] result = new bool[rows][];
            for (int i = 0; i < rows; i++)
            {
                result[i] = new bool[cols];
            }
            return result;
        }

        /// <summary>
        /// Copy an array of doubles.
        /// </summary>
        /// <param name="source">The source array.</param>
        /// <param name="sourceIndex">The source index.</param>
        /// <param name="output">The output array.</param>
        /// <param name="targetIndex">The output index.</param>
        /// <param name="size">The size to copy.</param>
        public static void ArrayCopy(double[] source, int sourceIndex, double[] output, int targetIndex, int size)
        {
            Array.Copy(source, sourceIndex, output, targetIndex, size);
        }

        /// <summary>
        /// Convert the collection to an array list of doubles.
        /// </summary>
        /// <param name="list">The list to convert.</param>
        /// <returns>The array of doubles.</returns>
        public static double[] ListToDouble(IList<double> list)
        {
            double[] result = new double[list.Count];
            int index = 0;
            foreach (Object obj in list)
            {
                result[index++] = (Double)obj;
            }

            return result;
        }

        /// <summary>
        /// Fill the specified array with the specified value.
        /// </summary>
        /// <param name="p">The array to fill.</param>
        /// <param name="value">The value to fill.</param>
        internal static void Fill(double[] p, double value)
        {
            for (int i = 0; i < p.Length; i++)
                p[i] = value;
        }
    }
}
