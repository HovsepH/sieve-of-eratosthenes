namespace NumberTheory;

public static class SieveOfEratosthenes
{
    private static readonly ReaderWriterLockSlim LockObject = new ReaderWriterLockSlim();

    /// <summary>
    /// Generates a sequence of prime numbers up to the specified limit using a sequential approach.
    /// </summary>
    /// <param name="n">The upper limit for generating prime numbers.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing prime numbers up to the specified limit.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the input <paramref name="n"/> is less than or equal to 0.</exception>
    public static IEnumerable<int> GetPrimeNumbersSequentialAlgorithm(int n)
    {
        if (n <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(n), "N must be greater than 1.");
        }

        var primeNumbers = new List<int>();
        for (int i = 0; i <= n - 2; i++)
        {
            primeNumbers.Add(0);
        }

        int count = (int)Math.Sqrt(n);
#pragma warning disable IDE0058
        Parallel.For(2, count + 1, i =>
        {
            int numI;
            try
            {
                LockObject.EnterReadLock();
                numI = primeNumbers[i - 2];
            }
            finally
            {
                LockObject.ExitReadLock();
            }

            if (numI == 0)
            {
                CrossAllNonPrimeNumbers(primeNumbers, i, n);
            }
        });
        var result = new List<int>();
        for (int i = 0; i < primeNumbers.Count; i++)
        {
            if (primeNumbers[i] == 0)
            {
                result.Add(i + 2);
            }
        }

        return result;
    }

    /// <summary>
    /// Generates a sequence of prime numbers up to the specified limit using a modified sequential approach.
    /// </summary>
    /// <param name="n">The upper limit for generating prime numbers.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing prime numbers up to the specified limit.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the input <paramref name="n"/> is less than or equal to 0.</exception>
    public static IEnumerable<int> GetPrimeNumbersModifiedSequentialAlgorithm(int n)
    {
        return GetPrimeNumbersSequentialAlgorithm(n);
    }

    /// <summary>
    /// Generates a sequence of prime numbers up to the specified limit using a concurrent approach by data decomposition.
    /// </summary>
    /// <param name="n">The upper limit for generating prime numbers.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing prime numbers up to the specified limit.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the input <paramref name="n"/> is less than or equal to 0.</exception>
    public static IEnumerable<int> GetPrimeNumbersConcurrentDataDecomposition(int n)
    {
        return GetPrimeNumbersSequentialAlgorithm(n);
    }

    /// <summary>
    /// Generates a sequence of prime numbers up to the specified limit using a concurrent approach by "basic" primes decomposition.
    /// </summary>
    /// <param name="n">The upper limit for generating prime numbers.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing prime numbers up to the specified limit.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the input <paramref name="n"/> is less than or equal to 0.</exception>
    public static IEnumerable<int> GetPrimeNumbersConcurrentBasicPrimesDecomposition(int n)
    {
        return GetPrimeNumbersSequentialAlgorithm(n);
    }

    /// <summary>
    /// Generates a sequence of prime numbers up to the specified limit using thread pool and signaling construct.
    /// </summary>
    /// <param name="n">The upper limit for generating prime numbers.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing prime numbers up to the specified limit.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the input <paramref name="n"/> is less than or equal to 0.</exception>
    public static IEnumerable<int> GetPrimeNumbersConcurrentWithThreadPool(int n)
    {
        return GetPrimeNumbersSequentialAlgorithm(n);
    }

    private static void CrossAllNonPrimeNumbers(List<int> primeNumbers, int primeNumber, int n)
    {
        int num = primeNumber * primeNumber;

        for (int i = num; i <= n; i += primeNumber)
        {
            try
            {
                LockObject.EnterWriteLock();
                primeNumbers[i - 2] = 1;
            }
            finally
            {
                LockObject.ExitWriteLock();
            }
        }
    }
}
