using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MorseCode
{
	internal class MorseChar
	{
		public char Character { get; private set; }
		public string MorseRepresentation { get; private set; }
		public string SoundFile { get; private set; }


		public MorseChar(char character, string morseRepresentation, string soundFile)
		{
			CreateSoundFile();
			this.Character = character;
			if (!(morseRepresentation.Contains(".") || morseRepresentation.Contains("-") || morseRepresentation.Contains(" ")))
			{
				throw new ArgumentException("Error: morseRepresentation must contain . or -");
			}
			//if (morseRepresentation.Length > 6)
			//{

			//}
			this.MorseRepresentation = morseRepresentation;
			this.SoundFile = soundFile;
		}
		
		private void CreateSoundFile()
		{
			List<SignalGenerator> beeps = new List<SignalGenerator>();
			List<SilenceProvider> silencers = new List<SilenceProvider>();
			foreach (char beep in MorseRepresentation)
			{
				beeps.Add(new SignalGenerator());
				SignalGenerator lastBeep = beeps.Last();
				lastBeep.Frequency = 600;
				lastBeep.Gain = 0.2;

				if (beep == '.')
					lastBeep.Take(TimeSpan.FromSeconds(1));
				else if (beep == '-')
					lastBeep.Take(TimeSpan.FromSeconds(2));
				silencers.Add(new SilenceProvider(lastBeep.WaveFormat));
				silencers.Last().ToSampleProvider().Take(TimeSpan.FromSeconds(0.25));
			}

			// Concatenate beeps and silences
			ISampleProvider concatenated = Concatenate(beeps);
			ISampleProvider finalOutput = Concatenate(concatenated, silencers.Select(s => s.ToSampleProvider()));

			// Save the final output to a WAV file
			WaveFileWriter.CreateWaveFile("output.wav", finalOutput.ToWaveProvider());

			//ISampleProvider sampleProvider;
			//sampleProvider = beeps.ElementAt(0).FollowedBy(silencers.ElementAt(0).ToSampleProvider()).;
			//var concat = beep1.FollowedBy(silence).FollowedBy(beep2);
			//WaveFileWriter wr = new WaveFileWriter("Test", concat.WaveFormat);
			////wr.WriteSamples();
			//using (var wo = new WaveOutEvent())
			//{
			//	wo.Init(concat);
			//	wo.Play();
			//	while (wo.PlaybackState == PlaybackState.Playing)
			//	{
			//		Thread.Sleep(500);
			//	}
			//}
		}

		private ISampleProvider Concatenate(IEnumerable<ISampleProvider> providers)
		{
			return new ConcatenatingSampleProvider(providers);
		}

		// Concatenate two ISampleProvider instances
		private ISampleProvider Concatenate(ISampleProvider first, ISampleProvider second)
		{
			return new ConcatenatingSampleProvider(new[] { first, second });
		}

		public void PlayMorse(SoundPlayer soundPlayer)
		{
			try
			{
				soundPlayer.SoundLocation = SoundFile;
				soundPlayer.Load();
				soundPlayer.PlaySync();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);
			}
		}

		public async Task PlayMorseAsync(SoundPlayer soundPlayer)
		{
			try
			{
				soundPlayer.SoundLocation = SoundFile;
				soundPlayer.Load();
				await Task.Run(() => soundPlayer.Play());
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);
			}
		}		

	}
}
