import tushare as ts
import storage as storage
import datetime as datetime
import sys
import os
from sqlalchemy import create_engine
import sqlalchemy as sqlalchemy

try:
    data = ts.get_today_all()
    data['ts'] = datetime.datetime.now()
    storage.save_sql(data,'kdata/ktoday',
                mode='append',
                dtype={'code': sqlalchemy.types.VARCHAR(64),
                       'name': sqlalchemy.types.VARCHAR(128)})
except:
    print('error ktoday')