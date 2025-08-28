#!/bin/bash
# This script navigates to the project root, installs/updates dependencies,
# and runs the main Python document generation module.

# Exit immediately if any command fails.
set -e

# Change the current directory to the parent directory of this script.
cd "$(dirname "$0")/.."

# Install or upgrade the required Python package.
pip install --upgrade windwardrestapi

# Run the Python module.
python -m src.fluent_demo.RetrieveMetrics

echo "Python sample executed successfully."