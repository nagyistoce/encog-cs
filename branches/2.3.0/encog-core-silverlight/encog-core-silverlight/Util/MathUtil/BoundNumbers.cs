﻿// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if logging
using log4net;
#endif

namespace Encog.Util.MathUtil
{
    /// <summary>
    /// A simple class that prevents numbers from getting either too
    /// big or too small.
    /// </summary>
    public sealed class BoundNumbers
    {

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(BoundNumbers));
#endif

        /// <summary>
        /// Private constructor.
        /// </summary>
        private BoundNumbers()
        {

        }

        /// <summary>
        /// Too small of a number.
        /// </summary>
        public const double TOO_SMALL = -1.0E20;

        /// <summary>
        /// Too big of a number.
        /// </summary>
        public const double TOO_BIG = 1.0E20;

        /// <summary>
        /// Bound the number so that it does not become too big or too small.
        /// </summary>
        /// <param name="d">The number to check.</param>
        /// <returns>The new number. Only changed if it was too big or too small.</returns>
        public static double Bound(double d)
        {
            if (d < TOO_SMALL)
            {
                return TOO_SMALL;
            }
            else if (d > TOO_BIG)
            {
                return TOO_BIG;
            }
            else
            {
                return d;
            }
        }
    }

}
