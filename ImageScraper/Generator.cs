using System.Linq;

namespace ImageScraper
{
	public class Generator
	{
		public string Symbols { get; set; }
		public int[] NumericId { get; set; }
		public int Length { get; set; }
		public bool IsEnd { get; set; }

		public Generator(string symbols, int length)
		{
			Length = length;
			NumericId = new int[Length];
			Initialize(symbols);
		}

		private void Initialize(string data)
		{
			Symbols = data;

			for (var i = 0; i < Length; i++)
			{
				NumericId[i] = 0;
			}
		}

		public string Next()
		{
			if (IsEnd)
			{
				return "";
			}

			var temp = NumericId.Aggregate("", (current, cc) => current + Symbols[cc].ToString());

			var index = NumericId.Length - 1;

			while (index >= 0)
			{
				if (NumericId[index] < Symbols.Length - 1)
				{
					NumericId[index]++;
					break;
				}

				for (var i = index; i < NumericId.Length; i++)
				{
					NumericId[i] = 0;
				}

				index--;

				if (index == -1)
				{
					IsEnd = true;
				}
			}

			return temp;
		}

		internal void SetCurrentNumericId(int[] numericId)
		{
			NumericId = numericId;
		}

		public string GetCurrentNumericId()
		{
			var id = "";

			foreach (var cc in NumericId)
			{
				id += cc + "-";
			}

			id = id.Substring(0, id.Length - 1);

			return id;
		}

		public long GetCurrentNumber()
		{
			long number = 0;

			for (var i = 0; i < NumericId.Length; i++)
			{
				var temp = 1;

				for (var j = 0; j < NumericId.Length - 1 - i; j++)
				{
					temp *= Symbols.Length - 1;
				}

				temp *= NumericId[i];

				number += temp;
			}

			return number;
		}
	}
}
