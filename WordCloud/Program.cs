using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace WordCloud
{
	class Program
	{
		static void Main(string[] args)
		{
			var wordCloudGenerator = WordCloudGenerator.Instance;
			var dirPath = "./txtFiles";
			var isRecursive = true;
			var outPath = "./output.txt";

			wordCloudGenerator.Generate(dirPath, isRecursive, outPath);

		}
	}
}
