@echo off
echo **********************copy data**********************
robocopy data/raw/basics c:\quant\data\raw\basics *.xls /PURGE
echo **********************deploy server**********************
robocopy bin c:\quant\bin /PURGE
echo **********************update server config**********************
c:\quant\bin\deploy updateconfig --p c:\quant\bin --q C:\quant\data --n 0-1800,1800-3600 --t C:/quant/data/
echo **********************done**********************