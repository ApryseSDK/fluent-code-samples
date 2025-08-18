@echo off
cd /d "%~dp0.."
pip install windwardrestapi==22.2.0.52
python -m src.fluent_demo.RetrieveTagTree
pause
