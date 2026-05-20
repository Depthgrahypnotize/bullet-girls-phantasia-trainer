using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Trainer.Core
{
    /// <summary>
    /// Provides low-level memory operations for the target process.
    /// </summary>
    public class MemoryManager : IDisposable
    {
        private IntPtr _processHandle;
        private Process _process;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        private const uint PROCESS_ALL_ACCESS = 0x1F0FFF;

        /// <summary>
        /// Attaches to a process by its name.
        /// </summary>
        /// <param name="processName">The process name (without extension).</param>
        /// <returns>True if successfully attached.</returns>
        public bool Attach(string processName)
        {
            var processes = Process.GetProcessesByName(processName);
            if (processes.Length == 0)
                return false;

            _process = processes[0];
            _processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, _process.Id);
            return _processHandle != IntPtr.Zero;
        }

        /// <summary>
        /// Reads bytes from the target process memory.
        /// </summary>
        public byte[] ReadBytes(IntPtr address, int size)
        {
            byte[] buffer = new byte[size];
            if (!ReadProcessMemory(_processHandle, address, buffer, size, out _))
                throw new InvalidOperationException($"Failed to read memory at 0x{address.ToInt64():X}");
            return buffer;
        }

        /// <summary>
        /// Writes bytes to the target process memory.
        /// </summary>
        public void WriteBytes(IntPtr address, byte[] data)
        {
            if (!WriteProcessMemory(_processHandle, address, data, data.Length, out _))
                throw new InvalidOperationException($"Failed to write memory at 0x{address.ToInt64():X}");
        }

        /// <summary>
        /// Reads a 4-byte integer from the target process memory.
        /// </summary>
        public int ReadInt32(IntPtr address)
        {
            return BitConverter.ToInt32(ReadBytes(address, 4), 0);
        }

        /// <summary>
        /// Writes a 4-byte integer to the target process memory.
        /// </summary>
        public void WriteInt32(IntPtr address, int value)
        {
            WriteBytes(address, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Reads a 4-byte float from the target process memory.
        /// </summary>
        public float ReadFloat(IntPtr address)
        {
            return BitConverter.ToSingle(ReadBytes(address, 4), 0);
        }

        /// <summary>
        /// Writes a 4-byte float to the target process memory.
        /// </summary>
        public void WriteFloat(IntPtr address, float value)
        {
            WriteBytes(address, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Gets the base address of the main module.
        /// </summary>
        public IntPtr GetBaseAddress()
        {
            return _process.MainModule.BaseAddress;
        }

        /// <summary>
        /// Calculates an absolute address from a base address and offset.
        /// </summary>
        public IntPtr CalculateAddress(IntPtr baseAddress, int offset)
        {
            return IntPtr.Add(baseAddress, offset);
        }

        public void Dispose()
        {
            if (_processHandle != IntPtr.Zero)
            {
                CloseHandle(_processHandle);
                _processHandle = IntPtr.Zero;
            }
            _process?.Dispose();
        }
    }
}
