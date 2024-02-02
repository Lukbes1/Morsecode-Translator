using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;

namespace MorseCode
{
	/// <summary>
	/// Converts .wav files into text (at the moment only files created within this project or those similiar to it).
	/// </summary>
	internal static class MorseAudioReader
	{
		/// <summary>
		/// Describes the interpretation of the audio samples
		/// </summary>
		private enum MorseIdentifier
		{
			Long = 45,
			Short = 46,
			NextDotOrDash, //If at the end of a stream, this refers to Last silence
			NewChar,
			NewWord,
		}
		/// <summary>
		/// Amount of zeros before a silence gets recognized, 50 is good enough. the bigger, the less precise
		/// </summary>
		private const int SAMPLE_SILENCE_THREASHOLD = 50;

		/// <summary>
		/// Factor of the amount of samples that can be different from a short- or longbeep or silence
		/// </summary>
		private static float sample_difference_threashold_factor = 0.10f; 
											
		/// <summary>
		/// Change the value from 0 to 1 in order to change the sample recognition precision based on the soundfiles in the MorseCodeAudio dir. <br/>
		/// 1 defines a big tolerance but can also break the algorithm, 0 will almost certainly break it too.
		/// </summary>
		public static float SampleDifferenceThreasholdFactor
		{
			get { return sample_difference_threashold_factor; }
			set 
			{
				if (value >= 0 && value <= 1)
					sample_difference_threashold_factor = value;
				else
					throw new ArgumentException("Error: please choose a factor thats between 0 and 1");
			}
		}

		/// <summary>
		/// Decodes the <paramref name="waveFile"/> to a string array containing the morse representations. <br/>
		/// Files are decoded based on the known pattern used by this project, namely the files in MorseCodeAudio.
		/// </summary>
		/// <param name="waveFile"></param>
		/// <returns></returns>
		/// <exception cref="FileNotFoundException"></exception>
		public static string[] DecodeWaveFileToMorse(string waveFile, bool returnWithBlanks = true)
		{
			if (!File.Exists(waveFile))
			{
				throw new FileNotFoundException("Error: the file " + waveFile + " was not found");
			}
				
			var samples = DecodeWaveFileToBeepSamples(waveFile);
			var representations = ConvertSamplesToMeaning(samples.Item1, samples.Item2);
			int currentCharacter = 0;
			var morseRepresentations = new List<string>();
			for (int i = 0; i < representations.Length; i++)
			{
				switch (representations[i])
				{
					case MorseIdentifier.NewWord:
						if (returnWithBlanks)
						{
							morseRepresentations.Add(" ");
							currentCharacter += 2;
						}					
						break;
					case MorseIdentifier.NextDotOrDash:
						break;
					case MorseIdentifier.NewChar:
						currentCharacter++;
						break;
					default:
						var morseRepr = (char)representations[i];
						if (morseRepresentations.Count <= currentCharacter)
						{	
							morseRepresentations.Add(morseRepr.ToString());
						}
						else 
							morseRepresentations[currentCharacter] += morseRepr.ToString();
						break;
				}
			}
			return morseRepresentations.ToArray();
		}

		/// <summary>
		/// Converts the <paramref name="beeps"/> and <paramref name="silences"/> into one array of MorseIdentifier with the enum representing what the read pile of samples means
		/// </summary>
		/// <param name="beeps"></param>
		/// <param name="silences"></param>
		/// <returns></returns>
		private static MorseIdentifier[] ConvertSamplesToMeaning(List<int> beeps, List<int> silences)
		{
			MorseIdentifier[] representations = new MorseIdentifier[beeps.Count + silences.Count];
			int indexBeeps = 0;
			int indexSilences = 0;
			int beepLong = beeps.Max();
			int beepShort = beeps.Min();
			int silenceShort = silences.Min();
			int silenceForWord = silenceShort * 10;		
			int silenceLong = silenceShort * 3;
			bool hasWord = true;
			if (IsInRange(silenceForWord,silenceShort, sample_difference_threashold_factor))
				hasWord = false;
			for (int i = 0; i < representations.Length; i++)
			{
				if (i % 2 == 0)
				{
					
					if (IsInRange(beeps[indexBeeps],beepLong, sample_difference_threashold_factor))
						representations[i] = MorseIdentifier.Long;
					else if (IsInRange(beeps[indexBeeps], beepShort, sample_difference_threashold_factor))
						representations[i] = MorseIdentifier.Short;
					else
						throw new ArgumentException("Error: the wav file contains data that can't be interpreted");
					indexBeeps++;
				}
				else
				{
					if (IsInRange(silences[indexSilences], silenceForWord, sample_difference_threashold_factor) && hasWord)
						representations[i] = MorseIdentifier.NewWord;
					else if (IsInRange(silences[indexSilences], silenceLong, sample_difference_threashold_factor))
						representations[i] = MorseIdentifier.NewChar;
					else if (IsInRange(silences[indexSilences], silenceShort, sample_difference_threashold_factor))
						representations[i] = MorseIdentifier.NextDotOrDash;
					else
						throw new ArgumentException("Error: the wav file contains data that can't be interpreted");
					indexSilences++;
				}
			}
			return representations;
		}

		/// <summary>
		/// Checks if <paramref name="checkForNum"/> is in the upper and lower boundary from <paramref name="boundaryDeterminer"/> +- * <paramref name="differenceThreashold"/>
		/// </summary>
		/// <param name="checkForNum"></param>
		/// <param name="boundaryDeterminer"></param>
		/// <returns></returns>
		private static bool IsInRange(int checkForNum, int boundaryDeterminer, float differenceThreashold)
		{
			return (checkForNum > (int)(boundaryDeterminer - (boundaryDeterminer * differenceThreashold)) && checkForNum < (int)(boundaryDeterminer + (boundaryDeterminer * differenceThreashold)));
		}

		/// <summary>
		/// Decodes the <paramref name="waveFile"/> into two lists. List one are the filtered beeps and list two are the filtered silences.
		/// </summary>
		/// <param name="waveFile"></param>
		/// <returns></returns>
		/// <exception cref="FileNotFoundException"></exception>

		private static Tuple<List<int>,List<int>> DecodeWaveFileToBeepSamples(string waveFile)	//Samples for Item 1: Beeps , Item 2: Silence
		{
			if (!File.Exists(waveFile))
			{
				throw new FileNotFoundException("Error: the file " + waveFile + " was not found");
			}
			WaveFileReader wavReader = new WaveFileReader(waveFile);
			float[] sampleData = ReadData(wavReader);
			int sampleSilenceCounter = 0;
			int sampleSilenceAmount = 0;
			bool searchingForNextChar = false; //false = searchingForNextSilence
			int samplesBeepCounter = -1;
			List<int> beeps = new List<int>();
			List<int> silences = new List<int>();

			for (int i = 0; i < sampleData.Length; i++)
			{
				if (sampleData[i] == 0)     //searching for next char indicates that we are coming from a beep (short or long) and are searching for silence.
				{
					if (!searchingForNextChar)
					{
						sampleSilenceCounter++;     //Counts till threashold to be sure its a silence
						sampleSilenceAmount++;      //Actually remembers the amount counted
						if (sampleSilenceCounter == SAMPLE_SILENCE_THREASHOLD)
						{
							beeps.Add(samplesBeepCounter);
							samplesBeepCounter = 0;
							sampleSilenceCounter = 0;
							searchingForNextChar = true;
						}
					}
					else if (searchingForNextChar)
					{
						sampleSilenceAmount++;
					}
				}
				else if (sampleData[i] != 0)
				{
					if (!searchingForNextChar)
					{
						samplesBeepCounter++;
					}
					else if (searchingForNextChar)
					{
						silences.Add(sampleSilenceAmount);
						sampleSilenceAmount = 0;
						sampleSilenceCounter = 0;
						searchingForNextChar = false;
					}
				}
				if (i == sampleData.Length - 1)    //Add last silence
				{
					if (sampleSilenceAmount >= SAMPLE_SILENCE_THREASHOLD)
						silences.Add(sampleSilenceAmount);
				}
			}
			return Tuple.Create(beeps, silences);
		}

		/// <summary>
		/// Reads the data given by the <paramref name="fileReader"/> and returns the samples as a float array
		/// </summary>
		/// <param name="fileReader"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		private static float[] ReadData(WaveFileReader fileReader)
		{
			if (fileReader.SampleCount == 0)
				throw new Exception("Error: There is no data in the file to be read");
			if (fileReader.WaveFormat.Channels != 1)
				throw new NotImplementedException("Error: file has 2 channels, the project can only read from one right now");
			List<float> sampleData = new List<float>(((int)fileReader.SampleCount));
			while (fileReader.CanRead)
			{
				float[] sampleDataTemp = fileReader.ReadNextSampleFrame();
				if (sampleDataTemp == null)
				{
					if (sampleData != null)
					{
						return sampleData.ToArray();
					}
					else
						return new float[] { 0f };
				}
				else
				{
					foreach (float sample in sampleDataTemp)
					{
						sampleData.Add(sample);
					}
				}			
			}
			return sampleData.ToArray();
		}
	}
}
