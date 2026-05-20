import logging
from memory_reader import GameMemory

logger = logging.getLogger(__name__)


class HackManager:
    """Manages all hack features for the trainer."""

    def __init__(self, game: GameMemory):
        self.game = game
        self.hacks = {
            "infinite_hp": {"enabled": False, "original_value": None},
            "infinite_ammo": {"enabled": False, "original_value": None},
            "infinite_special": {"enabled": False, "original_value": None},
            "speed_hack": {"enabled": False, "original_value": None},
            "one_hit_kill": {"enabled": False, "original_value": None},
        }

    def initialize_hacks(self):
        """Read original values to allow toggling."""
        try:
            self.hacks["infinite_hp"]["original_value"] = self.game.read_float(GameMemory.HP_OFFSET)
            self.hacks["infinite_ammo"]["original_value"] = self.game.read_int(GameMemory.AMMO_OFFSET)
            self.hacks["infinite_special"]["original_value"] = self.game.read_float(GameMemory.SPECIAL_OFFSET)
            self.hacks["speed_hack"]["original_value"] = self.game.read_float(GameMemory.SPEED_OFFSET)
            self.hacks["one_hit_kill"]["original_value"] = self.game.read_float(GameMemory.DAMAGE_MULT_OFFSET)
            logger.info("Hack values initialized.")
        except Exception as e:
            logger.error(f"Failed to initialize hacks: {e}")

    def toggle_hack(self, hack_name: str):
        """Toggle a specific hack on/off."""
        if hack_name not in self.hacks:
            logger.warning(f"Unknown hack: {hack_name}")
            return

        hack = self.hacks[hack_name]
        hack["enabled"] = not hack["enabled"]
        state = "enabled" if hack["enabled"] else "disabled"
        logger.info(f"{hack_name} {state}.")

        if hack_name == "infinite_hp":
            if hack["enabled"]:
                self.game.write_float(GameMemory.HP_OFFSET, 9999.0)
            else:
                self.game.write_float(GameMemory.HP_OFFSET, hack["original_value"])
        elif hack_name == "infinite_ammo":
            if hack["enabled"]:
                self.game.write_int(GameMemory.AMMO_OFFSET, 999)
            else:
                self.game.write_int(GameMemory.AMMO_OFFSET, hack["original_value"])
        elif hack_name == "infinite_special":
            if hack["enabled"]:
                self.game.write_float(GameMemory.SPECIAL_OFFSET, 100.0)
            else:
                self.game.write_float(GameMemory.SPECIAL_OFFSET, hack["original_value"])
        elif hack_name == "speed_hack":
            if hack["enabled"]:
                self.game.write_float(GameMemory.SPEED_OFFSET, 500.0)  # Faster movement
            else:
                self.game.write_float(GameMemory.SPEED_OFFSET, hack["original_value"])
        elif hack_name == "one_hit_kill":
            if hack["enabled"]:
                self.game.write_float(GameMemory.DAMAGE_MULT_OFFSET, 100.0)  # Multiplier
            else:
                self.game.write_float(GameMemory.DAMAGE_MULT_OFFSET, hack["original_value"])

    def update(self):
        """Check for hotkey presses and update hacks (simplified, no keyboard lib to keep minimal)."""
        # In a real project, you'd use keyboard module or pynput.
        # Here we just keep the hacks active by re-writing each frame if enabled.
        for hack_name, hack in self.hacks.items():
            if hack["enabled"]:
                if hack_name == "infinite_hp":
                    self.game.write_float(GameMemory.HP_OFFSET, 9999.0)
                elif hack_name == "infinite_ammo":
                    self.game.write_int(GameMemory.AMMO_OFFSET, 999)
                elif hack_name == "infinite_special":
                    self.game.write_float(GameMemory.SPECIAL_OFFSET, 100.0)
                elif hack_name == "speed_hack":
                    self.game.write_float(GameMemory.SPEED_OFFSET, 500.0)
                elif hack_name == "one_hit_kill":
                    self.game.write_float(GameMemory.DAMAGE_MULT_OFFSET, 100.0)

    def disable_all(self):
        """Disable all hacks and restore original values."""
        logger.info("Disabling all hacks...")
        for hack_name, hack in self.hacks.items():
            if hack["enabled"]:
                self.toggle_hack(hack_name)
