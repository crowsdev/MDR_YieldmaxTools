using System;

namespace MDR_YieldmaxTools.Utils
{
    /// <summary>
    /// High-performance moving average calculator with zero allocations.
    /// Uses circular buffer and running sum for O(1) add and average operations.
    /// </summary>
    public struct MovingAvg
    {
        private double[] _buffer;
        private int _capacity;
        private int _index;
        private int _count;
        private double _sum;

        /// <summary>
        /// Creates a moving average calculator with specified window size.
        /// </summary>
        /// <param name="windowSize">Number of values to average (must be > 0)</param>
        public MovingAvg(int windowSize)
        {
            if (windowSize <= 0)
                throw new ArgumentException("Window size must be greater than zero", nameof(windowSize));

            _capacity = windowSize;
            _buffer = new double[windowSize];
            _index = 0;
            _count = 0;
            _sum = 0;
        }

        /// <summary>
        /// Current number of values in the buffer (0 to capacity).
        /// </summary>
        public readonly int Count => _count;

        /// <summary>
        /// Maximum number of values that can be stored.
        /// </summary>
        public readonly int Capacity => _capacity;

        /// <summary>
        /// Current average of all values in the buffer.
        /// Returns zero if no values have been added.
        /// </summary>
        public readonly double Average
        {
            get
            {
                if (_count == 0)
                    return 0;

                return _sum / _count;
            }
        }

        /// <summary>
        /// Adds a new value to the moving average.
        /// If buffer is full, replaces the oldest value.
        /// </summary>
        /// <param name="value">Value to add</param>
        public void Add(double value)
        {
            // Subtract old value if buffer is full
            if (_count == _capacity)
            {
                _sum -= _buffer[_index];
            }
            else
            {
                _count++;
            }

            // Add new value
            _buffer[_index] = value;
            _sum += value;

            // Move to next position (circular)
            _index = (_index + 1) % _capacity;
        }

        /// <summary>
        /// Resets the moving average to empty state.
        /// </summary>
        public void Clear()
        {
            _buffer = new double[_capacity];
            _index = 0;
            _count = 0;
            _sum = 0;
        }
    }
}