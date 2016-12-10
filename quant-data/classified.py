import tushare as ts
import storage as storage

storage.save(ts.get_industry_classified(),"basics/industry_classified.csv")
storage.save(ts.get_concept_classified(),"basics/concept_classified.csv")
storage.save(ts.get_area_classified(),"basics/area_classified.csv")
storage.save(ts.get_st_classified(),"basics/st_classified.csv")
storage.save(ts.get_hs300s(),"basics/hs300s.csv")
storage.save(ts.get_sz50s(),"basics/sz50s.csv")
storage.save(ts.get_zz500s(),"basics/zz500s.csv")
storage.save(ts.get_terminated(),"basics/terminated.csv")
storage.save(ts.get_suspended(),"basics/suspended.csv")
storage.save(ts.get_sme_classified(),"basics/sme_classified.csv")
storage.save(ts.get_gem_classified(),"basics/gem_classified.csv")
