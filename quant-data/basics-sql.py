import tushare as ts
import storage as storage
import sqlalchemy as sqlalchemy

storage.save_sql(ts.get_stock_basics(),"basics/stock_basics",dtype={'code': sqlalchemy.types.VARCHAR(64)})