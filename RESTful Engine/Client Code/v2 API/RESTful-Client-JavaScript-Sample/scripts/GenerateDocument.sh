#!/bin/bash
# This script navigates to the project root, installs dependencies,
# and runs the GenerateDocument JavaScript sample.

# Exit immediately if any command fails.
set -e

# Change the current directory to the parent directory of this script.
cd "$(dirname "$0")/.."

# Install dependencies.
npm install

# Run the sample.
node generate-document.js

echo "JavaScript sample executed successfully."
