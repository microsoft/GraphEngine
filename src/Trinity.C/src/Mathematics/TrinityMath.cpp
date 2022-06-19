// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "TrinityCommon.h"
#include "Mathematics/TrinityMath.h"

double multiply_double_vector(double *  dv1, double *  dv2, int32_t count)
{
	//double res = 0;
	//   for(int32_t i=0;i<count;i++) {
	//       res+= (*(dv2+i) * *(dv1+i));
	//   }
	//   return res;

	int32_t loop = count & 0xfffffffe; // count/2 * 2;
	int32_t i = 0;
	__m128d a, b;
	__m128d sum = _mm_setzero_pd();

	for (; i<loop; i += 2)
	{
		// http://msdn.microsoft.com/en-us/library/cww3b12t(v=vs.80).aspx
		a = _mm_load_pd(dv1 + i); //Loads two double-precision, floating-point values.
		b = _mm_load_pd(dv2 + i); //Loads two double-precision, floating-point values.
		// http://msdn.microsoft.com/en-US/library/07b5e56h(v=vs.80).aspx

		sum = _mm_add_pd(sum, _mm_mul_pd(a, b));
	}

	// calculate the last element when the count is odd
	if (i<count)
	{
		a = _mm_load_sd(dv1 + i); //Loads a double to the lower 64-bits.
		b = _mm_load_sd(dv2 + i); //Loads a double to the lower 64-bits.
		// http://msdn.microsoft.com/en-US/library/we4dxyk3(v=vs.80).aspx
		sum = _mm_add_sd(sum, _mm_mul_sd(a, b)); // r0 := a0 + b0, r1 := a1
	}

	double ALIGNED(16) tmp[2];
	//double * dp =   (double*) ( ((int64_t)buff & mask16b) + 16 );
	//! Stores two double-precision, floating-point values. The address p must be 16-byte aligned.
	_mm_store_pd(tmp, sum);
	return tmp[0] + tmp[1];
}

double multiply_sparse_double_vector(double *  dv1, double *  dv2, int32_t * idx, int32_t count)
{
	int32_t loop = count & 0xfffffffe; // count/2 * 2;
	int32_t i = 0;
	__m128d a, b;
	__m128d sum = _mm_setzero_pd();

#ifdef TRINITY_PLATFORM_WINDOWS
	double* __dv1 = (double*)_aligned_malloc((size_t)count << 3, 16);
#else
	double* __dv1 = (double*)aligned_alloc((size_t)count << 3, 16);
#endif

	for (int32_t i = 0; i<count; i++)
	{
#pragma warning(suppress: 6011)
		__dv1[i] = dv1[idx[i]];
	}

	for (; i<loop; i += 2)
	{
		// http://msdn.microsoft.com/en-us/library/cww3b12t(v=vs.80).aspx
		a = _mm_load_pd(__dv1 + i); //Loads two double-precision, floating-point values.
		b = _mm_load_pd(dv2 + i); //Loads two double-precision, floating-point values.
		// http://msdn.microsoft.com/en-US/library/07b5e56h(v=vs.80).aspx

		sum = _mm_add_pd(sum, _mm_mul_pd(a, b));
	}

	// calculate the last element when the count is odd
	if (i<count)
	{
		a = _mm_load_sd(__dv1 + i); //Loads a double to the lower 64-bits.
		b = _mm_load_sd(dv2 + i); //Loads a double to the lower 64-bits.
		// http://msdn.microsoft.com/en-US/library/we4dxyk3(v=vs.80).aspx
		sum = _mm_add_sd(sum, _mm_mul_sd(a, b)); // r0 := a0 + b0, r1 := a1
	}

#ifdef TRINITY_PLATFORM_WINDOWS
	_aligned_free(__dv1);
#else
	free(__dv1);
#endif

	double ALIGNED(16) tmp[2];
	//double * dp =   (double*) ( ((int64_t)buff & mask16b) + 16 );
	//! Stores two double-precision, floating-point values. The address p must be 16-byte aligned.
	_mm_store_pd(tmp, sum);
	return tmp[0] + tmp[1];
}
