@echo off

cd /d "%~dp0.."

npm install & node retrieve-metrics.js & pause
