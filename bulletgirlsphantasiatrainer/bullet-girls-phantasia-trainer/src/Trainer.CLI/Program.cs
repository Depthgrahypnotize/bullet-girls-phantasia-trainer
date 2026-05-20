using System;
using Trainer.Core;

namespace Trainer.CLI
{
    /// <summary>
    /// Console-based trainer for Bullet Girls Phantasia.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Bullet Girls Phantasia Trainer");
            Console.WriteLine("==============================");
            Console.WriteLine();

            using var memory = new MemoryManager();
            if (!memory.Attach("BulletGirlsPhantasia"))
            {
                Console.WriteLine("Error: Could not find game process. Make sure Bullet Girls Phantasia is running.");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Attached to game process successfully.");
            Console.WriteLine();

            using var engine = new TrainerEngine(memory);
            engine.Start();

            Console.WriteLine("Controls:");
            Console.WriteLine("  F1 - Toggle Infinite Health");
            Console.WriteLine("  F2 - Toggle Infinite Ammo");
            Console.WriteLine("  F3 - Toggle One-Hit Kill");
            Console.WriteLine("  ESC - Exit");
            Console.WriteLine();

            bool running = true;
            while (running)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    switch (key)
                    {
                        case ConsoleKey.F1:
                            engine.InfiniteHealthEnabled = !engine.InfiniteHealthEnabled;
                            Console.WriteLine($"Infinite Health: {(engine.InfiniteHealthEnabled ? "ON" : "OFF")}");
                            break;
                        case ConsoleKey.F2:
                            engine.InfiniteAmmoEnabled = !engine.InfiniteAmmoEnabled;
                            Console.WriteLine($"Infinite Ammo: {(engine.InfiniteAmmoEnabled ? "ON" : "OFF")}");
                            break;
                        case ConsoleKey.F3:
                            engine.OneHitKillEnabled = !engine.OneHitKillEnabled;
                            Console.WriteLine($"One-Hit Kill: {(engine.OneHitKillEnabled ? "ON" : "OFF")}");
                            break;
                        case ConsoleKey.Escape:
                            running = false;
                            break;
                    }
                }

                System.Threading.Thread.Sleep(100);
            }

            engine.Stop();
            Console.WriteLine("Trainer stopped. Goodbye!");
        }
    }
}
