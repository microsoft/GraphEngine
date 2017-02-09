// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <unordered_set>
namespace Trinity
{
	namespace Collections{
		template <
			class Key,                        // unordered_set::key_type/value_type
			class Hash = std::hash<Key>,           // unordered_set::hasher
			class Pred = std::equal_to<Key>,       // unordered_set::key_equal
			class Alloc = std::allocator<Key>      // unordered_set::allocator_type
		> using HashSet = std::unordered_set < Key, Hash, Pred, Alloc > ;
	}
}