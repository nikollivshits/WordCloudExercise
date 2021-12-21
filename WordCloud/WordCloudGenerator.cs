using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WordCloud
{
	class WordCloudGenerator
	{
		private const string FilePattern = "*.txt";

		private static readonly Lazy<WordCloudGenerator> WordCloudGeneratorInstance =
			new Lazy<WordCloudGenerator>(() => new WordCloudGenerator());

		public static WordCloudGenerator Instance => WordCloudGeneratorInstance.Value;

		private WordCloudGenerator()
		{
		}

		public void Generate(string dirPath, bool isRecursive, string outPath)
		{
			Console.WriteLine("Starting reading file");
			var wordCounters = CountWords(dirPath,
				isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
			Console.WriteLine("Finished reading file");

			Console.WriteLine("Starting calculating word cloud");
			var wordCloud = BuildWordCloud(wordCounters);
			Console.WriteLine("Finished calculating word cloud");

			using StreamWriter writer = new StreamWriter(outPath);
			foreach (WordCloudEntry entry in wordCloud)
			{
				writer.WriteLine(entry.ToString());
			}
		}

		private ConcurrentDictionary<string, int> CountWords(string dirPath, SearchOption searchOption)
		{
			var wordCounters = new ConcurrentDictionary<string, int>();
			var lockObj = new object();
			IEnumerable<string> files;
			try
			{
				files = Directory.EnumerateFiles(dirPath, FilePattern, searchOption);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error listing files in directory {e}");
				return wordCounters;
			}

			Parallel.ForEach(files, file =>
			{
				IEnumerable<string> lines;
				try
				{
					lines = File.ReadLines(@file);
				}
				catch (IOException e)
				{
					Console.WriteLine($"Error reading file {e}");
					return;
				}

				foreach (var line in lines)
				{
					foreach (var word in line.Split(new[] {' '}))
					{
						lock (lockObj)
						{
							wordCounters.AddOrUpdate(word, 1, (k, v) => v + 1);
						}
					}
				}
			});
			return wordCounters;
		}

		private IEnumerable<WordCloudEntry> BuildWordCloud(IDictionary<string, int> wordCounts)
		{
			var filteredWordCounts = wordCounts
				.OrderByDescending(c => c.Value)
				.Where((k, v) => v > 1);
			var highestFrequency = filteredWordCounts.First().Value;
			return filteredWordCounts.Select(wordCount =>
				CreateEntry(wordCount.Key, wordCount.Value, highestFrequency));
		}

		private WordCloudEntry CreateEntry(string key, int value, int highestFrequency)
		{
			if (value == highestFrequency)
			{
				return new WordCloudEntry(key, value, TagSize.Huge);
			}

			if (value > highestFrequency * 0.6)
			{
				return new WordCloudEntry(key, value, TagSize.Big);
			}

			if (value > highestFrequency * 0.3)
			{
				return new WordCloudEntry(key, value, TagSize.Normal);
			}

			return new WordCloudEntry(key, value, TagSize.Small);
		}
	}
}