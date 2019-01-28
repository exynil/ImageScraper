using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using HtmlAgilityPack;
using ImageScraper.Properties;

namespace ImageScraper.ViewModels
{
	class MainViewModel : INotifyPropertyChanged
	{
		private string _url;
		private string _imageId;
		private string _numericId;
		private string _imageNumber;
		private string _savePath;
		private string _version;
		private string _numericIdA;
		private string _numericIdB;
		private string _numericIdC;
		private string _numericIdD;
		private string _numericIdE;
		private string _numericIdF;

		public string Url
		{
			get => _url;
			set
			{ 
				_url = value;
				OnPropertyChanged(nameof(Url));
			}
		}

		public string ImageId
		{
			get => _imageId;
			set
			{
				_imageId = value;
				OnPropertyChanged(nameof(ImageId));
			}
		}

		public string NumericId
		{
			get => _numericId;
			set
			{
				_numericId = value;
				OnPropertyChanged(nameof(NumericId));
			}
		}

		public string ImageNumber
		{
			get => _imageNumber;
			set
			{
				_imageNumber = value;
				OnPropertyChanged(nameof(ImageNumber));
			}
		}

		public string SavePath
		{
			get => _savePath;
			set
			{
				_savePath = value;
				OnPropertyChanged(nameof(SavePath));
			}
		}

		public string Version
		{
			get => _version;
			set
			{
				_version = value;
				OnPropertyChanged(nameof(Version));
			}
		}

		public string NumericIdA
		{
			get => _numericIdA;
			set
			{
				if (value == _numericIdA) return;
				_numericIdA = value;
				OnPropertyChanged(nameof(NumericIdA));
				OnPropertyChanged(nameof(StartFromPosition));
			}
		}

		public string NumericIdB
		{
			get => _numericIdB;
			set
			{
				if (value == _numericIdB) return;
				_numericIdB = value;
				OnPropertyChanged(nameof(NumericIdB));
				OnPropertyChanged(nameof(StartFromPosition));
			}
		}

		public string NumericIdC
		{
			get => _numericIdC;
			set
			{
				if (value == _numericIdC) return;
				_numericIdC = value;
				OnPropertyChanged(nameof(NumericIdC));
				OnPropertyChanged(nameof(StartFromPosition));
			}
		}

		public string NumericIdD
		{
			get => _numericIdD;
			set
			{
				if (value == _numericIdD) return;
				_numericIdD = value;
				OnPropertyChanged(nameof(NumericIdD));
				OnPropertyChanged(nameof(StartFromPosition));
			}
		}

		public string NumericIdE
		{
			get => _numericIdE;
			set
			{
				if (value == _numericIdE) return;
				_numericIdE = value;
				OnPropertyChanged(nameof(NumericIdE));
				OnPropertyChanged(nameof(StartFromPosition));
			}
		}

		public string NumericIdF
		{
			get => _numericIdF;
			set
			{
				if (value == _numericIdF) return;
				_numericIdF = value;
				OnPropertyChanged(nameof(NumericIdF));
				OnPropertyChanged(nameof(StartFromPosition));
			}
		}

		public Generator Generator { get; set; }
		public int Length { get; set; }
		public bool Stop { get; set; }

		public MainViewModel()
		{
			const string data = "abcdefghijklmnopqrstuvwxyz0123456789";
			Length = 6;
			Generator = new Generator(data, Length);
			var version = Assembly.GetExecutingAssembly().GetName().Version;
			Version = "Версия: " + version.Major + "." + version.Minor;
		}


		public ICommand Start
		{
			get { return new DelegateCommand(o =>
			{
				Task.Factory.StartNew(() =>
				{
					while (true)
					{
						if (Stop)
						{
							Stop = false;
							return;
						}

						const string baseUrl = @"https://prnt.sc/";

						var imageId = Generator.Next();

						Url = $"URL: {baseUrl}{imageId}";
						ImageId = $"ID: {imageId}";
						NumericId = $"Числовой ID: {Generator.GetCurrentNumericId()}";
						ImageNumber = $"Номер: {Generator.GetCurrentNumber()}";

						if (!Directory.Exists(Settings.Default.SavePath))
						{
							Directory.CreateDirectory(Settings.Default.SavePath);
						}

						var filename = $@"{Generator.GetCurrentNumber():0000000000}-{imageId}.jpg";

						var path = Settings.Default.SavePath + @"\" + filename;

						var document = new HtmlWeb().Load(baseUrl + imageId);

						var urls = document.DocumentNode.Descendants("img")
							.Select(x => x.GetAttributeValue("src", null))
							.Where(x => !string.IsNullOrEmpty(x)).ToArray();

						Download(new Uri(urls[0]), path);
					}
				});
			}); }
		}

		public ICommand StartFromPosition
		{
			get
			{
				return new DelegateCommand(o =>
				{
					var numericId = new int[Length];

					try
					{
						numericId[0] = int.Parse(NumericIdA);
						numericId[1] = int.Parse(NumericIdB);
						numericId[2] = int.Parse(NumericIdC);
						numericId[3] = int.Parse(NumericIdD);
						numericId[4] = int.Parse(NumericIdE);
						numericId[5] = int.Parse(NumericIdF);
					}
					catch (Exception)
					{
						return;
					}

					foreach (var i in numericId)
					{
						if (i > Generator.Symbols.Length - 1)
						{
							return;
						}
					}

					Generator.SetCurrentNumericId(numericId);

					if (Start.CanExecute(null))
					{
						Start.Execute(null);
					}
				});
			}
		}

		public ICommand StopProcess
		{
			get
			{
				return new DelegateCommand(o =>
				{
					Stop = true;
					NumericIdA = Generator.NumericId[0].ToString();
					NumericIdB = Generator.NumericId[1].ToString();
					NumericIdC = Generator.NumericId[2].ToString();
					NumericIdD = Generator.NumericId[3].ToString();
					NumericIdE = Generator.NumericId[4].ToString();
					NumericIdF = Generator.NumericId[5].ToString();
				});
			}
		}

		public ICommand SelectSavePath
		{
			get
			{
				return new DelegateCommand(o =>
				{
					using (var fbd = new FolderBrowserDialog())
					{
						// Открываем диалоговое окно
						var result = fbd.ShowDialog();

						if (result != DialogResult.OK || string.IsNullOrWhiteSpace(fbd.SelectedPath)) return;

						SavePath = fbd.SelectedPath;

						Settings.Default.SavePath = fbd.SelectedPath;
						Settings.Default.Save();
					}
				});
			}
		}

		public ICommand SetRandomNumericId
		{
			get
			{
				return new DelegateCommand(o =>
				{
					var random = new Random();

					NumericIdA = random.Next(0, 36).ToString();
					NumericIdB = random.Next(0, 36).ToString();
					NumericIdC = random.Next(0, 36).ToString();
					NumericIdD = random.Next(0, 36).ToString();
					NumericIdE = random.Next(0, 36).ToString();
					NumericIdF = random.Next(0, 36).ToString();
				});
			}
		}

		public ICommand Hyperlink
		{
			get
			{
				return new DelegateCommand(o =>
				{
					Process.Start(new ProcessStartInfo("https://github.com/exynil/ImageScraper"));
				});
			}
		}

		private void Download(Uri address, string path)
		{
			try
			{
				var webClient = new WebClient();
				webClient.DownloadFileAsync(address, path);
			}
			catch (Exception)
			{
				// ignored
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
