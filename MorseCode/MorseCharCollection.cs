using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Runtime.CompilerServices;

namespace MorseCode
{
	/// <summary>
	/// Contains a list of MorseChar aswell as a soundplayer and the alphabet, therefore acts as a wrapper and helper.
	/// </summary>
	public class MorseCharCollection : List<MorseChar>
	{
		private readonly SoundPlayer _soundPlayer;

		public SoundPlayer SoundPlayer => _soundPlayer;

		private readonly MorseChar _blankMorse = MorseChar.BlankMorseChar(); // to handle blanks

		public MorseChar BlankMorse => _blankMorse;

		private static readonly Dictionary<char, string> _morseCodeRepresentations = new Dictionary<char, string>
		{
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


		private static string _morseCodeAudioDir = @"..\..\MorseCodeAudio"; 

		/// <summary>
		/// This is where all of the pre defined beeps and silences are found. Change this property to change the dir in which the files are found. <br/>
		/// This only changes where the program searches for the files and doesn't automatically move them. <br/>
		/// Changing the files themselves might break something. 
		/// </summary>
		public static string MorseCodeAudioDir
		{
			get
			{
				return _morseCodeAudioDir;
			}
			set
			{
				if (value != null && Directory.Exists(value))
				{
					_morseCodeAudioDir = value;
					MorseChar.Beep_long_path = value + @"\Beep_long.wav";
					MorseChar.Beep_short_path = value + @"\Beep_short.wav";
					MorseChar.Silence_path = value + @"\Silence.wav";
				}
				else
				{
					throw new ArgumentException("Error: directory does not exist");
				}
			}
		}

		private static string _morseSoundFilesDir = @"MorseSoundFiles";

		/// <summary>
		/// This is where all of the created soundfiles (e.g the alphabet) are stored. Change this property to change the dir in which the files are found <br/>
		/// This only changes where the program searches for the files and doesn't automatically move them. <br/>
		/// This does however automatically change the internally saved <see cref="MorseChar.SoundFile"/> of each <see cref="MorseChar"/>
		/// </summary>
		public static string MorseSoundFilesDir
		{
			get
			{
				return _morseSoundFilesDir;
			}
			set
			{ 
				if (value != null && Directory.Exists(value))
				{
					MorseSoundFilesDirChanged?.Invoke(null,new DirectoryChangedEventArgs(value));
					_morseSoundFilesDir = value;
				}
				else
				{
					throw new ArgumentException("Error: directory does not exist");
				}
			}
		}

		private void ChangeSoundFilesDir(object sender, DirectoryChangedEventArgs e)
		{
			foreach (MorseChar morseChar in this)
			{
				string olddir = Path.GetDirectoryName(morseChar.SoundFile);
				morseChar.SoundFile.Replace(olddir, e.Directory);
			}
		}

		public class DirectoryChangedEventArgs : EventArgs
		{
			public string Directory;
			public DirectoryChangedEventArgs(string directory)
			{
				this.Directory = directory;
			}
		}

		public static event EventHandler<DirectoryChangedEventArgs> MorseSoundFilesDirChanged;

		/// <summary>
		/// Creates a new instance of <see cref="MorseCharCollection"/>.
		/// Each <see cref="MorseChar"/> represent its <see cref="MorseChar.Character"/>,<see cref="MorseChar.MorseRepresentation"/> and holds its <see cref="MorseChar.SoundFile"/>. <br/>
		/// Holds all characters of the alphabet in morse upon initialization.  <br/>
		/// "MorseSoundFiles" dir will hold all files, each named after 'character'.wav. E.g 'a'.wav. <br/>
		/// Use this constructor if you want the basic 26 chars of the alphabet
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		public MorseCharCollection()
		{
			MorseSoundFilesDirChanged += ChangeSoundFilesDir;
			_soundPlayer = new SoundPlayer();
			foreach (char morseChar in _morseCodeRepresentations.Keys)
			{
				this.Add(new MorseChar(morseChar, _morseCodeRepresentations[morseChar]));
			}
			this.Add(_blankMorse);
		}
		/// <summary>
		/// Creates a new instance of <see cref="MorseCharCollection"/>. 
		/// Each <see cref="MorseChar"/> represent its <see cref="MorseChar.Character"/>,<see cref="MorseChar.MorseRepresentation"/> and holds its <see cref="MorseChar.SoundFile"/>. <br/>
		/// Holds all <paramref name="morseChars"/>.  <br/>
		/// Use this constructor if you'd like to add new morse characters that dont exist. E.g. '$' ".....--.". or only specific ones <br/>
		/// All morse characters must be different from eachother. 
		/// </summary>
		/// <param name="morseChars"></param>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		public MorseCharCollection(IEnumerable<MorseChar> morseChars)
		{
			MorseSoundFilesDirChanged += ChangeSoundFilesDir;
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
		/// A new <see cref="MorseChar"/> must be different from the others in the <see cref="MorseCharCollection"/>.
		/// <paramref name="soundFile"/> takes the whole path of the file
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
				throw new MorseCharNotFoundException("Error: there is no morseChar with the char " + character + " in MorseCharCollection yet");
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
	}
}
