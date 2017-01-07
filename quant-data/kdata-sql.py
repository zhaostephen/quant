import tushare as ts
import storage as storage
import datetime as datetime
import sys
import os
from sqlalchemy import create_engine
import sqlalchemy as sqlalchemy

def kdata(code, ktype):
    try:
        data = ts.get_k_data(code,"","",ktype)
        if data is None:
            return

        data['ts'] = datetime.datetime.now()

        storage.save_sql(data,
                         'kdata/' + 'k' + ktype, 
                         mode='append',
                         dtype={'code': sqlalchemy.types.VARCHAR(64)})
    except Exception as e:
        print('error kdata ',code,' | ',ktype,e)

def make(code):
    kdata(code, "D")
    kdata(code, "W")
    kdata(code, "M")
    kdata(code, "5")
    kdata(code, "15")
    kdata(code, "30")
    kdata(code, "60")

print("get codes")
codes = storage.getCodes()
count = len(codes)
i = 0
for code in codes:
    i = i + 1
    print(datetime.datetime.now().strftime('%Y-%m-%d %H:%M:%S') + " - ", i, "/", count, "make "+code)
    make(code)