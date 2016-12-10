import tushare as ts
import storage as storage

storage.save_sql(ts.get_stock_basics(),"basics/stock_basics.csv")