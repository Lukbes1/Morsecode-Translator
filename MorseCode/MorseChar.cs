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
	internal class MorseChar
	{
		public char Character { get; private set; }
		public string MorseRepresentation { get; private set; }
		public string SoundFile { get; private set; }

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
		/// <exception cref="ArgumentException"></exception>
		public MorseChar(char character, string morseRepresentation, bool overrideFiles = true)
		{
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
		/// Creates the soundfile for the given Character. If the name of the character isnt valid then a random id will be inserted. <br/>
		/// WARNING! Dont forget that invalid filenames will cause a random id in the name, thus old files wont be overwritten. <br/>
		/// <paramref name="fileName"/> takes just the name of the file and puts it with .wav extension into the MorseSoundFiles dir.
		/// </summary>
		/// <param name="morseRepresentation"></param>
		/// <param name="fileName"></param>
		/// <param name="overrideFiles"></param>
		/// <exception cref="ArgumentException"></exception>
		public static void CreateSoundFile(string morseRepresentation, string fileName, bool overrideFiles = true)
		{
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
				File.Delete(fileName);							//Delete if file could be created
			}
			catch (ArgumentException)
			{				
				string newFileName = $@"MorseSoundFiles\{GenerateRandomID(10)}.wav";
				Console.WriteLine("Error: file with name: " + fileName + " could not be created.\nChanged name to: " + newFileName);
				fileName = newFileName;
			}

			const string BEEP_SHORT_PATH = @"MorseCodeAudio\Beep_short.wav";
			const string BEEP_LONG_PATH = @"MorseCodeAudio\Beep_long.wav";
			const string SILENCE_PATH = @"MorseCodeAudio\Silence.wav";
			
			List<ISampleProvider> beepsAndSilences = new List<ISampleProvider>();
		
			foreach (char beep in morseRepresentation)      //e.g. ['a'] = ".-" 1x BeepShort, 1xBeepLong, 2x Silence
			{
				if (beep == '.')
					beepsAndSilences.Add(new AudioFileReader(BEEP_SHORT_PATH));
				else if (beep == '-')
					beepsAndSilences.Add(new AudioFileReader(BEEP_LONG_PATH));
				else
					throw new ArgumentException("Error: morseRepresentation must be . or -");
				beepsAndSilences.Add(new AudioFileReader(SILENCE_PATH));
			}
			ConcatenatingSampleProvider morseBeeps = new ConcatenatingSampleProvider(beepsAndSilences);

			WaveFileWriter.CreateWaveFile(fileName, morseBeeps.ToWaveProvider());
		}
		private void CreateSoundFile(bool overrideFiles = true)
		{
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

			const string BEEP_SHORT_PATH = @"MorseCodeAudio\Beep_short.wav";
			const string BEEP_LONG_PATH = @"MorseCodeAudio\Beep_long.wav";
			const string SILENCE_PATH = @"MorseCodeAudio\Silence.wav";

			List<ISampleProvider> beepsAndSilences = new List<ISampleProvider>();

			foreach (char beep in MorseRepresentation)      //e.g. ['a'] = ".-" 1x BeepShort, 1xBeepLong, 2x Silence
			{
				if (beep == '.')
					beepsAndSilences.Add(new AudioFileReader(BEEP_SHORT_PATH));
				else if (beep == '-')
					beepsAndSilences.Add(new AudioFileReader(BEEP_LONG_PATH));
				else
					throw new ArgumentException("Error: morseRepresentation must be . or -");
				beepsAndSilences.Add(new AudioFileReader(SILENCE_PATH));
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
