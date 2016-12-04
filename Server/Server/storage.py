import tushare as ts

storage = "../data/raw/fundamental/"

def save(df, file):
	path = storage+file
	df.to_csv(path)
