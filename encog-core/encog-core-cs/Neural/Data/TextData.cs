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
using Encog.Persist;
using Encog.Persist.Persistors;
#if logging
using log4net;
#endif
namespace Encog.Neural.Data
{
    /// <summary>
    /// An Encog object that can hold text data. This object can be stored in an
    /// Encog persisted file.
    /// </summary>
    [Serializable]
    public class TextData : IEncogPersistedObject
    {
        /// <summary>
        /// The text data that is stored.
        /// </summary>
        private String text;

        /// <summary>
        /// The name of this object.
        /// </summary>
        private String name;

        /// <summary>
        /// The description of this object.
        /// </summary>
        private String description;

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        [NonSerialized]
        private readonly ILog logger = LogManager.GetLogger(typeof(TextData));
#endif
        /// <summary>
        /// Clone this object.
        /// </summary>
        /// <returns>A cloned version of this object.</returns>
        public Object Clone()
        {
            TextData result = new TextData();
            result.Name = this.Name;
            result.Description = this.Description;
            result.Text = this.Text;
            return result;
        }



        /// <summary>
        /// The name of this object.
        /// </summary>
        public String Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// The description for this object.
        /// </summary>
        public String Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }

        /// <summary>
        /// The text that this object holds.
        /// </summary>
        public String Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
            }
        }

        /// <summary>
        /// Create a persistor to store this object.
        /// </summary>
        /// <returns>A persistor.</returns>
        public IPersistor CreatePersistor()
        {
            return new TextDataPersistor();
        }

    }

}
