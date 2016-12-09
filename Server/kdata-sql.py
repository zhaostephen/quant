import tushare as ts
import storage as storage
import datetime as datetime
import sys
import os
from sqlalchemy import create_engine

engine = create_engine('mysql://quant:Woaiquant123@10.66.111.191/kdata?charset=utf8')

def getCodes():
    stocks = None
    try:
        stocks = ts.get_area_classified()
    except:
        file = storage.getPath('basics/area_classified.csv')
        print("get codes from file ", file)
        stocks = pd.read_csv(file, encoding="gbk")
    if(stocks is None):
        return []
    return stocks['code']

def kdata(code, ktype):
    try:
        data = ts.get_k_data(code,"","",ktype)
        data.to_sql('k'+ktype,engine,if_exists='append')
    except:
        print('error kdata ',code,' | ',ktype)

def make(code):
    kdata(code, "D")
    kdata(code, "W")
    kdata(code, "M")
    kdata(code, "5")
    kdata(code, "15")
    kdata(code, "30")
    kdata(code, "60")

print("get codes")
codes = ['sh','sz','hs300','sz50','zxb','cyb']
codes.extend(getCodes())
count = len(codes)
i = 0
for code in codes:
    i = i + 1
    print(datetime.datetime.now().strftime('%Y-%m-%d %H:%M:%S') + " - ", i, "/", count, "make "+code)
    make(code)