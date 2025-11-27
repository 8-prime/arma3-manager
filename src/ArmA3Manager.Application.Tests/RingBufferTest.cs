using ArmA3Manager.Application.Common.DataTypes;

namespace ArmA3Manager.Application.Tests;

public class RingBufferTest
{
    [Fact]
    public void EmptyRingBufferReturnsEmptyEnumerable()
    {
        var rb = new RingBuffer<int>(10);
        var items = rb.Get();
        Assert.Empty(items);
    }

    [Fact]
    public void SingleItemRingBufferReturnsSingleItem()
    {
        var rb = new RingBuffer<int>(10);
        rb.Add(1);
        var items = rb.Get().ToList();
        Assert.Single(items);
        Assert.Equal(1, items.FirstOrDefault());
    }

    [Fact]
    public void MultipleItemsRingBufferReturnsMultipleItems()
    {
        var rb = new RingBuffer<int>(5);
        rb.Add(1);
        rb.Add(2);
        rb.Add(3);
        rb.Add(4);
        rb.Add(5);
        rb.Add(6);
        var items = rb.Get().ToList();
        var expected = new[] { 2, 3, 4, 5, 6 };
        Assert.Equal(expected, items);
    }

    [Fact]
    public void MultipleItemsRingBufferReturnsMultipleItemsWithDifferentCapacity()
    {
        var rb = new RingBuffer<int>(6);
        rb.Add(1);
        rb.Add(2);
        rb.Add(3);
        rb.Add(4);
        rb.Add(5);
        rb.Add(6);
        var items = rb.Get().ToList();
        var expected = new[] { 1, 2, 3, 4, 5, 6 };
        Assert.Equal(expected, items);
    }

    [Fact]
    public void MultipleItemsRingBufferReturnsMultipleItemsWithSameCapacity()
    {
        var rb = new RingBuffer<int>(6);
        rb.Add(1);
        rb.Add(2);
        rb.Add(3);
        var items = rb.Get().ToList();
        var expected = new[] { 1, 2, 3 };
        Assert.Equal(expected, items);
    }

    [Fact]
    public void LoopMultipleTimesOverCapacity()
    {
        var rb = new RingBuffer<int>(5);
        rb.Add(1);
        rb.Add(2);
        rb.Add(3);
        rb.Add(4);
        rb.Add(5);
        rb.Add(6);
        rb.Add(7);
        rb.Add(8);
        rb.Add(9);
        rb.Add(10);
        rb.Add(11);
        rb.Add(12);
        rb.Add(13);
        rb.Add(14);
        rb.Add(15);
        rb.Add(16);
        var items = rb.Get().ToList();
        var expected = new[] { 12, 13, 14, 15, 16 };
        Assert.Equal(expected, items);
    }
}