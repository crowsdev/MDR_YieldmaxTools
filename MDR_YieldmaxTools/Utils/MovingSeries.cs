using System;
using System.Collections.Generic;

namespace MDR_YieldmaxTools.Utils
{
    public struct MovingSeries
    {
        private Queue<double> _buffer;
        private int _capacity;
        private int _count;

        public MovingSeries(int windowSize)
        {
            if (windowSize <= 0)
                throw new ArgumentException("Window size must be greater than zero", nameof(windowSize));

            _capacity = windowSize;
            _buffer = new Queue<double>(windowSize);
            _count = 0;
        }

        public readonly int Count => _count;

        public readonly int Capacity => _capacity;

        public bool Add(double value, out double[] _series)
        {
            bool result = false;
            _count++;

            if (_count >= _capacity)
            {
                double dropped = _buffer.Dequeue();
                _count--;
                result = true;
            }

            // Add new value
            _buffer.Enqueue(value);

            if (result)
            {
                _series = _buffer.ToArray();
            }
            else
            {
                _series = null;
            }

            return result;
        }

        /// <summary>
        /// Resets the moving average to empty state.
        /// </summary>
        public void Clear()
        {
            _buffer = new Queue<double>(_capacity);
            _count = 0;
        }
    }
}