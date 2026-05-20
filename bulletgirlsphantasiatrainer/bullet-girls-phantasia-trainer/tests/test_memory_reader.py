import unittest
from unittest.mock import MagicMock, patch
import sys
import os

sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '..')))
from memory_reader import GameMemory


class TestGameMemory(unittest.TestCase):
    """Unit tests for GameMemory class."""

    @patch('memory_reader.pymem.Pymem')
    def test_attach_success(self, mock_pymem):
        """Test successful attachment to process."""
        mock_pymem_instance = MagicMock()
        mock_pymem.return_value = mock_pymem_instance
        mock_module = MagicMock()
        mock_module.lpBaseOfDll = 0x400000
        mock_pymem_instance.process_handle = MagicMock()
        with patch('memory_reader.pymem.process.module_from_name', return_value=mock_module):
            game = GameMemory("TestGame.exe")
            result = game.attach()
            self.assertTrue(result)
            self.assertEqual(game.base_address, 0x400000)

    @patch('memory_reader.pymem.Pymem')
    def test_attach_failure(self, mock_pymem):
        """Test failed attachment."""
        mock_pymem.side_effect = Exception("Process not found")
        game = GameMemory("Nonexistent.exe")
        result = game.attach()
        self.assertFalse(result)
        self.assertIsNone(game.pm)

    @patch('memory_reader.pymem.Pymem')
    def test_read_float(self, mock_pymem):
        """Test reading a float from memory."""
        mock_pymem_instance = MagicMock()
        mock_pymem.return_value = mock_pymem_instance
        mock_pymem_instance.read_float.return_value = 100.0
        mock_module = MagicMock()
        mock_module.lpBaseOfDll = 0x400000
        mock_pymem_instance.process_handle = MagicMock()
        with patch('memory_reader.pymem.process.module_from_name', return_value=mock_module):
            game = GameMemory("TestGame.exe")
            game.attach()
            value = game.read_float(0x100)
            self.assertEqual(value, 100.0)
            mock_pymem_instance.read_float.assert_called_with(0x400100)

    @patch('memory_reader.pymem.Pymem')
    def test_write_float(self, mock_pymem):
        """Test writing a float to memory."""
        mock_pymem_instance = MagicMock()
        mock_pymem.return_value = mock_pymem_instance
        mock_module = MagicMock()
        mock_module.lpBaseOfDll = 0x400000
        mock_pymem_instance.process_handle = MagicMock()
        with patch('memory_reader.pymem.process.module_from_name', return_value=mock_module):
            game = GameMemory("TestGame.exe")
            game.attach()
            game.write_float(0x100, 999.0)
            mock_pymem_instance.write_float.assert_called_with(0x400100, 999.0)


if __name__ == '__main__':
    unittest.main()
