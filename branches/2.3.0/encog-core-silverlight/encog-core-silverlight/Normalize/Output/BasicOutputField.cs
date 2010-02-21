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
using Encog.Persist.Attributes;

namespace Encog.Normalize.Output
{
    /// <summary>
    /// Provides very basic functionality for output fields.  Primarily provides
    /// the ideal instance variable.
    /// </summary>
    public abstract class BasicOutputField : IOutputField
    {
        /// <summary>
        /// Is this field part of the ideal data uses to train the
        /// neural network.
        /// </summary>
        [EGAttribute]
        private bool ideal;

        /// <summary>
        /// Init this field for a new row.
        /// </summary>
        public abstract void RowInit();

        /// <summary>
        /// The numebr of fields that will actually be generated by 
        /// this field. For a simple field, this value is 1.
        /// </summary>
        public abstract int SubfieldCount { get; }

        /// <summary>
        /// Calculate the value for this field.  Specify subfield of zero
        /// if this is a simple field.
        /// </summary>
        /// <param name="subfield"> The subfield index.</param>
        /// <returns>The calculated value for this field.</returns>
        public abstract double Calculate(int subfield);

        /// <summary>
        /// Is this field part of the ideal data uses to train the
        /// neural network.
        /// </summary>
        public bool Ideal
        {
            get
            {
                return this.ideal;
            }
            set
            {
                this.ideal = value;
            }
        }
    }
}
