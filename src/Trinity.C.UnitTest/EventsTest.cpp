#define CATCH_CONFIG_MAIN
#include "catch_wrapper.hpp"
#include "Trinity/Events.h"
#include "Trinity.h"
#include "Trinity/IO/Console.h"
#include <atomic>

TEST_CASE("Eventloop can be started / stopped successfully", "[events]")
{
    REQUIRE(TrinityErrorCode::E_SUCCESS == StartEventLoop());
    REQUIRE(TrinityErrorCode::E_SUCCESS == StopEventLoop());
}

Trinity::Events::work_t* continuation_simple(void* pdata, Trinity::Events::work_t* pcompleted)
{
    auto pcounter = reinterpret_cast<std::atomic<int32_t>*>(pdata);
    ++*pcounter;
    return nullptr;
}

Trinity::Events::work_t* continuation_chained(void* pdata, Trinity::Events::work_t* pcompleted)
{
    auto pcounter = reinterpret_cast<std::atomic<int32_t>*>(pdata);
    ++*pcounter;
    return AllocContinuation(continuation_simple, pdata, nullptr);
}

Trinity::Events::work_t* compute_simple(void* pdata)
{
    auto pcounter = reinterpret_cast<std::atomic<int32_t>*>(pdata);
    ++*pcounter;
    return nullptr;
}

Trinity::Events::work_t* compute_with_continuation(void* pdata)
{
    auto pcounter = reinterpret_cast<std::atomic<int32_t>*>(pdata);
    ++*pcounter;
    auto cont = AllocContinuation(continuation_simple, pdata, nullptr);
    return cont;
}

Trinity::Events::work_t* compute_with_continuation_chained(void* pdata)
{
    auto pcounter = reinterpret_cast<std::atomic<int32_t>*>(pdata);
    ++*pcounter;
    auto cont = AllocContinuation(continuation_chained, pdata, nullptr);
    return cont;
}

Trinity::Events::work_t* compute_with_continuation_deps(void* pdata)
{
    auto pcounter = reinterpret_cast<std::atomic<int32_t>*>(pdata);
    ++*pcounter;
    auto cont = AllocContinuation(continuation_chained, pdata, 
                    AllocContinuation(continuation_chained, pdata, nullptr));
    return cont;
}

void post_simple_1(int count)
{
    std::atomic<int32_t> counter = 0;
    for (int i=0; i < count; ++i)
    {
        REQUIRE(TrinityErrorCode::E_SUCCESS == PostCompute(compute_simple, &counter));
    }
    std::this_thread::sleep_for(std::chrono::seconds(1));
    REQUIRE(count == counter);
}

void post_simple_2(int count)
{
    std::atomic<int32_t> counter_1 = 0;
    std::atomic<int32_t> counter_2 = 0;
    for (int i=0; i < count; ++i)
    {
        REQUIRE(TrinityErrorCode::E_SUCCESS == PostCompute(compute_simple, &counter_1));
        REQUIRE(TrinityErrorCode::E_SUCCESS == PostCompute(compute_simple, &counter_2));
    }
    std::this_thread::sleep_for(std::chrono::seconds(1));
    REQUIRE(count == counter_1);
    REQUIRE(count == counter_2);
}

void post_w_cont_simple(int count)
{
    std::atomic<int32_t> counter = 0;
    for (int i=0; i < count; ++i)
    {
        REQUIRE(TrinityErrorCode::E_SUCCESS == PostCompute(compute_with_continuation, &counter));
    }
    std::this_thread::sleep_for(std::chrono::seconds(1));
    REQUIRE(count * 2 == counter);
}

void post_w_cont_chained(int count)
{
    std::atomic<int32_t> counter = 0;
    for (int i=0; i < count; ++i)
    {
        REQUIRE(TrinityErrorCode::E_SUCCESS == PostCompute(compute_with_continuation_chained, &counter));
    }
    std::this_thread::sleep_for(std::chrono::seconds(1));
    REQUIRE(count * 3 == counter);
}

void post_w_cont_deps(int count)
{
    std::atomic<int32_t> counter = 0;
    for (int i=0; i < count; ++i)
    {
        REQUIRE(TrinityErrorCode::E_SUCCESS == PostCompute(compute_with_continuation_deps, &counter));
    }
    std::this_thread::sleep_for(std::chrono::seconds(1));
    REQUIRE(count * 5 == counter);
}

TEST_CASE("PostCompute accepts simple work items", "[events]")
{
    REQUIRE(TrinityErrorCode::E_SUCCESS == StartEventLoop());

    SECTION("one item")
    {
        post_simple_1(1);
    }

    SECTION("10 items")
    {
        post_simple_1(10);
    }

    SECTION("100 items")
    {
        post_simple_1(100);
    }

    SECTION("1000 items")
    {
        post_simple_1(1000);
    }

    REQUIRE(TrinityErrorCode::E_SUCCESS == StopEventLoop());
}

TEST_CASE("PostCompute accepts simple interleaved work items", "[events]")
{
    REQUIRE(TrinityErrorCode::E_SUCCESS == StartEventLoop());

    SECTION("one item")
    {
        post_simple_2(1);
    }

    SECTION("10 items")
    {
        post_simple_2(10);
    }

    SECTION("100 items")
    {
        post_simple_2(100);
    }

    SECTION("1000 items")
    {
        post_simple_2(1000);
    }

    REQUIRE(TrinityErrorCode::E_SUCCESS == StopEventLoop());
}

TEST_CASE("PostCompute accepts items with simple continuation", "[events]")
{
    REQUIRE(TrinityErrorCode::E_SUCCESS == StartEventLoop());

    SECTION("one item")
    {
        post_w_cont_simple(1);
    }

    SECTION("10 items")
    {
        post_w_cont_simple(10);
    }

    SECTION("100 items")
    {
        post_w_cont_simple(100);
    }

    SECTION("1000 items")
    {
        post_w_cont_simple(1000);
    }

    REQUIRE(TrinityErrorCode::E_SUCCESS == StopEventLoop());
}

TEST_CASE("PostCompute accepts items with chained continuation", "[events]")
{
    REQUIRE(TrinityErrorCode::E_SUCCESS == StartEventLoop());

    SECTION("one item")
    {
        post_w_cont_chained(1);
    }

    SECTION("10 items")
    {
        post_w_cont_chained(10);
    }

    SECTION("100 items")
    {
        post_w_cont_chained(100);
    }

    SECTION("1000 items")
    {
        post_w_cont_chained(1000);
    }

    REQUIRE(TrinityErrorCode::E_SUCCESS == StopEventLoop());
}

TEST_CASE("PostCompute accepts items with deps continuation", "[events]")
{
    REQUIRE(TrinityErrorCode::E_SUCCESS == StartEventLoop());

    SECTION("one item")
    {
        post_w_cont_deps(1);
    }

    SECTION("10 items")
    {
        post_w_cont_deps(10);
    }

    SECTION("100 items")
    {
        post_w_cont_deps(100);
    }

    SECTION("1000 items")
    {
        post_w_cont_deps(1000);
    }

    REQUIRE(TrinityErrorCode::E_SUCCESS == StopEventLoop());
}