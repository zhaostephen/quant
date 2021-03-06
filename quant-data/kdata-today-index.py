﻿import tushare as ts
import storage as storage
import datetime as datetime
import sys
import os

def getCodes():
    return ['002233','sh','sz','hs300','sz50','zxb','cyb']

def kdata(code, ktype):
    file = "kdata/" + ktype +"/" + code + ".csv";
    data = ts.get_k_data(code,"","",ktype)
    if(not data is None):
        storage.save(ts.get_k_data(code,"","",ktype),file)

def make(code):
    kdata(code, "D")
    kdata(code, "W")
    kdata(code, "M")
    kdata(code, "5")
    kdata(code, "15")
    kdata(code, "30")
    kdata(code, "60")

print("get codes")
codes = getCodes()
count = len(codes)
i = 0
for code in codes:
    i = i + 1
    print(datetime.datetime.now().strftime('%Y-%m-%d %H:%M:%S') + " - ", i, "/", count, "make "+code)
    make(code)