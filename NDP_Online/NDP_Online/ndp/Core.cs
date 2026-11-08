using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ndp
{
	// Token: 0x02000015 RID: 21
	public class Core
	{
		// Token: 0x17000017 RID: 23
		// (get) Token: 0x060000F7 RID: 247 RVA: 0x000201DC File Offset: 0x0001E3DC
		public bool HasSlowMotion
		{
			get
			{
				return this.SlowMusicToRealFrame != null;
			}
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x000201E8 File Offset: 0x0001E3E8
		public void RegisterSlowEvent(int Value)
		{
			if (!this.GetTimeCompile || this.SoundId != 0)
			{
				return;
			}
			this.SlowEvents.Add(new Core.SlowEvent
			{
				Frame = Math.Max(0, this.GetCurrentPlaybackFrame()),
				Pattern = Core.SlowParameter[Value],
				Sequence = this.SlowEvents.Count
			});
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x00020248 File Offset: 0x0001E448
		public void BuildSlowTimeMap()
		{
			this.SlowMusicToRealFrame = null;
			if (this.SlowEvents.Count != 0)
			{
				if (this.SlowEvents.Any<Core.SlowEvent>((Core.SlowEvent e) => e.Pattern != 0))
				{
					Core.SlowEvent[] array = (from e in this.SlowEvents
						orderby e.Frame, e.Sequence
						select e).ToArray<Core.SlowEvent>();
					int num = Math.Max(this.TotalFrame, 1);
					int[] array2 = new int[num + 1];
					int num2 = 0;
					int num3 = 0;
					int num4 = 0;
					int i = 0;
					while (i < num)
					{
						bool flag = (num2 & 1) != 0;
						num2 = ((num2 >> 1) | ((num2 & 1) << 7)) & 255;
						num3++;
						if (!flag)
						{
							while (num4 < array.Length && array[num4].Frame <= i)
							{
								num2 = array[num4++].Pattern;
							}
							i++;
							array2[i] = num3;
						}
					}
					this.SlowMusicToRealFrame = array2;
					return;
				}
			}
		}

		// Token: 0x060000FA RID: 250 RVA: 0x00020368 File Offset: 0x0001E568
		public double MusicToRealTime(double MusicSeconds)
		{
			if (!this.HasSlowMotion || MusicSeconds <= 0.0)
			{
				return MusicSeconds;
			}
			double num = MusicSeconds * 59.94080511738504;
			int num2 = (int)Math.Floor(num);
			if (num2 >= this.SlowMusicToRealFrame.Length - 1)
			{
				return ((double)this.SlowMusicToRealFrame[this.SlowMusicToRealFrame.Length - 1] + (num - (double)this.SlowMusicToRealFrame.Length + 1.0) * (double)(this.SlowMusicToRealFrame[this.SlowMusicToRealFrame.Length - 1] - this.SlowMusicToRealFrame[this.SlowMusicToRealFrame.Length - 2])) / 59.94080511738504;
			}
			double num3 = num - (double)num2;
			return ((double)this.SlowMusicToRealFrame[num2] + (double)(this.SlowMusicToRealFrame[num2 + 1] - this.SlowMusicToRealFrame[num2]) * num3) / 59.94080511738504;
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00020434 File Offset: 0x0001E634
		public double RealToMusicTime(double RealSeconds)
		{
			if (!this.HasSlowMotion || RealSeconds <= 0.0)
			{
				return RealSeconds;
			}
			double num = RealSeconds * 59.94080511738504;
			int num2 = Array.BinarySearch<int>(this.SlowMusicToRealFrame, (int)Math.Floor(num));
			if (num2 < 0)
			{
				num2 = ~num2 - 1;
			}
			num2 = Math.Max(0, num2);
			if (num2 >= this.SlowMusicToRealFrame.Length - 1)
			{
				int num3 = this.SlowMusicToRealFrame.Length - 1;
				int num4 = this.SlowMusicToRealFrame[num3] - this.SlowMusicToRealFrame[num3 - 1];
				return ((double)num3 + Math.Max(0.0, num - (double)this.SlowMusicToRealFrame[num3]) / (double)num4) / 59.94080511738504;
			}
			int num5 = this.SlowMusicToRealFrame[num2 + 1] - this.SlowMusicToRealFrame[num2];
			return ((double)num2 + Math.Max(0.0, Math.Min(1.0, (num - (double)this.SlowMusicToRealFrame[num2]) / (double)num5))) / 59.94080511738504;
		}

		// Token: 0x060000FC RID: 252 RVA: 0x0002052C File Offset: 0x0001E72C
		public Core()
		{
			this.TrackFrame = new int[4, 100];
			this.BeforeLoopFrame = new int[4, 100];
			this.AlignPoints = new Dictionary<string, AlignPointData>();
			this.AlignOccurrenceCounter = new Dictionary<string, int[]>();
			this.AlignTrackEventOrder = new List<string>[]
			{
				new List<string>(),
				new List<string>(),
				new List<string>(),
				new List<string>()
			};
			this.AlignSegmentFrame = new int[4];
			this.AlignPlanReady = false;
			this.DefineAddressAbsolute = 0;
			this.MetaData = new string[] { "", "", "", "", "" };
			this.BGMDataSize = 28672;
			this.InterruptTrackPoints = new List<InterruptTrackPointData>();
			this.AutoRepeatBoundaries = new List<AutoRepeatBoundaryData>();
			this.InterruptTrackPointSequence = 0;
			this.ParamInit();
			this.HeaderTempo = 120m;
			this.HeaderKeyShift = 0;
			this.HeaderKeySign = new int[13];
			this.SkipDefine = false;
			this.SkipF = false;
			this.SkipSE = false;
			this.DefaultVibrato = 0;
			this.DefaultPitchEnvelope = 0;
			this.UseRhythmAllAlphabet = false;
			int[] array = new int[] { -1, -1, -1, -1 };
			int[] array2 = new int[] { -1, -1, -1, -1 };
			int[] array3 = new int[] { -1, -1, -1, -1 };
			this.AtF_Frame = new int[][] { array, array2, array3 };
			this.FrameAnalyzeCount = new int[4];
			this.FrameAnalyzeText = new string[] { "", "", "", "" };
			this.StoreRhythmCode = "";
			this.RegisteredRhythmCode = "";
			this.RegisteredRhythmCodeLineNumber = new Dictionary<char, int>();
			this.PitchEnvelopeType = new byte[16];
			this.PitchEnvelopeShapeType = new int[16];
			this.PitchEnvelopeRegisterLine = new int[16];
			this.TrackNameLength = new int[100000];
			this.RepeatMode = new int[] { 4, 4, 4, 4 };
			this.TrackEnable = new bool[] { true, true, true, true };
			this.Mute = new bool[4];
			this.FastForward = false;
			this.FastForwardTrack = new bool[4];
			this.UseJ = new bool[4];
			this.SharpFrameMode = false;
			this.MacroPoint = new List<MacroPointData>();
			this.MacroSourceMap = new Dictionary<string, List<MMLSourcePosition>>();
			this.TrackSourceMap = new List<MMLSourcePosition>[100, 4];
			this.CurrentSourceMap = null;
			this.MultiLineStartLine = new Dictionary<int, int>();
			this.StartTime = 0.0;
			this.JumpStartTime = new double[7];
			this.JumpStartLine = new int[7];
			this.JumpOffset = -0.15;
			this.UseJump = false;
			this.FastForwardMode = false;
			this.DefaultOn = true;
			this.DefaultSeed = Environment.TickCount;
			this.ReverseTrack = new int[] { -1, -1 };
			this.SetJumpNumber = new bool[4, 7];
			this.ReleaseVolumeOffset = new int[4];
			this.VoiceMMLFromFile = new List<string>();
			this.RhythmVoiceMMLFromFile = new List<string>();
			this.UsePitchEnvelope = new bool[32];
			this.UseNoteEnvelope = new bool[32];
			this.RegisteredVolumeSequence = new bool[32];
			this.RegisteredQuantizeSequence = new bool[32];
			this.RegisteredDetuneSequence = Enumerable.Repeat<bool>(false, 32).ToArray<bool>();
			this.RegisteredHardwareEnvelopeDetuneSequence = Enumerable.Repeat<bool>(false, 32).ToArray<bool>();
			this.RegisteredHardwareEnvelopeKeyShiftSequence = Enumerable.Repeat<bool>(false, 32).ToArray<bool>();
			this.RegisteredHardwareEnvelopeDecaySequence = Enumerable.Repeat<bool>(false, 32).ToArray<bool>();
			this.RegisteredAtQuantizeSequence = new bool[32];
			this.RegisteredVolumeSequenceLineNumber = new Dictionary<int, int>();
			this.RegisteredQuantizeSequenceLineNumber = new Dictionary<int, int>();
			this.RegisteredAtQuantizeSequenceLineNumber = new Dictionary<int, int>();
			this.RegisteredDetuneSequenceLineNumber = new Dictionary<int, int>();
			this.RegisteredHardwareEnvelopeDetuneSequenceLineNumber = new Dictionary<int, int>();
			this.RegisteredHardwareEnvelopeKeyShiftSequenceLineNumber = new Dictionary<int, int>();
			this.RegisteredHardwareEnvelopeDecaySequenceLineNumber = new Dictionary<int, int>();
			this.VoiceHardwareEnvelope = new int[]
			{
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, -1, -1
			};
			this.BsaveFileName = null;
			this.StepMacroMML = new Dictionary<string, List<string>>();
			this.SelectStepMacroName = new string[4, 3];
			this.StepMacroCount = new int[4, 3];
			this.LinePlayTime = new List<List<double>>
			{
				new List<double>(),
				new List<double>(),
				new List<double>(),
				new List<double>()
			};
			this.LinePlayLineNumber = new List<List<int>>
			{
				new List<int>(),
				new List<int>(),
				new List<int>(),
				new List<int>()
			};
			this.NotePlayTime = new List<List<double>>
			{
				new List<double>(),
				new List<double>(),
				new List<double>(),
				new List<double>()
			};
			this.NotePlayLineNumber = new List<List<int>>
			{
				new List<int>(),
				new List<int>(),
				new List<int>(),
				new List<int>()
			};
			this.NotePlayColumn = new List<List<int>>
			{
				new List<int>(),
				new List<int>(),
				new List<int>(),
				new List<int>()
			};
			this.NotePlayParameters = new List<List<NotePlayParameter>>
			{
				new List<NotePlayParameter>(),
				new List<NotePlayParameter>(),
				new List<NotePlayParameter>(),
				new List<NotePlayParameter>()
			};
			this.LastNoteLengthText = new string[] { "", "", "", "" };
			this.PianoRollNotes = new List<PianoRollNote>();
			this.PianoRollBars = new List<PianoRollNote>();
			this.AdjustPianoRoll = 0.0;
			this.PianoRollLoop = 1;
			this.ParameterText = new List<string>();
			this.VoiceInfoText = new List<string>();
			this.VoiceInfoMML = new List<string>();
			this.InfoText = new List<string>();
			this.AutoSetHeader = new Dictionary<string, string>();
			this.InitializePresetVoice();
		}

		// Token: 0x060000FD RID: 253 RVA: 0x00020CA8 File Offset: 0x0001EEA8
		public void ParamInit()
		{
			this.PitchTable = new int[]
			{
				3421, 3228, 3047, 2876, 2715, 2562, 2419, 2283, 2155, 2034,
				1920, 1812, 1711, 1614, 1524, 1438, 1358, 1281, 1210, 1142,
				1078, 1017, 960, 906, 855, 807, 762, 719, 679, 641,
				605, 571, 539, 509, 480, 453, 428, 404, 381, 360,
				339, 320, 302, 285, 269, 254, 240, 227, 214, 202,
				190, 180, 170, 160, 151, 143, 135, 127, 120, 113,
				107, 101, 95, 90, 85, 80, 76, 71, 67, 64,
				60, 57, 53, 50, 48, 45, 42, 40, 38, 36,
				34, 32, 30, 28, 27, 25, 24, 22, 21, 20,
				19, 18, 17, 16, 15, 14
			};
			for (int i = 0; i < 4; i++)
			{
				this.TrackFrame[i, this.SoundId] = 0;
				this.BeforeLoopFrame[i, this.SoundId] = -1;
			}
			this.Tempo = new decimal[] { 120m, 120m, 120m, 120m };
			this.SetTempo = new bool[4];
			this.SetTimebase = new bool[4];
			this.KeyShift = new int[4];
			this.RelativeKeyShift = new int[4];
			this.Octave = new int[] { 4, 4, 4, 4 };
			this.Volume = new int[] { 15, 15, 15, 15 };
			this.VolumeLimit = new int[4];
			this.RhythmVolumeLimit = null;
			this.RestoreVolume = new int[] { 15, 15, 15, 15 };
			this.VolumeOffset = new int[4];
			this.ReleaseVolume = new int[] { 15, 15, 15, 15 };
			this.ReleaseVolumeAfter = new int[] { -1, -1, -1, -1 };
			this.ReleaseRate = new int[4];
			this.ReleaseDelay = new int[] { -1, -1, -1, -1 };
			this.ReleaseDelayFrame = new int[] { -1, -1, -1, -1 };
			this.FadeOutFrame = 0;
			this.FadeOutLoopCount = 0;
			this.vi = new int[4];
			this.viMax = new int[] { -1, -1, -1, -1 };
			decimal[] array = new decimal[]
			{
				4m, -1m, 4m, 4m, 4m, 4m, 4m, 4m, 4m, 4m,
				4m, 4m, 4m, 4m, 4m, 4m
			};
			decimal[] array2 = new decimal[]
			{
				4m, -1m, 4m, 4m, 4m, 4m, 4m, 4m, 4m, 4m,
				4m, 4m, 4m, 4m, 4m, 4m
			};
			decimal[] array3 = new decimal[]
			{
				4m, -1m, 4m, 4m, 4m, 4m, 4m, 4m, 4m, 4m,
				4m, 4m, 4m, 4m, 4m, 4m
			};
			decimal[] array4 = new decimal[]
			{
				4m, -1m, 4m, 4m, 4m, 4m, 4m, 4m, 4m, 4m,
				4m, 4m, 4m, 4m, 4m, 4m
			};
			this.DefaultLength = new decimal[][] { array, array2, array3, array4 };
			string[] array5 = Enumerable.Repeat<string>("", 16).ToArray<string>();
			string[] array6 = Enumerable.Repeat<string>("", 16).ToArray<string>();
			string[] array7 = Enumerable.Repeat<string>("", 16).ToArray<string>();
			string[] array8 = Enumerable.Repeat<string>("", 16).ToArray<string>();
			if (this.DefaultLengthAddString == null)
			{
				this.DefaultLengthAddString = new List<string[]>();
			}
			else
			{
				this.DefaultLengthAddString.Clear();
			}
			this.DefaultLengthAddString.Add(array5);
			this.DefaultLengthAddString.Add(array6);
			this.DefaultLengthAddString.Add(array7);
			this.DefaultLengthAddString.Add(array8);
			decimal[] array9 = new decimal[]
			{
				16m, -1m, 4m, 4m, 4m, 4m, 4m, 4m, 4m, 4m,
				4m, 4m, 4m, 4m, 4m, 4m
			};
			decimal[] array10 = new decimal[]
			{
				16m, -1m, 4m, 4m, 4m, 4m, 4m, 4m, 4m, 4m,
				4m, 4m, 4m, 4m, 4m, 4m
			};
			decimal[] array11 = new decimal[]
			{
				16m, -1m, 4m, 4m, 4m, 4m, 4m, 4m, 4m, 4m,
				4m, 4m, 4m, 4m, 4m, 4m
			};
			decimal[] array12 = new decimal[]
			{
				16m, -1m, 4m, 4m, 4m, 4m, 4m, 4m, 4m, 4m,
				4m, 4m, 4m, 4m, 4m, 4m
			};
			this.ArpeggioDefaultLength = new decimal[][] { array9, array10, array11, array12 };
			this.ArpeggioRest = new int[4];
			this.NoteLengthIndex = new int[4];
			this.ArpeggioNoteLengthIndex = new int[4];
			this.Quantize = new int[] { 8, 8, 8, 8 };
			this.QuantizeBackup = new int[] { 8, 8, 8, 8 };
			this.QuantizeCount = new int[4];
			this.QuantizeNumber = new int[4];
			this.AtQuantize = new int[4];
			this.AtQuantizeBackup = new int[4];
			this.AtQuantizeCount = new int[4];
			this.AtQuantizeNumber = new int[4];
			this.FrameMod = new decimal[4];
			this.VoiceNumber = new int[] { -1, -1, -1, -1 };
			this.DelayVoiceNumber = new int[] { -1, -1, -1, -1 };
			this.VolumeSequenceNumber = new int[] { 255, 255, 255, 255 };
			this.QuantizeSequenceNumber = new int[] { 255, 255, 255, 255 };
			this.AtQuantizeSequenceNumber = new int[] { 255, 255, 255, 255 };
			this.DetuneSequenceNumber = new int[] { 255, 255, 255, 255 };
			this.HardwareEnvelopeDetuneSequenceNumber = new int[] { 255, 255, 255, 255 };
			this.HardwareEnvelopeKeyShiftSequenceNumber = new int[] { 255, 255, 255, 255 };
			this.HardwareEnvelopeDecaySequenceNumber = new int[] { 255, 255, 255, 255 };
			this.PortamentoKeyShift = new int[4];
			this.PortamentoContinuousMode = new int[4];
			this.PortamentoContinuousKeyShift = new int[4];
			this.PortamentoContinuousStartNote = new int[4];
			this.ArpeggioVolumeSequenceNumber = new int[] { 255, 255, 255, 255 };
			this.ArpeggioQuantizeSequenceNumber = new int[] { 255, 255, 255, 255 };
			this.ArpeggioAtQuantizeSequenceNumber = new int[] { 255, 255, 255, 255 };
			this.ArpeggioDetuneSequenceNumber = new int[] { 255, 255, 255, 255 };
			this.ArpeggioHardwareEnvelopeDetuneSequenceNumber = new int[] { 255, 255, 255, 255 };
			this.ArpeggioHardwareEnvelopeKeyShiftSequenceNumber = new int[] { 255, 255, 255, 255 };
			this.ArpeggioHardwareEnvelopeDecaySequenceNumber = new int[] { 255, 255, 255, 255 };
			this.ArpeggioEndVolume = new int[] { 255, 255, 255, 255 };
			this.ArpeggioEndQuantize = new int[] { 255, 255, 255, 255 };
			this.ArpeggioEndAtQuantize = new int[] { 255, 255, 255, 255 };
			this.ArpeggioPoint1 = new int[] { -1, -1, -1, -1 };
			this.ArpeggioPoint2 = new int[] { -1, -1, -1, -1 };
			this.ArpeggioCount1 = new int[] { 1, 1, 1, 1 };
			this.ArpeggioCount2 = new int[] { 1, 1, 1, 1 };
			this.VolumeSequenceCounter = new int[4];
			this.QuantizeSequenceCounter = new int[4];
			this.AtQuantizeSequenceCounter = new int[4];
			this.DetuneSequenceCounter = new int[4];
			this.HardwareEnvelopeDetuneSequenceCounter = new int[4];
			this.HardwareEnvelopeKeyShiftSequenceCounter = new int[4];
			this.HardwareEnvelopeDecaySequenceCounter = new int[4];
			this.NoteEnvelopeNumber = new int[] { -1, -1, -1, -1 };
			this.PitchEnvelopeNumber = new int[] { -1, -1, -1, -1 };
			this.HardwareEnvelopeNumber = new int[] { -1, -1, -1, -1 };
			this.HardwareEnvelopePeriod = new int[4];
			this.HardwareEnvelopeVolumeDecayFrame = new int[4];
			this.MixMode = new int[] { 1, 1, 1, 1 };
			this.LoopAddress = new int[4];
			this.Portamento = new int[4];
			this.PortamentoStartNote = new int[4];
			this.FrameAnalyzeNote = new int[4];
			this.FrameAnalyzeLength = new int[4];
			this.PortamentoMode = new int[4];
			this.PortamentoWait = new int[4];
			this.PortamentoReverse = new bool[4];
			this.GlissStartNote = new int[4];
			this.GlissLegato = new bool[4];
			this.DetunePortamentoMode = new bool[4];
			this.PortamentoNoteSetAndSaveOctave = new int[4];
			this.PortamentoCommandSet = new bool[4];
			this.RandomStartNote = new int[4];
			this.SelfDelayRestFill = new bool[] { true, true, true, true };
			this.SetVoice = new bool[4];
			this.SetRhythmTrack = new bool[3];
			this.PutNote = new bool[4];
			this.RestNoteCount = new int[4];
			this.RestNoteSpan = new int[4];
			this.UseRhythmTrackAtC = 2;
			this.RepeatNest = new int[4, 2];
			this.RepeatCount = new int[4, 8, 2];
			this.RepeatEscapeOctave = new int[4, 8];
			this.RepeatEscapeLength = new decimal[4, 8][];
			this.RepeatEscapeLengthIndex = new int[4, 8];
			this.RepeatFrame1 = new int[4, 8, 2];
			this.RepeatFrame2 = new int[4, 8, 2];
			this.RepeatFrameMod = new decimal[4, 8];
			this.RepeatEscapeFrameMod = new decimal[4, 8];
			this.RepeatFrameModPending = new bool[4];
			this.RepeatNotClose = new bool[4];
			this.MacroMML = new Dictionary<string, string>();
			this.MacroLine = new Dictionary<string, int>();
			this.Slur = new bool[4];
			this.Legato = new bool[4];
			this.LegatoOffFlagSet = new bool[4];
			this.LoopSlur = new bool[4];
			this.PastNoteNumber = new int[4, 10];
			this.SelfDelayCount = new int[] { 255, 255, 255, 255 };
			this.SelfDelayVolume = new int[4];
			this.SelfDelayContinueCount = new int[4];
			this.UsePitchEnvelopeType = new int[] { -1, -1, -1, -1 };
			this.Detune = new int[4];
			this.EnableArpeggio = new bool[4];
			this.ArpeggioLegato = new bool[4];
			this.ArpeggioNoteNumber = new List<List<int>>
			{
				new List<int>(),
				new List<int>(),
				new List<int>(),
				new List<int>()
			};
			this.ArpeggioMode = new int[4];
			this.ArpeggioFirstNoteFrameAddress = new int[] { -1, -1, -1, -1 };
			this.HardwareEnvelopeDetune = 0;
			this.HardwareEnvelopeOctave = 3;
			this.HardwareEnvelopeKeyShift = 0;
			this.HardwareEnvelopeAuto = new bool[4];
			int[] array13 = new int[13];
			this.KeySign = new int[4][];
			this.KeySign[0] = array13;
			this.KeySign[1] = array13;
			this.KeySign[2] = array13;
			this.KeySign[3] = array13;
			this.SetVolume = new bool[4];
			this.SetQuantize = new bool[4];
			this.SetAtQuantize = new bool[4];
			this.Message = null;
			this.ErrorString = null;
			this.ErrorLine = 0;
			this.RandomParam = new Random(this.DefaultSeed);
			this.UseMacroCount = 0;
			if (this.SoundId < 1)
			{
				this.UseVoice = Enumerable.Repeat<bool>(false, 16).ToArray<bool>();
				this.UseRhythmVoice = Enumerable.Repeat<bool>(false, 32).ToArray<bool>();
				this.RegisteredVoice = Enumerable.Repeat<bool>(false, 16).ToArray<bool>();
				this.RegisteredVoiceLineNumber = new Dictionary<int, int>();
				this.SeekMarker = new Dictionary<string, double>();
				this.SeekMarkerLine = new Dictionary<string, int>();
				this.Status = null;
				this.BGMData = new byte[28672];
				this.StartAddress = 0;
				this.SEAddress = Enumerable.Repeat<int>(-1, 100).ToArray<int>();
				this.SEUseLine = new List<int>();
				this.SEUseAddress = new List<int>();
				this.SEUseNumber = new List<int>();
				this.BGMData[11] = 1;
				Version version = Assembly.GetExecutingAssembly().GetName().Version;
				this.BGMData[12] = (byte)version.Build;
				this.BGMData[13] = 0;
				this.Address = 14;
				return;
			}
			this.StartAddress = this.Address;
			this.SEAddress[this.SoundId] = this.StartAddress;
			this.Address = this.StartAddress + 8;
		}

		// Token: 0x060000FE RID: 254 RVA: 0x00021E78 File Offset: 0x00020078
		public void SetMetaData()
		{
			foreach (string text in this.MetaData)
			{
				byte[] bytes = Encoding.GetEncoding("Shift_JIS").GetBytes(text);
				if (bytes.Length != 0)
				{
					for (int j = 0; j < bytes.Length; j++)
					{
						this.BGMData[this.Address] = bytes[j];
						this.Address++;
					}
				}
				this.BGMData[this.Address] = byte.MaxValue;
				this.Address++;
			}
			if (this.Address > 14)
			{
				byte[] bgmdata = this.BGMData;
				int num = 11;
				bgmdata[num] |= 2;
			}
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00021F24 File Offset: 0x00020124
		public void InitializePresetVoice()
		{
			this.VoiceData = new List<byte[]>();
			this.VoiceDataUsePM = new List<int>();
			this.VoiceDataUseHENVHarfVolume = new List<bool>();
			this.RhythmVoiceData = new List<byte[]>();
			this.RhythmVoiceFrameLength = new List<int>();
			this.PitchEnvelopeData = new List<byte[]>
			{
				new byte[]
				{
					20, 1, 1, 1, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue,
					1, 1, 1, 128, 13
				},
				new byte[]
				{
					1, 14, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue,
					byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, 0, 128, 2
				},
				new byte[] { 1, 20, 128, 2 },
				new byte[] { 1, 240, 128, 2 }
			};
			for (int i = 0; i < 12; i++)
			{
				this.PitchEnvelopeData.Add(new byte[0]);
			}
			this.NoteEnvelopeData = new List<byte[]>();
			for (int j = 0; j < 16; j++)
			{
				this.NoteEnvelopeData.Add(new byte[0]);
			}
			this.RegisteredNoteEnvelopeLineNumber = new Dictionary<int, int>();
			this.VolumeSequenceData = new List<List<int>>();
			this.QuantizeSequenceData = new List<List<int>>();
			this.AtQuantizeSequenceData = new List<List<int>>();
			this.DetuneSequenceData = new List<List<int>>();
			this.HardwareEnvelopeDetuneSequenceData = new List<List<int>>();
			this.HardwareEnvelopeKeyShiftSequenceData = new List<List<int>>();
			this.HardwareEnvelopeDecaySequenceData = new List<List<int>>();
			for (int k = 0; k < 32; k++)
			{
				this.VolumeSequenceData.Add(new List<int>());
				this.QuantizeSequenceData.Add(new List<int>());
				this.AtQuantizeSequenceData.Add(new List<int>());
				this.DetuneSequenceData.Add(new List<int>());
				this.HardwareEnvelopeDetuneSequenceData.Add(new List<int>());
				this.HardwareEnvelopeKeyShiftSequenceData.Add(new List<int>());
				this.HardwareEnvelopeDecaySequenceData.Add(new List<int>());
			}
		}

		// Token: 0x06000100 RID: 256 RVA: 0x000220F0 File Offset: 0x000202F0
		public void SetHeaderParameter()
		{
			if (this.HeaderTempo != 120m)
			{
				for (int i = 0; i < 4; i++)
				{
					this.Tempo[i] = this.HeaderTempo;
				}
			}
			if (this.HeaderVolumeOffset != 0)
			{
				for (int j = 0; j < 4; j++)
				{
					this.VolumeOffset[j] = this.HeaderVolumeOffset;
				}
			}
			if (this.HeaderKeyShift != 0)
			{
				for (int k = 1; k < 4; k++)
				{
					this.KeyShift[k] = this.HeaderKeyShift;
				}
			}
			bool flag = false;
			int[] headerKeySign = this.HeaderKeySign;
			for (int l = 0; l < headerKeySign.Length; l++)
			{
				if (headerKeySign[l] != 0)
				{
					flag = true;
				}
			}
			if (flag)
			{
				for (int m = 0; m < 4; m++)
				{
					this.KeySign[m] = this.HeaderKeySign;
				}
			}
		}

		// Token: 0x06000101 RID: 257 RVA: 0x000221BB File Offset: 0x000203BB
		public void SkipSpace()
		{
			while (this.CheckNextCharacter(' '))
			{
			}
			while (this.CheckNextCharacter('\t'))
			{
			}
		}

		// Token: 0x06000102 RID: 258 RVA: 0x000221D4 File Offset: 0x000203D4
		public bool CheckNextCharacter(char Character)
		{
			this.Index++;
			if (this.Index >= this.LineBuffer.Length)
			{
				this.Index--;
				return false;
			}
			if (this.LineBuffer[this.Index] == '\f')
			{
				this.ExecJ6();
				this.CheckNextCharacter(Character);
			}
			char c = this.LineBuffer[this.Index];
			if (this.LineBuffer[this.Index] != (char)((byte)Character))
			{
				this.Index--;
				return false;
			}
			return true;
		}

		// Token: 0x06000103 RID: 259 RVA: 0x00022270 File Offset: 0x00020470
		public int NumberRead(bool SignSkip = false, bool EndSpaceSkip = true)
		{
			int num = this.NumberReadSub(SignSkip, EndSpaceSkip);
			if (this.CheckNextCharacter('~'))
			{
				int num2 = this.NumberReadSub(true, EndSpaceSkip);
				if (num > num2)
				{
					int num3 = num2;
					num2 = num;
					num = num3;
				}
				if (num < num2)
				{
					num2++;
					num = this.RandomParam.Next(num, num2);
				}
			}
			return num;
		}

		// Token: 0x06000104 RID: 260 RVA: 0x000222B8 File Offset: 0x000204B8
		public int NumberReadSub(bool SignSkip = false, bool EndSpaceSkip = true)
		{
			this.SkipSpace();
			bool flag = true;
			if (!SignSkip)
			{
				if (this.CheckNextCharacter('+'))
				{
					flag = true;
				}
				if (this.CheckNextCharacter('-'))
				{
					flag = false;
				}
			}
			this.Index++;
			if (this.Index >= this.LineBuffer.Length)
			{
				return -32769;
			}
			bool flag2 = false;
			string text = "";
			while (this.Index < this.LineBuffer.Length)
			{
				char c = this.LineBuffer[this.Index];
				if (c == '\f')
				{
					this.ExecJ6();
				}
				else if (c < '0' || c > '9')
				{
					this.Index--;
					if (!flag2)
					{
						return -32769 - (int)c;
					}
					break;
				}
				else
				{
					flag2 = true;
					ReadOnlySpan<char> readOnlySpan = text;
					char c2 = this.LineBuffer[this.Index];
					text = readOnlySpan + new ReadOnlySpan<char>(ref c2);
				}
				this.Index++;
			}
			if (EndSpaceSkip)
			{
				this.SkipSpace();
			}
			return int.Parse(flag ? text : ("-" + text));
		}

		// Token: 0x06000105 RID: 261 RVA: 0x000223C8 File Offset: 0x000205C8
		public decimal DecimalNumberRead(bool SignSkip = false, bool EndSpaceSkip = true)
		{
			decimal num = this.DecimalNumberReadSub(SignSkip, EndSpaceSkip);
			if (this.CheckNextCharacter('~'))
			{
				decimal num2 = this.DecimalNumberReadSub(true, EndSpaceSkip);
				if (num > num2)
				{
					decimal num3 = num2;
					num2 = num;
					num = num3;
				}
				if (num < num2)
				{
					num2 += 1m;
					num = this.RandomParam.Next((int)num, (int)num2);
				}
			}
			return num;
		}

		// Token: 0x06000106 RID: 262 RVA: 0x0002242C File Offset: 0x0002062C
		public decimal DecimalNumberReadSub(bool SignSkip = false, bool EndSpaceSkip = true)
		{
			this.SkipSpace();
			bool flag = true;
			if (!SignSkip)
			{
				if (this.CheckNextCharacter('+'))
				{
					flag = true;
				}
				if (this.CheckNextCharacter('-'))
				{
					flag = false;
				}
			}
			this.Index++;
			if (this.Index >= this.LineBuffer.Length)
			{
				return -32769m;
			}
			bool flag2 = false;
			string text = "";
			while (this.Index < this.LineBuffer.Length)
			{
				char c = this.LineBuffer[this.Index];
				if (c == '\f')
				{
					this.ExecJ6();
				}
				else if (c != '.' && (c < '0' || c > '9'))
				{
					this.Index--;
					if (!flag2)
					{
						return -32769 - (int)c;
					}
					break;
				}
				else
				{
					flag2 = true;
					ReadOnlySpan<char> readOnlySpan = text;
					char c2 = this.LineBuffer[this.Index];
					text = readOnlySpan + new ReadOnlySpan<char>(ref c2);
				}
				this.Index++;
			}
			if (EndSpaceSkip)
			{
				this.SkipSpace();
			}
			return decimal.Parse(flag ? text : ("-" + text));
		}

		// Token: 0x06000107 RID: 263 RVA: 0x0002254C File Offset: 0x0002074C
		public void StartTrack(bool SetAddress = true)
		{
			int num = ((this.SoundId == 0) ? this.TrackIndex : (this.TrackIndex - 1));
			this.BGMData[this.StartAddress + num * 2] = (SetAddress ? ((byte)((this.Address - this.StartAddress) & 255)) : 0);
			this.BGMData[this.StartAddress + num * 2 + 1] = (SetAddress ? ((byte)((this.Address - this.StartAddress) / 256)) : 0);
		}

		// Token: 0x06000108 RID: 264 RVA: 0x000225CC File Offset: 0x000207CC
		public void EndTrack(bool SetPlayLine = false)
		{
			if (this.TrackIndex < 4)
			{
				this.FrameAnalyzeCount[this.TrackIndex] = 0;
			}
			if (this.TrackIndex > 0 && this.SoundId < 1 && this.TrackIndex < 4)
			{
				if (this.LoopSlur[this.TrackIndex])
				{
					this.DataWrite(133);
				}
				else if (this.Slur[this.TrackIndex])
				{
					this.DataWrite(132);
				}
			}
			this.DataWrite(255);
			if (this.TrackIndex > 3 || this.SoundId > 0)
			{
				return;
			}
			if (SetPlayLine && this.GetTimeCompile)
			{
				this.LinePlayTime[this.TrackIndex].Add(100000.0);
				this.LinePlayLineNumber[this.TrackIndex].Add(100000);
			}
			if (this.TrackFrame[this.TrackIndex, this.SoundId] > 0)
			{
				this.DataWrite((int)((byte)(this.LoopAddress[this.TrackIndex] & 255)));
				this.DataWrite((int)((byte)(this.LoopAddress[this.TrackIndex] / 256)));
				return;
			}
			this.DataWrite(0);
			this.DataWrite(0);
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00022704 File Offset: 0x00020904
		public List<byte> SetNoteLength(bool AddInformationFrame = true, bool NotStackFraction = false)
		{
			decimal num = 0m;
			decimal num2 = 0m;
			bool flag = true;
			bool flag2 = false;
			bool flag3 = true;
			List<byte> list = new List<byte>();
			while (flag3)
			{
				bool flag4 = this.CheckFrameMode();
				decimal num3 = this.NumberRead(true, true);
				this.Index++;
				decimal num4 = 0m;
				decimal num5 = 0m;
				int num6 = 0;
				decimal num7;
				if (!flag4)
				{
					if (num3 <= -32769m)
					{
						num3 = this.DefaultLength[this.TrackIndex][this.NoteLengthIndex[this.TrackIndex]];
						this.NoteLengthIndex[this.TrackIndex]++;
						if (this.NoteLengthIndex[this.TrackIndex] > 15 || num3 < 0m)
						{
							num3 = this.DefaultLength[this.TrackIndex][0];
							this.NoteLengthIndex[this.TrackIndex] = 1;
						}
						string text = this.DefaultLengthAddString[this.TrackIndex][this.NoteLengthIndex[this.TrackIndex] - 1];
						if (text.Length > 0)
						{
							if (this.Index < this.LineBuffer.Length)
							{
								this.LineBuffer = this.LineBuffer.Insert(this.Index, text);
								if (this.CurrentSourceMap != null)
								{
									this.CurrentSourceMap.InsertRange(Math.Min(this.Index, this.CurrentSourceMap.Count), Enumerable.Repeat<MMLSourcePosition>(default(MMLSourcePosition), text.Length));
								}
							}
							else
							{
								this.LineBuffer += text;
								if (this.CurrentSourceMap != null)
								{
									this.CurrentSourceMap.AddRange(Enumerable.Repeat<MMLSourcePosition>(default(MMLSourcePosition), text.Length));
								}
								this.Index = this.LineBuffer.Length - text.Length;
							}
						}
					}
					else if (num3 < 1m)
					{
						this.Message = "0 以下の音長は指定できません。";
						if (this.Index >= this.LineBuffer.Length)
						{
							this.Index = this.LineBuffer.Length;
						}
						this.ErrorString = this.LineBuffer.Substring(this.Index - 2, this.Index - (this.Index - 2));
						return null;
					}
					num7 = ((num3 < 1000m) ? (14400.0m / this.Tempo[this.TrackIndex] / num3) : (num3 - 1000m)) + num6;
					if (num3 < 1000m)
					{
						num4 = 1m / num3;
						num5 = num4;
					}
					else
					{
						flag = false;
					}
				}
				else
				{
					if (num3 < 1m)
					{
						this.Message = ((num3 <= -32769m) ? "フレーム数が省略されています。" : "0 フレームの音長は指定できません。");
						if (this.Index >= this.LineBuffer.Length)
						{
							this.Index = this.LineBuffer.Length;
						}
						this.ErrorString = this.LineBuffer.Substring(this.Index - 2, this.Index - (this.Index - 2));
						return null;
					}
					num7 = num3;
					flag = false;
				}
				decimal num8 = num7;
				while (this.Index < this.LineBuffer.Length && this.LineBuffer[this.Index] == '.')
				{
					num7 /= 2m;
					num8 += num7;
					if (flag)
					{
						num4 /= 2m;
						num5 += num4;
					}
					this.SkipSpace();
					this.Index++;
				}
				if (flag2)
				{
					num -= num8;
					if (flag)
					{
						num2 -= num5;
					}
					if (num < 1m)
					{
						this.Message = "音長が 0 フレーム以下になりました。";
						this.ErrorString = "-";
						this.Index--;
						return null;
					}
				}
				else
				{
					num += num8;
					if (flag)
					{
						num2 += num5;
					}
				}
				if (this.Index < this.LineBuffer.Length)
				{
					char c = this.LineBuffer[this.Index];
					if (c != '+')
					{
						if (c == '-')
						{
							flag2 = true;
							flag3 = true;
							continue;
						}
						if (c != '^')
						{
							flag3 = false;
							continue;
						}
					}
					flag2 = false;
					flag3 = true;
				}
				else
				{
					flag3 = false;
				}
			}
			this.Index--;
			if (!NotStackFraction)
			{
				this.FrameMod[this.TrackIndex] += num - (int)num;
				while (this.FrameMod[this.TrackIndex] >= 0.9m)
				{
					decimal[] frameMod = this.FrameMod;
					int trackIndex = this.TrackIndex;
					frameMod[trackIndex] -= 1m;
					num += 1m;
				}
				if (AddInformationFrame && this.RepeatFrameModPending[this.TrackIndex])
				{
					this.RepeatFrameModPending[this.TrackIndex] = false;
				}
			}
			int i;
			for (i = (int)num; i >= 255; i -= 255)
			{
				list.Add(byte.MaxValue);
				if (AddInformationFrame)
				{
					this.AddTrackFrame(255);
				}
			}
			list.Add((byte)i);
			if (AddInformationFrame)
			{
				this.AddTrackFrame(i);
			}
			if (i == 0)
			{
				this.Status = "0 フレームの音符を検出しました。";
			}
			if (i < 0)
			{
				this.Message = "音長が 0 フレーム未満になりました。";
				this.ErrorString = "（" + i.ToString() + "フレーム）";
			}
			this.LastNoteLengthText[this.TrackIndex] = (flag ? Core.FormatMusicalLength(num2) : "");
			return list;
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00022D00 File Offset: 0x00020F00
		private static string FormatMusicalLength(decimal MusicalLength)
		{
			if (MusicalLength <= 0m)
			{
				return "";
			}
			Dictionary<decimal, string> dictionary = Core.MusicalLengthTextCache;
			lock (dictionary)
			{
				string text;
				if (Core.MusicalLengthTextCache.TryGetValue(MusicalLength, out text))
				{
					return text;
				}
			}
			for (int i = 0; i <= 3; i++)
			{
				for (int j = 1; j <= 999; j++)
				{
					decimal num = 1m / j;
					decimal num2 = num;
					for (int k = 0; k < i; k++)
					{
						num2 /= 2m;
						num += num2;
					}
					if (Math.Abs(num - MusicalLength) <= 0.0000001m)
					{
						string text2 = j.ToString() + new string('.', i);
						dictionary = Core.MusicalLengthTextCache;
						lock (dictionary)
						{
							Core.MusicalLengthTextCache[MusicalLength] = text2;
						}
						return text2;
					}
				}
			}
			dictionary = Core.MusicalLengthTextCache;
			lock (dictionary)
			{
				Core.MusicalLengthTextCache[MusicalLength] = "";
			}
			return "";
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00022E80 File Offset: 0x00021080
		public void AddTrackFrame(int Frame)
		{
			if (this.GetTimeCompile && !this.AlignPlanReady && this.SoundId == 0 && this.TrackIndex >= 0 && this.TrackIndex < this.AlignSegmentFrame.Length)
			{
				this.AlignSegmentFrame[this.TrackIndex] += Frame;
			}
			int num = ((this.RepeatMode[this.TrackIndex] != 0) ? 1 : 0);
			int num2 = this.RepeatNest[this.TrackIndex, num];
			if (num2 == 0)
			{
				this.TrackFrame[this.TrackIndex, this.SoundId] += Frame;
				return;
			}
			num2--;
			if (this.RepeatEscapeOctave[this.TrackIndex, num2] > 0)
			{
				this.RepeatFrame2[this.TrackIndex, num2, num] += Frame;
				return;
			}
			this.RepeatFrame1[this.TrackIndex, num2, num] += Frame;
		}

		// Token: 0x0600010C RID: 268 RVA: 0x00022F64 File Offset: 0x00021164
		public bool CheckFrameMode()
		{
			if (this.Index + 1 < this.LineBuffer.Length && (this.LineBuffer[this.Index + 1] == '%' || (this.SharpFrameMode && this.LineBuffer[this.Index + 1] == '#')))
			{
				this.Index++;
				return true;
			}
			return false;
		}

		// Token: 0x0600010D RID: 269 RVA: 0x00022FCD File Offset: 0x000211CD
		public void ExecJ6()
		{
			this.UseJump = true;
			this.SetJumpNumber[this.TrackIndex, 6] = true;
			this.JumpStartTime[6] = (double)this.GetCurrentPlaybackFrame() / 59.94080511738504;
		}

		// Token: 0x0600010E RID: 270 RVA: 0x00023004 File Offset: 0x00021204
		public int GetCurrentPlaybackFrame()
		{
			int num = this.GetFrame();
			int num2 = ((this.RepeatMode[this.TrackIndex] != 0) ? 1 : 0);
			if (num2 == 1)
			{
				for (int i = 0; i < this.RepeatNest[this.TrackIndex, num2]; i++)
				{
					num += this.RepeatFrame1[this.TrackIndex, i, num2] + this.RepeatFrame2[this.TrackIndex, i, num2];
				}
			}
			return num;
		}

		// Token: 0x0600010F RID: 271 RVA: 0x00023078 File Offset: 0x00021278
		public void BeginAlignPass()
		{
			this.AlignOccurrenceCounter.Clear();
			if (this.GetTimeCompile && !this.AlignPlanReady)
			{
				this.AlignPoints.Clear();
				List<string>[] alignTrackEventOrder = this.AlignTrackEventOrder;
				for (int i = 0; i < alignTrackEventOrder.Length; i++)
				{
					alignTrackEventOrder[i].Clear();
				}
				Array.Clear(this.AlignSegmentFrame, 0, this.AlignSegmentFrame.Length);
			}
		}

		// Token: 0x06000110 RID: 272 RVA: 0x000230DC File Offset: 0x000212DC
		private int GetAlignOccurrence(string SourceKey)
		{
			int[] array;
			if (!this.AlignOccurrenceCounter.TryGetValue(SourceKey, out array))
			{
				array = new int[4];
				this.AlignOccurrenceCounter[SourceKey] = array;
			}
			int[] array2 = array;
			int trackIndex = this.TrackIndex;
			int num = array2[trackIndex];
			array2[trackIndex] = num + 1;
			return num;
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00023124 File Offset: 0x00021324
		public void ResetAlignFrameMod()
		{
			this.FrameMod[this.TrackIndex] = 0m;
			for (int i = 0; i < this.RepeatFrameMod.GetLength(1); i++)
			{
				this.RepeatFrameMod[this.TrackIndex, i] = 0m;
				this.RepeatEscapeFrameMod[this.TrackIndex, i] = 0m;
			}
		}

		// Token: 0x06000112 RID: 274 RVA: 0x00023190 File Offset: 0x00021390
		public void RegisterAlignPoint(string SourceKey)
		{
			int alignOccurrence = this.GetAlignOccurrence(SourceKey);
			string text = SourceKey + "#" + alignOccurrence.ToString();
			AlignPointData alignPointData;
			if (!this.AlignPoints.TryGetValue(text, out alignPointData))
			{
				alignPointData = new AlignPointData
				{
					SourceKey = SourceKey,
					Occurrence = alignOccurrence
				};
				this.AlignPoints[text] = alignPointData;
			}
			alignPointData.Frame[this.TrackIndex] = this.GetCurrentPlaybackFrame();
			alignPointData.SegmentFrame[this.TrackIndex] = this.AlignSegmentFrame[this.TrackIndex];
			this.AlignSegmentFrame[this.TrackIndex] = 0;
			this.ResetAlignFrameMod();
			alignPointData.RepeatMode[this.TrackIndex] = this.RepeatMode[this.TrackIndex];
			int num = ((this.RepeatMode[this.TrackIndex] != 0) ? 1 : 0);
			alignPointData.RepeatDepth[this.TrackIndex] = this.RepeatNest[this.TrackIndex, num];
			this.AlignTrackEventOrder[this.TrackIndex].Add(text);
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00023288 File Offset: 0x00021488
		private string GetPreviousAlignEventKey(int Track, string EventKey)
		{
			string text = null;
			foreach (string text2 in this.AlignTrackEventOrder[Track])
			{
				if (text2 == EventKey)
				{
					return text;
				}
				text = text2;
			}
			return null;
		}

		// Token: 0x06000114 RID: 276 RVA: 0x000232EC File Offset: 0x000214EC
		public bool FinalizeAlignPoints()
		{
			if (this.AlignPoints.Count == 0)
			{
				this.AlignPlanReady = false;
				return true;
			}
			foreach (IGrouping<string, AlignPointData> grouping in from Point in this.AlignPoints.Values
				group Point by Point.SourceKey)
			{
				bool[] array = new bool[4];
				foreach (AlignPointData alignPointData in grouping)
				{
					for (int i = 0; i < 4; i++)
					{
						if (alignPointData.Frame[i] >= 0)
						{
							array[i] = true;
						}
					}
				}
				foreach (AlignPointData alignPointData2 in grouping)
				{
					for (int j = 0; j < 4; j++)
					{
						if (array[j] && alignPointData2.Frame[j] < 0)
						{
							this.Message = "!A コマンドを含むリピートの実行回数がトラック間で一致していません。";
							this.ErrorString = "!A";
							return false;
						}
					}
				}
			}
			foreach (AlignPointData alignPointData3 in this.AlignPoints.Values)
			{
				alignPointData3.Resolved = false;
				Array.Clear(alignPointData3.WaitFrame, 0, alignPointData3.WaitFrame.Length);
				int num = -1;
				int num2 = -1;
				for (int k = 0; k < 4; k++)
				{
					if (alignPointData3.Frame[k] >= 0)
					{
						if (num < 0)
						{
							num = alignPointData3.RepeatMode[k];
							num2 = alignPointData3.RepeatDepth[k];
						}
						else if (num != alignPointData3.RepeatMode[k] || num2 != alignPointData3.RepeatDepth[k])
						{
							this.Message = "!A コマンドでは、対象トラックのリピートモードとリピート深度を揃えてください。";
							this.ErrorString = "!A";
							return false;
						}
					}
				}
			}
			foreach (AlignPointData alignPointData4 in this.AlignPoints.Values)
			{
				int num3 = 0;
				for (int l = 0; l < 4; l++)
				{
					if (alignPointData4.Frame[l] >= 0)
					{
						num3 = Math.Max(num3, alignPointData4.SegmentFrame[l]);
					}
				}
				for (int m = 0; m < 4; m++)
				{
					if (alignPointData4.Frame[m] >= 0)
					{
						alignPointData4.WaitFrame[m] = num3 - alignPointData4.SegmentFrame[m];
					}
				}
				alignPointData4.Resolved = true;
			}
			foreach (IGrouping<string, AlignPointData> grouping2 in from Point in this.AlignPoints.Values
				group Point by Point.SourceKey)
			{
				AlignPointData alignPointData5 = grouping2.OrderBy<AlignPointData, int>((AlignPointData Point) => Point.Occurrence).FirstOrDefault<AlignPointData>();
				if (alignPointData5 != null)
				{
					int num4 = -1;
					for (int n = 0; n < 4; n++)
					{
						if (alignPointData5.Frame[n] >= 0)
						{
							num4 = alignPointData5.RepeatMode[n];
							break;
						}
					}
					if (num4 > 0 && num4 != 4)
					{
						foreach (AlignPointData alignPointData6 in grouping2)
						{
							if (alignPointData6 != alignPointData5)
							{
								List<string> list = new List<string>();
								for (int num5 = 0; num5 < 4; num5++)
								{
									if (alignPointData5.Frame[num5] >= 0 && alignPointData6.WaitFrame[num5] != alignPointData5.WaitFrame[num5])
									{
										list.Add(this.TrackCode[num5]);
									}
								}
								if (list.Count > 0)
								{
									this.Message = "各トラックの !A に到達したときの長さがリピート回ごとに異なっています。\n各リピート回の長さを揃えるか、#REPEAT-MODE 0 を使用してください。\n\nトラック " + string.Join(", ", list) + " の確認が必要です。";
									this.ErrorString = "!A";
									return false;
								}
							}
						}
					}
				}
			}
			this.AlignPlanReady = true;
			return true;
		}

		// Token: 0x06000115 RID: 277 RVA: 0x000237D8 File Offset: 0x000219D8
		public int GetAlignWaitFrame(string SourceKey)
		{
			string text = SourceKey + "#" + this.GetAlignOccurrence(SourceKey).ToString();
			AlignPointData alignPointData;
			if (!this.AlignPoints.TryGetValue(text, out alignPointData))
			{
				this.AlignPoints.TryGetValue(SourceKey + "#0", out alignPointData);
			}
			if (alignPointData == null || alignPointData.Frame[this.TrackIndex] < 0)
			{
				this.Message = "!A コマンドの同期情報を取得できませんでした。";
				this.ErrorString = "!A";
				return 0;
			}
			return alignPointData.WaitFrame[this.TrackIndex];
		}

		// Token: 0x06000116 RID: 278 RVA: 0x00023861 File Offset: 0x00021A61
		public void ResetInterruptTrackPoints()
		{
			this.InterruptTrackPoints.Clear();
			this.InterruptTrackPointSequence = 0;
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00023878 File Offset: 0x00021A78
		public void AddInterruptTrackPoint(int InterruptTrack)
		{
			if (!this.GetTimeCompile || this.SoundId != 0)
			{
				return;
			}
			List<InterruptTrackPointData> interruptTrackPoints = this.InterruptTrackPoints;
			InterruptTrackPointData interruptTrackPointData = new InterruptTrackPointData();
			interruptTrackPointData.Frame = this.GetFrame();
			interruptTrackPointData.LineNumber = this.LineNumber;
			interruptTrackPointData.TrackIndex = this.TrackIndex;
			interruptTrackPointData.InterruptTrack = InterruptTrack;
			int interruptTrackPointSequence = this.InterruptTrackPointSequence;
			this.InterruptTrackPointSequence = interruptTrackPointSequence + 1;
			interruptTrackPointData.Sequence = interruptTrackPointSequence;
			interruptTrackPoints.Add(interruptTrackPointData);
		}

		// Token: 0x06000118 RID: 280 RVA: 0x000238E8 File Offset: 0x00021AE8
		public int GetInterruptTrackAtFrame(int Frame)
		{
			int num = 3;
			foreach (InterruptTrackPointData interruptTrackPointData in from Point in this.InterruptTrackPoints
				orderby Point.Frame, Point.LineNumber, Point.TrackIndex, Point.Sequence
				select Point)
			{
				if (interruptTrackPointData.Frame > Frame)
				{
					break;
				}
				num = interruptTrackPointData.InterruptTrack;
			}
			return num;
		}

		// Token: 0x06000119 RID: 281 RVA: 0x000239D4 File Offset: 0x00021BD4
		public void ApplyInterruptTrackPointsToNotePlayParameters()
		{
			foreach (List<NotePlayParameter> list in this.NotePlayParameters)
			{
				foreach (NotePlayParameter notePlayParameter in list)
				{
					notePlayParameter.InterruptTrack = this.GetInterruptTrackAtFrame(notePlayParameter.CurrentFrame);
				}
			}
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00023A68 File Offset: 0x00021C68
		public void ResetTimeCompileOutput()
		{
			this.SlowEvents.Clear();
			this.SlowMusicToRealFrame = null;
			foreach (List<double> list in this.LinePlayTime)
			{
				list.Clear();
			}
			foreach (List<int> list2 in this.LinePlayLineNumber)
			{
				list2.Clear();
			}
			foreach (List<double> list3 in this.NotePlayTime)
			{
				list3.Clear();
			}
			foreach (List<int> list4 in this.NotePlayLineNumber)
			{
				list4.Clear();
			}
			foreach (List<int> list5 in this.NotePlayColumn)
			{
				list5.Clear();
			}
			foreach (List<NotePlayParameter> list6 in this.NotePlayParameters)
			{
				list6.Clear();
			}
			this.PianoRollNotes.Clear();
			this.PianoRollBars.Clear();
			this.SelectionPlayCurrentNoteTarget = true;
			this.SelectionPlayCurrentNoteLine = -1;
			this.SelectionPlayCurrentNoteColumn = -1;
			this.SelectionPlayLastNoteLine = new int[] { -1, -1, -1, -1 };
			this.SelectionPlayLastNoteColumn = new int[] { -1, -1, -1, -1 };
			this.SelectionPlayCarryNoteLine = new int[] { -1, -1, -1, -1 };
			this.SelectionPlayCarryNoteColumn = new int[] { -1, -1, -1, -1 };
			this.SelectionPlayFirstTargetNoteFound = new bool[4];
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00023C98 File Offset: 0x00021E98
		public void ErrorParameter(string String, int Number)
		{
			this.ErrorString = String;
			if (Number > -32769)
			{
				this.ErrorString += Number.ToString();
			}
		}

		// Token: 0x0600011C RID: 284 RVA: 0x00023CC4 File Offset: 0x00021EC4
		public void ErrorParameter(string String, int Number1, int Number2)
		{
			this.ErrorString = String;
			if (Number1 > -32769)
			{
				this.ErrorString += Number1.ToString();
			}
			this.ErrorString += ",";
			if (Number2 > -32769)
			{
				this.ErrorString += Number2.ToString();
			}
		}

		// Token: 0x0600011D RID: 285 RVA: 0x00023D2E File Offset: 0x00021F2E
		public void ErrorParameter(string String, decimal Number)
		{
			this.ErrorString = String;
			if (Number > -32769m)
			{
				this.ErrorString += Number.ToString();
			}
		}

		// Token: 0x0600011E RID: 286 RVA: 0x00023D64 File Offset: 0x00021F64
		public char Read1(int ReadIndex = -1)
		{
			if (ReadIndex < 0)
			{
				ReadIndex = this.Index;
			}
			if (ReadIndex < this.LineBuffer.Length)
			{
				if (this.LineBuffer[ReadIndex] == '\f')
				{
					this.ExecJ6();
					this.Index++;
					ReadIndex++;
					if (ReadIndex >= this.LineBuffer.Length)
					{
						return '\0';
					}
				}
				return this.LineBuffer[ReadIndex];
			}
			return '\0';
		}

		// Token: 0x0600011F RID: 287 RVA: 0x00023DD2 File Offset: 0x00021FD2
		public void ResetAutoRepeatBoundaries()
		{
			if (this.AutoRepeatBoundaries == null)
			{
				this.AutoRepeatBoundaries = new List<AutoRepeatBoundaryData>();
				return;
			}
			this.AutoRepeatBoundaries.Clear();
		}

		// Token: 0x06000120 RID: 288 RVA: 0x00023DF4 File Offset: 0x00021FF4
		public void MarkAutoRepeatBoundary()
		{
			if (this.PreCompile || this.GetTimeCompile || this.InfoCompile || this.SoundId != 0)
			{
				return;
			}
			if (this.AutoRepeatBoundaries == null)
			{
				this.AutoRepeatBoundaries = new List<AutoRepeatBoundaryData>();
			}
			this.AutoRepeatBoundaries.Add(new AutoRepeatBoundaryData(this.Address, this.RepeatMode[this.TrackIndex]));
		}

		// Token: 0x06000121 RID: 289 RVA: 0x00023E58 File Offset: 0x00022058
		public void DataWrite(int Data)
		{
			if (this.PreCompile || this.InfoCompile)
			{
				return;
			}
			if (this.Address >= this.BGMDataSize)
			{
				this.Message = "曲データの容量が " + this.BGMDataSize.ToString() + " バイト以上になりました。\nコンパイルを中止します。";
				this.ErrorString = null;
				return;
			}
			this.BGMData[this.Address] = (byte)Data;
			this.Address++;
		}

		// Token: 0x06000122 RID: 290 RVA: 0x00023ECC File Offset: 0x000220CC
		public void RangeNotice(string Name, int Min, int Max, string AdditionalText = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0} は {1} ～ {2} で指定してください。", Name, Min, Max);
			if (AdditionalText != null)
			{
				stringBuilder.Append("\n");
				stringBuilder.Append(AdditionalText);
			}
			this.Message = stringBuilder.ToString();
		}

		// Token: 0x06000123 RID: 291 RVA: 0x00023F1D File Offset: 0x0002211D
		public void SetRandomSeed(int Seed)
		{
			if (Seed > 0)
			{
				this.RandomParam = new Random(Seed);
				return;
			}
			this.RandomParam = new Random(this.DefaultSeed);
		}

		// Token: 0x06000124 RID: 292 RVA: 0x00023F44 File Offset: 0x00022144
		public void RegisterMacro(string Line, int LineNumber, int ColumnOffset = 0)
		{
			int i = Line.IndexOf('=');
			if (i < 0)
			{
				this.Message = "マクロに定義する MML がありません。";
				this.ErrorString = "$";
				LineNumber = (this.ErrorLine = LineNumber + 1);
				return;
			}
			int num = i;
			string text = Line.Substring(0, i);
			string text2 = Line.Substring(i + 1);
			text = text.Substring(1).Replace("\t", "").Replace(" ", "");
			if (this.MacroMML.ContainsKey(text))
			{
				this.Message = "マクロ名 「 " + text + " 」 は既に定義されています。";
				this.ErrorString = "$" + text + "=";
				LineNumber = (this.ErrorLine = LineNumber + 1);
				return;
			}
			foreach (char c in text)
			{
				if (c < '0' || (c > '9' && c < 'A') || c > 'Z')
				{
					ReadOnlySpan<char> readOnlySpan = "マクロ名に 「 ";
					char c2 = c;
					this.Message = readOnlySpan + new ReadOnlySpan<char>(ref c2) + " 」 は使用できません。";
					this.ErrorString = "$" + text + "=";
					LineNumber = (this.ErrorLine = LineNumber + 1);
					return;
				}
			}
			if (text.Length == 0)
			{
				this.Message = "マクロ名がありません。";
				this.ErrorString = "$";
				LineNumber = (this.ErrorLine = LineNumber + 1);
				return;
			}
			string text3 = "$" + text;
			int num2 = text2.IndexOf(text3);
			if (num2 >= 0)
			{
				num2 += text3.Length;
				if (num2 >= text2.Length)
				{
					this.Message = "同じマクロ自身を呼び出しています。";
					if (text.Length == text2.Length - 1)
					{
						this.ErrorString = text3 + " =  " + text3;
					}
					else
					{
						this.ErrorString = text3 + " = ... " + text3;
					}
					LineNumber = (this.ErrorLine = LineNumber + 1);
					return;
				}
				char c3 = text2[num2];
				if (c3 < '0' || (c3 > '9' && c3 < 'A') || c3 > 'Z')
				{
					this.Message = "同じマクロ自身の呼び出しはできません。";
					this.ErrorString = text3 + " = " + text3 + "...";
					LineNumber = (this.ErrorLine = LineNumber + 1);
					return;
				}
			}
			int num3 = 0;
			while (num3 < text2.Length && char.IsWhiteSpace(text2[num3]))
			{
				num3++;
			}
			string text4 = text2.Trim().Replace(':', ' ').Replace('\t', ' ')
				.Replace('|', ' ');
			this.MacroMML.Add(text, text4);
			this.MacroLine.Add(text, LineNumber + 1);
			List<MMLSourcePosition> list = new List<MMLSourcePosition>(text4.Length);
			int num4 = ColumnOffset + num + 1 + num3;
			for (int j = 0; j < text4.Length; j++)
			{
				int num5 = num3 + j;
				char c4 = ((num5 < text2.Length) ? text2[num5] : '\0');
				list.Add(new MMLSourcePosition(LineNumber + 1, num4 + j, c4, false, false));
			}
			this.MacroSourceMap.Add(text, list);
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00024260 File Offset: 0x00022460
		public void RegisterStepMacro(string Line, int LineNumber)
		{
			int i = Line.IndexOf('=');
			if (i < 0)
			{
				this.Message = "ステップマクロに定義する MML がありません。";
				this.ErrorString = "?";
				LineNumber = (this.ErrorLine = LineNumber + 1);
				return;
			}
			string text = Line.Substring(0, i);
			string text2 = Line.Substring(i + 1);
			text = text.Substring(1).Replace("\t", "").Replace(" ", "");
			if (this.StepMacroMML.ContainsKey(text))
			{
				this.Message = "ステップマクロ名 「 " + text + " 」 は既に定義されています。";
				this.ErrorString = "?" + text + "=";
				LineNumber = (this.ErrorLine = LineNumber + 1);
				return;
			}
			foreach (char c in text)
			{
				if (c < '0' || (c > '9' && c < 'A') || c > 'Z')
				{
					ReadOnlySpan<char> readOnlySpan = "ステップマクロ名に 「 ";
					char c2 = c;
					this.Message = readOnlySpan + new ReadOnlySpan<char>(ref c2) + " 」 は使用できません。";
					this.ErrorString = "?" + text + "=";
					LineNumber = (this.ErrorLine = LineNumber + 1);
					return;
				}
			}
			if (text.Length == 0)
			{
				this.Message = "ステップマクロ名がありません。";
				this.ErrorString = "?";
				LineNumber = (this.ErrorLine = LineNumber + 1);
				return;
			}
			List<string> list = new List<string>();
			string[] array = text2.Split(new char[] { '/', '|' });
			for (int j = 0; j < array.Length; j++)
			{
				string text3 = array[j].Trim().Replace(':', ' ').Replace('\t', ' ');
				list.Add(text3);
			}
			this.StepMacroMML.Add(text, list);
		}

		// Token: 0x06000126 RID: 294 RVA: 0x0002442C File Offset: 0x0002262C
		public int[] SetKeySignSub()
		{
			int[] array = new int[13];
			this.SkipSpace();
			int num;
			if (this.CheckNextCharacter('-'))
			{
				num = -1;
			}
			else if (this.CheckNextCharacter('+'))
			{
				num = 1;
			}
			else if (this.CheckNextCharacter('#'))
			{
				num = 1;
			}
			else
			{
				if (this.Index >= this.LineBuffer.Length - 1)
				{
					return array;
				}
				this.Index--;
				if (this.CheckNextCharacter(' '))
				{
					return array;
				}
				if (this.CheckNextCharacter(':'))
				{
					return array;
				}
				if (this.CheckNextCharacter('|'))
				{
					return array;
				}
				if (this.CheckNextCharacter('\t'))
				{
					return array;
				}
				this.Message = "調号指定の記号が正しくありません。\n「 + 」「 - 」「 # 」のいずれかをご使用ください。\nまた、空白や区切り記号だけを入れるとすべての指定を解除します。";
				return array;
			}
			for (;;)
			{
				this.Index++;
				if (this.Index >= this.LineBuffer.Length)
				{
					break;
				}
				char c = this.Read1(-1);
				int num2 = " C D EF G A B".IndexOf(c);
				if (num2 < 0 || c == ' ')
				{
					break;
				}
				array[num2] = num;
			}
			return array;
		}

		// Token: 0x06000127 RID: 295 RVA: 0x0002451C File Offset: 0x0002271C
		public int GetFrame()
		{
			if (this.BeforeLoopFrame[this.TrackIndex, this.SoundId] > 0)
			{
				return this.BeforeLoopFrame[this.TrackIndex, this.SoundId] + this.TrackFrame[this.TrackIndex, this.SoundId];
			}
			return this.TrackFrame[this.TrackIndex, this.SoundId];
		}

		// Token: 0x06000128 RID: 296 RVA: 0x0002458C File Offset: 0x0002278C
		private int SumLengthData(List<byte> LengthData)
		{
			int num = 0;
			for (int i = 0; i < LengthData.Count; i++)
			{
				num += (int)LengthData[i];
			}
			return num;
		}

		// Token: 0x06000129 RID: 297 RVA: 0x000245B8 File Offset: 0x000227B8
		public bool IsSelectionPlayPosition(int Line, int Column)
		{
			return !this.SelectionPlay || (Line >= this.SelectionPlayStartLine && Line <= this.SelectionPlayEndLine && (Line != this.SelectionPlayStartLine || Column >= this.SelectionPlayStartColumn) && (Line != this.SelectionPlayEndLine || Column < this.SelectionPlayEndColumn));
		}

		// Token: 0x0600012A RID: 298 RVA: 0x0002460C File Offset: 0x0002280C
		public bool IsCurrentNoteMuted()
		{
			return this.Mute[this.TrackIndex] || (this.SelectionPlay && !this.SelectionPlayCurrentNoteTarget && (this.SelectionPlayCurrentNoteLine != this.SelectionPlayCarryNoteLine[this.TrackIndex] || this.SelectionPlayCurrentNoteColumn != this.SelectionPlayCarryNoteColumn[this.TrackIndex]));
		}

		// Token: 0x0600012B RID: 299 RVA: 0x0002466C File Offset: 0x0002286C
		public void AddPianoRollNote(int NoteNumber, List<byte> LengthData, int Length = 0, int Offset = 0, int StartNote = 0)
		{
			if (this.SelectionPlay && !this.SelectionPlayCurrentNoteTarget)
			{
				return;
			}
			if (this.GetTimeCompile)
			{
				int num = ((Length > 0) ? Length : this.SumLengthData(LengthData));
				PianoRollNote pianoRollNote = new PianoRollNote();
				pianoRollNote.Track = this.TrackIndex;
				pianoRollNote.NoteNumber = NoteNumber;
				pianoRollNote.StartNote = StartNote;
				double num2 = (double)num / 8.0 * (double)this.Quantize[this.TrackIndex];
				if (this.AtQuantize[this.TrackIndex] > 0 && this.AtQuantize[this.TrackIndex] < 129)
				{
					num2 -= (double)this.AtQuantize[this.TrackIndex];
					if (num2 < 1.0)
					{
						num2 = 1.0;
					}
				}
				if (this.AtQuantize[this.TrackIndex] > 128 && this.AtQuantize[this.TrackIndex] < 256 && this.Quantize[this.TrackIndex] < 8)
				{
					num2 += (double)(this.AtQuantize[this.TrackIndex] - 128);
					if (num2 > (double)num)
					{
						num2 = (double)num;
					}
				}
				if (this.AtQuantize[this.TrackIndex] > 255)
				{
					num2 = (double)Math.Min(this.AtQuantize[this.TrackIndex] - 257, num);
				}
				pianoRollNote.GateTime = ((Length > 0) ? ((double)Length) : num2);
				pianoRollNote.StartPoint = (double)(this.GetFrame() - num + Offset);
				this.PianoRollNotes.Add(pianoRollNote);
			}
		}

		// Token: 0x0600012C RID: 300 RVA: 0x000247DF File Offset: 0x000229DF
		public void AddInfoText(string Text)
		{
			if (this.GetTimeCompile)
			{
				if (this.LineNumber > 0)
				{
					Text = "[" + this.LineNumber.ToString() + "行目] " + Text;
				}
				this.InfoText.Add(Text);
			}
		}

		// Token: 0x0600012D RID: 301 RVA: 0x0002481C File Offset: 0x00022A1C
		public void RegisterHeader(string Line, int LineNumber)
		{
			char[] array = new char[] { ' ', '\t', '=' };
			string[] array2 = Line.Split(array);
			array2[0] = array2[0].Replace("\t", "").Replace(" ", "").Replace("_", "-")
				.ToUpper();
			this.LineBuffer = Line.Substring(array2[0].Length).TrimEnd();
			this.Index = 0;
			string text = array2[0] + " ";
			LineNumber = (this.ErrorLine = LineNumber + 1);
			string text2 = array2[0];
			if (text2 != null)
			{
				bool[] array4;
				switch (text2.Length)
				{
				case 3:
					if (!(text2 == "#FF"))
					{
						goto IL_1AAF;
					}
					goto IL_12F9;
				case 4:
				case 16:
				case 18:
				case 19:
					goto IL_1AAF;
				case 5:
				{
					char c = text2[3];
					if (c <= 'L')
					{
						if (c != 'E')
						{
							if (c != 'L')
							{
								goto IL_1AAF;
							}
							if (!(text2 == "#SOLO"))
							{
								goto IL_1AAF;
							}
							this.TrackEnable = new bool[4];
							this.Mute = new bool[] { true, true, true, true };
							char c2;
							for (;;)
							{
								c2 = this.Read1(-1);
								if (c2 == '\0')
								{
									return;
								}
								this.Index++;
								if (c2 != ' ' && c2 != '\t' && c2 != ',')
								{
									int num = Array.IndexOf<string>(this.TrackCode, c2.ToString().ToUpper());
									if (num == -1)
									{
										break;
									}
									this.TrackEnable[num] = true;
									this.Mute[num] = false;
								}
							}
							this.Message = "#SOLO で指定されたトラック名が正しくありません。";
							this.ErrorString = c2.ToString();
							return;
						}
						else
						{
							if (!(text2 == "#SEED"))
							{
								goto IL_1AAF;
							}
							goto IL_13E6;
						}
					}
					else if (c != 'M')
					{
						if (c != 'T')
						{
							goto IL_1AAF;
						}
						if (!(text2 == "#MUTE"))
						{
							goto IL_1AAF;
						}
						this.Mute = new bool[4];
						char c3;
						for (;;)
						{
							c3 = this.Read1(-1);
							if (c3 == '\0')
							{
								return;
							}
							this.Index++;
							if (c3 != ' ' && c3 != '\t' && c3 != ',')
							{
								int num2 = Array.IndexOf<string>(this.TrackCode, c3.ToString().ToUpper());
								if (num2 == -1)
								{
									break;
								}
								this.TrackEnable[num2] = false;
								this.Mute[num2] = true;
							}
						}
						this.Message = "#MUTE で指定されたトラック名が正しくありません。";
						this.ErrorString = c3.ToString();
						return;
					}
					else
					{
						if (!(text2 == "#MEMO"))
						{
							goto IL_1AAF;
						}
						this.MetaData[4] = this.ReadText(array2[0], this.LineBuffer);
						return;
					}
					break;
				}
				case 6:
				{
					char c = text2[5];
					if (c <= 'F')
					{
						if (c != 'E')
						{
							if (c != 'F')
							{
								goto IL_1AAF;
							}
							if (!(text2 == "#SKIPF"))
							{
								goto IL_1AAF;
							}
							goto IL_16F0;
						}
						else
						{
							if (!(text2 == "#TITLE"))
							{
								goto IL_1AAF;
							}
							this.MetaData[0] = this.ReadText(array2[0], this.LineBuffer);
							return;
						}
					}
					else if (c != 'O')
					{
						if (c != 'R')
						{
							goto IL_1AAF;
						}
						if (!(text2 == "#METER"))
						{
							goto IL_1AAF;
						}
						string text3 = this.LineBuffer.Trim().TrimStart(new char[] { '=', ' ', '\t' }).Replace(" ", "")
							.Replace("\t", "");
						if (string.IsNullOrEmpty(text3) || string.Equals(text3, "OFF", StringComparison.OrdinalIgnoreCase) || string.Equals(text3, "ON", StringComparison.OrdinalIgnoreCase))
						{
							return;
						}
						string[] array3 = text3.Split('/', StringSplitOptions.None);
						int num3;
						int num4;
						if (array3.Length != 2 || !int.TryParse(array3[0], out num3) || !int.TryParse(array3[1], out num4) || num3 < 1 || num3 > 32 || num4 < 1 || num4 > 64 || (num4 & (num4 - 1)) != 0)
						{
							this.Message = "拍子は #METER、#METER OFF、#METER ON、または #METER 1/1 ～ 32/64 の範囲で、分母を 1, 2, 4, 8, 16, 32, 64 のいずれかで指定してください。";
							this.ErrorString = text + text3;
							return;
						}
						return;
					}
					else
					{
						if (!(text2 == "#TEMPO"))
						{
							goto IL_1AAF;
						}
						array4 = this.SetTimebase;
						for (int i = 0; i < array4.Length; i++)
						{
							if (array4[i])
							{
								this.Message = "既にタイムベース指定によってテンポが設定されています。\nどちらか片方で設定してください。\n\nタイムベースは 4 分音符のフレーム数で、設定すると自動的にテンポも決定されます。";
								this.ErrorString = text;
								break;
							}
						}
						decimal num5 = this.DecimalNumberRead(false, true);
						if (num5 < 1m || num5 > 999m)
						{
							this.RangeNotice("テンポ", 1, 999, null);
							this.ErrorParameter(text, num5);
							return;
						}
						this.HeaderTempo = num5;
						this.SetTempo = new bool[] { true, true, true, true };
						return;
					}
					break;
				}
				case 7:
				{
					char c = text2[5];
					if (c != '-')
					{
						if (c != 'S')
						{
							if (c != '_')
							{
								goto IL_1AAF;
							}
							if (!(text2 == "#SKIP_F"))
							{
								goto IL_1AAF;
							}
							goto IL_16F0;
						}
						else
						{
							if (!(text2 == "#SKIPSE"))
							{
								goto IL_1AAF;
							}
							goto IL_16F8;
						}
					}
					else
					{
						if (!(text2 == "#SKIP-F"))
						{
							goto IL_1AAF;
						}
						goto IL_16F0;
					}
					break;
				}
				case 8:
				{
					char c = text2[5];
					if (c <= 'I')
					{
						if (c != '-')
						{
							if (c != 'I')
							{
								goto IL_1AAF;
							}
							if (!(text2 == "#KEYSIGN"))
							{
								goto IL_1AAF;
							}
							goto IL_1427;
						}
						else
						{
							if (!(text2 == "#SKIP-SE"))
							{
								goto IL_1AAF;
							}
							goto IL_16F8;
						}
					}
					else if (c != 'U')
					{
						if (c != '_')
						{
							goto IL_1AAF;
						}
						if (!(text2 == "#SKIP_SE"))
						{
							goto IL_1AAF;
						}
						goto IL_16F8;
					}
					else
					{
						if (!(text2 == "#INCLUDE"))
						{
							goto IL_1AAF;
						}
						this.Message = "MML のインクルード機能はありません。\n\n#INCLUDE-VOICE（音色ファイル）\n#INCLUDE-RHYTHMVOICE（リズム音色ファイル）\nのいずれかをご使用ください。";
						this.ErrorString = array2[0];
						if (array2.Length > 1)
						{
							this.ErrorString = this.ErrorString + "  " + this.LineBuffer;
							return;
						}
						return;
					}
					break;
				}
				case 9:
				{
					char c = text2[4];
					if (c <= 'K')
					{
						if (c != '-')
						{
							switch (c)
							{
							case 'A':
								if (!(text2 == "#ARRANGER"))
								{
									goto IL_1AAF;
								}
								this.MetaData[2] = this.ReadText(array2[0], this.LineBuffer);
								return;
							case 'B':
							case 'D':
								goto IL_1AAF;
							case 'C':
								if (!(text2 == "#TRACKREV"))
								{
									goto IL_1AAF;
								}
								goto IL_121F;
							case 'E':
								if (!(text2 == "#TIMEBASE"))
								{
									if (!(text2 == "#RESETVIB"))
									{
										goto IL_1AAF;
									}
									goto IL_1301;
								}
								break;
							case 'F':
								if (!(text2 == "#V-OFFSET"))
								{
									goto IL_1AAF;
								}
								goto IL_14BD;
							default:
								if (c != 'K')
								{
									goto IL_1AAF;
								}
								if (!(text2 == "#SEEKTIME"))
								{
									goto IL_1AAF;
								}
								goto IL_1311;
							}
						}
						else
						{
							if (!(text2 == "#KEY-SIGN"))
							{
								goto IL_1AAF;
							}
							goto IL_1427;
						}
					}
					else if (c != 'P')
					{
						if (c != 'S')
						{
							if (c != 'V')
							{
								goto IL_1AAF;
							}
							if (!(text2 == "#HENVMODE"))
							{
								goto IL_1AAF;
							}
							goto IL_1055;
						}
						else
						{
							if (!(text2 == "#KEYSHIFT"))
							{
								goto IL_1AAF;
							}
							goto IL_101C;
						}
					}
					else
					{
						if (!(text2 == "#COMPOSER"))
						{
							goto IL_1AAF;
						}
						this.MetaData[1] = this.ReadText(array2[0], this.LineBuffer);
						return;
					}
					break;
				}
				case 10:
				{
					char c = text2[6];
					if (c <= 'H')
					{
						if (c != '-')
						{
							switch (c)
							{
							case 'B':
								if (!(text2 == "#TIME-BASE"))
								{
									goto IL_1AAF;
								}
								break;
							case 'C':
							case 'G':
								goto IL_1AAF;
							case 'D':
								if (!(text2 == "#INCLUDE-V"))
								{
									goto IL_1AAF;
								}
								goto IL_15E3;
							case 'E':
								if (!(text2 == "#OCTAVEREV"))
								{
									goto IL_1AAF;
								}
								goto IL_10E5;
							case 'F':
							{
								if (!(text2 == "#RV-OFFSET"))
								{
									goto IL_1AAF;
								}
								int num6 = this.NumberRead(false, true);
								if (num6 < -15 || num6 > 15)
								{
									this.RangeNotice("音量オフセット値", -15, 15, null);
									this.ErrorParameter(text, num6);
									return;
								}
								if (!this.PreCompile)
								{
									for (int j = 0; j < 4; j++)
									{
										this.ReleaseVolumeOffset[j] = num6;
									}
									return;
								}
								return;
							}
							case 'H':
								if (!(text2 == "#KEY-SHIFT"))
								{
									goto IL_1AAF;
								}
								goto IL_101C;
							default:
								goto IL_1AAF;
							}
						}
						else
						{
							if (text2 == "#TRACK-REV")
							{
								goto IL_121F;
							}
							if (!(text2 == "#RESET-VIB"))
							{
								goto IL_1AAF;
							}
							goto IL_1301;
						}
					}
					else if (c != 'M')
					{
						switch (c)
						{
						case 'P':
							if (!(text2 == "#TRANSPOSE"))
							{
								goto IL_1AAF;
							}
							goto IL_101C;
						case 'Q':
						case 'R':
							goto IL_1AAF;
						case 'S':
							if (!(text2 == "#ADDRESSSE"))
							{
								goto IL_1AAF;
							}
							goto IL_18C4;
						case 'T':
							if (!(text2 == "#SEEK-TIME"))
							{
								goto IL_1AAF;
							}
							goto IL_1311;
						default:
							if (c != '_')
							{
								goto IL_1AAF;
							}
							if (!(text2 == "#RESET_VIB"))
							{
								goto IL_1AAF;
							}
							goto IL_1301;
						}
					}
					else
					{
						if (!(text2 == "#HENV-MODE"))
						{
							goto IL_1AAF;
						}
						goto IL_1055;
					}
					break;
				}
				case 11:
					switch (text2[3])
					{
					case 'A':
						if (text2 == "#SHARPFRAME")
						{
							goto IL_12E9;
						}
						if (!(text2 == "#BSAVE-NAME") && !(text2 == "#BSAVE_NAME") && !(text2 == "#BSAVE-FILE") && !(text2 == "#BSAVE_FILE"))
						{
							goto IL_1AAF;
						}
						goto IL_1592;
					case 'B':
					case 'E':
					case 'G':
					case 'H':
					case 'J':
					case 'K':
					case 'Q':
					case 'R':
					case 'U':
					case 'V':
					case 'W':
					case 'X':
						goto IL_1AAF;
					case 'C':
						if (!(text2 == "#INCLUDE-RV"))
						{
							goto IL_1AAF;
						}
						goto IL_1641;
					case 'D':
						if (!(text2 == "#ADDRESS-SE") && !(text2 == "#ADDRESS_SE"))
						{
							goto IL_1AAF;
						}
						goto IL_18C4;
					case 'F':
						if (!(text2 == "#DEFAULTOFF"))
						{
							goto IL_1AAF;
						}
						goto IL_12F1;
					case 'I':
						if (!(text2 == "#SKIPDEFINE"))
						{
							goto IL_1AAF;
						}
						goto IL_16E8;
					case 'L':
						if (!(text2 == "#VOL-OFFSET"))
						{
							goto IL_1AAF;
						}
						goto IL_14BD;
					case 'M':
						if (!(text2 == "#JUMPOFFSET"))
						{
							goto IL_1AAF;
						}
						goto IL_1392;
					case 'N':
						if (!(text2 == "#RANDOMSEED"))
						{
							goto IL_1AAF;
						}
						goto IL_13E6;
					case 'O':
						if (!(text2 == "#PROGRAMMER"))
						{
							goto IL_1AAF;
						}
						this.MetaData[3] = this.ReadText(array2[0], this.LineBuffer);
						return;
					case 'P':
						if (!(text2 == "#REPEATMODE"))
						{
							goto IL_1AAF;
						}
						goto IL_108E;
					case 'S':
						if (!(text2 == "#FASTFOWARD"))
						{
							goto IL_1AAF;
						}
						goto IL_12F9;
					case 'T':
						if (!(text2 == "#OCTAVE-REV"))
						{
							goto IL_1AAF;
						}
						goto IL_10E5;
					case 'Y':
						if (!(text2 == "#RHYTHMATOZ"))
						{
							goto IL_1AAF;
						}
						goto IL_1700;
					default:
						goto IL_1AAF;
					}
					break;
				case 12:
				{
					char c = text2[3];
					if (c <= 'C')
					{
						if (c != 'A')
						{
							if (c != 'C')
							{
								goto IL_1AAF;
							}
							if (text2 == "#INCLUDE-NDV")
							{
								goto IL_15E3;
							}
							if (!(text2 == "#INCLUDE-NDR"))
							{
								goto IL_1AAF;
							}
							goto IL_1641;
						}
						else
						{
							if (!(text2 == "#SHARP-FRAME"))
							{
								goto IL_1AAF;
							}
							goto IL_12E9;
						}
					}
					else if (c != 'F')
					{
						switch (c)
						{
						case 'I':
							if (!(text2 == "#SKIP-DEFINE") && !(text2 == "#SKIP_DEFINE"))
							{
								goto IL_1AAF;
							}
							goto IL_16E8;
						case 'J':
						case 'K':
						case 'L':
						case 'O':
						case 'Q':
							goto IL_1AAF;
						case 'M':
							if (!(text2 == "#JUMP-OFFSET"))
							{
								goto IL_1AAF;
							}
							goto IL_1392;
						case 'N':
							if (!(text2 == "#RANDOM-SEED"))
							{
								goto IL_1AAF;
							}
							goto IL_13E6;
						case 'P':
							if (!(text2 == "#REPEAT-MODE"))
							{
								goto IL_1AAF;
							}
							goto IL_108E;
						case 'R':
							if (!(text2 == "#SCROLLTRACK"))
							{
								goto IL_1AAF;
							}
							goto IL_1A13;
						case 'S':
							if (!(text2 == "#FASTFORWARD") && !(text2 == "#FAST-FOWARD"))
							{
								goto IL_1AAF;
							}
							goto IL_12F9;
						default:
							if (c != 'Y')
							{
								goto IL_1AAF;
							}
							if (!(text2 == "#RHYTHM-ATOZ") && !(text2 == "#RHYTHM_ATOZ"))
							{
								goto IL_1AAF;
							}
							goto IL_1700;
						}
					}
					else
					{
						if (!(text2 == "#DEFAULT-OFF"))
						{
							goto IL_1AAF;
						}
						goto IL_12F1;
					}
					break;
				}
				case 13:
				{
					char c = text2[1];
					if (c != 'F')
					{
						if (c != 'K')
						{
							switch (c)
							{
							case 'S':
								if (!(text2 == "#SCROLL-TRACK") && !(text2 == "#SCROLL_TRACK"))
								{
									goto IL_1AAF;
								}
								goto IL_1A13;
							case 'T':
								if (!(text2 == "#TRACKREVERSE"))
								{
									goto IL_1AAF;
								}
								goto IL_121F;
							case 'U':
								goto IL_1AAF;
							case 'V':
								if (!(text2 == "#VOLUMEOFFSET"))
								{
									goto IL_1AAF;
								}
								goto IL_14BD;
							default:
								goto IL_1AAF;
							}
						}
						else
						{
							if (!(text2 == "#KEYSIGNATURE"))
							{
								goto IL_1AAF;
							}
							goto IL_1427;
						}
					}
					else
					{
						if (!(text2 == "#FAST-FORWARD"))
						{
							goto IL_1AAF;
						}
						goto IL_12F9;
					}
					break;
				}
				case 14:
				{
					char c = text2[1];
					if (c <= 'I')
					{
						if (c != 'A')
						{
							if (c != 'E')
							{
								if (c != 'I')
								{
									goto IL_1AAF;
								}
								if (!(text2 == "#INCLUDE-VOICE"))
								{
									goto IL_1AAF;
								}
								goto IL_15E3;
							}
							else
							{
								if (!(text2 == "#EXPAND-MEMORY"))
								{
									goto IL_1AAF;
								}
								this.InfoText.Add("#EXPAND-MEMORYは過去のMMLとの互換用です。現在は常に最大28KBまで使用できます。");
								return;
							}
						}
						else
						{
							if (!(text2 == "#ADDRESSDEFINE"))
							{
								goto IL_1AAF;
							}
							goto IL_1775;
						}
					}
					else if (c != 'K')
					{
						if (c != 'O')
						{
							switch (c)
							{
							case 'R':
								if (text2 == "#RESET-VIBRATO" || text2 == "#RESET_VIBRATO")
								{
									goto IL_1301;
								}
								if (!(text2 == "#RESETPITCHENV"))
								{
									goto IL_1AAF;
								}
								goto IL_1309;
							case 'S':
							case 'U':
								goto IL_1AAF;
							case 'T':
								if (!(text2 == "#TRACK-REVERSE"))
								{
									goto IL_1AAF;
								}
								goto IL_121F;
							case 'V':
								if (!(text2 == "#VOLUME-OFFSET"))
								{
									goto IL_1AAF;
								}
								goto IL_14BD;
							default:
								goto IL_1AAF;
							}
						}
						else
						{
							if (!(text2 == "#OCTAVEREVERSE"))
							{
								goto IL_1AAF;
							}
							goto IL_10E5;
						}
					}
					else
					{
						if (!(text2 == "#KEY-SIGNATURE"))
						{
							goto IL_1AAF;
						}
						goto IL_1427;
					}
					break;
				}
				case 15:
				{
					char c = text2[5];
					if (c <= 'E')
					{
						if (c != '-')
						{
							if (c != 'E')
							{
								goto IL_1AAF;
							}
							if (text2 == "#BSAVE-FILENAME" || text2 == "#BSAVE_FILENAME")
							{
								goto IL_1592;
							}
							if (!(text2 == "#ADDRESS-DEFINE") && !(text2 == "#ADDRESS_DEFINE"))
							{
								goto IL_1AAF;
							}
							goto IL_1775;
						}
						else if (!(text2 == "#LOOP-PIANOROLL"))
						{
							goto IL_1AAF;
						}
					}
					else if (c != 'T')
					{
						if (c != 'V')
						{
							if (c != '_')
							{
								goto IL_1AAF;
							}
							if (!(text2 == "#LOOP_PIANOROLL"))
							{
								goto IL_1AAF;
							}
						}
						else
						{
							if (!(text2 == "#OCTAVE-REVERSE"))
							{
								goto IL_1AAF;
							}
							goto IL_10E5;
						}
					}
					else
					{
						if (!(text2 == "#RESET-PITCHENV") && !(text2 == "#RESET_PITCHENV"))
						{
							goto IL_1AAF;
						}
						goto IL_1309;
					}
					int num7 = this.NumberRead(false, true);
					if (num7 <= -32769)
					{
						num7 = 2;
					}
					if (num7 < 1 || num7 > 100)
					{
						this.RangeNotice("無限ループをピアノロールに展開する回数", 1, 100, null);
						this.ErrorParameter(text, num7);
						return;
					}
					this.PianoRollLoop = num7;
					return;
				}
				case 17:
				{
					char c = text2[7];
					if (c != '-')
					{
						if (c != '_')
						{
							goto IL_1AAF;
						}
						if (!(text2 == "#ADJUST_PIANOROLL"))
						{
							goto IL_1AAF;
						}
					}
					else if (!(text2 == "#ADJUST-PIANOROLL"))
					{
						goto IL_1AAF;
					}
					if (array2.Length < 2)
					{
						this.Message = "値が設定されていません。";
						this.ErrorString = array2[0];
						return;
					}
					decimal num8 = this.DecimalNumberRead(false, true);
					if (num8 <= -32769m)
					{
						this.Message = "値が正しくありません。";
						this.ErrorString = array2[0];
						return;
					}
					this.AdjustPianoRoll = (double)num8;
					return;
				}
				case 20:
				{
					char c = text2[6];
					if (c != '-')
					{
						if (c != 'D')
						{
							if (c != '_')
							{
								goto IL_1AAF;
							}
							if (!(text2 == "#RESET_PITCHENVELOPE"))
							{
								goto IL_1AAF;
							}
							goto IL_1309;
						}
						else
						{
							if (!(text2 == "#INCLUDE-RHYTHMVOICE"))
							{
								goto IL_1AAF;
							}
							goto IL_1641;
						}
					}
					else
					{
						if (!(text2 == "#RESET-PITCHENVELOPE"))
						{
							goto IL_1AAF;
						}
						goto IL_1309;
					}
					break;
				}
				default:
					goto IL_1AAF;
				}
				array4 = this.SetTempo;
				for (int i = 0; i < array4.Length; i++)
				{
					if (array4[i])
					{
						this.Message = "既にテンポ指定によってテンポが設定されています。\nどちらか片方で設定してください。\n\nタイムベースは任意の音長に対するフレーム数で、設定すると自動的にテンポも決定されます。";
						this.ErrorString = text;
						break;
					}
				}
				decimal num9 = this.DecimalNumberRead(false, true);
				if (this.CheckNextCharacter(','))
				{
					decimal num10 = this.DecimalNumberRead(false, true);
					if (num9 < 1m || num9 > 64m)
					{
						this.RangeNotice(text + "<n>,<m> の n は音長です。範囲", 1, 64, "\nパラメータを 2 つ指定することにより、任意の音長に対するフレーム数を定義できます。この値によって自動的にテンポが決まり、たとえば #TIMEBASE 4,32 なら T112.5 を設定したことになります。");
						this.ErrorParameter(text + num9.ToString() + ",", num10);
						return;
					}
					if (num10 < 1m || num10 > 100m)
					{
						this.RangeNotice(text + "<n>,<m> の m はタイムベース値です。範囲", 1, 100, "\nパラメータを 2 つ指定することにより、任意の音長に対するフレーム数を定義できます。この値によって自動的にテンポが決まり、たとえば #TIMEBASE 4,32 なら T112.5 を設定したことになります。");
						this.ErrorParameter(text + num9.ToString() + ",", num10);
						return;
					}
					this.HeaderTempo = 14400m / num9 / num10;
				}
				else
				{
					if (num9 < 4m || num9 > 100m)
					{
						this.RangeNotice(text + "<n> はタイムベース指定です。範囲", 4, 100, "\nこの指定では、4 分音符のフレーム数を定義できます。この値によって自動的にテンポが決まり、たとえば #TIMBASE 32 なら T112.5 を設定したことになります。");
						this.ErrorParameter(text, num9);
						return;
					}
					this.HeaderTempo = 3600m / num9;
				}
				this.SetTimebase = new bool[] { true, true, true, true };
				return;
				IL_101C:
				int num11 = this.NumberRead(false, true);
				if (num11 < -84 || num11 > 84)
				{
					this.RangeNotice("トランスポーズの値", -84, 84, null);
					this.ErrorParameter(text, num11);
					return;
				}
				this.HeaderKeyShift = num11;
				return;
				IL_1055:
				int num12 = this.NumberRead(false, true);
				if (num12 < 0 || num12 > 1)
				{
					this.RangeNotice("ハードエンベロープモードの値", 0, 1, "\n0: コンパイラでエンベロープ周期を展開\n1: ドライバの機能を使用（音符に自動追尾）");
					this.ErrorParameter(text, num12);
					return;
				}
				this.HardwareEnvelopeMode = num12;
				return;
				IL_108E:
				int num13 = this.NumberRead(false, true);
				if (num13 < 0 || num13 > 4)
				{
					this.RangeNotice("リピートモードの値", 0, 4, "\n0: コンパイル時に展開し、ドライバのリピート機能は使用しない\n1: ドライバの機能を使用（リピート終了直後の音符にフレーム加算）\n2: ドライバの機能を使用（1 フレーム未満の端数は切り捨て）\n3: ドライバの機能を使用（フレーム補正せずにエラー検出）\n4: 同一バイナリを自動検出し、ドライバのリピート機能で圧縮");
					this.ErrorParameter(text, num13);
					return;
				}
				if (!this.PreCompile)
				{
					for (int k = 0; k < 4; k++)
					{
						this.RepeatMode[k] = num13;
					}
					return;
				}
				return;
				IL_10E5:
				this.Command_OctaveDown = '>';
				this.Command_OctaveUp = '<';
				return;
				IL_121F:
				int num14 = 0;
				char c4;
				for (;;)
				{
					c4 = this.Read1(-1);
					if (c4 == '\0')
					{
						goto IL_12BF;
					}
					this.Index++;
					if (c4 != ' ' && c4 != '\t' && c4 != ',')
					{
						int num15 = Array.IndexOf<string>(this.TrackCode, c4.ToString().ToUpper());
						if (num15 == -1)
						{
							break;
						}
						if (num14 > 1)
						{
							goto Block_199;
						}
						this.ReverseTrack[num14] = num15;
						num14++;
					}
				}
				this.Message = "#TRACK-REV で指定されたトラック名が正しくありません。";
				this.ErrorString = c4.ToString();
				goto IL_12BF;
				Block_199:
				this.Message = "#TRACK-REV で指定されているトラックが多すぎます。";
				this.ErrorString = c4.ToString();
				IL_12BF:
				if (num14 < 2)
				{
					this.Message = "#TRACK-REV のパラメータが正しくありません。";
					this.ErrorString = "#TRACK-REV" + this.LineBuffer;
					return;
				}
				return;
				IL_12E9:
				this.SharpFrameMode = true;
				return;
				IL_12F1:
				this.DefaultOn = false;
				return;
				IL_12F9:
				this.FastForwardMode = true;
				return;
				IL_1301:
				this.DefaultVibrato = 1;
				return;
				IL_1309:
				this.DefaultPitchEnvelope = 1;
				return;
				IL_1311:
				int num16 = this.NumberRead(false, true);
				if (num16 < 0 || num16 > 600)
				{
					this.RangeNotice("再生開始秒", 0, 600, null);
					this.ErrorParameter(text, num16);
					return;
				}
				this.StartTime = (double)num16;
				return;
				IL_1392:
				int num17 = this.NumberRead(false, true);
				if (num17 < -9999 || num17 > 9999)
				{
					this.RangeNotice("J コマンドのオフセット値", -9999, 9999, "\n単位はミリ秒で、1000 で1秒になります。\n初期値は -150 （0.15秒手前）です。");
					this.ErrorParameter(text, num17);
					return;
				}
				this.JumpOffset = (double)num17 / 1000.0;
				return;
				IL_13E6:
				int num18 = this.NumberRead(false, true);
				if (num18 < 0 || num18 > 9999)
				{
					this.RangeNotice("乱数のシード値", 0, 9999, "0 で毎回異なる値となります。");
					this.ErrorParameter(text, num18);
					return;
				}
				this.SetRandomSeed(num18);
				return;
				IL_1427:
				this.LineBuffer = this.LineBuffer.ToUpper();
				this.HeaderKeySign = this.SetKeySignSub();
				return;
				IL_14BD:
				int num19 = this.NumberRead(false, true);
				if (num19 < -15 || num19 > 15)
				{
					this.RangeNotice("音量オフセット値", -15, 15, null);
					this.ErrorParameter(text, num19);
					return;
				}
				if (!this.PreCompile)
				{
					this.HeaderVolumeOffset = num19;
					return;
				}
				return;
				IL_1592:
				this.ErrorString = array2[0];
				if (array2.Length > 1)
				{
					this.ErrorString = this.ErrorString + "  " + this.LineBuffer;
				}
				string text4 = this.ReadText(array2[0], this.LineBuffer);
				if (text4 != null)
				{
					this.BsaveFileName = text4.ToUpper();
					return;
				}
				return;
				IL_15E3:
				this.ErrorString = array2[0];
				if (array2.Length > 1)
				{
					this.ErrorString = this.ErrorString + "  " + this.LineBuffer;
				}
				string text5 = this.ReadText(array2[0], this.LineBuffer);
				if (text5 == null)
				{
					return;
				}
				string text6 = this.ReadVoiceFile(text5, false);
				if (text6 != null)
				{
					this.SetVoiceDataFromNDV(text6);
					return;
				}
				return;
				IL_1641:
				this.ErrorString = array2[0];
				if (array2.Length > 1)
				{
					this.ErrorString = this.ErrorString + "  " + this.LineBuffer;
				}
				string text7 = this.ReadText(array2[0], this.LineBuffer);
				if (text7 == null)
				{
					return;
				}
				string text8 = this.ReadVoiceFile(text7, true);
				if (text8 == null)
				{
					return;
				}
				string[] array5 = text8.Split('\n', StringSplitOptions.None);
				try
				{
					for (int l = 0; l < array5.Length; l++)
					{
						this.RhythmVoiceMMLFromFile.Add(array5[l]);
					}
					return;
				}
				catch (Exception ex)
				{
					this.Message = "音色ファイル内のデータを正常に読み込めませんでした。\n\n" + ex.Message;
					return;
				}
				IL_16E8:
				this.SkipDefine = true;
				return;
				IL_16F0:
				this.SkipF = true;
				return;
				IL_16F8:
				this.SkipSE = true;
				return;
				IL_1700:
				this.UseRhythmAllAlphabet = true;
				return;
				IL_1775:
				if (array2.Length < 2)
				{
					this.Message = "値が設定されていません。";
					this.ErrorString = array2[0];
					return;
				}
				this.LineBuffer = this.LineBuffer.Trim();
				this.ErrorString = array2[0] + " " + this.LineBuffer;
				if (this.LineBuffer.Length < 4)
				{
					this.Message = "桁数が足りません。";
					return;
				}
				if (this.LineBuffer.Substring(0, 2) == "0x" || this.LineBuffer.Substring(0, 2) == "0X")
				{
					this.LineBuffer = this.LineBuffer.Remove(0, 2);
				}
				if (this.LineBuffer.EndsWith("h", true, null))
				{
					this.LineBuffer = this.LineBuffer.Remove(this.LineBuffer.Length - 1);
				}
				try
				{
					this.DefineAddressAbsolute = int.Parse(this.LineBuffer, NumberStyles.HexNumber);
				}
				catch (Exception ex2)
				{
					this.Message = ex2.Message + "\n4 桁の 16 進数を設定してください。";
					return;
				}
				if (this.DefineAddressAbsolute < 16384)
				{
					this.Message = "音色データの絶対アドレスは 0x4000 以上にしてください。";
				}
				if (this.DefineAddressAbsolute > 65535)
				{
					this.Message = "音色データの絶対アドレスは 0xFFFF 以下にしてください。";
				}
				this.AutoSetHeader.Remove("ADDRESS-DEFINE");
				return;
				IL_18C4:
				if (array2.Length < 2)
				{
					this.Message = "値が設定されていません。";
					this.ErrorString = array2[0];
					return;
				}
				this.LineBuffer = this.LineBuffer.Trim();
				this.ErrorString = array2[0] + " " + this.LineBuffer;
				if (this.LineBuffer.Length < 4)
				{
					this.Message = "桁数が足りません。";
					return;
				}
				if (this.LineBuffer.Substring(0, 2) == "0x" || this.LineBuffer.Substring(0, 2) == "0X")
				{
					this.LineBuffer = this.LineBuffer.Remove(0, 2);
				}
				if (this.LineBuffer.EndsWith("h", true, null))
				{
					this.LineBuffer = this.LineBuffer.Remove(this.LineBuffer.Length - 1);
				}
				try
				{
					this.SEAddressAbsolute = int.Parse(this.LineBuffer, NumberStyles.HexNumber);
				}
				catch (Exception ex3)
				{
					this.Message = ex3.Message + "\n4 桁の 16 進数を設定してください。";
					return;
				}
				if (this.SEAddressAbsolute < 16384)
				{
					this.Message = "効果音データの絶対アドレスは 0x4000 以上にしてください。";
				}
				if (this.SEAddressAbsolute > 65535)
				{
					this.Message = "効果音データの絶対アドレスは 0xFFFF 以下にしてください。";
				}
				this.AutoSetHeader.Remove("ADDRESS-SE");
				return;
				IL_1A13:
				int num20 = -1;
				char c5;
				for (;;)
				{
					c5 = this.Read1(-1);
					if (c5 == '\0')
					{
						goto IL_1AA6;
					}
					this.Index++;
					if (c5 != ' ' && c5 != '\t' && c5 != ',')
					{
						int num21 = Array.IndexOf<string>(this.TrackCode, c5.ToString().ToUpper());
						if (num21 == -1)
						{
							break;
						}
						if (num20 != -1)
						{
							goto Block_242;
						}
						num20 = num21;
					}
				}
				this.Message = "#SCROLL-TRACK で指定されたトラック名が正しくありません。";
				this.ErrorString = c5.ToString();
				goto IL_1AA6;
				Block_242:
				this.Message = "#SCROLL-TRACK で 2 つ以上のトラックは指定できません。";
				this.ErrorString = c5.ToString();
				IL_1AA6:
				this.ScrollTrack = num20;
				return;
			}
			IL_1AAF:
			this.Message = "ヘッダの文法エラーです。";
			this.ErrorString = array2[0];
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00026314 File Offset: 0x00024514
		public string ReadText(string Header, string Source)
		{
			string text = "";
			bool flag = true;
			for (;;)
			{
				char c = this.Read1(-1);
				if (c == '\0')
				{
					goto IL_0073;
				}
				this.Index++;
				if (!flag || (c != ' ' && c != '\t' && c != ','))
				{
					if (c != '"' && text.Length == 0)
					{
						break;
					}
					flag = false;
					ReadOnlySpan<char> readOnlySpan = text;
					char c2 = c;
					text = readOnlySpan + new ReadOnlySpan<char>(ref c2);
				}
			}
			this.Message = Header + " ヘッダで定義する文字列は \" で括ってください。";
			this.ErrorString = Source;
			return null;
			IL_0073:
			if (text.Length == 0)
			{
				this.Message = Header + " ヘッダで文字列が指定されていません。";
				this.ErrorString = Header;
				return null;
			}
			if (!text.EndsWith("\""))
			{
				this.Message = "文字列が \" で正しく閉じられていません。";
				this.ErrorString = Header + "  " + Source;
				return null;
			}
			return text.Trim('"');
		}

		// Token: 0x0600012F RID: 303 RVA: 0x000263EC File Offset: 0x000245EC
		public string ReadVoiceFile(string VoiceFilePath, bool RhythmMode = false)
		{
			string text = VoiceFilePath.Replace("\"", "");
			if (!text.EndsWith(".ndv") && !text.EndsWith(".ndr"))
			{
				text += (RhythmMode ? ".ndr" : ".ndv");
			}
			string text2 = (RhythmMode ? "リズム" : "");
			if (!File.Exists(text))
			{
				text = Path.GetDirectoryName(this.MMLFilePath) + "\\" + text;
				if (!File.Exists(text))
				{
					this.Message = "指定した" + text2 + "音色ファイルが存在しません。\n\n";
					if (this.MMLFilePath == null)
					{
						this.Message += "MML ファイルを保存すると、そのフォルダが優先的に読み込まれます。\nまた、フルパスも指定可能です。";
					}
					else
					{
						this.Message = this.Message + "MML ファイルと同じフォルダに" + text2 + "音色ファイルを置いてください。\nまた、MML ファイルのあるフォルダからの相対パス、フルパスも指定可能です。";
					}
					return null;
				}
			}
			string text4;
			try
			{
				string text3 = File.ReadAllText(text);
				this.ErrorString = "";
				text4 = text3;
			}
			catch (Exception ex)
			{
				this.Message = text2 + "音色ファイルを正常に読み込めませんでした。\n\n" + ex.Message;
				text4 = null;
			}
			return text4;
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00026508 File Offset: 0x00024708
		private void SetVoiceDataFromNDV(string Data)
		{
			string[] array = Data.Split('\n', StringSplitOptions.None);
			try
			{
				for (int i = 0; i < 16; i++)
				{
					string[] array2 = array[i].Split(',', StringSplitOptions.None);
					array2[2] = array2[2].Insert(int.Parse(array2[0]), "L").Substring(0, int.Parse(array2[1]) + 2);
					this.VoiceMMLFromFile.Add("@" + i.ToString() + "=" + array2[2]);
				}
			}
			catch (Exception ex)
			{
				this.Message = "音色ファイル内のデータを正常に読み込めませんでした。\n\n" + ex.Message;
			}
		}

		// Token: 0x0400009E RID: 158
		public const int LFrameMode = 1000;

		// Token: 0x0400009F RID: 159
		public const string ZeroOne = "指定は 0 で OFF、1 で ON となります。";

		// Token: 0x040000A0 RID: 160
		public const string RhythmCodeChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

		// Token: 0x040000A1 RID: 161
		public const int ErrorNumber = -32769;

		// Token: 0x040000A2 RID: 162
		public const int MaxBGMDataSize = 28672;

		// Token: 0x040000A3 RID: 163
		public const int MaxLine = 100000;

		// Token: 0x040000A4 RID: 164
		public const double NTSC_FPS = 59.94080511738504;

		// Token: 0x040000A5 RID: 165
		public static int[] SlowParameter = new int[] { 0, 1, 17, 41, 85, 109, 119, 127 };

		// Token: 0x040000A6 RID: 166
		public readonly List<Core.SlowEvent> SlowEvents = new List<Core.SlowEvent>();

		// Token: 0x040000A7 RID: 167
		private int[] SlowMusicToRealFrame;

		// Token: 0x040000A8 RID: 168
		public int[] PitchTable;

		// Token: 0x040000A9 RID: 169
		public decimal HeaderTempo;

		// Token: 0x040000AA RID: 170
		public int HeaderKeyShift;

		// Token: 0x040000AB RID: 171
		public int[] HeaderKeySign;

		// Token: 0x040000AC RID: 172
		public int HeaderVolumeOffset;

		// Token: 0x040000AD RID: 173
		public bool SkipDefine;

		// Token: 0x040000AE RID: 174
		public bool SkipF;

		// Token: 0x040000AF RID: 175
		public bool SkipSE;

		// Token: 0x040000B0 RID: 176
		public string MMLFilePath;

		// Token: 0x040000B1 RID: 177
		public string BsaveFileName;

		// Token: 0x040000B2 RID: 178
		public string[] MetaData;

		// Token: 0x040000B3 RID: 179
		public string[,] TrackBuffer;

		// Token: 0x040000B4 RID: 180
		public int SoundId;

		// Token: 0x040000B5 RID: 181
		public string Message;

		// Token: 0x040000B6 RID: 182
		public string Status;

		// Token: 0x040000B7 RID: 183
		public string ErrorString;

		// Token: 0x040000B8 RID: 184
		public int ErrorLine;

		// Token: 0x040000B9 RID: 185
		public int LineNumber;

		// Token: 0x040000BA RID: 186
		public string[] TrackCode = new string[] { "R", "1", "2", "3" };

		// Token: 0x040000BB RID: 187
		public string[] TrackName = new string[] { "リズムトラック", "トラック1", "トラック2", "トラック3" };

		// Token: 0x040000BC RID: 188
		public List<int> LinesIndex;

		// Token: 0x040000BD RID: 189
		public Dictionary<int, int> MultiLineStartLine;

		// Token: 0x040000BE RID: 190
		public int BGMDataSize;

		// Token: 0x040000BF RID: 191
		public List<int> RegisterdSoundNumber;

		// Token: 0x040000C0 RID: 192
		public int[] SEAddress;

		// Token: 0x040000C1 RID: 193
		public Dictionary<int, int> RegisterSoundPriority;

		// Token: 0x040000C2 RID: 194
		public List<int> SEUseLine;

		// Token: 0x040000C3 RID: 195
		public List<int> SEUseAddress;

		// Token: 0x040000C4 RID: 196
		public List<int> SEUseNumber;

		// Token: 0x040000C5 RID: 197
		public char Command_OctaveUp = '>';

		// Token: 0x040000C6 RID: 198
		public char Command_OctaveDown = '<';

		// Token: 0x040000C7 RID: 199
		public int[,] TrackFrame;

		// Token: 0x040000C8 RID: 200
		public int[,] BeforeLoopFrame;

		// Token: 0x040000C9 RID: 201
		public Dictionary<string, AlignPointData> AlignPoints;

		// Token: 0x040000CA RID: 202
		public Dictionary<string, int[]> AlignOccurrenceCounter;

		// Token: 0x040000CB RID: 203
		public List<string>[] AlignTrackEventOrder;

		// Token: 0x040000CC RID: 204
		public int[] AlignSegmentFrame;

		// Token: 0x040000CD RID: 205
		public bool AlignPlanReady;

		// Token: 0x040000CE RID: 206
		public int[][] AtF_Frame;

		// Token: 0x040000CF RID: 207
		public int[] LoopAddress;

		// Token: 0x040000D0 RID: 208
		public int[] TrackNameLength;

		// Token: 0x040000D1 RID: 209
		public int[] FrameAnalyzeCount;

		// Token: 0x040000D2 RID: 210
		public string[] FrameAnalyzeText;

		// Token: 0x040000D3 RID: 211
		public decimal[] Tempo;

		// Token: 0x040000D4 RID: 212
		public bool[] SetTempo;

		// Token: 0x040000D5 RID: 213
		public bool[] SetTimebase;

		// Token: 0x040000D6 RID: 214
		public int[] Octave;

		// Token: 0x040000D7 RID: 215
		public int[] Volume;

		// Token: 0x040000D8 RID: 216
		public int[] VolumeOffset;

		// Token: 0x040000D9 RID: 217
		public int[] RestoreVolume;

		// Token: 0x040000DA RID: 218
		public int[] RhythmVolume;

		// Token: 0x040000DB RID: 219
		public int[] RhythmVolumeLimit;

		// Token: 0x040000DC RID: 220
		public int[] VolumeLimit;

		// Token: 0x040000DD RID: 221
		public int[] vi;

		// Token: 0x040000DE RID: 222
		public int[] viMax;

		// Token: 0x040000DF RID: 223
		public decimal[][] DefaultLength;

		// Token: 0x040000E0 RID: 224
		public List<string[]> DefaultLengthAddString;

		// Token: 0x040000E1 RID: 225
		public int[] NoteLengthIndex;

		// Token: 0x040000E2 RID: 226
		public int[] Quantize;

		// Token: 0x040000E3 RID: 227
		public int[] QuantizeBackup;

		// Token: 0x040000E4 RID: 228
		public int[] QuantizeCount;

		// Token: 0x040000E5 RID: 229
		public int[] QuantizeNumber;

		// Token: 0x040000E6 RID: 230
		public int[] AtQuantize;

		// Token: 0x040000E7 RID: 231
		public int[] AtQuantizeBackup;

		// Token: 0x040000E8 RID: 232
		public int[] AtQuantizeCount;

		// Token: 0x040000E9 RID: 233
		public int[] AtQuantizeNumber;

		// Token: 0x040000EA RID: 234
		public int[] KeyShift;

		// Token: 0x040000EB RID: 235
		public int[] RelativeKeyShift;

		// Token: 0x040000EC RID: 236
		public int[][] KeySign;

		// Token: 0x040000ED RID: 237
		public decimal[] FrameMod;

		// Token: 0x040000EE RID: 238
		public bool[] Slur;

		// Token: 0x040000EF RID: 239
		public bool[] Legato;

		// Token: 0x040000F0 RID: 240
		public bool[] LegatoOffFlagSet;

		// Token: 0x040000F1 RID: 241
		public int[] Detune;

		// Token: 0x040000F2 RID: 242
		public bool[] LoopSlur;

		// Token: 0x040000F3 RID: 243
		public bool[] SetVoice;

		// Token: 0x040000F4 RID: 244
		public int[] VoiceNumber;

		// Token: 0x040000F5 RID: 245
		public int[] DelayVoiceNumber;

		// Token: 0x040000F6 RID: 246
		public int[] VolumeSequenceNumber;

		// Token: 0x040000F7 RID: 247
		public int[] QuantizeSequenceNumber;

		// Token: 0x040000F8 RID: 248
		public int[] AtQuantizeSequenceNumber;

		// Token: 0x040000F9 RID: 249
		public int[] DetuneSequenceNumber;

		// Token: 0x040000FA RID: 250
		public int[] HardwareEnvelopeDetuneSequenceNumber;

		// Token: 0x040000FB RID: 251
		public int[] HardwareEnvelopeKeyShiftSequenceNumber;

		// Token: 0x040000FC RID: 252
		public int[] HardwareEnvelopeDecaySequenceNumber;

		// Token: 0x040000FD RID: 253
		public int[] PortamentoKeyShift;

		// Token: 0x040000FE RID: 254
		public int[] PortamentoContinuousMode;

		// Token: 0x040000FF RID: 255
		public int[] PortamentoContinuousKeyShift;

		// Token: 0x04000100 RID: 256
		public int[] PortamentoContinuousStartNote;

		// Token: 0x04000101 RID: 257
		public int DefaultVibrato;

		// Token: 0x04000102 RID: 258
		public int DefaultPitchEnvelope;

		// Token: 0x04000103 RID: 259
		public int[,] PastNoteNumber;

		// Token: 0x04000104 RID: 260
		public int[] SelfDelayCount;

		// Token: 0x04000105 RID: 261
		public int[] SelfDelayVolume;

		// Token: 0x04000106 RID: 262
		public int[] SelfDelayContinueCount;

		// Token: 0x04000107 RID: 263
		public bool[] SelfDelayRestFill;

		// Token: 0x04000108 RID: 264
		public bool[] Mute;

		// Token: 0x04000109 RID: 265
		public int[] ReleaseVolume;

		// Token: 0x0400010A RID: 266
		public int[] ReleaseVolumeOffset;

		// Token: 0x0400010B RID: 267
		public int[] ReleaseVolumeAfter;

		// Token: 0x0400010C RID: 268
		public int[] ReleaseRate;

		// Token: 0x0400010D RID: 269
		public int[] ReleaseDelay;

		// Token: 0x0400010E RID: 270
		public int[] ReleaseDelayFrame;

		// Token: 0x0400010F RID: 271
		public bool[] SetRhythmTrack;

		// Token: 0x04000110 RID: 272
		public int[] SetRhythmTrackIndex;

		// Token: 0x04000111 RID: 273
		public int[] MixMode;

		// Token: 0x04000112 RID: 274
		public int UseRhythmTrackAtC;

		// Token: 0x04000113 RID: 275
		public List<InterruptTrackPointData> InterruptTrackPoints;

		// Token: 0x04000114 RID: 276
		public List<AutoRepeatBoundaryData> AutoRepeatBoundaries;

		// Token: 0x04000115 RID: 277
		private int InterruptTrackPointSequence;

		// Token: 0x04000116 RID: 278
		public bool UseRhythmAllAlphabet;

		// Token: 0x04000117 RID: 279
		public int[] RepeatMode;

		// Token: 0x04000118 RID: 280
		public int[,] RepeatNest;

		// Token: 0x04000119 RID: 281
		public int[,,] RepeatCount;

		// Token: 0x0400011A RID: 282
		public int[,] RepeatEscapeOctave;

		// Token: 0x0400011B RID: 283
		public decimal[,][] RepeatEscapeLength;

		// Token: 0x0400011C RID: 284
		public int[,] RepeatEscapeLengthIndex;

		// Token: 0x0400011D RID: 285
		public List<string[]> RepeatEscapeLengthAddString;

		// Token: 0x0400011E RID: 286
		public int[,,] RepeatFrame1;

		// Token: 0x0400011F RID: 287
		public int[,,] RepeatFrame2;

		// Token: 0x04000120 RID: 288
		public decimal[,] RepeatFrameMod;

		// Token: 0x04000121 RID: 289
		public decimal[,] RepeatEscapeFrameMod;

		// Token: 0x04000122 RID: 290
		public bool[] RepeatFrameModPending;

		// Token: 0x04000123 RID: 291
		public bool RepeatLengthReturn;

		// Token: 0x04000124 RID: 292
		public bool[] RepeatNotClose = new bool[4];

		// Token: 0x04000125 RID: 293
		public int[] Portamento;

		// Token: 0x04000126 RID: 294
		public int[] PortamentoStartNote;

		// Token: 0x04000127 RID: 295
		public int[] FrameAnalyzeNote;

		// Token: 0x04000128 RID: 296
		public int[] FrameAnalyzeLength;

		// Token: 0x04000129 RID: 297
		public bool[] DetunePortamentoMode;

		// Token: 0x0400012A RID: 298
		public int[] PortamentoNoteSetAndSaveOctave;

		// Token: 0x0400012B RID: 299
		public bool[] PortamentoCommandSet;

		// Token: 0x0400012C RID: 300
		public int[] PortamentoMode;

		// Token: 0x0400012D RID: 301
		public bool[] PortamentoReverse;

		// Token: 0x0400012E RID: 302
		public int[] PortamentoWait;

		// Token: 0x0400012F RID: 303
		public int[] RandomStartNote;

		// Token: 0x04000130 RID: 304
		public int[] GlissStartNote;

		// Token: 0x04000131 RID: 305
		public bool[] GlissLegato;

		// Token: 0x04000132 RID: 306
		public bool[] PutNote;

		// Token: 0x04000133 RID: 307
		public int Address;

		// Token: 0x04000134 RID: 308
		public int StartAddress;

		// Token: 0x04000135 RID: 309
		public byte[] BGMData;

		// Token: 0x04000136 RID: 310
		public byte[] BackupBGMData;

		// Token: 0x04000137 RID: 311
		public int TrackIndex;

		// Token: 0x04000138 RID: 312
		public int Index;

		// Token: 0x04000139 RID: 313
		public string LineBuffer;

		// Token: 0x0400013A RID: 314
		public List<List<double>> LinePlayTime;

		// Token: 0x0400013B RID: 315
		public List<List<int>> LinePlayLineNumber;

		// Token: 0x0400013C RID: 316
		public List<List<double>> NotePlayTime;

		// Token: 0x0400013D RID: 317
		public List<List<int>> NotePlayLineNumber;

		// Token: 0x0400013E RID: 318
		public List<List<int>> NotePlayColumn;

		// Token: 0x0400013F RID: 319
		public List<List<NotePlayParameter>> NotePlayParameters;

		// Token: 0x04000140 RID: 320
		public string[] LastNoteLengthText;

		// Token: 0x04000141 RID: 321
		public int PlayLineColumn;

		// Token: 0x04000142 RID: 322
		public int PlayLineBufferIndex;

		// Token: 0x04000143 RID: 323
		public List<byte[]> VoiceData;

		// Token: 0x04000144 RID: 324
		public List<byte[]> RhythmVoiceData;

		// Token: 0x04000145 RID: 325
		public List<int> RhythmVoiceFrameLength;

		// Token: 0x04000146 RID: 326
		public List<byte[]> PitchEnvelopeData;

		// Token: 0x04000147 RID: 327
		public List<int> VoiceDataUsePM;

		// Token: 0x04000148 RID: 328
		public List<bool> VoiceDataUseHENVHarfVolume;

		// Token: 0x04000149 RID: 329
		public int DefineAddressAbsolute;

		// Token: 0x0400014A RID: 330
		public byte[] VoiceBinData;

		// Token: 0x0400014B RID: 331
		public int SEAddressAbsolute;

		// Token: 0x0400014C RID: 332
		public byte[] SEData;

		// Token: 0x0400014D RID: 333
		public int[] NoteEnvelopeNumber;

		// Token: 0x0400014E RID: 334
		public List<byte[]> NoteEnvelopeData;

		// Token: 0x0400014F RID: 335
		public Dictionary<int, int> RegisteredNoteEnvelopeLineNumber;

		// Token: 0x04000150 RID: 336
		public List<List<int>> VolumeSequenceData;

		// Token: 0x04000151 RID: 337
		public int[] VolumeSequenceCounter;

		// Token: 0x04000152 RID: 338
		public bool[] RegisteredVolumeSequence;

		// Token: 0x04000153 RID: 339
		public Dictionary<int, int> RegisteredVolumeSequenceLineNumber;

		// Token: 0x04000154 RID: 340
		public List<List<int>> QuantizeSequenceData;

		// Token: 0x04000155 RID: 341
		public List<List<int>> AtQuantizeSequenceData;

		// Token: 0x04000156 RID: 342
		public int[] QuantizeSequenceCounter;

		// Token: 0x04000157 RID: 343
		public int[] AtQuantizeSequenceCounter;

		// Token: 0x04000158 RID: 344
		public bool[] RegisteredQuantizeSequence;

		// Token: 0x04000159 RID: 345
		public bool[] RegisteredAtQuantizeSequence;

		// Token: 0x0400015A RID: 346
		public Dictionary<int, int> RegisteredQuantizeSequenceLineNumber;

		// Token: 0x0400015B RID: 347
		public Dictionary<int, int> RegisteredAtQuantizeSequenceLineNumber;

		// Token: 0x0400015C RID: 348
		public List<List<int>> DetuneSequenceData;

		// Token: 0x0400015D RID: 349
		public List<List<int>> HardwareEnvelopeDetuneSequenceData;

		// Token: 0x0400015E RID: 350
		public List<List<int>> HardwareEnvelopeKeyShiftSequenceData;

		// Token: 0x0400015F RID: 351
		public List<List<int>> HardwareEnvelopeDecaySequenceData;

		// Token: 0x04000160 RID: 352
		public int[] DetuneSequenceCounter;

		// Token: 0x04000161 RID: 353
		public int[] HardwareEnvelopeDetuneSequenceCounter;

		// Token: 0x04000162 RID: 354
		public int[] HardwareEnvelopeKeyShiftSequenceCounter;

		// Token: 0x04000163 RID: 355
		public int[] HardwareEnvelopeDecaySequenceCounter;

		// Token: 0x04000164 RID: 356
		public bool[] RegisteredDetuneSequence;

		// Token: 0x04000165 RID: 357
		public bool[] RegisteredHardwareEnvelopeDetuneSequence;

		// Token: 0x04000166 RID: 358
		public bool[] RegisteredHardwareEnvelopeKeyShiftSequence;

		// Token: 0x04000167 RID: 359
		public bool[] RegisteredHardwareEnvelopeDecaySequence;

		// Token: 0x04000168 RID: 360
		public Dictionary<int, int> RegisteredDetuneSequenceLineNumber;

		// Token: 0x04000169 RID: 361
		public Dictionary<int, int> RegisteredHardwareEnvelopeDetuneSequenceLineNumber;

		// Token: 0x0400016A RID: 362
		public Dictionary<int, int> RegisteredHardwareEnvelopeKeyShiftSequenceLineNumber;

		// Token: 0x0400016B RID: 363
		public Dictionary<int, int> RegisteredHardwareEnvelopeDecaySequenceLineNumber;

		// Token: 0x0400016C RID: 364
		public bool[] UseVoice;

		// Token: 0x0400016D RID: 365
		public bool[] UseRhythmVoice;

		// Token: 0x0400016E RID: 366
		public bool[] UsePitchEnvelope;

		// Token: 0x0400016F RID: 367
		public bool[] UseNoteEnvelope;

		// Token: 0x04000170 RID: 368
		public int[] UsePitchEnvelopeType;

		// Token: 0x04000171 RID: 369
		public int[] PitchEnvelopeNumber;

		// Token: 0x04000172 RID: 370
		public byte[] PitchEnvelopeType;

		// Token: 0x04000173 RID: 371
		public int[] PitchEnvelopeShapeType;

		// Token: 0x04000174 RID: 372
		public int[] PitchEnvelopeRegisterLine;

		// Token: 0x04000175 RID: 373
		public bool[] HardwareEnvelopeAuto;

		// Token: 0x04000176 RID: 374
		public int HardwareEnvelopeMode = 1;

		// Token: 0x04000177 RID: 375
		public string StoreRhythmCode;

		// Token: 0x04000178 RID: 376
		public int[] HardwareEnvelopePitchTable = new int[]
		{
			216, 202, 193, 180, 170, 160, 152, 144, 136, 128,
			120, 114
		};

		// Token: 0x04000179 RID: 377
		public int[] NoteNumberTable = new int[] { 0, 2, 4, 5, 7, 9, 11 };

		// Token: 0x0400017A RID: 378
		public int[] HardwareEnvelopeNumber;

		// Token: 0x0400017B RID: 379
		public int[] HardwareEnvelopePeriod;

		// Token: 0x0400017C RID: 380
		public int[] HardwareEnvelopeVolumeDecayFrame;

		// Token: 0x0400017D RID: 381
		public int HardwareEnvelopeOctave;

		// Token: 0x0400017E RID: 382
		public int HardwareEnvelopeDetune;

		// Token: 0x0400017F RID: 383
		public int HardwareEnvelopeKeyShift;

		// Token: 0x04000180 RID: 384
		public bool FastForward;

		// Token: 0x04000181 RID: 385
		public bool PreCompile;

		// Token: 0x04000182 RID: 386
		public bool GetTimeCompile;

		// Token: 0x04000183 RID: 387
		public bool InfoCompile;

		// Token: 0x04000184 RID: 388
		public bool[] FastForwardTrack;

		// Token: 0x04000185 RID: 389
		public bool[] UseJ;

		// Token: 0x04000186 RID: 390
		public bool[] TrackEnable;

		// Token: 0x04000187 RID: 391
		public int[] ReverseTrack;

		// Token: 0x04000188 RID: 392
		public bool SharpFrameMode;

		// Token: 0x04000189 RID: 393
		public int UseMacroCount;

		// Token: 0x0400018A RID: 394
		public double StartTime;

		// Token: 0x0400018B RID: 395
		public double[] JumpStartTime;

		// Token: 0x0400018C RID: 396
		public int[] JumpStartLine;

		// Token: 0x0400018D RID: 397
		public bool UseJump;

		// Token: 0x0400018E RID: 398
		public bool[,] SetJumpNumber;

		// Token: 0x0400018F RID: 399
		public bool FastForwardMode;

		// Token: 0x04000190 RID: 400
		public bool DefaultOn;

		// Token: 0x04000191 RID: 401
		public int TotalFrame;

		// Token: 0x04000192 RID: 402
		public bool InfinityLoop;

		// Token: 0x04000193 RID: 403
		public int FadeOutLoopCount;

		// Token: 0x04000194 RID: 404
		public int FadeOutFrame;

		// Token: 0x04000195 RID: 405
		public double JumpOffset;

		// Token: 0x04000196 RID: 406
		public Random RandomParam;

		// Token: 0x04000197 RID: 407
		public int DefaultSeed;

		// Token: 0x04000198 RID: 408
		public decimal[][] ArpeggioDefaultLength;

		// Token: 0x04000199 RID: 409
		public int[] ArpeggioNoteLengthIndex;

		// Token: 0x0400019A RID: 410
		public int[] ArpeggioRest;

		// Token: 0x0400019B RID: 411
		public int[] ArpeggioVolumeSequenceNumber;

		// Token: 0x0400019C RID: 412
		public int[] ArpeggioQuantizeSequenceNumber;

		// Token: 0x0400019D RID: 413
		public int[] ArpeggioAtQuantizeSequenceNumber;

		// Token: 0x0400019E RID: 414
		public int[] ArpeggioDetuneSequenceNumber;

		// Token: 0x0400019F RID: 415
		public int[] ArpeggioHardwareEnvelopeDetuneSequenceNumber;

		// Token: 0x040001A0 RID: 416
		public int[] ArpeggioHardwareEnvelopeKeyShiftSequenceNumber;

		// Token: 0x040001A1 RID: 417
		public int[] ArpeggioHardwareEnvelopeDecaySequenceNumber;

		// Token: 0x040001A2 RID: 418
		public int[] ArpeggioEndVolume;

		// Token: 0x040001A3 RID: 419
		public int[] ArpeggioEndQuantize;

		// Token: 0x040001A4 RID: 420
		public int[] ArpeggioEndAtQuantize;

		// Token: 0x040001A5 RID: 421
		public int[] ArpeggioPoint1;

		// Token: 0x040001A6 RID: 422
		public int[] ArpeggioPoint2;

		// Token: 0x040001A7 RID: 423
		public int[] ArpeggioCount1;

		// Token: 0x040001A8 RID: 424
		public int[] ArpeggioCount2;

		// Token: 0x040001A9 RID: 425
		public bool[] EnableArpeggio;

		// Token: 0x040001AA RID: 426
		public List<List<int>> ArpeggioNoteNumber;

		// Token: 0x040001AB RID: 427
		public bool[] ArpeggioLegato;

		// Token: 0x040001AC RID: 428
		public int[] ArpeggioMode;

		// Token: 0x040001AD RID: 429
		public int[] ArpeggioFirstNoteFrameAddress;

		// Token: 0x040001AE RID: 430
		public int[] RestNoteCount;

		// Token: 0x040001AF RID: 431
		public int[] RestNoteSpan;

		// Token: 0x040001B0 RID: 432
		public List<string> VoiceMMLFromFile;

		// Token: 0x040001B1 RID: 433
		public List<string> RhythmVoiceMMLFromFile;

		// Token: 0x040001B2 RID: 434
		public bool[] RegisteredVoice;

		// Token: 0x040001B3 RID: 435
		public Dictionary<int, int> RegisteredVoiceLineNumber;

		// Token: 0x040001B4 RID: 436
		public string RegisteredRhythmCode;

		// Token: 0x040001B5 RID: 437
		public Dictionary<char, int> RegisteredRhythmCodeLineNumber;

		// Token: 0x040001B6 RID: 438
		public int[] VoiceHardwareEnvelope;

		// Token: 0x040001B7 RID: 439
		public List<MacroPointData> MacroPoint;

		// Token: 0x040001B8 RID: 440
		public Dictionary<string, string> MacroMML;

		// Token: 0x040001B9 RID: 441
		public Dictionary<string, int> MacroLine;

		// Token: 0x040001BA RID: 442
		public Dictionary<string, List<MMLSourcePosition>> MacroSourceMap;

		// Token: 0x040001BB RID: 443
		public List<MMLSourcePosition>[,] TrackSourceMap;

		// Token: 0x040001BC RID: 444
		public List<MMLSourcePosition> CurrentSourceMap;

		// Token: 0x040001BD RID: 445
		public Dictionary<string, List<string>> StepMacroMML;

		// Token: 0x040001BE RID: 446
		public string[,] SelectStepMacroName;

		// Token: 0x040001BF RID: 447
		public int[,] StepMacroCount;

		// Token: 0x040001C0 RID: 448
		public Dictionary<string, double> SeekMarker;

		// Token: 0x040001C1 RID: 449
		public Dictionary<string, int> SeekMarkerLine;

		// Token: 0x040001C2 RID: 450
		public bool[] SetQuantize;

		// Token: 0x040001C3 RID: 451
		public bool[] SetAtQuantize;

		// Token: 0x040001C4 RID: 452
		public bool[] SetVolume;

		// Token: 0x040001C5 RID: 453
		public List<string> ParameterText;

		// Token: 0x040001C6 RID: 454
		public List<string> InfoText;

		// Token: 0x040001C7 RID: 455
		public List<string> VoiceInfoMML;

		// Token: 0x040001C8 RID: 456
		public List<string> VoiceInfoText;

		// Token: 0x040001C9 RID: 457
		public Dictionary<string, string> AutoSetHeader;

		// Token: 0x040001CA RID: 458
		public List<PianoRollNote> PianoRollNotes;

		// Token: 0x040001CB RID: 459
		public List<PianoRollNote> PianoRollBars;

		// Token: 0x040001CC RID: 460
		public double AdjustPianoRoll;

		// Token: 0x040001CD RID: 461
		public int PianoRollLoop;

		// Token: 0x040001CE RID: 462
		public int ScrollTrack = -1;

		// Token: 0x040001CF RID: 463
		public bool SelectionPlay;

		// Token: 0x040001D0 RID: 464
		public int SelectionPlayStartLine;

		// Token: 0x040001D1 RID: 465
		public int SelectionPlayStartColumn;

		// Token: 0x040001D2 RID: 466
		public int SelectionPlayEndLine;

		// Token: 0x040001D3 RID: 467
		public int SelectionPlayEndColumn;

		// Token: 0x040001D4 RID: 468
		public bool SelectionPlayCurrentNoteTarget = true;

		// Token: 0x040001D5 RID: 469
		public int SelectionPlayCurrentNoteLine = -1;

		// Token: 0x040001D6 RID: 470
		public int SelectionPlayCurrentNoteColumn = -1;

		// Token: 0x040001D7 RID: 471
		public int[] SelectionPlayLastNoteLine = new int[] { -1, -1, -1, -1 };

		// Token: 0x040001D8 RID: 472
		public int[] SelectionPlayLastNoteColumn = new int[] { -1, -1, -1, -1 };

		// Token: 0x040001D9 RID: 473
		public int[] SelectionPlayCarryNoteLine = new int[] { -1, -1, -1, -1 };

		// Token: 0x040001DA RID: 474
		public int[] SelectionPlayCarryNoteColumn = new int[] { -1, -1, -1, -1 };

		// Token: 0x040001DB RID: 475
		public bool[] SelectionPlayFirstTargetNoteFound = new bool[4];

		// Token: 0x040001DC RID: 476
		private static readonly Dictionary<decimal, string> MusicalLengthTextCache = new Dictionary<decimal, string>();

		// Token: 0x02000045 RID: 69
		public sealed class SlowEvent
		{
			// Token: 0x040002C4 RID: 708
			public int Frame;

			// Token: 0x040002C5 RID: 709
			public int Pattern;

			// Token: 0x040002C6 RID: 710
			public int Sequence;
		}
	}
}
