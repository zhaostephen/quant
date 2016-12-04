import tushare as ts
import storage as storage

storage.save(ts.get_industry_classified(),"raw/basics/industry_classified.csv")
storage.save(ts.get_concept_classified(),"raw/basics/concept_classified.csv")
storage.save(ts.get_area_classified(),"raw/basics/area_classified.csv")
storage.save(ts.get_st_classified(),"raw/basics/st_classified.csv")
storage.save(ts.get_hs300s(),"raw/basics/hs300s.csv")
storage.save(ts.get_sz50s(),"raw/basics/sz50s.csv")
storage.save(ts.get_zz500s(),"raw/basics/zz500s.csv")
storage.save(ts.get_terminated(),"raw/basics/terminated.csv")
storage.save(ts.get_suspended(),"raw/basics/suspended.csv")
storage.save(ts.get_sme_classified(),"raw/basics/sme_classified.csv")
storage.save(ts.get_gem_classified(),"raw/basics/gem_classified.csv")
