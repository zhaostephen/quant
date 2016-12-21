import tushare as ts
import storage as storage
import datetime as datetime
import sys
import os
from sqlalchemy import create_engine
import sqlalchemy as sqlalchemy

engine = create_engine('mysql://quant:Woaiquant123@10.66.111.191/kdata?charset=utf8')

try:
    data = ts.get_today_all()
    data['ts'] = datetime.datetime.now()
    data.to_sql('ktoday',engine,
                if_exists='append',
                index=False,
                dtype={'code': sqlalchemy.types.VARCHAR(64),
                       'name': sqlalchemy.types.VARCHAR(128)
                       })
except:
    print('error ktoday')