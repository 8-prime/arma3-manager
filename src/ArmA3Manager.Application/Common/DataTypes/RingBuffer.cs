namespace ArmA3Manager.Application.Common.DataTypes;

public class RingBuffer<T>
{
    private readonly T[] _buffer;
    private int _count;
    private int _currentElement;


    public RingBuffer(int capacity)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 0);
        _buffer = new T[capacity];
    }

    public void Add(T item)
    {
        _buffer[_currentElement] = item;
        _currentElement = (_currentElement + 1) % _buffer.Length;

        if (_count < _buffer.Length) _count++;
    }

    public IEnumerable<T> Get()
    {
        if (_count == 0) yield break;
        //
        // if (_count == _buffer.Length)
        // {
        //     for (var i = 0; i < _count; i++)
        //     {
        //         yield return _buffer[i];
        //     }
        //
        //     yield break;
        // }

        for (var i = 0; i < _count; i++)
        {
            yield return _buffer[(_currentElement + i) % _count];
        }
    }
}


//
//   0 1 2 3 4 5 6 7 8 9 
//
//   5 4 7 2 6 1 . 
//                 
//     currentElement
// Means that 4 was last written. when starting to read i would want to go
// _length = 10
// _count = 6
// _currentElement = 6
//


//
//   0 1 2 3 4 5 6 7 8 9 
//
//   5 4 7 2 6 1 7 2 9 3
//       .
//     currentElement
// Means that 4 was last written. when starting to read i would want to go
// _length = 10
// _count = 10
// _currentElement = 2
//