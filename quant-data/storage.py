import tushare as ts
import os

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

def save_sql(data, file, mode='replace'):
    if df is None:
        return
    
    splits = file.split('/', 1)
    db = splits[0]
    table = splits[1].replace(".csv","")

    print(mode + " sql to ", db, "/", table)

    data.to_sql(table,engine,if_exists=mode)
