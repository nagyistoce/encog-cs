﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Encog.Util.CSV
{
    /// <summary>
    /// Describes how to format number lists, such as CSV.
    /// </summary>
    public class CSVFormat
    {
        /// <summary>
        /// A format that uses a decimal point and a comma to separate fields.
        /// </summary>
        public static CSVFormat DECIMAL_POINT
        {
            get
            {
                return _DECIMAL_POINT;
            }
        }

        /// <summary>
        /// A format that uses a decimal comma and a semicolon to separate fields.
        /// </summary>
        public static CSVFormat DECIMAL_COMMA
        {
            get
            {
                return _DECIMAL_COMMA;
            }
        }

        /// <summary>
        /// The typical format for English speaking countries is a decimal
        /// point and a comma for field separation.  
        /// </summary>
        public static CSVFormat ENGLISH
        {
            get
            {
                return _DECIMAL_POINT;
            }
        }

        /// <summary>
        /// It is important that an EG file produced on one system, in one region
        /// be readable by another system in a different region.  Because of this
        /// EG files internally use a decimal point and comma separator.  Of course
        /// programs should display numbers to the user using regional settings.
        /// </summary>
        public static CSVFormat EG_FORMAT
        {
            get
            {
                return _DECIMAL_POINT;
            }
        }

        private static CSVFormat _DECIMAL_POINT = new CSVFormat('.', ',');
        private static CSVFormat _DECIMAL_COMMA = new CSVFormat(',', ';');
        private static CSVFormat _ENGLISH = DECIMAL_POINT;
        private static CSVFormat _NONENGLISH = DECIMAL_COMMA;
        private static CSVFormat _EG_FORMAT = DECIMAL_POINT;

        private char decimalChar;
        private char separatorChar;
        private NumberFormatInfo numberFormat;

        /// <summary>
        /// Create a CSV format for the specified decimal char and separator char.
        /// </summary>
        /// <param name="decimalChar">The character for a decimal point or comma.</param>
        /// <param name="separatorChar">The separator char for a number list, likely comma or semicolon.</param>
        public CSVFormat(char decimalChar, char separatorChar)
        {
            this.decimalChar = decimalChar;
            this.separatorChar = separatorChar;

            if (decimalChar == '.')
            {
                this.numberFormat = (new CultureInfo("en-us")).NumberFormat;
            }
            else if (decimalChar == ',')
            {
                this.numberFormat = (new CultureInfo("de-DE")).NumberFormat;
            }
            else
            {
                this.numberFormat = NumberFormatInfo.CurrentInfo;
            }
        }

        /// <summary>
        /// The decimal character, usually either a period or comma.
        /// </summary>
        public char DecimalChar
        {
            get
            {
                return decimalChar;
            }
        }

        /// <summary>
        /// The separator character for a list of fields in CSV, usually either comma or
        /// semicolon.
        /// </summary>
        public char Separator
        {
            get
            {
                return separatorChar;
            }
        }

        /// <summary>
        /// Parse the specified string into a number.
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <returns>The number that has been parsed.</returns>
        public double Parse(String str)
        {
            return double.Parse(str, this.numberFormat);
        }

        /// <summary>
        /// Format the specified number into a string.
        /// </summary>
        /// <param name="d">The number to parse.</param>
        /// <param name="digits">The number of fractional digits.</param>
        /// <returns>The formatted number.</returns>
        public String Format(double d, int digits)
        {
            return d.ToString(this.numberFormat);
        }

        /// <summary>
        /// The decimal character for the current region.
        /// </summary>
        public static char DecimalCharacter
        {
            get
            {
                return NumberFormatInfo.CurrentInfo.NumberDecimalSeparator[0];
            }
        }

    }
}
