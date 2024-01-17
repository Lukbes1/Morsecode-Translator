using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MorseCode
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Console.ReadKey();		
		}


		private static void ExampleWholeConversion()
		{
			List<MorseChar> morse = new List<MorseChar>
			{
				new MorseChar('n', "..."),
				new MorseChar('ä',".--"),
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

			MorseCodeTranslator.DecodeMorseToSoundFile(".-.-", "Salsa");		//Create soundfile from scratch without use of new MorseChars
		}
	}
}
