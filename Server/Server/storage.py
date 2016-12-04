import tushare as ts

storage = "../data/raw/"

def save(df, file):
	path = storage+file
	df.to_csv(path)
