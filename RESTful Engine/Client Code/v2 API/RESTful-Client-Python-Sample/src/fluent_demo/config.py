from pathlib import Path
from configparser import ConfigParser
from functools import lru_cache
import os


def project_root() -> Path:
    """
    Project root = one folder above 'src'.
    Works whether running as a module or directly.
    """
    return Path(__file__).resolve().parents[2]


@lru_cache(maxsize=1)
def load_config() -> ConfigParser:
    """
    Load config.ini from the project root and expand env vars.
    """
    cfg_path = project_root() / "config.ini"
    if not cfg_path.exists():
        raise FileNotFoundError(f"Configuration file not found: {cfg_path}")

    parser = ConfigParser()
    parser.read(cfg_path, encoding="utf-8")

    # Expand ${VAR} in all sections (including DEFAULT)
    for section in ["DEFAULT", *parser.sections()]:
        for key, value in list(parser.items(section)):
            parser.set(section, key, os.path.expandvars(value))
    return parser


def require_file(path: Path, label: str) -> str:
    """Assert that a file exists and return its absolute string path."""
    abs_path = str(path.resolve())
    if not path.exists():
        raise FileNotFoundError(f"Missing {label}: {abs_path}")
    return abs_path
