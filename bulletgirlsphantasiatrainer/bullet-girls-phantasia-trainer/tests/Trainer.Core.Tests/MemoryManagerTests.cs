using System;
using Trainer.Core;
using Xunit;

namespace Trainer.Core.Tests
{
    /// <summary>
    /// Unit tests for MemoryManager.
    /// Note: These tests require a running instance of BulletGirlsPhantasia.exe to pass.
    /// </summary>
    public class MemoryManagerTests
    {
        [Fact]
        public void Attach_WithValidProcessName_ReturnsTrue()
        {
            // Arrange
            using var memory = new MemoryManager();

            // Act
            bool result = memory.Attach("BulletGirlsPhantasia");

            // Assert
            Assert.True(result, "Game process should be found and attached.");
        }

        [Fact]
        public void Attach_WithInvalidProcessName_ReturnsFalse()
        {
            // Arrange
            using var memory = new MemoryManager();

            // Act
            bool result = memory.Attach("NonexistentProcess");

            // Assert
            Assert.False(result, "Attach should fail for nonexistent process.");
        }

        [Fact]
        public void GetBaseAddress_AfterAttach_ReturnsNonZero()
        {
            // Arrange
            using var memory = new MemoryManager();
            memory.Attach("BulletGirlsPhantasia");

            // Act
            IntPtr baseAddr = memory.GetBaseAddress();

            // Assert
            Assert.NotEqual(IntPtr.Zero, baseAddr);
        }

        [Fact]
        public void ReadWriteInt32_RoundTrip_Succeeds()
        {
            // This test writes to a known unused memory region (example offset)
            // WARNING: This can crash the game if offset is wrong; use with caution.
            // In production, you'd use a dedicated test harness or mock.

            // Arrange
            using var memory = new MemoryManager();
            memory.Attach("BulletGirlsPhantasia");
            IntPtr testAddr = memory.CalculateAddress(memory.GetBaseAddress(), 0x00FFFFFF); // Likely invalid
            int originalValue = 12345;
            int readValue;

            // Act
            memory.WriteInt32(testAddr, originalValue);
            readValue = memory.ReadInt32(testAddr);

            // Assert
            Assert.Equal(originalValue, readValue);
        }

        [Fact]
        public void CalculateAddress_WithBaseAndOffset_ReturnsExpected()
        {
            // Arrange
            using var memory = new MemoryManager();
            IntPtr baseAddr = new IntPtr(0x1000);
            int offset = 0x100;

            // Act
            IntPtr result = memory.CalculateAddress(baseAddr, offset);

            // Assert
            Assert.Equal(new IntPtr(0x1100), result);
        }
    }
}
