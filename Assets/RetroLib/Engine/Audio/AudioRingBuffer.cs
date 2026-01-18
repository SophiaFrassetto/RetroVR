using System;

namespace RetroLib.Engine.Audio
{
    public sealed class AudioRingBuffer
    {
        private readonly float[] buffer;
        private readonly int capacity;

        private int writePos;
        private int readPos;
        private int available;

        private readonly object sync = new();

        public AudioRingBuffer(int capacity)
        {
            this.capacity = capacity;
            buffer = new float[capacity];
        }

        /// <summary>
        /// Escrita pelo producer (Libretro)
        /// </summary>
        public void Write(float[] data, int count)
        {
            lock (sync)
            {
                for (int i = 0; i < count; i++)
                {
                    buffer[writePos] = data[i];
                    writePos = (writePos + 1) % capacity;

                    if (available < capacity)
                    {
                        available++;
                    }
                    else
                    {
                        // overwrite: avança leitura
                        readPos = (readPos + 1) % capacity;
                    }
                }
            }
        }

        /// <summary>
        /// Leitura pelo consumer (Unity Audio Thread)
        /// </summary>
        public int Read(float[] output, int count)
        {
            int read = 0;

            lock (sync)
            {
                while (read < count && available > 0)
                {
                    output[read] = buffer[readPos];
                    readPos = (readPos + 1) % capacity;
                    available--;
                    read++;
                }
            }

            return read;
        }

        public void Clear()
        {
            lock (sync)
            {
                writePos = 0;
                readPos = 0;
                available = 0;
            }
        }
    }
}
