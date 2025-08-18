@echo off
cd /d "%~dp0.."
pip install --upgrade windwardrestapi
python -m src.fluent_demo.RetrieveMetrics
pause
