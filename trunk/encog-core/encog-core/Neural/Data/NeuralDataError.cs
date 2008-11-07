﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Neural.Data
{
    public class NeuralDataError: EncogError
    {
        /// <summary>
        /// Construct a message exception.
        /// </summary>
        /// <param name="str">The message.</param>
        public NeuralDataError(String str)
            : base(str)
        {
        }

        /// <summary>
        /// Pass on an exception.
        /// </summary>
        /// <param name="e">The other exception.</param>
        public NeuralDataError(Exception e)
            : base(e)
        {
        }
    }
}