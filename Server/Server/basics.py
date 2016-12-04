import tushare as ts
import storage as storage

storage.save(ts.get_stock_basics(),"stock_basics.csv")