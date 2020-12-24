using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.IO;
namespace Lab15
{
    class Program
    {
        public static ManualResetEventSlim res1 = new ManualResetEventSlim(true);
        public static ManualResetEventSlim res2 = new ManualResetEventSlim(false);
        public static ManualResetEventSlim res3 = new ManualResetEventSlim(false);
        public static ManualResetEventSlim res4 = new ManualResetEventSlim(true);
        public static SemaphoreSlim sem = new SemaphoreSlim(1);
        public static SemaphoreSlim sem2 = new SemaphoreSlim(1);
        public static bool isSimple(int n)
        {
            var result = true;
            if (n > 1)
            {
                for (var i = 2u; i < n; i++)
                {
                    if (n % i == 0)
                    {
                        result = false;
                        break;
                    }
                }
            }
            else
            {
                result = false;
            }
            return result;
        }

        public static void EvenFirst()
        {

            int n = 10;
            sem.Wait();
            for (int i = 2; i < n; i += 2)
            {
                Console.WriteLine(i);
            }
            res2.Set();
            res1.Reset();
            sem.Release();
        }
        public static void OddLast()
        {
            int n = 10;
            sem.Wait();
            res2.Wait();
            for (int i = 1; i < n; i += 2)
            {
                Console.WriteLine(i);
            }
        }

        public static void EvenAndOdd()
        {

            int n = 10;
            
            
            for (int i = 2; i < n; i += 2)
            {
                res3.Wait();
                sem2.Wait();

                Console.WriteLine(i);
                sem2.Release();
                res4.Set();

                res3.Reset();

            }

        }
        public static void OddAndEven()
        {
            int n = 10;
            
            for (int i = 1; i < n; i += 2)
            {
                res4.Wait();
                sem2.Wait();

                Console.WriteLine(i);
                sem2.Release();
                res3.Set();
                res4.Reset();

            }
        }

        public static void Even()
        {

            int n = 10;
            for (int i = 2; i < n; i += 2)
            {
                Thread.Sleep(30);
                Console.WriteLine(i);

            }
        }
        public static void Odd()
        {
            int n = 10;
            for (int i = 1; i < n; i += 2)
            {
                Thread.Sleep(60);
                Console.WriteLine(i);

            }
        }

        public static void Count(object obj)
        {
            int x = (int)obj;
            for (int i = 1; i < 9; i++, x++)
            {
                Console.WriteLine($"{x * i}");
            }
        }
        static void Main(string[] args)
        {
            var allProcess = Process.GetProcesses();
            Console.WriteLine("Process:");
            foreach (Process proc in Process.GetProcesses())
            {
                Console.WriteLine($"Имя процесса: {proc.ProcessName}\nID процесса: {proc.Id}\n\n");
            }

            AppDomain newD = AppDomain.CreateDomain("New Domain");
            newD.Load(System.Reflection.Assembly.GetExecutingAssembly().GetName());
            AppDomain.Unload(newD);

            AppDomain domain = AppDomain.CurrentDomain;
            Console.WriteLine($"Базовый каталог: {domain.BaseDirectory}, Имя домена: {domain.FriendlyName}, ID домена: {domain.Id}");

            Console.WriteLine("Введите n:");
            int n = Convert.ToInt32(Console.ReadLine());

            Thread thSimple = new Thread(() => isSimple(n));
            thSimple.Start();
            using (StreamWriter sw = new StreamWriter(@"C:\Users\User\Desktop\ООП\Lab15\Lab15\primenumbers.txt", false, System.Text.Encoding.Default))
            {
                for (int i = 2; i <= n; i++)
                {
                    if (isSimple(i))
                    {
                        Console.WriteLine(i);
                        sw.WriteLine(i);
                    }
                }
            }
            
            Thread t = Thread.CurrentThread;
            t.Name = "Поток th1";
            Console.WriteLine($"Имя потока : {t.Name}");
            Console.WriteLine($"Приотирет потока : {t.Priority}");
            Console.WriteLine($"ID потока : {Thread.CurrentThread.ManagedThreadId}");
            Thread.Sleep(5000);
            Console.WriteLine($"Текущее состояние потока : {Thread.CurrentThread.ThreadState}");
            Console.WriteLine($"Текущее состояние потока : {thSimple.ThreadState}");

            Thread th1 = new Thread(new ThreadStart(EvenFirst));
            Thread th2 = new Thread(new ThreadStart(OddLast));

            Thread th3 = new Thread(new ThreadStart(Even));
            Thread th4 = new Thread(new ThreadStart(Odd));

            Thread th5 = new Thread(new ThreadStart(EvenAndOdd));
            Thread th6 = new Thread(new ThreadStart(OddAndEven));

            th3.Priority = ThreadPriority.Lowest;
            th1.Start();
            th2.Start();
            th1.Join();
            th2.Join();
            Console.WriteLine("Поочередно:");

            th5.Start();
            th6.Start();
            th5.Join();
            th6.Join();
            Console.WriteLine("Разная скорость:");

            th3.Start();
            th4.Start();
            th3.Join();
            th4.Join();

            int num = 0;
            TimerCallback tm = new TimerCallback(Count);
            Timer timer = new Timer(tm, num, 0, 2000);
            Console.ReadLine();
        }
    }
}
