@echo off
echo **********************copy data**********************
robocopy data/raw/fundamental \\139.199.73.92\c$\quant\data\raw\fundamental /PURGE
echo **********************deploy server**********************
robocopy bin \\139.199.73.92\c$\quant\bin /PURGE
echo **********************update server config**********************
\\139.199.73.92\c$\quant\bin\server-cli updateconfig --p \\139.199.73.92\c$\quant\bin --q C:\quant\data
