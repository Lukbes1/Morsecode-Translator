﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MorseCode
{
	internal class Program
	{
		static void Main(string[] args)
		{

			ExampleEverythingSimple();


			Console.ReadKey();		
		}

		private static void ExampleEverythingSimple()
		{
			List<MorseChar> morseList = new List<MorseChar>
			{
				new MorseChar('n', "..."),
				new MorseChar('a',".--"),
				new MorseChar('h',".-.-")
			};

			MorseCharCollection morseCharCollection = new MorseCharCollection(morseList);
			MorseCodeTranslator translator = new MorseCodeTranslator(morseCharCollection);

			//Morse from char
			char exampleChar = 'a';
			string morseFromChar = translator.ConvertCharToMorse(exampleChar);
			Console.WriteLine("morseFromChar " + exampleChar + ": " + morseFromChar);
			//Char to morse
			char charFromMorse = translator.ConvertMorseToChar(morseFromChar);
			Console.WriteLine("CharFromMorse "+ morseFromChar + ": " + charFromMorse);

			Console.ReadKey();
			//Morse from collection
			string[] morseFromCollection = morseCharCollection.ConvertToMorseCharRepresentation();
			Console.WriteLine("Whole morseFromCollection:");
			int j = 0;
			foreach (string s in morseFromCollection)
			{
				Console.WriteLine(j + ". morse: " + s);
				j++;
			}
			//string from morse
			string textFromMorse = translator.ConvertMorseToString(morseFromCollection);
			Console.WriteLine("textFromMorse (fromMorseCharcollection): " + textFromMorse);
			string[] morseFromString = translator.ConvertStringToMorse(textFromMorse);
			Console.WriteLine("morseFromText: ");
			j = 0;
			foreach (string str in morseFromString)
			{
				Console.WriteLine(j + ". morseFromText: " + str);
				j++;
			}
			Console.ReadKey();

			//Sound operations
			translator.PlayMorseFromChar('a');
			translator.PlayMorseFromString(textFromMorse);

			//Manual Soundfile creation

			MorseCodeTranslator.EncodeMorseToSoundFile(".-.-.-", "MyNewChar");
			MorseCodeTranslator.EncodeMorseToSoundFile(new string[] { "-.-.", "..." }, "MyNewWord");
		}


		private static void ExampleStringToSoundFile()
		{
			MorseCodeTranslator translator = new MorseCodeTranslator();

			string[] morseRepresentations = translator.ConvertStringToMorse("I love programming");
			MorseCodeTranslator.EncodeMorseToSoundFile(morseRepresentations, "ILP");
			
			SoundPlayer player = new SoundPlayer();
			player.SoundLocation = @"MorseSoundFiles\ILP.wav";
			player.Load();
			while (true)
			{
				player.PlaySync();
				Thread.Sleep(1000);
			}
		}

		private static void ExampleWholeConversion()
		{
			List<MorseChar> morse = new List<MorseChar>
			{
				new MorseChar('n', "....-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-."),
				new MorseChar('+',".--"),
				new MorseChar('h',".-.-")
			};

			MorseCharCollection morseChars = new MorseCharCollection(morse);
			string[] morseRepresentations = morseChars.ConvertToMorseCharRepresentation();
			for (int i = 0; i < morseRepresentations.Length; i++)
			{
				Console.WriteLine(morseRepresentations[i]);
			}	
		}

		private static void ExampleAudioPlaying()
		{
			MorseCodeTranslator translator = new MorseCodeTranslator(new MorseCharCollection());
			while (true)
			{
				Console.WriteLine("Type in some text: ");
				string input = Console.ReadLine();
				try
				{
					string[] inputToMorse = translator.ConvertStringToMorse(input);
					for (int i = 0; i < inputToMorse.Length; i++)
					{
						Console.WriteLine(inputToMorse[i]);
					}
					translator.PlayMorseFromString(input);
				}
				catch (MorseCharNotFoundException ex)
				{
					Console.WriteLine("Wrong input: " + ex.Message);
				}
			}

		}

		private static void ExampleConversion()
		{
			MorseCodeTranslator translator = new MorseCodeTranslator(new MorseCharCollection());		//Basic alphabet morsechars 
			while (true)
			{
				Console.WriteLine("Type in some text: ");
				string input = Console.ReadLine();
				try
				{
					string[] inputToMorse = translator.ConvertStringToMorse(input);
					for (int i = 0; i < inputToMorse.Length; i++)
					{
						Console.WriteLine(inputToMorse[i]);
					}
				}
				catch (MorseCharNotFoundException ex)
				{
					Console.WriteLine("Wrong input: " + ex.Message);
				}
			}
		}

		private static void ExampleNewChars()
		{
			List<MorseChar> morse = new List<MorseChar>
			{
				new MorseChar('n', "..."),
				new MorseChar('ä',".--"),
				new MorseChar('h',".-.-")
			};

			MorseCharCollection morseChars = new MorseCharCollection(morse);
			MorseCodeTranslator morseTranslator = new MorseCodeTranslator(morseChars);


			string[] morseRepresentations = morseTranslator.ConvertStringToMorse("näh");
			do
			{
				for (int i = 0; i < morseRepresentations.Length; i++)
				{
					Console.WriteLine(morseRepresentations[i]);
				}
			} while (true == false);

			MorseCodeTranslator.EncodeMorseToSoundFile(".-.-", "Salsa");		//Create soundfile from scratch without use of new MorseChars
		}
	}
}
