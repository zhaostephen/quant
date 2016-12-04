import tushare as ts
import storage as storage
import datetime as datetime
import sys

def get_year_season(offset):
    y = datetime.datetime.now().year
    s = ((datetime.datetime.now().month + 2) // 3) + offset

    if (s==0):
        y = y -1
        s = 4

    return (y,s)

def get_report_data(year,season):
    csv = str(year)+"_"+str(season)+".csv"
    print("get_report_data")
    r = ts.get_report_data(year,season)
    if r is None:
        return r

    storage.save(ts.get_report_data(year,season),"report_data"+"_"+csv)
    print("get_profit_data")
    storage.save(ts.get_profit_data(year,season),"profit_data"+"_"+csv)
    print("get_operation_data")
    storage.save(ts.get_operation_data(year,season),"operation_data"+"_"+csv)
    print("get_growth_data")
    storage.save(ts.get_growth_data(year,season),"growth_data"+"_"+csv)
    print("get_debtpaying_data")
    storage.save(ts.get_debtpaying_data(year,season),"debtpaying_data"+"_"+csv)
    print("get_cashflow_data")
    storage.save(ts.get_cashflow_data(year,season),"cashflow_data"+"_"+csv)

(year, season) = get_year_season(0)
print("get report data for season ",year, "/", season)
r = get_report_data(year, season)
if r is None:
    print("unavailable season ",year, "/", season)
    (prevyear, prevseason) = get_year_season(-1)
    print("get report data for season ",prevyear, "/", prevseason)
    get_report_data(prevyear, prevseason)
