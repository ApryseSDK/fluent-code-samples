@echo off

cd /d "%~dp0.."

npm install & node generate-document.js & pause
