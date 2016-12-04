import tushare as ts
import storage as storage
import datetime as datetime

year = datetime.datetime.now().year
season = (datetime.datetime.now().month+2)//3

storage.save(ts.get_report_data(year,season),"report_data"+"_"+year+"_"+season+".csv")
storage.save(ts.get_profit_data(year,season),"profit_data"+"_"+year+"_"+season+".csv")
storage.save(ts.get_operation_data(year,season),"operation_data"+"_"+year+"_"+season+".csv")
storage.save(ts.get_growth_data(year,season),"growth_data"+"_"+year+"_"+season+".csv")
storage.save(ts.get_debtpaying_data(year,season),"debtpaying_data"+"_"+year+"_"+season+".csv")
storage.save(ts.get_cashflow_data(year,season),"cashflow_data"+"_"+year+"_"+season+".csv")