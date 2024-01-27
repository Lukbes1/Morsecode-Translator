using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MorseCode;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Principal;
using System.IO;
using System.Collections.Generic;

namespace MorseCodeTests
{
	[TestClass]
	public class MorseCodeTranslatorTests
	{
		/* standard Alphabet
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
		*/
		[TestMethod]
		public void ConvertStringToMorse_WithBlanks_WithValidText_ReturnsMorseWithBlanks()
		{
			// Arrange
			MorseCodeTranslator translatorWithWholeAlphabet = new MorseCodeTranslator();
			string textWithBlanks = "i l p";
			string[] expectedMorse = new string[] { ".."," ", ".-.."," " ,".--."};
			
			// Act
			string[] actualMorse = translatorWithWholeAlphabet.ConvertStringToMorse(textWithBlanks,true);

			// Assert
			CollectionAssert.AreEqual(expectedMorse, actualMorse);
		}

		[TestMethod]
		public void ConvertStringToMorse_WithInvalidText_ThrowsMorseCharNotFoundException()
		{
			// Arrange
			MorseCodeTranslator translatorWithWholeAlphabet = new MorseCodeTranslator();
			string textWithBlanks = "I L P";

			// Act and Assert 
			Assert.ThrowsException<MorseCharNotFoundException>(() => translatorWithWholeAlphabet.ConvertStringToMorse(textWithBlanks, true));
			Assert.ThrowsException<MorseCharNotFoundException>(() => translatorWithWholeAlphabet.ConvertStringToMorse(textWithBlanks, false));
		}

		[TestMethod]
		public void ConvertStringToMorse_WithoutBlanks_WithValidText_ReturnsMorseWithoutBlanks()
		{
			// Arrange
			MorseCodeTranslator translatorWithWholeAlphabet = new MorseCodeTranslator();
			string textWithoutBlanks = "i l p";
			string[] expectedMorse = new string[] { "..", ".-..", ".--." };

			// Act
			string[] actualMorse = translatorWithWholeAlphabet.ConvertStringToMorse(textWithoutBlanks, false);

			// Assert
			CollectionAssert.AreEqual(expectedMorse, actualMorse);
			
		}

		[TestMethod]
		public void ConvertMorseToString_WithBlanks_WithValidText_ReturnsMorseWithBlanks()
		{
			// Arrange
			MorseCodeTranslator translatorWithWholeAlphabet = new MorseCodeTranslator();		
			string[] morseRepresentationsWithBlanks = new string[] { "..", " " ,".-..", " ", ".--." };
			string expectedMorseString = "i l p";

			// Act
			string actualMorseString = translatorWithWholeAlphabet.ConvertMorseToString(morseRepresentationsWithBlanks, true);

			// Assert
			Assert.AreEqual(expectedMorseString, actualMorseString);
		}

		[TestMethod]
		public void ConvertMorseToString_WithoutBlanks_WithValidText_ReturnsMorseWithoutBlanks()
		{
			// Arrange
			MorseCodeTranslator translatorWithWholeAlphabet = new MorseCodeTranslator();
			string[] morseRepresentationsWithBlanks = new string[] { "..", " ", ".-..", " ", ".--." };
			string expectedMorseString = "ilp";

			// Act
			string actualMorseString = translatorWithWholeAlphabet.ConvertMorseToString(morseRepresentationsWithBlanks, false);

			// Assert
			Assert.AreEqual(expectedMorseString, actualMorseString);
		}

		[TestMethod]
		public void ConvertMorseToString_WithInvalidText_ThrowsMorseCharNotFoundException()
		{
			// Arrange
			MorseCodeTranslator translatorWithWholeAlphabet = new MorseCodeTranslator();
			string[] morseRepresentationsWithBlanks = new string[] { ".......", " "};

			// Act and Assert 
			Assert.ThrowsException<MorseCharNotFoundException>(() => translatorWithWholeAlphabet.ConvertMorseToString(morseRepresentationsWithBlanks, false),"Without blanks failed");
			Assert.ThrowsException<MorseCharNotFoundException>(() => translatorWithWholeAlphabet.ConvertMorseToString(morseRepresentationsWithBlanks, true), "With blanks failed");
		}

		//Only checking if its found or not, checking for the actual sound played would be too much effort
		[TestMethod]
		public void PlayMorseFromString_WithInvalidText_ThrowsMorseCharNotFoundException()
		{
			// Arrange
			MorseCodeTranslator translatorWithWholeAlphabet = new MorseCodeTranslator();
			string invalidtext = "I L P";

			// Act Assert
			Assert.ThrowsException<MorseCharNotFoundException>(() => translatorWithWholeAlphabet.PlayMorseFromString(invalidtext));
		}


		[TestMethod]
		public void PlayMorseFromStringAsync_WithInvalidText_ThrowsMorseCharNotFoundException()
		{
			// Arrange
			MorseCodeTranslator translatorWithWholeAlphabet = new MorseCodeTranslator();
			string invalidtext = "I L P";

			// Act Assert
			Assert.ThrowsExceptionAsync<MorseCharNotFoundException>(() => translatorWithWholeAlphabet.PlayMorseFromStringAsync(invalidtext));
		}


		[TestMethod]
		public void PlayMorseFromChar_WithInvalidChar_ThrowsMorseCharNotFoundException()
		{
			// Arrange
			MorseCodeTranslator translatorWithWholeAlphabet = new MorseCodeTranslator();
			char invalidtext = 'I';

			// Act Assert
			Assert.ThrowsException<MorseCharNotFoundException>(() => translatorWithWholeAlphabet.PlayMorseFromChar(invalidtext));
		}

		[TestMethod]
		public void PlayMorseFromCharAsync_WithInvalidChar_ThrowsMorseCharNotFoundException()
		{
			// Arrange
			MorseCodeTranslator translatorWithWholeAlphabet = new MorseCodeTranslator();
			char invalidtext = 'I';

			// Act Assert
			Assert.ThrowsExceptionAsync<MorseCharNotFoundException>(() => translatorWithWholeAlphabet.PlayMorseFromCharAsync(invalidtext));
		}

		[TestMethod]
		public void ConvertCharToMorse_WithValidChar_ReturnsMorse()
		{
			// Arrange
			MorseCodeTranslator translatorWithNewDefinition = new MorseCodeTranslator(new MorseCharCollection(new List<MorseChar>() { new MorseChar('+', "...", true) }));
			char validChar = '+';
			string expectedMorse = "...";

			// Act 
			string actualMorse = translatorWithNewDefinition.ConvertCharToMorse(validChar);

			// Assert
			Assert.AreEqual(expectedMorse, actualMorse);		  
		}

		[TestMethod]
		public void ConvertCharToMorse_WithOnlyAlphabetKnownChar_ReturnsMorse()
		{
			// Arrange
			MorseCodeTranslator translatorWithNewDefinition = new MorseCodeTranslator(new MorseCharCollection(new List<MorseChar>() { new MorseChar('+', "...", true) }));
			char alphabetChar = 'm';
			string expectedMorse = "--";

			// Act 
			string actualMorse = translatorWithNewDefinition.ConvertCharToMorse(alphabetChar);

			// Assert
			Assert.AreEqual(expectedMorse, actualMorse);
		}

		[TestMethod]
		public void ConvertCharToMorse_WithUnknownChar_ThrowsMorseCharNotFoundException()
		{
			// Arrange
			MorseCodeTranslator translatorWithNewDefinition = new MorseCodeTranslator(new MorseCharCollection(new List<MorseChar>() { new MorseChar('+', "...", true) }));
			char UnknownChar = '#';

			// Act Assert
			Assert.ThrowsException<MorseCharNotFoundException>(() => translatorWithNewDefinition.ConvertCharToMorse(UnknownChar));
		}

		[TestMethod]
		public void ConvertMorseToChar_WithValidMorse_ReturnsChar()
		{
			// Arrange
			MorseCodeTranslator translatorWithNewDefinition = new MorseCodeTranslator(new MorseCharCollection(new List<MorseChar>() { new MorseChar('+', "...", true) }));
			string validMorse = "...";
			char expectedChar = '+';
			

			// Act 
			char actualChar = translatorWithNewDefinition.ConvertMorseToChar(validMorse);

			// Assert
			Assert.AreEqual(expectedChar, actualChar);
		}

		[TestMethod]
		public void ConvertMorseToChar_WithUnknownMorse_ThrowsMorseCharNotFoundException()
		{
			// Arrange
			MorseCodeTranslator translatorWithNewDefinition = new MorseCodeTranslator(new MorseCharCollection(new List<MorseChar>() { new MorseChar('+', "...", true) }));
			string UnknownMorse = "...-------";

			// Act Assert
			Assert.ThrowsException<MorseCharNotFoundException>(() => translatorWithNewDefinition.ConvertMorseToChar(UnknownMorse));
		}

		[TestMethod]
		public void ConvertMorseToChar_WithOnlyAlphabetKnownMorse_ReturnsChar()
		{
			// Arrange
			MorseCodeTranslator translatorWithNewDefinition = new MorseCodeTranslator(new MorseCharCollection(new List<MorseChar>() { new MorseChar('+', "...", true) }));
			string alphabetMorse = "--";
			char expectedChar = 'm';

			// Act 
			char actualChar = translatorWithNewDefinition.ConvertMorseToChar(alphabetMorse);

			// Assert
			Assert.AreEqual(expectedChar, actualChar);
		}


		//Only checking if a new file is created, checking if its correct maybe comes later on (More effort)
		[TestMethod]
		public void EncodeMorseToSoundFileOverride_WithOneValidMorseAndFileName_CreatesSoundFile()
		{
			//Arrange 
			string morseRepresentation = "...-.";
			string fileName = "Test";
			string outputDir = @"MorseSoundFiles\";
			string fullPath = outputDir + fileName + ".wav";
			bool fileExists;

			//Act 
			MorseCodeTranslator.EncodeMorseToSoundFile(morseRepresentation, fileName, true);
			if (File.Exists(fullPath))
			{
				fileExists = true;
			}
			else
				fileExists = false;

			//Assert 
			Assert.IsTrue(fileExists);
			File.Delete(fullPath);
		}

		[TestMethod]
		public void EncodeMorseToSoundFileNoOverride_WithOneValidMorseAndExistingFile_ThrowsArgumentException()
		{
			//Arrange 
			string morseRepresentation = "...-.";
			string fileName = "Test";
			string outputDir = @"MorseSoundFiles\";
			string fullPath = outputDir + fileName + ".wav";
			var file = File.Create(fullPath);
			file.Close();

			//Act and assert
			Assert.ThrowsException<ArgumentException>( () => MorseCodeTranslator.EncodeMorseToSoundFile(morseRepresentation, fileName, false));
			File.Delete(fullPath);
		}

		[TestMethod]
		public void EncodeMorseToSoundFile_WithOneInvaildMorseAndFileName_ThrowsArgumentException()
		{
			//Arrange 
			string morseRepresentation = ".-Blubb";
			string fileName = "Test";

			//Act and Assert 
			Assert.ThrowsException<ArgumentException>( () => MorseCodeTranslator.EncodeMorseToSoundFile(morseRepresentation, fileName, true));
			
		}

		[TestMethod]
		public void EncodeMorseToSoundFileOverride_WithManyValidMorseAndFileName_CreatesSoundFile()
		{
			//Arrange 
			string[] morseRepresentation = new string[] { "....",".-.-"};
			string fileName = "Test";
			string outputDir = @"MorseSoundFiles\";
			string fullPath = outputDir + fileName + ".wav";
			bool fileExists;

			//Act 
			MorseCodeTranslator.EncodeMorseToSoundFile(morseRepresentation, fileName, true);
			if (File.Exists(fullPath))
			{
				fileExists = true;
			}
			else
				fileExists = false;

			//Assert 
			Assert.IsTrue(fileExists);
			File.Delete(fullPath);
		}

		[TestMethod]
		public void EncodeMorseToSoundFileOverride_WithManyInvaildMorseAndFileName_ThrowsArgumentException()
		{
			//Arrange 
			string[] morseRepresentation = new string[] { "..Blibb", ".-.-_Blabb" };
			string fileName = "Test";

			//Act and Assert 
			Assert.ThrowsException<ArgumentException>(() => MorseCodeTranslator.EncodeMorseToSoundFile(morseRepresentation, fileName, true));
		}

		[TestMethod]
		public void EncodeMorseToSoundFileNoOverride_WithManyValidMorseAndExistingFile_ThrowsArgumentException()
		{
			//Arrange 
			string[] morseRepresentation = new string[] { "....", ".-.-" };
			string fileName = "Test";
			string outputDir = @"MorseSoundFiles\";
			string fullPath = outputDir + fileName + ".wav";
			var file = File.Create(fullPath);
			file.Close();

			//Act and assert
			Assert.ThrowsException<ArgumentException>(() => MorseCodeTranslator.EncodeMorseToSoundFile(morseRepresentation, fileName, false));
			File.Delete(fullPath);
		}
	}	
}

