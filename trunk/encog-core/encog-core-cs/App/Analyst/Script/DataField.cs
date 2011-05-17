using System;
using System.Collections.Generic;

namespace Encog.App.Analyst.Script
{
    /// <summary>
    /// Holds stats on a data field for the Encog Analyst. This data is used to
    /// normalize the field.
    /// </summary>
    ///
    public class DataField
    {
        /// <summary>
        /// The class members.
        /// </summary>
        ///
        private readonly IList<AnalystClassItem> classMembers;

        /// <summary>
        /// Construct the data field.
        /// </summary>
        ///
        /// <param name="theName">The name of this field.</param>
        public DataField(String theName)
        {
            classMembers = new List<AnalystClassItem>();
            Name = theName;
            Min = Double.MaxValue;
            Max = Double.MinValue;
            Mean = Double.NaN;
            StandardDeviation = Double.NaN;
            Integer = true;
            Real = true;
            Class = true;
            Complete = true;
        }


        /// <value>the classMembers</value>
        public IList<AnalystClassItem> ClassMembers
        {
            /// <returns>the classMembers</returns>
            get { return classMembers; }
        }


        /// <value>the max to set</value>
        public double Max { /// <returns>the max</returns>
            get; /// <param name="theMax">the max to set</param>
            set; }


        /// <value>the mean to set</value>
        public double Mean { /// <returns>the mean</returns>
            get; /// <param name="theMean">the mean to set</param>
            set; }


        /// <value>the theMin to set</value>
        public double Min { /// <returns>the min</returns>
            get; /// <param name="theMin">the theMin to set</param>
            set; }


        /// <summary>
        /// Determine the minimum class count. This is the count of the
        /// classification field that is the smallest.
        /// </summary>
        ///
        /// <value>The minimum class count.</value>
        public int MinClassCount
        {
            /// <summary>
            /// Determine the minimum class count. This is the count of the
            /// classification field that is the smallest.
            /// </summary>
            ///
            /// <returns>The minimum class count.</returns>
            get
            {
                int cmin = Int32.MaxValue;

                foreach (AnalystClassItem cls  in  classMembers)
                {
                    cmin = Math.Min(cmin, cls.Count);
                }
                return cmin;
            }
        }


        /// <value>the name to set</value>
        public String Name { /// <returns>the name</returns>
            get; /// <param name="theName">the name to set</param>
            set; }


        /// <value>the standardDeviation to set</value>
        public double StandardDeviation { /// <returns>the standardDeviation</returns>
            get; /// <param name="theStandardDeviation">the standardDeviation to set</param>
            set; }


        /// <value>the isClass to set</value>
        public bool Class { /// <returns>the isClass</returns>
            get; /// <param name="theClass">the isClass to set</param>
            set; }


        /// <value>the isComplete to set</value>
        public bool Complete { /// <returns>the isComplete</returns>
            get; /// <param name="theComplete">the isComplete to set</param>
            set; }


        /// <value>the isInteger to set</value>
        public bool Integer { /// <returns>the isInteger</returns>
            get; /// <param name="theInteger">the isInteger to set</param>
            set; }


        /// <value>the isReal to set</value>
        public bool Real { /// <returns>the isReal</returns>
            get; /// <param name="theReal">the isReal to set</param>
            set; }
    }
}