import tushare as ts
import storage as storage
import datetime as datetime
import sys
import os
import sqlalchemy as sqlalchemy

def get_year_season(offset):
    y = datetime.datetime.now().year
    s = ((datetime.datetime.now().month + 2) // 3) + offset

    if (s==0):
        y = y -1
        s = 4

    return (y,s)

def available(year,season):
    print("valid report data")
    r = ts.get_report_data(year,season)
    if r is None:
        return False
    return True

def save(r, name,year,season):
    if r is None:
        return None

    r['year'] = year
    r['season'] = season
    r['ts'] = datetime.datetime.now()

    storage.save_sql(r,name, mode='append',
                     dtype={'year': sqlalchemy.types.VARCHAR(20),
                            'season': sqlalchemy.types.VARCHAR(20)
                            })

def get_report_data(year,season):
    if not available(year, season):
        return None;

    print("get_report_data")
    save(ts.get_report_data(year,season),"basics/report_data",year,season)

    print("get_profit_data")
    save(ts.get_profit_data(year,season),"basics/profit_data",year,season)

    filename = "operation_data"
    print("get_operation_data")
    save(ts.get_operation_data(year,season),"basics/operation_data",year,season)

    filename = "growth_data"
    print("get_growth_data")
    save(ts.get_growth_data(year,season),"basics/growth_data",year,season)

    filename = "get_debtpaying_data"
    print("get_debtpaying_data")
    save(ts.get_debtpaying_data(year,season),"basics/debtpaying_data",year,season)

    filename = "get_debtpaying_data"
    print("get_cashflow_data")
    save(ts.get_cashflow_data(year,season),"basics/cashflow_data",year,season)

if len(sys.argv) < 2:
    (year, season) = get_year_season(0)
    print("get report data for season ",year, "/", season)
    get_report_data(year, season)
else:
    thisyear = datetime.datetime.now().year
    year = thisyear + int(sys.argv[1])
    for iyear in range(year,thisyear):
        for iseason in range(1,4):
            print("get report data for season ",iyear, "/", iseason)
            get_report_data(iyear, iseason)
