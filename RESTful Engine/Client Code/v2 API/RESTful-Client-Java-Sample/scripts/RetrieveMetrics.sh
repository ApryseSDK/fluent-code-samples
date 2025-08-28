#!/bin/bash
# This script builds the Java project and then runs the RetrieveMetrics class.

# Exit immediately if any command fails
set -e

cd "$(dirname "$0")/.."

mvn clean install -U && mvn compile exec:java -Dexec.mainClass="com.apryse.fluent.RetrieveMetrics"

echo "Java sample executed successfully."