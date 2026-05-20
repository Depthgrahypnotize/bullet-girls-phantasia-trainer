import sys
import time
import logging
from memory_reader import GameMemory
from hack_manager import HackManager

logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(name)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)


def main():
    """Main entry point for the trainer."""
    logger.info("Starting Bullet Girls Phantasia Trainer...")

    game = GameMemory("BulletGirlsPhantasia.exe")
    if not game.attach():
        logger.error("Failed to attach to game process. Is the game running?")
        sys.exit(1)

    manager = HackManager(game)
    manager.initialize_hacks()

    print("\n=== Bullet Girls Phantasia Trainer ===")
    print("Hotkeys:")
    print("  F1 - Toggle Infinite HP")
    print("  F2 - Toggle Infinite Ammo")
    print("  F3 - Toggle Infinite Special")
    print("  F4 - Toggle Speed Hack")
    print("  F5 - Toggle One-Hit Kill")
    print("  ESC - Exit")
    print("=" * 40)

    try:
        while True:
            manager.update()
            time.sleep(0.05)
    except KeyboardInterrupt:
        logger.info("Trainer shutting down...")
        manager.disable_all()
        game.detach()


if __name__ == "__main__":
    main()
