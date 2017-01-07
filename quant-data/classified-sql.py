import tushare as ts
import storage as storage

storage.save_sql(ts.get_industry_classified(),"basics/industry_classified")
storage.save_sql(ts.get_concept_classified(),"basics/concept_classified")
storage.save_sql(ts.get_area_classified(),"basics/area_classified")
storage.save_sql(ts.get_st_classified(),"basics/st_classified")
storage.save_sql(ts.get_hs300s(),"basics/hs300s")
storage.save_sql(ts.get_sz50s(),"basics/sz50s")
storage.save_sql(ts.get_zz500s(),"basics/zz500s")
storage.save_sql(ts.get_terminated(),"basics/terminated")
storage.save_sql(ts.get_suspended(),"basics/suspended")
storage.save_sql(ts.get_sme_classified(),"basics/sme_classified")
storage.save_sql(ts.get_gem_classified(),"basics/gem_classified")
