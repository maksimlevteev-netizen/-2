using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

// ===================================================================================
// ГЛАВНЫЙ КЛАСС УПРАВЛЕНИЯ (ИНТЕРАКТИВНОЕ МЕНЮ НАВИГАЦИИ)
// ===================================================================================
class Program
{
    static async Task Main()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== ВЫБЕРИТЕ ЗАДАНИЕ ДЛЯ ЗАПУСКА ===");
            Console.WriteLine("1. Задание 1.1 - Версия 1 (Monitor / lock)");
            Console.WriteLine("2. Задание 1.1 - Версия 2 (Mutex)");
            Console.WriteLine("3. Задание 1.1 - Версия 3 (Semaphore)");
            Console.WriteLine("4. Задание 1.2 - Обработка 15 наборов (Семафор + lock + Мьютекс)");
            Console.WriteLine("5. Задание 2   - Версия 1 (Синхронные запросы)");
            Console.WriteLine("6. Задание 2   - Версия 2 (Асинхронные запросы)");
            Console.WriteLine("0. Выход из программы");
            Console.WriteLine("=====================================");
            Console.Write("Введите номер пункта: ");

            string choice = Console.ReadLine();
            Console.Clear();

            switch (choice)
            {
                case "1":
                    Lab_1_1_Monitor.Run();
                    break;
                case "2":
                    Lab_1_1_Mutex.Run();
                    break;
                case "3":
                    Lab_1_1_Semaphore.Run();
                    break;
                case "4":
                    Lab_1_2_Sets.Run();
                    break;
                case "5":
                    Lab_2_Sync.Run();
                    break;
                case "6":
                    await Lab_2_Async.RunAsync();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Неверный ввод! Нажмите любую клавишу...");
                    break;
            }

            Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
            Console.ReadKey();
        }
    }
}

// ===================================================================================
// ЗАДАНИЕ 1.1 - ВЕРСИЯ 1 (БЛОКИРОВКА КРИТИЧЕСКОЙ СЕКЦИИ ЧЕРЕЗ MONITOR / LOCK)
// ===================================================================================
class Lab_1_1_Monitor
{
    static int primeCounter = 0;
    static object locker = new object();

    public static void Run()
    {
        primeCounter = 0;
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        Thread[] threads = new Thread[4];

        for (int i = 0; i < 4; i++)
        {
            int threadId = i;
            threads[i] = new Thread(() => CheckNumbers(threadId));
            threads[i].Start();
        }

        for (int i = 0; i < 4; i++)
        {
            threads[i].Join();
        }

        stopwatch.Stop();

        Console.WriteLine($"Общее количество простых чисел: {primeCounter}");
        Console.WriteLine($"Время выполнения (Monitor): {stopwatch.ElapsedMilliseconds} мс");
    }

    static bool IsPrime(int number)
    {
        if (number < 2) return false;
        for (int i = 2; i * i <= number; i++)
        {
            if (number % i == 0) return false;
        }
        return true;
    }

    static void CheckNumbers(int threadId)
    {
        int rangeSize = 2500;
        int start = threadId * rangeSize + 1;
        int end = start + rangeSize - 1;

        for (int i = start; i <= end; i++)
        {
            Console.WriteLine($"Поток {threadId} обрабатывает число: {i}");

            if (IsPrime(i))
            {
                lock (locker)
                {
                    primeCounter++;
                }
            }
        }
    }
}

// ===================================================================================
// ЗАДАНИЕ 1.1 - ВЕРСИЯ 2 (БЛОКИРОВКА КРИТИЧЕСКОЙ СЕКЦИИ ЧЕРЕЗ MUTEX)
// ===================================================================================
class Lab_1_1_Mutex
{
    static int primeCounter = 0;
    static Mutex mutex = new Mutex();

    public static void Run()
    {
        primeCounter = 0;
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        Thread[] threads = new Thread[4];

        for (int i = 0; i < 4; i++)
        {
            int threadId = i;
            threads[i] = new Thread(() => CheckNumbers(threadId));
            threads[i].Start();
        }

        for (int i = 0; i < 4; i++)
        {
            threads[i].Join();
        }

        stopwatch.Stop();

        Console.WriteLine($"Общее количество простых чисел: {primeCounter}");
        Console.WriteLine($"Время выполнения (Mutex): {stopwatch.ElapsedMilliseconds} мс");
    }

    static bool IsPrime(int number)
    {
        if (number < 2) return false;
        for (int i = 2; i * i <= number; i++)
        {
            if (number % i == 0) return false;
        }
        return true;
    }

    static void CheckNumbers(int threadId)
    {
        int rangeSize = 2500;
        int start = threadId * rangeSize + 1;
        int end = start + rangeSize - 1;

        for (int i = start; i <= end; i++)
        {
            Console.WriteLine($"Поток {threadId} обрабатывает число: {i}");

            if (IsPrime(i))
            {
                mutex.WaitOne();
                try
                {
                    primeCounter++;
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
        }
    }
}

// ===================================================================================
// ЗАДАНИЕ 1.1 - ВЕРСИЯ 3 (БЛОКИРОВКА КРИТИЧЕСКОЙ СЕКЦИИ ЧЕРЕЗ ДВОИЧНЫЙ SEMAPHORE)
// ===================================================================================
class Lab_1_1_Semaphore
{
    static int primeCounter = 0;
    static Semaphore semaphore = new Semaphore(1, 1);

    public static void Run()
    {
        primeCounter = 0;
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        Thread[] threads = new Thread[4];

        for (int i = 0; i < 4; i++)
        {
            int threadId = i;
            threads[i] = new Thread(() => CheckNumbers(threadId));
            threads[i].Start();
        }

        for (int i = 0; i < 4; i++)
        {
            threads[i].Join();
        }

        stopwatch.Stop();

        Console.WriteLine($"Общее количество простых чисел: {primeCounter}");
        Console.WriteLine($"Время выполнения (Semaphore): {stopwatch.ElapsedMilliseconds} мс");
    }

    static bool IsPrime(int number)
    {
        if (number < 2) return false;
        for (int i = 2; i * i <= number; i++)
        {
            if (number % i == 0) return false;
        }
        return true;
    }

    static void CheckNumbers(int threadId)
    {
        int rangeSize = 2500;
        int start = threadId * rangeSize + 1;
        int end = start + rangeSize - 1;

        for (int i = start; i <= end; i++)
        {
            Console.WriteLine($"Поток {threadId} обрабатывает число: {i}");

            if (IsPrime(i))
            {
                semaphore.WaitOne();
                try
                {
                    primeCounter++;
                }
                finally
                {
                    semaphore.Release();
                }
            }
        }
    }
}

// ===================================================================================
// ЗАДАНИЕ 1.2 - СЛОЖНОЕ МОДЕЛИРОВАНИЕ ОБРАБОТКИ 15 НАБОРОВ ЧИСЕЛ В ПОТОКАХ
// ===================================================================================
class Lab_1_2_Sets
{
    static int totalSum = 0;
    static Semaphore threadLimiter = new Semaphore(3, 3);
    static object logLocker = new object();
    static Mutex totalMutex = new Mutex();
    static List<string> globalLog = new List<string>();
    static string[] savedSets = GenerateFixedSets();

    public static void Run()
    {
        totalSum = 0;
        globalLog.Clear();

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        Thread[] threads = new Thread[15];

        for (int i = 0; i < 15; i++)
        {
            int setIndex = i;
            threads[i] = new Thread(() => ProcessSet(setIndex));
            threads[i].Start();
        }

        for (int i = 0; i < 15; i++)
        {
            threads[i].Join();
        }

        stopwatch.Stop();

        Console.WriteLine("--- Результаты обработки наборов ---");
        foreach (string logEntry in globalLog)
        {
            Console.WriteLine(logEntry);
        }

        Console.WriteLine("------------------------------------");
        Console.WriteLine($"Общий итог по всем наборам: {totalSum}");
        Console.WriteLine($"Время выполнения программы: {stopwatch.ElapsedMilliseconds} мс");
    }

    static void ProcessSet(int index)
    {
        threadLimiter.WaitOne();
        try
        {
            string currentSetString = savedSets[index];
            string[] stringNumbers = currentSetString.Split(' ');
            int currentSetSum = 0;

            foreach (string numStr in stringNumbers)
            {
                if (!string.IsNullOrEmpty(numStr))
                {
                    currentSetSum += int.Parse(numStr);
                }
            }

            string currentThreadName = $"Thread-{Thread.CurrentThread.ManagedThreadId}";
            string resultMessage = $"Набор №{index + 1}: Сумма = {currentSetSum} (Обработал: {currentThreadName})";

            lock (logLocker)
            {
                globalLog.Add(resultMessage);
            }

            totalMutex.WaitOne();
            try
            {
                totalSum += currentSetSum;
            }
            finally
            {
                totalMutex.ReleaseMutex();
            }
        }
        finally
        {
            threadLimiter.Release();
        }
    }

    static string[] GenerateFixedSets()
    {
        string[] sets = new string[15];
        Random rand = new Random(42);

        for (int i = 0; i < 15; i++)
        {
            List<string> numbers = new List<string>();
            for (int j = 0; j < 100; j++)
            {
                numbers.Add(rand.Next(1, 101).ToString());
            }
            sets[i] = string.Join(" ", numbers);
        }
        return sets;
    }
}

// ===================================================================================
// ЗАДАНИЕ 2 - ВЕРСИЯ 1 (СИНХРОННЫЕ СЕТЕВЫЕ ЗАПРОСЫ БЕЗ ASYNC/AWAIT)
// ===================================================================================
class Lab_2_Sync
{
    public static void Run()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        using (HttpClient client = new HttpClient())
        {
            string[] urls = new string[]
            {
                "https://jsonplaceholder.typicode.com/posts/1",
                "https://jsonplaceholder.typicode.com/posts/2",
                "https://jsonplaceholder.typicode.com/posts/3"
            };

            try
            {
                foreach (string url in urls)
                {
                    Console.WriteLine($"Отправка синхронного запроса к: {url}");

                    HttpResponseMessage response = client.GetAsync(url).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Негативный ответ от сервера. Статус-код: {response.StatusCode}");
                    }

                    string jsonResult = response.Content.ReadAsStringAsync().Result;

                    Console.WriteLine("Ответ от сервера в формате JSON:");
                    Console.WriteLine(jsonResult);
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Программа аварийно завершена. Текст ошибки: {ex.Message}");
                return;
            }
        }

        stopwatch.Stop();
        Console.WriteLine($"Общая время работы синхронных запросов: {stopwatch.ElapsedMilliseconds} мс");
    }
}

// ===================================================================================
// ЗАДАНИЕ 2 - ВЕРСИЯ 2 (ПОЛНОСТЬЮ АСИНХРОННЫЕ СЕТЕВЫЕ ЗАПРОСЫ С ASYNC/AWAIT)
// ===================================================================================
class Lab_2_Async
{
    public static async Task RunAsync()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
            Task firstTask = MakeRequestAsync("https://jsonplaceholder.typicode.com/posts/1");
            Task secondTask = MakeRequestAsync("https://jsonplaceholder.typicode.com/posts/2");
            Task thirdTask = MakeRequestAsync("https://jsonplaceholder.typicode.com/posts/3");

            await Task.WhenAll(firstTask, secondTask, thirdTask);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Программа завершена с ошибкой: {ex.Message}");
            return;
        }

        stopwatch.Stop();
        Console.WriteLine($"Общая время работы асинхронных запросов: {stopwatch.ElapsedMilliseconds} мс");
    }

    static async Task MakeRequestAsync(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            Console.WriteLine($"Запуск асинхронного запроса к: {url}");

            HttpResponseMessage response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Сервер {url} вернул ошибку: {response.StatusCode}");
            }

            string jsonResult = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Успешный асинхронный ответ от {url} (JSON):");
            Console.WriteLine(jsonResult);
            Console.WriteLine();
        }
    }
}
