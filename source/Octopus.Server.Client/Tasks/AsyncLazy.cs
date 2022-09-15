using System;
using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Client.Tasks
{
    /// <summary>
    /// The <see cref="Lazy{T}"/> implementation with <see cref="CancellationToken"/> support.
    /// </summary>
    public class AsyncLazy<T> where T : class
    {
        private readonly Func<CancellationToken, Task<T>> factoryFunc;

        private readonly SemaphoreSlim semaphore = new(1, 1);
        private T value;

        public AsyncLazy(Func<CancellationToken, Task<T>> factoryFunc)
        {
            this.factoryFunc = factoryFunc;
        }

        public bool HasValue { get; private set; }

        public async Task<T> Value(CancellationToken cancellationToken)
        {
            if (HasValue) return value;

            await semaphore.WaitAsync(cancellationToken);
            try
            {
                if (HasValue) return value;
                value = await factoryFunc(cancellationToken);
                HasValue = true;
                return value;
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
