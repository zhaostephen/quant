import tushare as ts
import os

storage = "D:/quant/data/raw/fundamental/"

def fileExists(filename):
    return os.path.exists(getPath(filename))

def getPath(file):
    return storage+file

def save(df, file):
    if df is None:
        return
    
    path = storage+file
    df.to_csv(path)
