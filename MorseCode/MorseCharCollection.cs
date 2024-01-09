﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Runtime.InteropServices.WindowsRuntime;

namespace MorseCode
{
	internal class MorseCharCollection : List<MorseChar>
	{
		private readonly SoundPlayer _soundPlayer;

		public SoundPlayer SoundPlayer => _soundPlayer;

		private readonly MorseChar _blankMorse = new MorseChar(' '," ","n/a"); // to handle blanks

		public MorseChar BlankMorse => _blankMorse;

		private static readonly Dictionary<char,string> _morseCodeRepresentations = new Dictionary<char, string>{
			['a'] = ".-",
			['b'] = "-...",
			['c'] = "-.-.",
			['d'] = "-..",
			['e'] = ".",
			['f'] = "..-.",
			['g'] = "--.",
			['h'] = "....",
			['i'] = "..",
			['j'] = ".---",
			['k'] = "-.-",
			['l'] = ".-..",
			['m'] = "--",
			['n'] = "-.",
			['o'] = "---",
			['p'] = ".--.",
			['q'] = "--.-",
			['r'] = ".-.",
			['s'] = "...",
			['t'] = "-",
			['u'] = "..-",
			['v'] = "...-",
			['w'] = ".--",
			['x'] = "-..-",
			['y'] = "-.--",
			['z'] = "--.."
		};

		public static Dictionary<char, string> MorseCodeRepresentations => _morseCodeRepresentations;

		private string filesDirectory = Environment.CurrentDirectory;

		/// <summary>
		/// Creates a new instance of <see cref="MorseCharCollection"/>.
		/// Each <see cref="MorseChar"/> represent its <see cref="MorseChar.Character"/>,<see cref="MorseChar.MorseRepresentation"/> and holds its <see cref="MorseChar.SoundFile"/>. <br/>
		/// Holds all characters of the alphabet in morse upon initialization.  <br/>
		/// <paramref name="soundFilesDirectory"/> must be in the working directory and files must be named after 'character'.wav. E.g 'a'.wav. <br/>
		/// Use this constructor if you want the basic 26 chars of the alphabet
		/// </summary>
		/// <param name="soundFile"></param>
		/// <param name="soundPlayer"></param>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		public MorseCharCollection(string soundFilesDirectory)
		{
			_soundPlayer = new SoundPlayer();
			string soundFile;
			string soundFileLowerCase;
			char[] alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();		
		
			for (int i = 0; i < alphabet.Length; i++)
			{
				soundFile = soundFilesDirectory + alphabet[i] + ".wav";
				soundFileLowerCase = soundFilesDirectory + alphabet[i].ToString().ToLower() + ".wav";				
				if (!File.Exists(soundFile))
				{
					if (!File.Exists(soundFileLowerCase))
						throw new FileNotFoundException("Error: a file with the name " + soundFileLowerCase  + " or " + soundFile + " doesn't exist or couldn't be found");
					else
						this.Add(new MorseChar(alphabet[i], _morseCodeRepresentations[alphabet[i]], soundFileLowerCase));
				}
				else if (new FileInfo(soundFile).Extension != ".wav")
				{
					throw new ArgumentException("Error: files had wrong extension (must be .wav)");
				}
				else
				{
					this.Add(alphabet[i], _morseCodeRepresentations[alphabet[i]], soundFile);
				}
			}
			this.Add(_blankMorse);
		}
		/// <summary>
		/// Creates a new instance of <see cref="MorseCharCollection"/>. 
		/// Each <see cref="MorseChar"/> represent its <see cref="MorseChar.Character"/>,<see cref="MorseChar.MorseRepresentation"/> and holds its <see cref="MorseChar.SoundFile"/>. <br/>
		/// Holds all <paramref name="morseChars"/>.  <br/>
		/// Use this constructor if you'd like to add new morse characters that dont exist. E.g. '$' ".....--.".  <br/>
		/// All morse characters must be different from eachother. 
		/// </summary>
		/// <param name="morseChars"></param>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		public MorseCharCollection(IEnumerable<MorseChar> morseChars)
		{
			_soundPlayer = new SoundPlayer();
			string soundFile;
			foreach (MorseChar morseChar in morseChars)
			{
				soundFile = morseChar.SoundFile;
				if (!File.Exists(soundFile))
				{
					throw new FileNotFoundException("Error: a file with the name " + soundFile + " doesn't exist or couldn't be found");
				}
				else if (new FileInfo(soundFile).Extension != ".wav")
				{
					throw new ArgumentException("Error: a file with the name " + soundFile + " had the wrong extension (must be .wav)");
				}				
				this.Add(morseChar);
			}
			this.Add(_blankMorse);
		}

		/// <summary>
		/// Adds a new <see cref="MorseChar"/> to the <see cref="MorseCharCollection"/>. 
		/// A new <see cref="MorseChar"/> must be differently from the others in the <see cref="MorseCharCollection"/>.
		/// </summary>
		/// <param name="morseChar"></param>
		/// <exception cref="ArgumentException"></exception>
		public new void Add(MorseChar morseChar)
		{
			if (this.Exists(c => c.Character == morseChar.Character))
			{
				throw new ArgumentException("Error: MorseChar already exists");
			}
			else if (this.Exists(mr => mr.MorseRepresentation == morseChar.MorseRepresentation))
			{
				throw new ArgumentException("Error: morse-representation already exists");
			}
			else if (this.Exists(sf => sf.SoundFile == morseChar.SoundFile))
			{
				throw new ArgumentException("Error: sound file already exists");
			}
			else
				base.Add(morseChar);
		}
		/// <summary>
		/// Adds a new <see cref="MorseChar"/> to the <see cref="MorseCharCollection"/>. 
		/// A new <see cref="MorseChar"/> must be differently from the others in the <see cref="MorseCharCollection"/>.
		/// </summary>
		/// <param name="character"></param>
		/// <param name="morseRepresentation"></param>
		/// <param name="soundFile"></param>
		/// <exception cref="ArgumentException"></exception>
		public void Add(char character, string morseRepresentation, string soundFile)
		{
			if (this.Exists(c => c.Character == character))
			{
				throw new ArgumentException("Error: MorseChar already exists");
			}
			else if (this.Exists(mr => mr.MorseRepresentation == morseRepresentation))
			{
				throw new ArgumentException("Error: morse-representation already exists");
			}
			else if (this.Exists(sf => sf.SoundFile == soundFile))
			{
				throw new ArgumentException("Error: sound file already exists");
			}
			else
				base.Add(new MorseChar(character, morseRepresentation, soundFile));
		}

		/// <summary>
		/// Converts the <see cref="MorseCharCollection"/> into an array of <see cref="MorseChar.MorseRepresentation"/> strings and returns it.
		/// Doesn't change the <see cref="MorseCharCollection"/> itself.
		/// </summary>
		/// <returns></returns>
		public string[] ConvertToMorseCharRepresentation()
		{
			string[] morseChars = new string[this.Count];
			for (int i = 0; i < this.Count; i++)
			{
				morseChars[i] = this[i].MorseRepresentation;
			}
			return morseChars;
		}

		/// <summary>
		/// Searches for the first <see cref="MorseChar"/> in <see cref="MorseCharCollection"/> that has the character <paramref name="character"/> and returns it.
		/// </summary>
		/// <param name="character"></param>
		/// <returns></returns>
		/// <exception cref="MorseCharNotFoundException"></exception>
		public MorseChar Find(char character)
		{
			MorseChar morseChar = base.Find(c => c.Character == character);
			if (morseChar != null)
			{
				return morseChar;
			}
			else
				throw new MorseCharNotFoundException();
		}

		/// <summary>
		/// Searches for the first <see cref="char"/> in <see cref="MorseCharCollection"/> that has the morse representation <paramref name="morseRepresentation"/> and returns it.
		/// </summary>
		/// <param name="morseRepresentation"></param>
		/// <exception cref="MorseCharNotFoundException"></exception>
		/// <returns></returns>
		public char Find(string morseRepresentation)
		{
			MorseChar morseChar = base.Find(mr => mr.MorseRepresentation == morseRepresentation);
			if (morseChar != null)
			{
				return morseChar.Character;
			}
			else 
				throw new MorseCharNotFoundException("Error: there is no morseChar with the morse representation " + morseRepresentation + " in MorseCharCollection yet");			
		}
		

		/// <summary>
		/// Changes the output of future generated SoundFiles to <paramref name="newDirectory"/>.
		/// </summary>
		/// <param name="newDirectory"></param>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="ArgumentNullException"></exception>
		public void ChangeSoundFilesOutputDir(string newDirectory)
		{
			if (newDirectory != null)
			{
				if (Directory.Exists(newDirectory))
					filesDirectory = newDirectory;
				else
					throw new DirectoryNotFoundException();
			}
			else 
				throw new ArgumentNullException();
				

		}
	}
	
}
