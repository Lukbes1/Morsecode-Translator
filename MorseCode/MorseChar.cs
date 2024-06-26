﻿using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Resources;
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

		public static string Beep_short_path { get; internal set; }
		public static string Beep_long_path { get; internal set; }
		public static string Silence_path { get; internal set; }
		public static string Audio_dir_path { get; internal set; }

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
			bool pathFound = false;
			string[] defaultPaths = new string[] { MorseCharCollection.MorseCodeAudioDir, @"..\..\MorseCodeAudio", @"MorseCodeAudio"};
			foreach (string path in defaultPaths)
			{
				if (Directory.Exists(path))
				{
					MorseCharCollection.MorseCodeAudioDir = path;
					Beep_short_path = MorseCharCollection.MorseCodeAudioDir + @"\Beep_short.wav";
					Beep_long_path = MorseCharCollection.MorseCodeAudioDir + @"\Beep_long.wav";
					Silence_path = MorseCharCollection.MorseCodeAudioDir + @"\Silence.wav";
					pathFound = true;
					break;
				}
			}
			if (!pathFound)
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
			if (!Directory.Exists(MorseCharCollection.MorseSoundFilesDir))
			{
				Directory.CreateDirectory(MorseCharCollection.MorseSoundFilesDir);
			}

			fileName = $@"{MorseCharCollection.MorseSoundFilesDir}\{fileName}.wav";
			if (File.Exists(fileName))
			{
				if (overrideFiles)
				{
					File.Delete(fileName);
				}
				else
				{
					throw new ArgumentException("Error: file with the name " + fileName + " already exists");
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
				string newFileName = $@"{MorseCharCollection.MorseSoundFilesDir}\{GenerateRandomID(10)}.wav";
				Console.WriteLine("Error: file with name: " + fileName + " could not be created.\nChanged name to: " + newFileName);
				fileName = newFileName;
			}

			List<ISampleProvider> beepsAndSilences = new List<ISampleProvider>();

			foreach (char beep in morseRepresentation)      //e.g. ['a'] = ".-" 1x BeepShort, 1xBeepLong, 2x Silence
			{
				switch (beep)
				{
					case '.':
						beepsAndSilences.Add(new AudioFileReader(Beep_short_path));
						break;
					case '-':
						beepsAndSilences.Add(new AudioFileReader(Beep_long_path));
						break;
					default:
						throw new ArgumentException("Error: morseRepresentation must be . or -");
				}
				beepsAndSilences.Add(new AudioFileReader(Silence_path));
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
			if (!Directory.Exists(MorseCharCollection.MorseSoundFilesDir))
			{
				Directory.CreateDirectory(MorseCharCollection.MorseSoundFilesDir);
			}

			fileName = $@"{MorseCharCollection.MorseSoundFilesDir}\{fileName}.wav";
			if (File.Exists(fileName))
			{
				if (overrideFiles)
				{
					File.Delete(fileName);
				}
				else
				{
					throw new ArgumentException("Error: file with the name " + fileName + " already exists");
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
				string newFileName = $@"{MorseCharCollection.MorseSoundFilesDir}\{GenerateRandomID(10)}.wav";
				Console.WriteLine("Error: file with name: " + fileName + " could not be created.\nChanged name to: " + newFileName);
				fileName = newFileName;
			}


			List<ISampleProvider> beepsAndSilences = new List<ISampleProvider>();

			foreach (string morseRepr in morseRepresentations)
			{
				foreach (char beep in morseRepr)      //e.g. ['a'] = ".-" 1x BeepShort, 1xBeepLong, 2x Silence
				{
					switch (beep)
					{
						case '.':
							beepsAndSilences.Add(new AudioFileReader(Beep_short_path));
							break;
						case '-':
							beepsAndSilences.Add(new AudioFileReader(Beep_long_path));
							break;
						case ' ':
							beepsAndSilences.Add(new AudioFileReader(Silence_path)); //1x silence = 1x beepShort
							beepsAndSilences.Add(new AudioFileReader(Silence_path)); //4x silence for new word
							beepsAndSilences.Add(new AudioFileReader(Silence_path));
							beepsAndSilences.Add(new AudioFileReader(Silence_path));
							break;
						default:
							throw new ArgumentException("Error: morseRepresentation must be . or -");
					}
					beepsAndSilences.Add(new AudioFileReader(Silence_path)); //1x silence garanteed after each
				}				
				beepsAndSilences.Add(new AudioFileReader(Silence_path)); 
				beepsAndSilences.Add(new AudioFileReader(Silence_path));
			
			}
			ConcatenatingSampleProvider morseBeeps = new ConcatenatingSampleProvider(beepsAndSilences);

			WaveFileWriter.CreateWaveFile(fileName, morseBeeps.ToWaveProvider());
		}

		private void CreateSoundFile(bool overrideFiles = true)
		{
			if (!Directory.Exists(MorseCharCollection.MorseSoundFilesDir))
			{
				Directory.CreateDirectory(MorseCharCollection.MorseSoundFilesDir);
			}
			SoundFile = $@"{MorseCharCollection.MorseSoundFilesDir}\{Character}.wav";
			if (File.Exists(SoundFile))
			{
				if (overrideFiles)
				{
					File.Delete(SoundFile);
				}
				else
				{
					throw new ArgumentException("Error: file with the name " + SoundFile + " already exists");
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
				string newFileName = $@"{MorseCharCollection.MorseSoundFilesDir}\{GenerateRandomID(10)}.wav";
				Console.WriteLine("Error: file with name: " + SoundFile + " could not be created.\nChanged name to: " + newFileName);
				SoundFile = newFileName;
			}

			List<ISampleProvider> beepsAndSilences = new List<ISampleProvider>();

			foreach (char beep in MorseRepresentation)      //e.g. ['a'] = ".-" 1x BeepShort, 1xBeepLong, 2x Silence
			{
				switch (beep)
				{
					case '.':
						beepsAndSilences.Add(new AudioFileReader(Beep_short_path));
						break;
					case '-':
						beepsAndSilences.Add(new AudioFileReader(Beep_long_path));
						break;
					default:
						throw new ArgumentException("Error: morseRepresentation must be . or -");
				}
				beepsAndSilences.Add(new AudioFileReader(Silence_path));
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
				if (Character != ' ')
				{
					soundPlayer.SoundLocation = SoundFile;
					soundPlayer.Load();
					soundPlayer.PlaySync();
				}			
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
				if (Character != ' ')
				{
					soundPlayer.SoundLocation = SoundFile;
					soundPlayer.LoadAsync();
					await Task.Run(() => soundPlayer.Play());
					soundPlayer.Dispose();	
				}			
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
