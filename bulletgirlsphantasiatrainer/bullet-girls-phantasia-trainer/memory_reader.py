import pymem
import pymem.process
import logging

logger = logging.getLogger(__name__)


class GameMemory:
    """Handles memory reading/writing for Bullet Girls Phantasia."""

    # Known offsets for game version 1.0.0 (Steam)
    # These are example offsets; real ones would need to be scanned
    HP_OFFSET = 0x00A3B2C0
    AMMO_OFFSET = 0x00A3B3E0
    SPECIAL_OFFSET = 0x00A3B500
    SPEED_OFFSET = 0x00A3B620
    DAMAGE_MULT_OFFSET = 0x00A3B740

    def __init__(self, process_name: str):
        self.process_name = process_name
        self.pm = None
        self.process = None
        self.base_address = None

    def attach(self) -> bool:
        """Attach to the game process."""
        try:
            self.pm = pymem.Pymem(self.process_name)
            self.process = pymem.process.module_from_name(self.pm.process_handle, self.process_name)
            self.base_address = self.process.lpBaseOfDll
            logger.info(f"Attached to {self.process_name} at base 0x{self.base_address:X}")
            return True
        except pymem.exception.ProcessNotFound:
            logger.error(f"Process {self.process_name} not found.")
            return False
        except Exception as e:
            logger.error(f"Failed to attach: {e}")
            return False

    def detach(self):
        """Detach from the process."""
        if self.pm:
            self.pm.close_process()
            logger.info("Detached from process.")

    def read_float(self, offset: int) -> float:
        """Read a float from the game's base address + offset."""
        try:
            addr = self.base_address + offset
            return self.pm.read_float(addr)
        except Exception as e:
            logger.warning(f"Failed to read at offset 0x{offset:X}: {e}")
            return 0.0

    def write_float(self, offset: int, value: float):
        """Write a float to the game's base address + offset."""
        try:
            addr = self.base_address + offset
            self.pm.write_float(addr, value)
        except Exception as e:
            logger.warning(f"Failed to write at offset 0x{offset:X}: {e}")

    def read_int(self, offset: int) -> int:
        """Read an integer from the game's base address + offset."""
        try:
            addr = self.base_address + offset
            return self.pm.read_int(addr)
        except Exception as e:
            logger.warning(f"Failed to read at offset 0x{offset:X}: {e}")
            return 0

    def write_int(self, offset: int, value: int):
        """Write an integer to the game's base address + offset."""
        try:
            addr = self.base_address + offset
            self.pm.write_int(addr, value)
        except Exception as e:
            logger.warning(f"Failed to write at offset 0x{offset:X}: {e}")
