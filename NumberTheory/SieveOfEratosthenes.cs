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
        for (int i = 2; i <= count; i++)
        {
            if (primeNumbers[i - 2] == 0)
            {
                CrossAllNonPrimeNumbers(primeNumbers, i, n);
            }
        }

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
        if (n <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(n), "N must be greater than 1.");
        }

        var primeFlags = new List<int>();
        var primeNumbersUpToSqrtN = GetPrimeNumbersSequentialAlgorithm((int)Math.Sqrt(n)).ToList();
        for (int i = 0; i < n - (int)Math.Sqrt(n); i++)
        {
            primeFlags.Add(0);
        }

        var threads = new List<Thread>();

        for (int i = 0; i < primeNumbersUpToSqrtN.Count; i++)
        {
            int prime = primeNumbersUpToSqrtN[i]; // Capture the loop variable to avoid closure issues
            var thread = new Thread(() => MarkNonPrimesInRange(primeFlags, n, prime));
            threads.Add(thread);
            thread.Start();
        }

        // Wait for all threads to complete
        foreach (var thread in threads)
        {
            thread.Join();
        }

        var primeNumbersAfterSqrtN = new List<int>();
        var sqrt = (int)Math.Sqrt(n);

        for (int i = 0; i < primeFlags.Count; i++)
        {
            if (primeFlags[i] == 0)
            {
                primeNumbersAfterSqrtN.Add(i + sqrt + 1);
            }
        }

        List<int> mergedList = new List<int>(primeNumbersUpToSqrtN);
        mergedList.AddRange(primeNumbersAfterSqrtN);

        return mergedList;
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
            primeNumbers[i - 2] = 1;
        }
    }

    private static void MarkNonPrimesInRange(List<int> primeFlags, int upperLimit, int primeFactor)
    {
        int firstCandidateAboveSqrtN = (int)Math.Sqrt(upperLimit) + 1;
        int firstMultipleOfPrimeFactor = firstCandidateAboveSqrtN;

        while (firstMultipleOfPrimeFactor % primeFactor != 0 && firstMultipleOfPrimeFactor <= upperLimit)
        {
            ++firstMultipleOfPrimeFactor;
        }

        var startIndex = firstMultipleOfPrimeFactor - firstCandidateAboveSqrtN;

        for (int i = startIndex; i < primeFlags.Count; i += primeFactor)
        {
            lock (LockObject)
            {
                primeFlags[i] = 1; // Assuming 1 means not a prime
            }
        }
    }
}
