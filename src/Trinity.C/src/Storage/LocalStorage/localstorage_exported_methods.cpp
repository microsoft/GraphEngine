// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "Storage/LocalStorage/LocalMemoryStorage.h"

using namespace Storage::LocalMemoryStorage;

DLL_EXPORT u16char* CGetStorageSlot(BOOL isPrimary) 
{
    auto slot = isPrimary ? GetPrimaryStorageSlot() : GetSecondaryStorageSlot();
	return slot.ToWcharArray().detach_data();
}
