import tushare as ts
import storage as storage
import datetime as datetime
import sys
import os

def getCodes():
    stocks = ts.get_area_classified()
    if(stocks is None):
        return []
    return stocks['code']

def kdata(code, ktype):
    file = "raw/kdata/" + ktype +"/" + code + ".csv";
    if(not storage.fileExists(file)):
        data = ts.get_k_data(code,"","",ktype)
        if(not data is None):
            storage.save(ts.get_k_data(code,"","",ktype),"raw/kdata/" + ktype +"/" + code + ".csv")

codes = getCodes()

for code in codes:
    print("make "+code)
    kdata(code, "D")
    kdata(code, "W")
    kdata(code, "M")
    kdata(code, "5")
    kdata(code, "15")
    kdata(code, "30")
    kdata(code, "60")