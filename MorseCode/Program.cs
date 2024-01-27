using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MorseCode
{
	internal class Program
	{
		static void Main(string[] args)
		{
			//MorseCodeTranslator translator = new MorseCodeTranslator();

			ExampleEncodingDecoding();
			//var text = MorseCodeTranslator.DecodeSoundFileToMorse(@"MorseSoundFiles\b.wav");
			//MorseAudioReader.SampleDifferenceThreasholdFactor = 0.10f;


			//MorseCodeTranslator.EncodeMorseToSoundFile(translator.ConvertStringToMorse("david is crazy",true),"David");
			//var morse = translator.DecodeSoundFileToText(@"MorseSoundFiles\D.wav");
			//var morse = translator.ConvertStringToMorse("h e l p", false);
			//translator.PlayMorseFromString(morse);



			Console.ReadKey();		
		}
		private static void ExampleEncodingDecoding()
		{
			MorseCodeTranslator translator = new MorseCodeTranslator();
			var morseFromString = translator.ConvertStringToMorse("csharp is the best language ever");


			MorseCodeTranslator.EncodeMorseToSoundFile(morseFromString, "CSHARP");

			var morseFromSoundFile = MorseCodeTranslator.DecodeSoundFileToMorse(@"MorseSoundFiles\CSHARP.wav");
			var textFromSoundFile = translator.DecodeSoundFileToText(@"MorseSoundFiles\CSHARP.wav");
			foreach (var morse in morseFromSoundFile)
			{
                Console.WriteLine("MorseFromSoundFile: " + morse);
            }
			Console.WriteLine("TextFromConversion: " + translator.ConvertMorseToString(morseFromSoundFile));
            Console.WriteLine("TextFromSoundFile: "+ textFromSoundFile);
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
			MorseCodeTranslator translator = new MorseCodeTranslator();
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
					translator.PlayMorseFromString(input,TimeSpan.FromSeconds(1));
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
