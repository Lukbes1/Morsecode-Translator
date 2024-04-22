using Microsoft.VisualStudio.TestTools.UnitTesting;
using MorseCode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MorseCodeTests
{
	[TestClass]
	public class MorseCharCollectionTests
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
		public void MorseCodeAudioDirProperty_WithValidDir_ChangesInternalDirs()
		{
			//Arrange
			MorseCharCollection collectionWithWholeAlphabet = new MorseCharCollection();
			string oldDir = "MorseCodeAudio";
			string newDir = @"..\MorseCodeAudio";
			string testMorse = "..-.";
			string testFileName = "Test_F";
			string newBeepShortPath = newDir + @"\Beep_short.wav";
			Directory.CreateDirectory(newDir);

			//Act
			MorseCharCollection.MorseCodeAudioDir = newDir;

			//Assert
			Assert.AreEqual(newDir, MorseCharCollection.MorseCodeAudioDir);
			Assert.AreEqual(newBeepShortPath, MorseChar.Beep_short_path);

			Directory.Delete(newDir);
		}

		[TestMethod]
		public void MorseCodeSoundFilesDirProperty_WithValidDir_ChangesInternalDirs()
		{
			//Arrange
			MorseCharCollection collectionWithWholeAlphabet = new MorseCharCollection();
			string oldDir = "MorseSoundFiles";
			string newDir = @"..\MorseSoundFiles";
			string testMorse = "--";
			string testFileName = "Test_M";
			Directory.CreateDirectory(newDir);

			//Act
			MorseCharCollection.MorseSoundFilesDir = newDir;
			MorseChar.CreateSoundFile(testMorse, testFileName); //Try to see if changes applied
			
			//Assert
			Assert.AreEqual(newDir, MorseCharCollection.MorseSoundFilesDir);
			Assert.IsTrue(File.Exists(MorseCharCollection.MorseSoundFilesDir + @"\" + testFileName + ".wav"));
			File.Delete(MorseCharCollection.MorseSoundFilesDir + @"\" + testFileName + ".wav");
			Directory.Delete(newDir);
		}
	}
}
