@echo off
echo **********************copy data**********************
robocopy data/raw/basics \\139.199.73.92\c$\quant\data\raw\basics *.xls /PURGE
echo **********************deploy server**********************
robocopy bin \\139.199.73.92\c$\quant\bin /PURGE
echo **********************update server config**********************
\\139.199.73.92\c$\quant\bin\deploy updateconfig --p \\139.199.73.92\c$\quant\bin --q C:\quant\data --n 0-1800,1800-3600 --t C:/quant/data/
echo **********************done**********************