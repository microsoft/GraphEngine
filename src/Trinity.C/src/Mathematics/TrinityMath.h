// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include "TrinityCommon.h"

/*
 * http://msdn.microsoft.com/en-US/library/07b5e56h(v=vs.80).aspx
 */

double multiply_double_vector(double *  dv1, double *  dv2, int32_t count);

double multiply_sparse_double_vector(double *  dv1, double *  dv2, int32_t * idx, int32_t count);
