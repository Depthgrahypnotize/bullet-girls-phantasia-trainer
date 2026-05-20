using System;
using System.Threading;
using System.Threading.Tasks;

namespace Trainer.Core
{
    /// <summary>
    /// Core trainer logic for Bullet Girls Phantasia.
    /// Manages cheats and applies memory patches.
    /// </summary>
    public class TrainerEngine : IDisposable
    {
        private readonly MemoryManager _memory;
        private CancellationTokenSource _cts;
        private Task _loopTask;

        // Offsets relative to game base address (example values, would be reverse-engineered)
        private const int HealthOffset = 0x00A1B2C0;
        private const int AmmoOffset = 0x00A1B2C4;
        private const int MaxHealthValue = 100;
        private const int MaxAmmoValue = 999;

        public bool InfiniteHealthEnabled { get; set; }
        public bool InfiniteAmmoEnabled { get; set; }
        public bool OneHitKillEnabled { get; set; }

        public TrainerEngine(MemoryManager memory)
        {
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));
        }

        /// <summary>
        /// Starts the trainer loop that continuously applies cheats.
        /// </summary>
        public void Start()
        {
            if (_loopTask != null)
                return;

            _cts = new CancellationTokenSource();
            _loopTask = Task.Run(() => TrainLoop(_cts.Token));
        }

        /// <summary>
        /// Stops the trainer loop.
        /// </summary>
        public void Stop()
        {
            _cts?.Cancel();
            _loopTask?.Wait(1000);
            _loopTask = null;
        }

        private void TrainLoop(CancellationToken token)
        {
            try
            {
                IntPtr baseAddr = _memory.GetBaseAddress();

                while (!token.IsCancellationRequested)
                {
                    if (InfiniteHealthEnabled)
                    {
                        IntPtr healthAddr = _memory.CalculateAddress(baseAddr, HealthOffset);
                        int currentHealth = _memory.ReadInt32(healthAddr);
                        if (currentHealth < MaxHealthValue)
                        {
                            _memory.WriteInt32(healthAddr, MaxHealthValue);
                        }
                    }

                    if (InfiniteAmmoEnabled)
                    {
                        IntPtr ammoAddr = _memory.CalculateAddress(baseAddr, AmmoOffset);
                        _memory.WriteInt32(ammoAddr, MaxAmmoValue);
                    }

                    if (OneHitKillEnabled)
                    {
                        // Example: Overwrite enemy health check instruction with NOPs (0x90)
                        // This is a placeholder; real implementation requires pattern scanning
                        IntPtr oneHitAddr = _memory.CalculateAddress(baseAddr, 0x00B3A100);
                        byte[] nopSled = new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90 };
                        _memory.WriteBytes(oneHitAddr, nopSled);
                    }

                    // Sleep to reduce CPU usage
                    Thread.Sleep(50);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected on stop
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Trainer loop error: {ex.Message}");
            }
        }

        public void Dispose()
        {
            Stop();
            _memory?.Dispose();
        }
    }
}
