from __future__ import division
import pandas as pd

csv_file = 'playtennis.csv'

cols = ['Outlook', 'Temperature', 'Humidity', 'Wind']
target_col = 'PlayTennis'
prob_targets = dict()

class Tree(dict):
	"""Implementation of perl's autovivification feature."""
	def __missing__(self, key):
		value = self[key] = type(self)()
		return value

# Function returns unique values of a list
def unique(mylist):
	return list(set(mylist))

if __name__ == "__main__":
	# Initialize naive bayes probabilities 
	tree = Tree()
	df = pd.DataFrame.from_csv(csv_file)
	for target in unique(df[target_col]):
		for attribute in cols:
			for unique_attribute in unique(df[attribute]):
				tree[target_col][target][attribute][unique_attribute] = (len(df.loc[(df[target_col]==target) & (df[attribute]==unique_attribute)]))/(len(df.loc[df[target_col] == target]))
	print tree

	for target in unique(df[target_col]):
		if target not in prob_targets:
			prob_targets[target] = len(df.loc[df[target_col] == target]) / len(df)
		print prob_targets

	# Testing the NB model
	test_example = {'Outlook':'Sunny', 'Temperature':'Hot', 'Humidity':'High', 'Wind':'Weak'}
	target_predicted = ""
	total_predictions = 0
	target_max = 0
	for target in unique(df[target_col]):
		prob = prob_targets[target]
		for attribute in cols:
			prob *= tree[target_col][target][attribute][test_example[attribute]]
		print target, prob
		total_predictions += prob	
		if prob > target_max:
			target_max = prob
			target_predicted = target

	print 'The predicted value is : '
	print target_predicted, target_max/total_predictions