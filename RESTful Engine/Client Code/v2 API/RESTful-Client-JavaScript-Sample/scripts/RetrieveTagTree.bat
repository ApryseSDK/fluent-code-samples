@echo off

cd /d "%~dp0.."

npm install & node retrieve-tag-tree.js & pause
