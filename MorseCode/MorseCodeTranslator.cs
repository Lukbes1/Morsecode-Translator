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
using System.Runtime.CompilerServices;
using System.Collections;

namespace MorseCode
{
	/// <summary>
	/// MorseCodeTranslator combines all functionalities of the MorseChars and helps converting and creating soundfiles from the internal MorseCharCollection
	/// </summary>
	public class MorseCodeTranslator
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
		/// Use this constructor if you want to start with the complete alphabet inside of the <see cref="MorseCharCollection"/>. <br/>		
		/// Files are stored in the "MorseSoundFiles" dir.
		/// </summary>
		public MorseCodeTranslator()
		{
			_morseCodes = new MorseCharCollection();
		}

		/// <summary>
		/// Converts the string <paramref name="text"/> into an array of <see cref="string"/> with each morse representation. <br/>
		/// If any <see cref="char"/> from <paramref name="text"/> is not known in either the <see cref="MorseCharCollection"/> or the alphabet a <see cref="ArgumentException"/> will be thrown. <br/>
		/// If <paramref name="returnWithBlanks"/> is false a <see cref="string"/> array with only the morse representations will be returned. <br/>
		/// The method is case sensitive as 'a' has a different definition than 'A'. <br/>
		/// Else the method returns blanks too.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="returnWithBlanks"></param>
		/// <returns></returns>
		/// <exception cref="MorseCharNotFoundException"></exception>
		/// 
		public string[] ConvertStringToMorse(string text, bool returnWithBlanks = true)
		{
			char[] allChractersFromText = text.ToCharArray();
			bool mourseCodeContainsAllText;
			bool mourseAlphabetContainsAllText;
			mourseCodeContainsAllText = allChractersFromText.All(t => _morseCodes.Any(c => c.Character == t)); //Checks if the text can be translated
			mourseAlphabetContainsAllText = allChractersFromText
				.Where(t => !char.IsWhiteSpace(t))
				.All(t => MorseCharCollection.MorseCodeRepresentations.ContainsKey(t));

			string[] AmorseRepresentations = new string[allChractersFromText.Length];
			if (!returnWithBlanks)
			{
				AmorseRepresentations = new string[allChractersFromText.Length - text.Where(c => c == ' ').Count()];
			}
			int index = 0;
			foreach (char morseRepr in allChractersFromText) 
			{
				if (morseRepr == ' ')
				{
					if (returnWithBlanks)
						AmorseRepresentations[index] = " ";
					else
						continue;
				}
				else if (mourseCodeContainsAllText) //Collection lookup
					AmorseRepresentations[index] = _morseCodes.Find(mc => mc.Character.ToString() == morseRepr.ToString()).MorseRepresentation;
				else if (mourseAlphabetContainsAllText) //Alphabet lookup				
					AmorseRepresentations[index] = MorseCharCollection.MorseCodeRepresentations[morseRepr];
				else
					throw new MorseCharNotFoundException("Error: cannot convert text into morse-representation because there are characters that were not defined yet");
				index++; 
			}
			return AmorseRepresentations;
		}

		/// <summary>
		/// Converts the array <paramref name="morseRepresentations"/> into readable text and returns it. <br/>
		/// If <paramref name="returnWithBlanks"/> is false a <see cref="string"/> with only the morse representations will be returned. <br/>
		/// Else the method only accepts valid <paramref name="morseRepresentations"/> that are contained in the instance of <see cref="MorseCharCollection"/>.
		/// </summary>
		/// <param name="morseRepresentations"></param>
		/// <param name="returnWithBlanks"></param>
		/// <returns></returns>
		/// <exception cref="MorseCharNotFoundException"></exception>
		public string ConvertMorseToString(string[] morseRepresentations, bool returnWithBlanks = true)
		{
			string text = string.Empty;
			for (int i = 0; i < morseRepresentations.Length; i++)
			{
				if (returnWithBlanks && morseRepresentations[i] == " ")
					text += " ";
				else if (!returnWithBlanks && morseRepresentations[i] == " ")
					continue;
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
		/// <param name="delayBetweenCharsInSeconds"></param>
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
			try
			{
				string morseRepresentation = _morseCodes.Find(character).MorseRepresentation;
				return morseRepresentation;
			}
			catch (MorseCharNotFoundException)
			{
				if (!MorseCharCollection.MorseCodeRepresentations.ContainsKey(character))
					throw new MorseCharNotFoundException("Error: " + character + " does not exist yet");
				else
					return MorseCharCollection.MorseCodeRepresentations[character];
			}		
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
				char morseCharacter = _morseCodes.Find(morseRepresentation);    //Incase of fail, alphabet lookup 
				return morseCharacter;
			}
			catch (MorseCharNotFoundException)
			{
				if (!MorseCharCollection.MorseCodeRepresentations.ContainsValue(morseRepresentation))
					throw new MorseCharNotFoundException("Error: morse representation" + morseRepresentation + " does not exist yet");
				else
					return MorseCharCollection.MorseCodeRepresentations.FirstOrDefault(mr => mr.Value == morseRepresentation).Key;
			}
		}

		/// <summary>
		/// Creates the soundfile for the given <paramref name="morseRepresentation"/>. If the name of the <paramref name="fileName"/> isnt valid then a random id will be inserted. <br/>
		/// <paramref name="fileName"/> takes just the name of the file and puts it with .wav extension into the MorseSoundFiles dir.
		/// </summary>
		/// <param name="morseRepresentation"></param>
		/// <param name="fileName"></param>
		/// <param name="overrideFiles"></param>
		/// <exception cref="ArgumentException"></exception>
		public static void EncodeMorseToSoundFile(string morseRepresentation, string fileName, bool overrideFiles = true)
		{
			MorseChar.CreateSoundFile(morseRepresentation, fileName, overrideFiles);
		}

		/// <summary>
		///  Creates the soundfile for the given <paramref name="morseRepresentations"/>. If the name of the <paramref name="fileName"/> isnt valid then a random id will be inserted. <br/>
		/// <paramref name="fileName"/> takes just the name of the file and puts it with .wav extension into the MorseSoundFiles dir.
		/// </summary>
		/// <param name="morseRepresentations"></param>
		/// <param name="fileName"></param>
		/// <param name="overrideFiles"></param>
		/// <exception cref="ArgumentException"></exception>
		public static void EncodeMorseToSoundFile(string[] morseRepresentations, string fileName, bool overrideFiles = true)
		{
			MorseChar.CreateSoundFile(morseRepresentations, fileName, overrideFiles);
		}

		/// <summary>
		/// Decodes the <paramref name="waveFile"/> into the morse representation. Only works with files created from this project or those similiar at the moment.
		/// </summary>
		/// <param name="waveFile"></param>
		/// <returns></returns>
		public static string[] DecodeSoundFileToMorse(string waveFile, bool returnWithBlanks = true)
		{
			return MorseAudioReader.DecodeWaveFileToMorse(waveFile,returnWithBlanks);
		}

		/// <summary>
		/// Decodes the <paramref name="waveFile"/> into text. Only works with files created from this project or those similiar at the moment.
		/// </summary>
		/// <param name="waveFile"></param>
		/// <returns></returns>
		public string DecodeSoundFileToText(string waveFile)
		{
			return ConvertMorseToString(DecodeSoundFileToMorse(waveFile));
		}

	}
}
