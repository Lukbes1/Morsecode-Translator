using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MorseCode
{
	/// <summary>
	/// Represents a MorseChar with a character, morseRepesentation and soundfile 
	/// </summary>
	public class MorseChar
	{
		public char Character { get; private set; }
		public string MorseRepresentation { get; private set; }
		public string SoundFile { get; private set; }

		private static string beep_short_path = string.Empty;
		private static string beep_long_path = string.Empty;
		private static string silence_path = string.Empty;
		private static string audio_dir_path = string.Empty;

		//private const string BEEP_SHORT_PATH = @"MorseCodeAudio\Beep_short.wav";
		//private const string BEEP_LONG_PATH =  @"MorseCodeAudio\Beep_long.wav";
		//private const string SILENCE_PATH =    @"MorseCodeAudio\Silence.wav";

		/// <summary>
		/// Use this constructor if you have the given soundfile already, use other constructor to create a new one automatically <br/>
		/// <paramref name="soundFile"/> takes the whole path
		/// </summary>
		/// <param name="character"></param>
		/// <param name="morseRepresentation"></param>
		/// <param name="soundFile"></param>
		/// <exception cref="ArgumentException"></exception>
		public MorseChar(char character, string morseRepresentation, string soundFile)
		{
			InitializeAudioFiles();
			this.Character = character;
			if (!(morseRepresentation.Contains(".") || morseRepresentation.Contains("-")))
				throw new ArgumentException("Error: morseRepresentation must contain . or -");
			else
				this.MorseRepresentation = morseRepresentation;
			if (!File.Exists(soundFile))
				throw new FileNotFoundException("Error: a file with the name " + soundFile + " could not be found");
			else
				this.SoundFile = soundFile;
		}

		/// <summary>
		/// Creates the morse audio file automatically for given <paramref name="character"/> and <paramref name="morseRepresentation"/>. <br/>
		/// If the name of the <paramref name="character"/> isnt valid then a random id will be inserted into the file name. <br/>
		/// !WARNING! Dont forget that invalid filenames will cause a random id in the name, thus old files wont be overwritten.
		/// </summary>
		/// <param name="character"></param>
		/// <param name="morseRepresentation"></param>
		/// <param name="overrideFiles"></param>
		/// <exception cref="ArgumentException"></exception>
		public MorseChar(char character, string morseRepresentation, bool overrideFiles = true)
		{
			InitializeAudioFiles();
			this.Character = character;
			if (!(morseRepresentation.Contains(".") || morseRepresentation.Contains("-")))
			{
				throw new ArgumentException("Error: morseRepresentation must contain . or -");
			}
			else
			{
				this.MorseRepresentation = morseRepresentation;
				CreateSoundFile(overrideFiles);
			}
		}

		private MorseChar()
		{
			this.Character = ' ';
			this.MorseRepresentation = " ";
			this.SoundFile = "n/a";
		}

		/// <summary>
		/// Searches for the three preset audio files and gets their dir
		/// </summary>
		/// <exception cref="DirectoryNotFoundException"></exception>
		private static void InitializeAudioFiles()
		{
			List<string> directories = new List<string>();
			audio_dir_path = @"..\..\MorseCodeAudio";

			if (Directory.Exists(audio_dir_path))
			{
				beep_short_path = audio_dir_path + @"\Beep_short.wav";
				beep_long_path = audio_dir_path + @"\Beep_long.wav";
				silence_path = audio_dir_path + @"\Silence.wav";
			}
			else if (Directory.Exists(@"MorseCodeAudio"))
			{
				beep_short_path = @"MorseCodeAudio\Beep_short.wav";
				beep_long_path = @"MorseCodeAudio\Beep_long.wav";
				silence_path = @"MorseCodeAudio\Silence.wav";

			}
			else
				throw new DirectoryNotFoundException("Error: there is no directory with the files from MorseCodeAudio");
		}
		/// <summary>
		/// Use this method for a single char (morseRepresentation) <br/>
		/// Creates the soundfile for the given <paramref name="morseRepresentation"/>.If the name of the <paramref name="fileName"/> isnt valid then a random id will be inserted. <br/>
		/// <paramref name="fileName"/> takes just the name of the file and puts it with .wav extension into the MorseSoundFiles dir.
		/// </summary>
		/// <param name="morseRepresentation"></param>
		/// <param name="fileName"></param>
		/// <param name="overrideFiles"></param>
		/// <exception cref="ArgumentException"></exception>
		public static void CreateSoundFile(string morseRepresentation, string fileName, bool overrideFiles = true)
		{
			InitializeAudioFiles();
			if (!Directory.Exists("MorseSoundFiles"))
			{
				Directory.CreateDirectory("MorseSoundFiles");
			}

			fileName = $@"MorseSoundFiles\{fileName}.wav";
			if (File.Exists(fileName))
			{
				if (overrideFiles)
				{
					File.Delete(fileName);
				}
			}

			try
			{
				FileStream testfile = File.Create(fileName);     //Check if file can be created
				testfile.Close();
				File.Delete(fileName);                          //Delete if file could be created
			}
			catch (ArgumentException)
			{
				string newFileName = $@"MorseSoundFiles\{GenerateRandomID(10)}.wav";
				Console.WriteLine("Error: file with name: " + fileName + " could not be created.\nChanged name to: " + newFileName);
				fileName = newFileName;
			}

			List<ISampleProvider> beepsAndSilences = new List<ISampleProvider>();

			foreach (char beep in morseRepresentation)      //e.g. ['a'] = ".-" 1x BeepShort, 1xBeepLong, 2x Silence
			{
				if (beep == '.')
					beepsAndSilences.Add(new AudioFileReader(beep_short_path));
				else if (beep == '-')
					beepsAndSilences.Add(new AudioFileReader(beep_long_path));
				else
					throw new ArgumentException("Error: morseRepresentation must be . or -");
				beepsAndSilences.Add(new AudioFileReader(silence_path));
			}
			ConcatenatingSampleProvider morseBeeps = new ConcatenatingSampleProvider(beepsAndSilences);

			WaveFileWriter.CreateWaveFile(fileName, morseBeeps.ToWaveProvider());
		}

		/// <summary>
		/// Use this method for whole texts (morseRepresentations) <br/>
		/// Creates the soundfile for the given <paramref name="morseRepresentations"/>.If the name of the <paramref name="fileName"/> isnt valid then a random id will be inserted. <br/>
		/// <paramref name="fileName"/> takes just the name of the file and puts it with .wav extension into the MorseSoundFiles dir.
		/// </summary>
		/// <param name="morseRepresentations"></param>
		/// <param name="fileName"></param>
		/// <param name="overrideFiles"></param>
		/// <exception cref="ArgumentException"></exception>
		public static void CreateSoundFile(string[] morseRepresentations, string fileName, bool overrideFiles = true)
		{
			InitializeAudioFiles();
			if (!Directory.Exists("MorseSoundFiles"))
			{
				Directory.CreateDirectory("MorseSoundFiles");
			}

			fileName = $@"MorseSoundFiles\{fileName}.wav";
			if (File.Exists(fileName))
			{
				if (overrideFiles)
				{
					File.Delete(fileName);
				}
			}

			try
			{
				FileStream testfile = File.Create(fileName);     //Check if file can be created
				testfile.Close();
				File.Delete(fileName);                          //Delete if file could be created
			}
			catch (ArgumentException)
			{
				string newFileName = $@"MorseSoundFiles\{GenerateRandomID(10)}.wav";
				Console.WriteLine("Error: file with name: " + fileName + " could not be created.\nChanged name to: " + newFileName);
				fileName = newFileName;
			}


			List<ISampleProvider> beepsAndSilences = new List<ISampleProvider>();

			foreach (string morseRepr in morseRepresentations)
			{
				foreach (char beep in morseRepr)      //e.g. ['a'] = ".-" 1x BeepShort, 1xBeepLong, 2x Silence
				{
					if (beep == '.')
						beepsAndSilences.Add(new AudioFileReader(beep_short_path));
					else if (beep == '-')
						beepsAndSilences.Add(new AudioFileReader(beep_long_path));
					else if (beep == ' ')
					{
						beepsAndSilences.Add(new AudioFileReader(silence_path)); //Two times the silence for distingushable space between words
						beepsAndSilences.Add(new AudioFileReader(silence_path));
					}
					else
						throw new ArgumentException("Error: morseRepresentation must be . or -");

					beepsAndSilences.Add(new AudioFileReader(silence_path));
				}
			}
			ConcatenatingSampleProvider morseBeeps = new ConcatenatingSampleProvider(beepsAndSilences);

			WaveFileWriter.CreateWaveFile(fileName, morseBeeps.ToWaveProvider());
		}
		private void CreateSoundFile(bool overrideFiles = true)
		{
			if (!Directory.Exists("MorseSoundFiles"))
			{
				Directory.CreateDirectory("MorseSoundFiles");
			}
			SoundFile = $@"MorseSoundFiles\{Character}.wav";
			if (File.Exists(SoundFile))
			{
				if (overrideFiles)
				{
					File.Delete(SoundFile);
				}
			}

			try
			{
				FileStream testfile = File.Create(SoundFile);     //Check if file can be created
				testfile.Close();
				File.Delete(SoundFile);                          //Delete if file could be created
			}
			catch (ArgumentException)
			{
				string newFileName = $@"MorseSoundFiles\{GenerateRandomID(10)}.wav";
				Console.WriteLine("Error: file with name: " + SoundFile + " could not be created.\nChanged name to: " + newFileName);
				SoundFile = newFileName;
			}

			List<ISampleProvider> beepsAndSilences = new List<ISampleProvider>();

			foreach (char beep in MorseRepresentation)      //e.g. ['a'] = ".-" 1x BeepShort, 1xBeepLong, 2x Silence
			{
				if (beep == '.')
					beepsAndSilences.Add(new AudioFileReader(beep_short_path));
				else if (beep == '-')
					beepsAndSilences.Add(new AudioFileReader(beep_long_path));
				else
					throw new ArgumentException("Error: morseRepresentation must be . or -");
				beepsAndSilences.Add(new AudioFileReader(silence_path));
			}
			ConcatenatingSampleProvider morseBeeps = new ConcatenatingSampleProvider(beepsAndSilences);

			WaveFileWriter.CreateWaveFile(SoundFile, morseBeeps.ToWaveProvider());
		}

		private static string GenerateRandomID(int length)
		{
			Random rng = new Random();

			string id = string.Empty;
			string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			while (id.Length < length)
			{
				id += characters[rng.Next(0, characters.Length)];
			}
			return id;
		}


		public void PlayMorse(SoundPlayer soundPlayer)
		{
			try
			{
				soundPlayer.SoundLocation = SoundFile;
				soundPlayer.Load();
				soundPlayer.PlaySync();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);
			}
		}

		public async Task PlayMorseAsync(SoundPlayer soundPlayer)
		{
			try
			{
				soundPlayer.SoundLocation = SoundFile;
				soundPlayer.Load();
				await Task.Run(() => soundPlayer.Play());
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);
			}
		}


		public static MorseChar BlankMorseChar()
		{
			return new MorseChar();
		}

		public int GetShorts()
		{
			int amountShorts = 0;
			foreach (char morseChar in MorseRepresentation)
			{
				if (morseChar == '.')
				{
					amountShorts++;
				}
			}
			return amountShorts;
		}

		public int GetLongs()
		{
			int amountLongs = 0;
			foreach (char morseChar in MorseRepresentation)
			{
				if (morseChar == '-')
				{
					amountLongs++;
				}
			}
			return amountLongs;
		}

	}
}
