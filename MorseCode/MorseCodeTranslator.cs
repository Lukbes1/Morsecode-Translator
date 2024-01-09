using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.IO;
using System.Threading;
using NAudio.Wave.SampleProviders;
using NAudio.Wave;

namespace MorseCode
{
	internal class MorseCodeTranslator
	{
		private readonly MorseCharCollection _morseCodes;

        public MorseCharCollection MorseCodes => _morseCodes;

		/// <summary>
		/// Creates a new instance of <see cref="MorseCodeTranslator"/>. Use this class for all sorts of translations and converts. <br/>
		/// The <see cref="MorseCharCollection"/> will be used in all methods of <see cref="MorseCodeTranslator"/>.
		/// Use this constructor if you want to start with only a given number of <see cref="MorseChar"/> in <see cref="MorseCharCollection"/>.
		/// </summary>
		/// <param name="morseCharCollection"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public MorseCodeTranslator(MorseCharCollection morseCharCollection)
        {
			if (morseCharCollection == null)
				throw new ArgumentNullException();			
			else
				this._morseCodes = morseCharCollection;
		}

		/// <summary>
		/// Creates a new instance of <see cref="MorseCodeTranslator"/>. Use this class for all sorts of translations and converts. <br/>
		/// The <see cref="MorseCharCollection"/> will be used in all methods of <see cref="MorseCodeTranslator"/>.
		/// Use this constructor if you want to start with the complete alphabet in inside of the <see cref="MorseCharCollection"/>. <br/>
		/// !WARNING!: This constructor assumes that you have all 26 audio files for the alphabet in the <paramref name="soundFilesDirectory"/>.
		/// </summary>
		/// <param name="soundFilesDirectory"></param>
		/// <exception cref="DirectoryNotFoundException"></exception>
		public MorseCodeTranslator(string soundFilesDirectory)
        {
			if (!Directory.Exists(soundFilesDirectory))
				throw new DirectoryNotFoundException();
			_morseCodes = new MorseCharCollection(soundFilesDirectory);
        }

		/// <summary>
		/// Converts the string <paramref name="text"/> into an array of <see cref="string"/> with each morse representation. <br/>
		/// If any <see cref="char"/> from <paramref name="text"/> is not known in either the <see cref="MorseCharCollection"/> or the alphabet a <see cref="ArgumentException"/> will be thrown. <br/>
		/// If <paramref name="withBlanks"/> is false a <see cref="string"/> array with only the morse representations will be returned. <br/>
		/// Else the method returns blanks too.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		/// <exception cref="MorseCharNotFoundException"></exception>
		/// //to do
		public string[] ConvertStringToMorse(string text, bool withBlanks = true, bool caseSensitive = false)
		{ 
			char[] allChractersFromText = text.ToCharArray();
			bool mourseCodeContainsAllText = allChractersFromText.Any(t => _morseCodes.Any(c => c.Character == t)); //Checks if the text can be translated
			bool mourseAlphabetContainsAllText = allChractersFromText.Any(t => MorseCharCollection.MorseCodeRepresentations.ContainsKey(t));
			List<string> morseRepresentation = new List<string>(allChractersFromText.Length);
			if (!withBlanks)
				morseRepresentation = new List<string>(allChractersFromText.Length - text.Split(' ').Count()); //Remove extra 	
			for (int i = 0; i < allChractersFromText.Length; i++)
			{
				if (allChractersFromText[i] == ' ' && withBlanks)
					morseRepresentation.Add(" ");//Special case in which a blank space must be left
				else if (allChractersFromText[i] == ' ' && !withBlanks)
					continue;
				else if (mourseCodeContainsAllText) //Collection lookup
					morseRepresentation.Add(_morseCodes.Find(mc => mc.Character.ToString() == allChractersFromText[i].ToString().ToLower()).MorseRepresentation);
				else if (mourseAlphabetContainsAllText) //Alphabet lookup
					morseRepresentation.Add(MorseCharCollection.MorseCodeRepresentations[allChractersFromText[i]]);
				else
					throw new MorseCharNotFoundException("Error: cannot convert text into morse-representation because there are characters that have not yet defined a representation");
			}
			return morseRepresentation.ToArray();
		}

		/// <summary>
		/// Converts the array <paramref name="morseRepresentations"/> into readable text and returns it. <br/>
		/// If <paramref name="withBlanks"/> is set to true, you can pass an array with blanks. <br/>
		/// Else the method only accepts valid <paramref name="morseRepresentations"/> that are contained in the instance of <see cref="MorseCharCollection"/>.
		/// </summary>
		/// <param name="morseRepresentations"></param>
		/// <param name="withBlanks"></param>
		/// <returns></returns>
		/// <exception cref="MorseCharNotFoundException"></exception>
		public string ConvertMorseToString(string[] morseRepresentations, bool withBlanks = true)
		{
			string text = string.Empty;
			for (int i = 0; i < morseRepresentations.Length; i++)
			{
				if (withBlanks && morseRepresentations[i] == " ")
					text += " ";
				else
					text += _morseCodes.Find(morseRepresentations[i]).ToString();
			}
			return text;
		}

		/// <summary>
		/// Plays the soundfile for each char in <paramref name="text"/> if they are contained in the <see cref="MorseCharCollection"/>. <br/>
		/// Does not work if text contains blanks, use other method <seealso cref="PlayMorseFromString(string ,TimeSpan)"/> instead.
		/// </summary>
		/// <param name="text"></param>
		/// <exception cref="MorseCharNotFoundException"></exception>
		public void PlayMorseFromString(string text)
		{
			string[] morseRepresentations = ConvertStringToMorse(text, false);
			for (int i = 0; i < morseRepresentations.Length; i++)
			{
				PlayMorseFromChar(ConvertMorseToChar(morseRepresentations[i]));
			}
		}
		/// <summary>
		/// Plays the soundfile for each char in <paramref name="text"/> if they are contained in the <see cref="MorseCharCollection"/>. <br/>
		/// Works with blanks. Pauses for <paramref name="delayBetweenCharsInSeconds"/> inbetween chars.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="delayBetweenCharsInSeconds"></param>
		public void PlayMorseFromString(string text, TimeSpan delayBetweenCharsInSeconds)
		{
			string[] morseRepresentations = ConvertStringToMorse(text, true);
			for (int i = 0; i < morseRepresentations.Length; i++)
			{
				PlayMorseFromChar(ConvertMorseToChar(morseRepresentations[i]));
				Thread.Sleep(delayBetweenCharsInSeconds);
			}
		}

		/// <summary>
		/// Plays the soundfile async for each char in <paramref name="text"/> if they are contained in the <see cref="MorseCharCollection"/>. <br/>
		/// Does not work if text contains blanks, use other method <seealso cref="PlayMorseFromStringAsync(string ,TimeSpan)"/> instead.
		/// </summary>
		/// <param name="text"></param>
		/// <exception cref="MorseCharNotFoundException"></exception>
		public async Task PlayMorseFromStringAsync(string text)
		{
			string[] morseRepresentations = ConvertStringToMorse(text, false);
			for (int i = 0; i < morseRepresentations.Length; i++)
			{
				await PlayMorseFromCharAsync(ConvertMorseToChar(morseRepresentations[i]));
			}
		}

		/// <summary>
		/// Plays the soundfile for each char in <paramref name="text"/> if they are contained in the <see cref="MorseCharCollection"/>. <br/>
		/// Works with blanks. Pauses for <paramref name="delayBetweenCharsInSeconds"/> inbetween chars. <br/>
		/// </summary>
		/// <param name="text"></param>
		/// <exception cref="MorseCharNotFoundException"></exception>
		public async Task PlayMorseFromStringAsync(string text, TimeSpan delayBetweenCharsInSeconds)
		{
			string[] morseRepresentations = ConvertStringToMorse(text, true);
			for (int i = 0; i < morseRepresentations.Length; i++)
			{
				await PlayMorseFromCharAsync(ConvertMorseToChar(morseRepresentations[i]));
				Thread.Sleep(delayBetweenCharsInSeconds);
			}
		}


		/// <summary>
		/// Plays the soundfile async from the <paramref name="morseChar"/> if its contained in the <see cref="MorseCharCollection"/>
		/// </summary>
		/// <param name="morseChar"></param>
		/// <exception cref="MorseCharNotFoundException"></exception>
		public async Task PlayMorseFromCharAsync(char morseChar)
		{
			MorseChar morseCharacter = _morseCodes.Find(morseChar);
			if (morseCharacter != null)
			{
				 await morseCharacter.PlayMorseAsync(_morseCodes.SoundPlayer);
			}
			else
				throw new MorseCharNotFoundException(morseChar + " does not exist yet");				
		}
		/// <summary>
		/// Plays the soundfile from the <paramref name="morseChar"/> if its contained in the <see cref="MorseCharCollection"/>
		/// </summary>
		/// <param name="morseChar"></param>
		/// <exception cref="MorseCharNotFoundException"></exception>
		public void PlayMorseFromChar(char morseChar)
		{
			MorseChar morseCharacter = _morseCodes.Find(morseChar);
			if (morseCharacter != null)
			{
				morseCharacter.PlayMorse(_morseCodes.SoundPlayer);
			}
			else
				throw new MorseCharNotFoundException("Error: " + morseChar + " does not exist yet");
		}

		/// <summary>
		/// Converts <paramref name="character"/> into its morse representation. <br/>
		/// Only works with characters that are known by the <see cref="MorseCharCollection"/> or are a part of the alphabet
		/// </summary>
		/// <param name="character"></param>
		/// <returns></returns>
		/// <exception cref="MorseCharNotFoundException"></exception>
		public string ConvertCharToMorse(char character)
		{
			if (character == _morseCodes.BlankMorse.Character)
				return _morseCodes.BlankMorse.MorseRepresentation;
			string morseRepresentation = _morseCodes.Find(character).MorseRepresentation;
			if (morseRepresentation == null)
			{
				if (!MorseCharCollection.MorseCodeRepresentations.ContainsKey(character))
					throw new MorseCharNotFoundException("Error: " + character + " does not exist yet");
				else 
					return MorseCharCollection.MorseCodeRepresentations[character];
			}
			else
				return morseRepresentation;
		}

		/// <summary>
		/// Converts <paramref name="morseRepresentation"/> into its char. <br/>
		/// Only works with characters that are known by the <see cref="MorseCharCollection"/> or are a part of the alphabet 
		/// </summary>
		/// <param name="morseRepresentation"></param>
		/// <returns></returns>
		/// <exception cref="MorseCharNotFoundException"></exception>
		public char ConvertMorseToChar(string morseRepresentation)
		{
			if (morseRepresentation == _morseCodes.BlankMorse.MorseRepresentation)
				return _morseCodes.BlankMorse.Character;
			try
			{			
				char morseCharacter = _morseCodes.Find(morseRepresentation);	//Incase of fail, alphabet lookup 
				return morseCharacter;
			}
			catch (NullReferenceException)
			{
				if (!MorseCharCollection.MorseCodeRepresentations.ContainsValue(morseRepresentation))
					throw new MorseCharNotFoundException("Error: morse representation" + morseRepresentation + " does not exist yet");
				else
					return MorseCharCollection.MorseCodeRepresentations.FirstOrDefault(mr => mr.Value == morseRepresentation).Key;
			}			
		}

		
	}
}
