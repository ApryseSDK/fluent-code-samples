@echo off

cd /d "%~dp0.."

mvn clean install -U & mvn compile exec:java -Dexec.mainClass="com.apryse.fluent.GenerateDocument" & pause