import tushare as ts
import storage as storage
import datetime as datetime
import sys
import os

print("get stocks")
file = "raw/mktdata/stock_" + datetime.datetime.now().strftime('%Y-%m-%d') + ".csv";
storage.save(ts.get_today_all(), file)

print("get index")
file = "raw/mktdata/index_" + datetime.datetime.now().strftime('%Y-%m-%d') + ".csv";
storage.save(ts.get_index(), file)