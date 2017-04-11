from __future__ import division
import pandas as pd

csv_file = 'spam_collection.csv'

col = 'Message'
target_col = 'IsSpam'

words_probs = dict()
word_occurrences = dict()
prob_targets = dict()
target_probs = dict()

def unique(l):
	return list(set(l))

class Tree(dict):
	"""Implementation of perl's autovivification feature."""
	def __missing__(self, key):
		value = self[key] = type(self)()
		return value

if __name__ == "__main__":
	tree = Tree()
	data = pd.read_csv(csv_file)
	for target in unique(data[target_col]):
		words = 0
		target_data = data[data[target_col] == target]['Message']
		message_dict = dict()
		for sentence in target_data:
			for word in sentence.split():
				words += 1
				if word not in message_dict:
					message_dict[word.lower()] = 1
				else:
					message_dict[word.lower()] += 1
		tree[target_col][target] = message_dict
		word_occurrences[target] = words
	print tree
	print word_occurrences

	# Compute Probabilties
	for target in unique(data[target_col]):
		target_dict = tree[target_col][target]
		for key in target_dict:
			target_dict[key] = target_dict[key] / word_occurrences[target]
		tree[target_col][target] = target_dict

	print tree

	# Compute target probabilities
	for target in unique(data[target_col]):
		if target not in target_probs:
			target_probs[target] = len(data[data[target_col] == target])/len(data)

	# Test example
	test_example = "lets go for a party tomorrow"
	target_col_value = ""
	target_value = 0
	total_probs = 0

	for target in unique(data[target_col]):
		prob = target_probs[target]
		target_dict = tree[target_col][target]
		for word in test_example.split():
			if word.lower() in target_dict:
				prob *= target_dict[word.lower()]
			else:
				prob *= 1 / word_occurrences[target]
		total_probs += prob
		if prob > target_value:
			target_value = prob
			target_col_value = target
		print target, prob

	# Print the result
	print("The target value is : " + target_col_value + " with prob : " + str(target_value/total_probs))