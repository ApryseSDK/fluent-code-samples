#!/bin/bash
# This script builds the Java project and then runs the GenerateDocument class.

# Exit immediately if any command fails
set -e

mvn clean install -U && mvn compile exec:java -Dexec.mainClass="com.apryse.fluent.GenerateDocument"

echo "Java sample executed successfully."