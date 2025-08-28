#!/bin/bash
# This script builds the Java project and then runs the RetrieveTagTree class.

# Exit immediately if any command fails
set -e

cd "$(dirname "$0")/.."

mvn clean install -U && mvn compile exec:java -Dexec.mainClass="com.apryse.fluent.RetrieveTagTree"

echo "Java sample executed successfully."