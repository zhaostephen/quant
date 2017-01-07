import tushare as ts
import storage as storage
import datetime as datetime
import sys
import os
from sqlalchemy import create_engine
import pandas as pd

storage = "D:/quant/data/"

def fileExists(filename):
    return os.path.exists(getPath(filename))

def getPath(file):
    return storage + file

def save(df, file):
    if df is None:
        return
    
    path = storage + file

    ensure_dir(path)

    df.to_csv(path)

def ensure_dir(f):
    d = os.path.dirname(f)
    if not os.path.exists(d):
        os.makedirs(d)

def getCodes():
    stocks = None
    codes = ['sh','sz','hs300','sz50','zxb','cyb']
    try:
        stocks = ts.get_area_classified()
    except:
        file = getPath('basics/basics.csv')
        print("get codes from file ", file)
        stocks = pd.read_csv(file, encoding="gbk")
    if(stocks is None):
        return codes
    codes.extend(stocks['code'])
    return codes

def save_sql(data, file, mode='replace', dtype=None,index=False):
    if data is None:
        return
    
    splits = file.split('/', 1)
    db = splits[0]
    table = splits[1].replace(".csv","")

    engine = create_engine('mysql://quant:Woaiquant123@10.66.111.191/' + db + '?charset=utf8')

    print(mode + " sql to ", db, "/", table)
    data.to_sql(table,engine,index=index,if_exists=mode, dtype=dtype)
