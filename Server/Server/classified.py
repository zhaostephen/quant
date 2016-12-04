import tushare as ts

storage = "../data/raw/"

def save(df, file):
	path = storage+file
	df.to_csv(path)

save(ts.get_industry_classified(),"industry_classified.csv")
save(ts.get_concept_classified(),"concept_classified.csv")
save(ts.get_area_classified(),"area_classified.csv")
save(ts.get_st_classified(),"st_classified.csv")
save(ts.get_hs300s(),"hs300s.csv")
save(ts.get_sz50s(),"sz50s.csv")
save(ts.get_zz500s(),"zz500s.csv")
save(ts.get_terminated(),"terminated.csv")
save(ts.get_suspended(),"suspended.csv")
save(ts.get_sme_classified(),"sme_classified.csv")
save(ts.get_gem_classified(),"gem_classified.csv")
