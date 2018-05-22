/*
 * This file is a part of the LICPP project,
 * licensed under MIT liense.
 * Please read LICENSE.TXT and README.MKD for more information.
 */

#ifndef __BASE_HACKS_HPP__
#define __BASE_HACKS_HPP__

#include <type_traits>
#include <functional>
#include <utility>
#include "core.hpp"

namespace licpp {
	template <typename R, typename C, bool cp, typename ... As>
		struct _lambda_type {
			static const bool constp = cp;
			enum { arity = sizeof...(As) };
			using return_type = R;
			using arg_type = typename _list_t<As...>::type;
		};
	template <typename L>
		struct lambda_type : lambda_type<decltype(&L::operator())> {};
	template <typename R, typename C, typename ... As>
		struct lambda_type<R(C::*)(As...)> : _lambda_type<R, C, false, As...> {};
	template <typename R, typename C, typename ... As>
		struct lambda_type<R(C::*)(As...) const> : _lambda_type<R, C, true, As...> {};
};

#endif
