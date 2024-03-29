//
// svm_model
//
using System;
namespace Encog.MathUtil.LIBSVM
{
    // This class was taken from the libsvm package.  We have made some
    // modifications for use in Encog.
    // 
    // http://www.csie.ntu.edu.tw/~cjlin/libsvm/
    //
    // The libsvm Copyright/license is listed here.
    // 
    // Copyright (c) 2000-2010 Chih-Chung Chang and Chih-Jen Lin
    // All rights reserved.
    // 
    // Redistribution and use in source and binary forms, with or without
    // modification, are permitted provided that the following conditions
    // are met:
    // 
    // 1. Redistributions of source code must retain the above copyright
    // notice, this list of conditions and the following disclaimer.
    // 
    // 2. Redistributions in binary form must reproduce the above copyright
    // notice, this list of conditions and the following disclaimer in the
    // documentation and/or other materials provided with the distribution.
    //
    // 3. Neither name of copyright holders nor the names of its contributors
    // may be used to endorse or promote products derived from this software
    // without specific prior written permission.
    // 
    // 
    // THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
    // ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
    // LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
    // A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE REGENTS OR
    // CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
    // EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
    // PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
    // PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
    // LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
    // NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
    // SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#if !SILVERLIGHT
	[Serializable]
#endif
	public class svm_model
	{
		internal svm_parameter param; // parameter
		internal int nr_class; // number of classes, = 2 in regression/one class svm
		internal int l; // total #SV
		internal svm_node[][] SV; // SVs (SV[l])
		internal double[][] sv_coef; // coefficients for SVs in decision functions (sv_coef[n-1][l])
		internal double[] rho; // constants in decision functions (rho[n*(n-1)/2])
		internal double[] probA; // pariwise probability information
		internal double[] probB;
		
		// for classification only
		
		internal int[] label; // label of each class (label[n])
		internal int[] nSV; // number of SVs for each class (nSV[n])
		// nSV[0] + nSV[1] + ... + nSV[n-1] = l
	}
	
}