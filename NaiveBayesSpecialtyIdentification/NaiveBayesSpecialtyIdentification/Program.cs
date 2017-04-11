using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using LumenWorks.Framework.IO.Csv;

namespace NaiveBayesSpecialtyIdentification
{
    class Program
    {
        static void Main(string[] args)
        {
            string csvPath = @"D:\MicrosoftResearchIndia\SecondOpinion\ML_Code\spam_collection.csv";
            Dictionary<string, Dictionary<string, double>> dictionary = new Dictionary<string, Dictionary<string, double>>();
            Dictionary<string, int> totalWords = new Dictionary<string, int>();
            Dictionary<string, double> targetProbs = new Dictionary<string, double>();
            Dictionary<string, int> targetWordsTotal = new Dictionary<string, int>();
            Dictionary<string, Dictionary<string, double>> wordProbs = new Dictionary<string, Dictionary<string, double>>();
            calculateNB(csvPath, dictionary, totalWords, targetWordsTotal, wordProbs, targetProbs   );
        }

        static void calculateNB(string csvPath, Dictionary<string, Dictionary<string, double>> dictionary, Dictionary<string, int> totalWords, 
            Dictionary<string, int> targetWordsTotal, Dictionary<string, Dictionary<string, double>> wordProbs, Dictionary<string, double> targetProbs)
        {
            int total = 0;
            targetWordsTotal = new Dictionary<string, int>();
            totalWords = new Dictionary<string, int>();
            using (CsvReader reader = new CsvReader(new StreamReader(csvPath), true))
            {
                while (reader.ReadNextRecord())
                {
                    total += 1;
                    string target = reader[1];
                    string message = reader[0];

                    if (!targetWordsTotal.ContainsKey(target))
                    {
                        targetWordsTotal[target] = 1;
                    }
                    else
                    {
                        targetWordsTotal[target] += 1;
                    }

                    Dictionary<string, double> wordCounts = new Dictionary<string, double>();

                    if (!dictionary.ContainsKey(target))
                    {
                        dictionary.Add(target, wordCounts);
                    }
                    else
                    {
                        wordCounts = dictionary[target];
                    }

                    foreach (string word in message.Split())
                    {
                        if (!totalWords.ContainsKey(target))
                        {
                            totalWords[target] = 1;
                        }

                        else
                        {
                            totalWords[target] += 1;
                        }

                        if (!wordCounts.ContainsKey(word.ToLower()))
                        {
                            wordCounts[word.ToLower()] = 1;
                        }
                        else
                        {
                            wordCounts[word.ToLower()] = wordCounts[word.ToLower()] + 1;
                        }
                    }

                    dictionary.Remove(target);
                    dictionary.Add(target, wordCounts);
                }
            }

            // Print the contents of the number of words per target
            foreach (KeyValuePair<string, int> keyvalue in totalWords)
            {
                Console.WriteLine(keyvalue.Key + " : " + keyvalue.Value);
            }

            // Find the word probabilities
            foreach (KeyValuePair<string, Dictionary<string, double>> keyvalue in dictionary)
            {
                string target = keyvalue.Key;
                if (!wordProbs.ContainsKey(target))
                {
                    wordProbs[target] = new Dictionary<string, double>();
                }
                foreach (string word in keyvalue.Value.Keys)
                {
                    wordProbs[target][word] = dictionary[target][word] / totalWords[target];
                }
            }

            // Find the target probs
            foreach (string target in targetWordsTotal.Keys)
            {
                targetProbs[target] = (double) targetWordsTotal[target] / total;
            }

            printDictionary(wordProbs);
            Console.ReadLine();

            // test example
            string test_example = "Lets go for a party Tomorrow";
            string target_value = "";
            double max_prob = 0.0;
            double totalProbs = 0.0;
            Console.WriteLine("Testing starts here");

            foreach (string target in targetProbs.Keys)
            {
                Console.WriteLine(target + " : " + targetProbs[target]);
            }

            foreach (string target in targetProbs.Keys)
            {
                Console.WriteLine(target);
                double prob = targetProbs[target];
                Console.WriteLine("The internal probability is : " + prob);
                foreach (string word in test_example.Split())
                {
                    if (!wordProbs[target].ContainsKey(word.ToLower()))
                    {
                        prob *= (double) 1 / totalWords[target];
                    }
                    else
                    {
                        prob *= (double) wordProbs[target][word.ToLower()];
                    }
                }
                totalProbs += prob;
                if (prob > max_prob)
                {
                    max_prob = prob;
                    target_value = target;
                }
                Console.WriteLine(target + " : " + prob);
            }

            // Print the results
            Console.WriteLine("The target value is : " + target_value + " with probability : " + max_prob / totalProbs);
            Console.ReadLine();
        }

        // Method to print the contents of the dictionary
        static void printDictionary(Dictionary<string, Dictionary<string, double>> dictionary)
        {
            // Print the contents of the dictionary
            foreach (KeyValuePair<string, Dictionary<string, double>> keyvalue in dictionary)
            {
                Console.WriteLine(keyvalue.Key);
                foreach (KeyValuePair<string, double> keyvalue2 in keyvalue.Value)
                {
                    Console.Write(keyvalue2.Key + " : " + keyvalue2.Value + "; ");
                }
                Console.WriteLine();
            }
        }
    }
}