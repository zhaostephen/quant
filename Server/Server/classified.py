import tushare as ts
import storage as storage

storage.save(ts.get_industry_classified(),"industry_classified.csv")
storage.save(ts.get_concept_classified(),"concept_classified.csv")
storage.save(ts.get_area_classified(),"area_classified.csv")
storage.save(ts.get_st_classified(),"st_classified.csv")
storage.save(ts.get_hs300s(),"hs300s.csv")
storage.save(ts.get_sz50s(),"sz50s.csv")
storage.save(ts.get_zz500s(),"zz500s.csv")
storage.save(ts.get_terminated(),"terminated.csv")
storage.save(ts.get_suspended(),"suspended.csv")
storage.save(ts.get_sme_classified(),"sme_classified.csv")
storage.save(ts.get_gem_classified(),"gem_classified.csv")
