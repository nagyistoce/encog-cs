﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloo;
using Encog.Util.CL.Kernels;

namespace Encog.Util.CL
{
    /// <summary>
    /// The main OpenCL class for Encog.  OpenCL allows Encog to use 
    /// other devices, such as graphics processors (GPUs) to assist in the 
    /// processing of neural networks.
    /// 
    /// Encog looks at these devices in two levels.
    /// 
    /// OpenCL Platforms - A platform holds a collection of devices.  A platform
    /// is basically a driver type.  If you have several GPU's, but all from 
    /// the same vendor, you will have only one platform.  If your OpenCL
    /// devices are a mix of vendors, you will have multiple platforms.  One
    /// platform per vendor.
    /// 
    /// OpenCL Devices - Encog keeps a list of OpenCL devices, which can be
    /// from different platforms.  This pool of devices will be used to 
    /// process tasks sent to OpenCL.
    /// 
    /// To enable Encog OpenCL processing, use the following call:
    /// 
    /// Encog.Instance.InitCL();
    /// 
    /// Encog makes use of CLOO to access the low-level OpenCL drivers.  Encog
    /// creates a thin wrapper around CLOO to make it appear similar to JOCL,
    /// which is the low-level framework we use on the Java side.  Encog's
    /// CL wrapper also abstracts CLOO in a way useful to Encog.  For more 
    /// information about CLOO visit the following URL:
    /// 
    /// http://sourceforge.net/projects/cloo/
    /// 
    /// </summary>
    public class EncogCL
    {
        /// <summary>
        /// The platforms detected.
        /// </summary>
        private IList<EncogCLPlatform> platforms = new List<EncogCLPlatform>();

        /// <summary>
        /// All devices, from all platforms.
        /// </summary>
        private IList<EncogCLDevice> devices = new List<EncogCLDevice>();

        /// <summary>
        /// Allows you to determine how much 
        /// work the CPU and GPU get.  For example, to give the CL/GPU 
        /// twice as much work as the CPU, specify 1.5.  To give it half as 
        /// much, choose 0.5.
        /// </summary>
        public double EnforcedCLRatio { get; set; }

        /// <summary>
        /// The number of CL threads to use, defaults to 200.
        /// </summary>
        public int CLThreads { get; set; }

        /// <summary>
        /// The size of a CL workload, defaults to 10.
        /// </summary>
        public int CLWorkloadSize { get; set; }

        /// <summary>
        /// Construct an Encog OpenCL object.
        /// </summary>
        public EncogCL()
        {
            this.EnforcedCLRatio = 1.0;
            this.CLThreads = 200;
            this.CLWorkloadSize = 10;

            if (ComputePlatform.Platforms.Count == 0)
                throw new EncogError("Can't find any OpenCL platforms");

            foreach(ComputePlatform platform in ComputePlatform.Platforms)
            {
                EncogCLPlatform encogPlatform = new EncogCLPlatform(platform);
                platforms.Add(encogPlatform);
                foreach( EncogCLDevice device in encogPlatform.Devices )
                {
                    devices.Add(device);
                }
            }
        }

        /// <summary>
        /// The devices used by EncogCL, this spans over platform boundaries.
        /// </summary>
        public IList<EncogCLDevice> Devices
        {
            get
            {
                return this.devices;
            }
        }

        /// <summary>
        /// The devices used by EncogCL, this spans over platform boundaries.
        /// Does not include disabled devices or devices from disabled platforms.
        /// </summary>
        public IList<EncogCLDevice> EnabledDevices
        {
            get
            {
                IList<EncogCLDevice> result = new List<EncogCLDevice>();
                foreach(EncogCLDevice device in devices)
                {
                    if ( device.Enabled && device.Platform.Enabled )
                        result.Add(device);
                }

                return result;
            }
        }

        /// <summary>
        /// All platforms detected.
        /// </summary>
        public IList<EncogCLPlatform> Platforms
        {
            get
            {
                return this.platforms;
            }
        }

        /// <summary>
        /// Choose a device.  Simply returns the first device detected.
        /// </summary>
        /// <returns>The first device detected.</returns>
        public EncogCLDevice ChooseDevice()
        {
            if (this.devices.Count < 1)
                return null;
            else
                return this.devices[0];
        }

        /// <summary>
        /// Disable all devices that are CPU's.  This is a good idea to do if 
        /// you are going to use regular CPU processing in cojunction with 
        /// OpenCL processing where the CPU is made to look like a OpenCL device.  
        /// Otherwise, you end up with the CPU serving as both a "regular CPU 
        /// training task" and as an "OpenCL training task".
        /// </summary>
        public void DisableAllCPUs()
        {
            foreach(EncogCLDevice device in this.devices)
            {
                if( device.IsCPU )
                    device.Enabled = false;
            }
        }

        /// <summary>
        /// Enable all devices that are CPU's.  
        /// </summary>
        public void EnableAllCPUs()
        {
            foreach (EncogCLDevice device in this.devices)
            {
                if (device.IsCPU)
                    device.Enabled = true;
            }
        }

        /// <summary>
        /// True if CPUs are present.
        /// </summary>
        public bool AreCPUsPresent
        {
            get
            {
                foreach (EncogCLDevice device in this.devices)
                {
                    if (device.IsCPU)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Dump all devices as a string.
        /// </summary>
        /// <returns>The devices.</returns>
        public override String ToString()
        {
            StringBuilder result = new StringBuilder();
            foreach (EncogCLDevice device in this.devices)
            {
                result.Append(device.ToString());
                result.Append("\n");
            }
            return result.ToString();
        }
    }
}