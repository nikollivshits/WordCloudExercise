namespace WordCloud
{
	class WordCloudEntry
	{
		private string _word;
		private int _count;
		private TagSize _tagSize;

		public WordCloudEntry(string word, int count, TagSize tagSize)
		{
			_word = word;
			_count = count;
			_tagSize = tagSize;
		}

		public override string ToString()
		{
			return $"{_word}\t{_count}\t{_tagSize}";
		}
	}
}