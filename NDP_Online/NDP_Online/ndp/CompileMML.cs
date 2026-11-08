using System;
using System.Collections.Generic;
using System.Linq;

namespace ndp
{
	// Token: 0x0200000A RID: 10
	public class CompileMML
	{
		// Token: 0x06000057 RID: 87 RVA: 0x000049C0 File Offset: 0x00002BC0
		public CompileMML(Core Core)
		{
			this.c = Core;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00004A1C File Offset: 0x00002C1C
		public void Compile()
		{
			this.c.Index = 0;
			while (this.c.Index < this.c.LineBuffer.Length)
			{
				this.c.MarkAutoRepeatBoundary();
				char c = this.c.Read1(-1);
				if (c != 'R' && c != ' ' && c != '\0' && c != '\f')
				{
					this.QuantizeGeneratedRest[this.c.TrackIndex] = false;
				}
				if (c <= '\f')
				{
					if (c != '\0')
					{
						if (c != '\v')
						{
							if (c != '\f')
							{
								goto IL_017F;
							}
							this.c.ExecJ6();
						}
						else
						{
							this.c.ResetAlignFrameMod();
						}
					}
					else
					{
						this.SetPlayLine();
					}
				}
				else
				{
					switch (c)
					{
					case ' ':
						break;
					case '!':
						this.PlaySE();
						break;
					case '"':
						this.SetArpeggio();
						break;
					default:
						switch (c)
						{
						case '<':
							this.SetRelativeOctave('<');
							break;
						case '=':
						case '?':
						case '@':
							goto IL_017F;
						case '>':
							this.SetRelativeOctave('>');
							break;
						case 'A':
							this.KeyOn(10);
							break;
						case 'B':
							this.KeyOn(12);
							break;
						case 'C':
							this.KeyOn(1);
							break;
						case 'D':
							this.KeyOn(3);
							break;
						case 'E':
							this.KeyOn(5);
							break;
						case 'F':
							this.KeyOn(6);
							break;
						case 'G':
							this.KeyOn(8);
							break;
						default:
							if (c != 'O')
							{
								goto IL_017F;
							}
							this.SetOctave();
							break;
						}
						break;
					}
				}
				IL_068A:
				if (this.c.Message == null)
				{
					this.c.Index++;
					continue;
				}
				break;
				IL_017F:
				if (this.c.PortamentoStartNote[this.c.TrackIndex] > 0)
				{
					this.c.Message = "ポルタメント指定の第 2 パラメータには c ～ b, o, >, < のみ使用できます。";
					this.c.ErrorString = c.ToString();
					goto IL_068A;
				}
				if (this.c.GlissStartNote[this.c.TrackIndex] > 0)
				{
					this.c.Message = "グリッサンド指定の第 2 パラメータには c ～ b, o, >, < のみ使用できます。";
					this.c.ErrorString = c.ToString();
					goto IL_068A;
				}
				if (this.c.RandomStartNote[this.c.TrackIndex] > 0)
				{
					this.c.Message = "ランダムノート指定では c ～ b, o, >, < のみ使用できます。";
					this.c.ErrorString = c.ToString();
					goto IL_068A;
				}
				if (this.c.PortamentoNoteSetAndSaveOctave[this.c.TrackIndex] > 0)
				{
					this.c.Message = "ポルタメントノート指定では c ～ b, o, >, < のみ使用できます。";
					this.c.ErrorString = c.ToString();
					goto IL_068A;
				}
				if (this.c.EnableArpeggio[this.c.TrackIndex] && c != 'R' && c != 'J' && c != '/')
				{
					this.c.Message = "アルペジオ内では c ～ b, r, o, >, <, / のみ使用できます。";
					this.c.ErrorString = c.ToString();
					goto IL_068A;
				}
				if (c <= '/')
				{
					switch (c)
					{
					case '&':
						goto IL_068A;
					case '\'':
						break;
					case '(':
						this.c.DataWrite(133);
						this.c.Legato[this.c.TrackIndex] = true;
						goto IL_068A;
					case ')':
						this.c.DataWrite(132);
						this.c.LegatoOffFlagSet[this.c.TrackIndex] = true;
						goto IL_068A;
					case '*':
						this.SetMixMode();
						goto IL_068A;
					default:
						if (c == '/')
						{
							int num = this.c.NumberRead(false, true);
							if (!this.c.EnableArpeggio[this.c.TrackIndex])
							{
								this.RepeatEscape(true);
								goto IL_068A;
							}
							if (this.c.ArpeggioPoint1[this.c.TrackIndex] < 0)
							{
								this.c.ArpeggioPoint1[this.c.TrackIndex] = this.c.ArpeggioNoteNumber[this.c.TrackIndex].Count - 1;
								if (num > 0)
								{
									this.c.ArpeggioCount1[this.c.TrackIndex] = num;
									goto IL_068A;
								}
								goto IL_068A;
							}
							else
							{
								if (this.c.ArpeggioPoint2[this.c.TrackIndex] >= 0)
								{
									this.c.Message = "アルペジオ内で 3 個目の「 / 」が検出されました。";
									this.c.ErrorString = "/";
									goto IL_068A;
								}
								this.c.ArpeggioPoint2[this.c.TrackIndex] = this.c.ArpeggioNoteNumber[this.c.TrackIndex].Count - 1;
								if (num > 0)
								{
									this.c.ArpeggioCount2[this.c.TrackIndex] = num;
									goto IL_068A;
								}
								goto IL_068A;
							}
						}
						break;
					}
				}
				else
				{
					switch (c)
					{
					case '?':
						this.SelectStepMacro();
						goto IL_068A;
					case '@':
						this.AtCommand();
						goto IL_068A;
					case 'A':
					case 'B':
					case 'C':
					case 'D':
					case 'E':
					case 'F':
					case 'G':
					case 'H':
					case 'I':
					case 'O':
					case 'U':
					case 'W':
					case 'Z':
						break;
					case 'J':
						this.SetJump(0);
						goto IL_068A;
					case 'K':
						this.SetKCommand();
						goto IL_068A;
					case 'L':
						this.SetLength();
						goto IL_068A;
					case 'M':
						this.HardwareEnvelopePeriod();
						goto IL_068A;
					case 'N':
						this.SetNoiseFrequency();
						goto IL_068A;
					case 'P':
						this.SetPortamento();
						goto IL_068A;
					case 'Q':
						this.SetQ();
						goto IL_068A;
					case 'R':
						this.KeyOn(0);
						goto IL_068A;
					case 'S':
						this.HardwareEnvelope();
						goto IL_068A;
					case 'T':
						this.SetTempo();
						goto IL_068A;
					case 'V':
						this.VCommand();
						goto IL_068A;
					case 'X':
						this.StepMacro();
						goto IL_068A;
					case 'Y':
						this.SetYCommand();
						goto IL_068A;
					case '[':
						if (!this.c.GetTimeCompile && !this.c.InfoCompile && this.c.SoundId == 0 && this.c.RepeatMode[this.c.TrackIndex] > 0 && this.c.RepeatMode[this.c.TrackIndex] < 4)
						{
							this.RepeatStart(true, false);
							goto IL_068A;
						}
						this.RepeatDeploy();
						goto IL_068A;
					case '\\':
						this.SetDetune();
						goto IL_068A;
					case ']':
						this.RepeatEnd(true);
						goto IL_068A;
					default:
						if (c == '|')
						{
							this.AddBar();
							goto IL_068A;
						}
						break;
					}
				}
				this.c.Message = "文法エラーです。";
				this.c.ErrorString = c.ToString();
				goto IL_068A;
			}
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000050F4 File Offset: 0x000032F4
		public void SetPlayLine()
		{
			int num;
			int num2;
			if (this.c.Index < 0 || this.c.Index > this.c.LineBuffer.Length - 10 || this.c.LineBuffer[this.c.Index] != '\0' || !int.TryParse(this.c.LineBuffer.Substring(this.c.Index + 1, 4), out num) || !int.TryParse(this.c.LineBuffer.Substring(this.c.Index + 5, 5), out num2))
			{
				this.c.Message = "行番号情報の解析に失敗しました。";
				this.c.ErrorString = null;
				return;
			}
			this.c.LineNumber = num;
			this.c.PlayLineColumn = num2;
			this.c.PlayLineBufferIndex = this.c.Index + 10;
			if (this.c.GetTimeCompile)
			{
				this.c.LinePlayLineNumber[this.c.TrackIndex].Add(this.c.LineNumber);
				int num3 = this.c.BeforeLoopFrame[this.c.TrackIndex, this.c.SoundId];
				int num4 = ((num3 > 0) ? num3 : 0) + this.c.TrackFrame[this.c.TrackIndex, this.c.SoundId];
				this.c.LinePlayTime[this.c.TrackIndex].Add((double)num4 / 59.94080511738504);
			}
			this.c.Index += 9;
		}

		// Token: 0x0600005A RID: 90 RVA: 0x000052C0 File Offset: 0x000034C0
		protected int AddNotePlayPosition()
		{
			this.c.SelectionPlayCurrentNoteTarget = !this.c.SelectionPlay;
			if (this.c.SoundId != 0 || this.c.LineNumber <= 0)
			{
				return -1;
			}
			int num = this.c.LineNumber;
			int num2 = this.c.PlayLineColumn + (this.c.Index - this.c.PlayLineBufferIndex);
			bool flag = this.c.IsSelectionPlayPosition(num, num2);
			bool flag2 = false;
			List<MMLSourcePosition> currentSourceMap = this.c.CurrentSourceMap;
			if (currentSourceMap != null && this.c.Index >= 0 && this.c.Index < currentSourceMap.Count)
			{
				MMLSourcePosition mmlsourcePosition = currentSourceMap[this.c.Index];
				flag = mmlsourcePosition.SelectionPlayTarget;
				flag2 = mmlsourcePosition.MacroExpanded;
				if (mmlsourcePosition.IsValid)
				{
					if (!mmlsourcePosition.Matches(this.c.Read1(-1)))
					{
						return -1;
					}
					num = mmlsourcePosition.Line;
					num2 = mmlsourcePosition.Column;
				}
			}
			this.c.SelectionPlayCurrentNoteLine = num;
			this.c.SelectionPlayCurrentNoteColumn = num2;
			this.c.SelectionPlayCurrentNoteTarget = !this.c.SelectionPlay || flag;
			if (!this.c.GetTimeCompile || (this.c.SelectionPlay && !flag) || num2 < 0)
			{
				return -1;
			}
			int num3 = this.c.BeforeLoopFrame[this.c.TrackIndex, this.c.SoundId];
			int num4 = ((num3 > 0) ? num3 : 0) + this.c.TrackFrame[this.c.TrackIndex, this.c.SoundId];
			this.c.NotePlayTime[this.c.TrackIndex].Add((double)num4 / 59.94080511738504);
			this.c.NotePlayLineNumber[this.c.TrackIndex].Add(num);
			this.c.NotePlayColumn[this.c.TrackIndex].Add(num2);
			NotePlayParameter notePlayParameter = new NotePlayParameter
			{
				TrackIndex = this.c.TrackIndex,
				LineNumber = num,
				Note = this.c.Read1(-1),
				NoteName = char.ToUpperInvariant(this.c.Read1(-1)).ToString(),
				WrittenNoteName = char.ToUpperInvariant(this.c.Read1(-1)).ToString(),
				NoteNameAccidental = 0,
				Octave = this.c.Octave[this.c.TrackIndex],
				KeyShift = this.c.KeyShift[this.c.TrackIndex],
				RelativeKeyShift = this.c.RelativeKeyShift[this.c.TrackIndex],
				Volume = this.c.Volume[this.c.TrackIndex],
				VolumeOffset = this.c.VolumeOffset[this.c.TrackIndex],
				VolumeIncrease = this.c.vi[this.c.TrackIndex],
				VolumeIncreaseMax = this.c.viMax[this.c.TrackIndex],
				VoiceNumber = this.c.VoiceNumber[this.c.TrackIndex],
				DelayVoiceNumber = this.c.DelayVoiceNumber[this.c.TrackIndex],
				Quantize = this.c.Quantize[this.c.TrackIndex],
				AtQuantize = this.c.AtQuantize[this.c.TrackIndex],
				Detune = this.c.Detune[this.c.TrackIndex],
				InterruptTrack = this.c.UseRhythmTrackAtC + 1,
				RepeatMode = this.c.RepeatMode[this.c.TrackIndex],
				GlissLegato = this.c.GlissLegato[this.c.TrackIndex],
				Portamento = this.c.Portamento[this.c.TrackIndex],
				Tempo = this.c.Tempo[this.c.TrackIndex],
				ReleaseVolume = this.c.ReleaseVolume[this.c.TrackIndex],
				ReleaseRate = this.c.ReleaseRate[this.c.TrackIndex],
				ReleaseDelay = this.c.ReleaseDelay[this.c.TrackIndex],
				ReleaseDelayFrame = this.c.ReleaseDelayFrame[this.c.TrackIndex],
				PitchEnvelopeNumber = this.c.PitchEnvelopeNumber[this.c.TrackIndex],
				PitchEnvelopeType = this.c.UsePitchEnvelopeType[this.c.TrackIndex],
				HardwareEnvelopeNumber = this.c.HardwareEnvelopeNumber[this.c.TrackIndex],
				HardwareEnvelopeVolumeDecayFrame = this.c.HardwareEnvelopeVolumeDecayFrame[this.c.TrackIndex],
				NoteEnvelopeNumber = this.c.NoteEnvelopeNumber[this.c.TrackIndex],
				SelfDelayCount = this.c.SelfDelayCount[this.c.TrackIndex],
				SelfDelayVolume = this.c.SelfDelayVolume[this.c.TrackIndex],
				CurrentFrame = num4,
				VolumeLimit = this.c.VolumeLimit[this.c.TrackIndex],
				LengthText = "",
				FrameLength = 0,
				GateFrameLength = 0,
				SelectionPlayTarget = flag,
				MacroExpanded = flag2
			};
			this.c.NotePlayParameters[this.c.TrackIndex].Add(notePlayParameter);
			return this.c.NotePlayParameters[this.c.TrackIndex].Count - 1;
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00005918 File Offset: 0x00003B18
		private void SetNotePlayParameterActual(int ParameterIndex, int NoteNumber, List<byte> LengthData)
		{
			if (!this.c.GetTimeCompile || ParameterIndex < 0)
			{
				return;
			}
			if (ParameterIndex >= this.c.NotePlayParameters[this.c.TrackIndex].Count)
			{
				return;
			}
			NotePlayParameter notePlayParameter = this.c.NotePlayParameters[this.c.TrackIndex][ParameterIndex];
			notePlayParameter.Note = ((NoteNumber > 0) ? 'N' : 'R');
			if (NoteNumber > 0)
			{
				string[] array = new string[]
				{
					"C", "C♯", "D", "D♯", "E", "F", "F♯", "G", "G♯", "A",
					"A♯", "B"
				};
				string[] array2 = new string[]
				{
					"C", "D♭", "D", "E♭", "E", "F", "G♭", "G", "A♭", "A",
					"B♭", "B"
				};
				string[] array3 = ((notePlayParameter.NoteNameAccidental < 0) ? array2 : array);
				notePlayParameter.NoteName = array3[(NoteNumber - 1) % 12];
			}
			else
			{
				notePlayParameter.NoteName = "REST";
			}
			notePlayParameter.Volume = this.c.Volume[this.c.TrackIndex];
			notePlayParameter.VolumeLimit = this.c.VolumeLimit[this.c.TrackIndex];
			notePlayParameter.VolumeOffset = this.c.VolumeOffset[this.c.TrackIndex];
			notePlayParameter.VolumeIncrease = this.c.vi[this.c.TrackIndex];
			notePlayParameter.VolumeIncreaseMax = this.c.viMax[this.c.TrackIndex];
			notePlayParameter.VoiceNumber = this.c.VoiceNumber[this.c.TrackIndex];
			notePlayParameter.DelayVoiceNumber = this.c.DelayVoiceNumber[this.c.TrackIndex];
			notePlayParameter.Quantize = this.c.Quantize[this.c.TrackIndex];
			notePlayParameter.AtQuantize = this.c.AtQuantize[this.c.TrackIndex];
			notePlayParameter.Detune = this.c.Detune[this.c.TrackIndex];
			notePlayParameter.InterruptTrack = this.c.UseRhythmTrackAtC + 1;
			notePlayParameter.RepeatMode = this.c.RepeatMode[this.c.TrackIndex];
			notePlayParameter.GlissLegato = this.c.GlissLegato[this.c.TrackIndex];
			notePlayParameter.Portamento = this.c.Portamento[this.c.TrackIndex];
			notePlayParameter.Tempo = this.c.Tempo[this.c.TrackIndex];
			notePlayParameter.KeyShift = this.c.KeyShift[this.c.TrackIndex];
			notePlayParameter.RelativeKeyShift = this.c.RelativeKeyShift[this.c.TrackIndex];
			notePlayParameter.ReleaseVolume = this.c.ReleaseVolume[this.c.TrackIndex];
			notePlayParameter.ReleaseRate = this.c.ReleaseRate[this.c.TrackIndex];
			notePlayParameter.ReleaseDelay = this.c.ReleaseDelay[this.c.TrackIndex];
			notePlayParameter.ReleaseDelayFrame = this.c.ReleaseDelayFrame[this.c.TrackIndex];
			notePlayParameter.PitchEnvelopeNumber = this.c.PitchEnvelopeNumber[this.c.TrackIndex];
			notePlayParameter.PitchEnvelopeType = this.c.UsePitchEnvelopeType[this.c.TrackIndex];
			notePlayParameter.HardwareEnvelopeNumber = this.c.HardwareEnvelopeNumber[this.c.TrackIndex];
			notePlayParameter.HardwareEnvelopeVolumeDecayFrame = this.c.HardwareEnvelopeVolumeDecayFrame[this.c.TrackIndex];
			notePlayParameter.NoteEnvelopeNumber = this.c.NoteEnvelopeNumber[this.c.TrackIndex];
			notePlayParameter.SelfDelayCount = this.c.SelfDelayCount[this.c.TrackIndex];
			notePlayParameter.SelfDelayVolume = this.c.SelfDelayVolume[this.c.TrackIndex];
			notePlayParameter.LengthText = this.c.LastNoteLengthText[this.c.TrackIndex];
			notePlayParameter.FrameLength = ((LengthData == null) ? 0 : this.SumLengthData(LengthData));
			double num = (double)notePlayParameter.FrameLength / 8.0 * (double)notePlayParameter.Quantize;
			if (notePlayParameter.AtQuantize > 0 && notePlayParameter.AtQuantize < 129)
			{
				num -= (double)notePlayParameter.AtQuantize;
				if (num < 1.0)
				{
					num = 1.0;
				}
			}
			if (notePlayParameter.AtQuantize > 128 && notePlayParameter.AtQuantize < 256 && notePlayParameter.Quantize < 8)
			{
				num += (double)(notePlayParameter.AtQuantize - 128);
				if (num > (double)notePlayParameter.FrameLength)
				{
					num = (double)notePlayParameter.FrameLength;
				}
			}
			if (notePlayParameter.AtQuantize > 255)
			{
				num = (double)Math.Min(notePlayParameter.AtQuantize - 257, notePlayParameter.FrameLength);
			}
			notePlayParameter.GateFrameLength = ((notePlayParameter.FrameLength > 0) ? ((int)num) : 0);
			this.SaveNotePlayParameterState(notePlayParameter, this.c.TrackIndex);
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00005ECC File Offset: 0x000040CC
		protected void SaveNotePlayParameterState(NotePlayParameter Parameter, int ti)
		{
			if (Parameter == null || ti < 0 || ti >= 4)
			{
				return;
			}
			Parameter.Octave = this.c.Octave[ti];
			Parameter.KeyShift = this.c.KeyShift[ti];
			Parameter.RelativeKeyShift = this.c.RelativeKeyShift[ti];
			Parameter.Volume = this.c.Volume[ti];
			Parameter.VolumeOffset = this.c.VolumeOffset[ti];
			Parameter.VolumeIncrease = this.c.vi[ti];
			Parameter.VolumeIncreaseMax = this.c.viMax[ti];
			Parameter.VoiceNumber = this.c.VoiceNumber[ti];
			Parameter.DelayVoiceNumber = this.c.DelayVoiceNumber[ti];
			Parameter.Quantize = this.c.Quantize[ti];
			Parameter.AtQuantize = this.c.AtQuantize[ti];
			Parameter.Detune = this.c.Detune[ti];
			Parameter.InterruptTrack = this.c.GetInterruptTrackAtFrame(Parameter.CurrentFrame);
			Parameter.RepeatMode = this.c.RepeatMode[ti];
			Parameter.GlissLegato = this.c.GlissLegato[ti];
			Parameter.Portamento = this.c.Portamento[ti];
			Parameter.Tempo = this.c.Tempo[ti];
			Parameter.ReleaseVolume = this.c.ReleaseVolume[ti];
			Parameter.ReleaseRate = this.c.ReleaseRate[ti];
			Parameter.ReleaseDelay = this.c.ReleaseDelay[ti];
			Parameter.ReleaseDelayFrame = this.c.ReleaseDelayFrame[ti];
			Parameter.PitchEnvelopeNumber = this.c.PitchEnvelopeNumber[ti];
			Parameter.PitchEnvelopeType = this.c.UsePitchEnvelopeType[ti];
			Parameter.HardwareEnvelopeNumber = this.c.HardwareEnvelopeNumber[ti];
			Parameter.HardwareEnvelopeVolumeDecayFrame = this.c.HardwareEnvelopeVolumeDecayFrame[ti];
			Parameter.NoteEnvelopeNumber = this.c.NoteEnvelopeNumber[ti];
			Parameter.SelfDelayCount = this.c.SelfDelayCount[ti];
			Parameter.SelfDelayVolume = this.c.SelfDelayVolume[ti];
			Parameter.VolumeLimit = this.c.VolumeLimit[ti];
			Parameter.KeySign = ((this.c.KeySign[ti] == null) ? null : ((int[])this.c.KeySign[ti].Clone()));
			Parameter.QuantizeBackup = this.c.QuantizeBackup[ti];
			Parameter.QuantizeCount = this.c.QuantizeCount[ti];
			Parameter.QuantizeNumber = this.c.QuantizeNumber[ti];
			Parameter.AtQuantizeBackup = this.c.AtQuantizeBackup[ti];
			Parameter.AtQuantizeCount = this.c.AtQuantizeCount[ti];
			Parameter.AtQuantizeNumber = this.c.AtQuantizeNumber[ti];
			Parameter.DefaultLength = ((this.c.DefaultLength[ti] == null) ? null : ((decimal[])this.c.DefaultLength[ti].Clone()));
			Parameter.DefaultLengthAddString = ((this.c.DefaultLengthAddString[ti] == null) ? null : ((string[])this.c.DefaultLengthAddString[ti].Clone()));
			Parameter.VolumeSequenceNumber = this.c.VolumeSequenceNumber[ti];
			Parameter.QuantizeSequenceNumber = this.c.QuantizeSequenceNumber[ti];
			Parameter.AtQuantizeSequenceNumber = this.c.AtQuantizeSequenceNumber[ti];
			Parameter.DetuneSequenceNumber = this.c.DetuneSequenceNumber[ti];
			Parameter.HardwareEnvelopeDetuneSequenceNumber = this.c.HardwareEnvelopeDetuneSequenceNumber[ti];
			Parameter.HardwareEnvelopeKeyShiftSequenceNumber = this.c.HardwareEnvelopeKeyShiftSequenceNumber[ti];
			Parameter.HardwareEnvelopeDecaySequenceNumber = this.c.HardwareEnvelopeDecaySequenceNumber[ti];
			Parameter.ArpeggioDefaultLength = ((this.c.ArpeggioDefaultLength[ti] == null) ? null : ((decimal[])this.c.ArpeggioDefaultLength[ti].Clone()));
			Parameter.ArpeggioLegato = this.c.ArpeggioLegato[ti];
			Parameter.ArpeggioRest = this.c.ArpeggioRest[ti];
			Parameter.ArpeggioEndAtQuantize = this.c.ArpeggioEndAtQuantize[ti];
			Parameter.ArpeggioVolumeSequenceNumber = this.c.ArpeggioVolumeSequenceNumber[ti];
			Parameter.ArpeggioQuantizeSequenceNumber = this.c.ArpeggioQuantizeSequenceNumber[ti];
			Parameter.ArpeggioAtQuantizeSequenceNumber = this.c.ArpeggioAtQuantizeSequenceNumber[ti];
			Parameter.ArpeggioDetuneSequenceNumber = this.c.ArpeggioDetuneSequenceNumber[ti];
			Parameter.ArpeggioHardwareEnvelopeDetuneSequenceNumber = this.c.ArpeggioHardwareEnvelopeDetuneSequenceNumber[ti];
			Parameter.ArpeggioHardwareEnvelopeKeyShiftSequenceNumber = this.c.ArpeggioHardwareEnvelopeKeyShiftSequenceNumber[ti];
			Parameter.ArpeggioHardwareEnvelopeDecaySequenceNumber = this.c.ArpeggioHardwareEnvelopeDecaySequenceNumber[ti];
			Parameter.RhythmCodes = this.c.StoreRhythmCode ?? "";
			Parameter.RhythmVolume = ((this.c.RhythmVolume == null) ? null : ((int[])this.c.RhythmVolume.Clone()));
		}

		// Token: 0x0600005D RID: 93 RVA: 0x000063D4 File Offset: 0x000045D4
		private void KeyOn(int NoteNumber)
		{
			int num = this.AddNotePlayPosition();
			if (this.c.SelectionPlay && this.c.GetTimeCompile && this.c.SoundId == 0)
			{
				int trackIndex = this.c.TrackIndex;
				if (this.c.SelectionPlayCurrentNoteTarget && !this.c.SelectionPlayFirstTargetNoteFound[trackIndex])
				{
					this.c.SelectionPlayFirstTargetNoteFound[trackIndex] = true;
					if (this.c.Quantize[trackIndex] == 0)
					{
						this.c.SelectionPlayCarryNoteLine[trackIndex] = this.c.SelectionPlayLastNoteLine[trackIndex];
						this.c.SelectionPlayCarryNoteColumn[trackIndex] = this.c.SelectionPlayLastNoteColumn[trackIndex];
					}
				}
				if (!this.c.SelectionPlayCurrentNoteTarget && NoteNumber > 0)
				{
					this.c.SelectionPlayLastNoteLine[trackIndex] = this.c.SelectionPlayCurrentNoteLine;
					this.c.SelectionPlayLastNoteColumn[trackIndex] = this.c.SelectionPlayCurrentNoteColumn;
				}
			}
			this.c.SkipSpace();
			if (this.c.PitchEnvelopeNumber[this.c.TrackIndex] >= 0 && this.CheckUseHardwareEnvelopeVolumeHarf("@M/@P", "音符の手前で解除"))
			{
				this.PitchEnvelopeOff();
			}
			if (this.c.ReleaseRate[this.c.TrackIndex] > 0 && this.CheckUseHardwareEnvelopeVolumeHarf("リリース設定", "音符の手前で解除"))
			{
				this.c.DataWrite(163);
				this.c.DataWrite(0);
				this.c.ReleaseRate[this.c.TrackIndex] = 0;
			}
			if (this.c.LegatoOffFlagSet[this.c.TrackIndex])
			{
				this.c.Legato[this.c.TrackIndex] = false;
				this.c.LegatoOffFlagSet[this.c.TrackIndex] = false;
			}
			if (NoteNumber > 0)
			{
				string text;
				switch (NoteNumber)
				{
				case 1:
					text = "C";
					goto IL_027D;
				case 3:
					text = "D";
					goto IL_027D;
				case 5:
					text = "E";
					goto IL_027D;
				case 6:
					text = "F";
					goto IL_027D;
				case 8:
					text = "G";
					goto IL_027D;
				case 10:
					text = "A";
					goto IL_027D;
				case 12:
					text = "B";
					goto IL_027D;
				}
				char c = char.ToUpperInvariant(this.c.Read1(-1));
				text = c.ToString();
				IL_027D:
				int num2 = this.c.KeySign[this.c.TrackIndex][NoteNumber];
				NoteNumber = (this.c.Octave[this.c.TrackIndex] - 1) * 12 + this.c.KeyShift[this.c.TrackIndex] + this.c.RelativeKeyShift[this.c.TrackIndex] + NoteNumber + num2;
				this.c.Index++;
				int num3 = 0;
				bool flag = false;
				bool flag2 = true;
				int num4 = 0;
				int num5 = ((num2 < 0) ? (-1) : ((num2 > 0) ? 1 : 0));
				bool flag3 = false;
				while (flag2)
				{
					if (this.c.Index >= this.c.LineBuffer.Length)
					{
						flag = true;
						break;
					}
					c = this.c.Read1(-1);
					if (c <= '+')
					{
						if (c != '#')
						{
							if (c != '+')
							{
								goto IL_040C;
							}
							if (num4 > 0 && num4 != 2)
							{
								flag2 = false;
							}
							else
							{
								num3++;
								num4 = 2;
								num5 = 1;
							}
						}
						else if (!this.c.SharpFrameMode)
						{
							if (num4 > 0 && num4 != 3)
							{
								flag2 = false;
							}
							else
							{
								num3++;
								num4 = 3;
								num5 = 1;
							}
						}
						else
						{
							flag2 = false;
						}
					}
					else if (c != '-')
					{
						if (c != '=')
						{
							goto IL_040C;
						}
						NoteNumber -= num2;
						num5 = 0;
						flag3 = true;
						this.c.Index++;
						flag2 = false;
					}
					else if (num4 > 0 && num4 != 1)
					{
						flag2 = false;
					}
					else
					{
						num3--;
						num4 = 1;
						num5 = -1;
					}
					IL_040F:
					this.c.Index++;
					continue;
					IL_040C:
					flag2 = false;
					goto IL_040F;
				}
				this.c.Index--;
				if (!flag)
				{
					this.c.Index--;
				}
				NoteNumber += num3;
				if (this.c.GetTimeCompile && num >= 0 && num < this.c.NotePlayParameters[this.c.TrackIndex].Count)
				{
					NotePlayParameter notePlayParameter = this.c.NotePlayParameters[this.c.TrackIndex][num];
					int num6 = (flag3 ? 0 : (num2 + num3));
					notePlayParameter.WrittenNoteName = text;
					if (num6 > 0)
					{
						NotePlayParameter notePlayParameter2 = notePlayParameter;
						notePlayParameter2.WrittenNoteName += new string('♯', num6);
					}
					if (num6 < 0)
					{
						NotePlayParameter notePlayParameter3 = notePlayParameter;
						notePlayParameter3.WrittenNoteName += new string('♭', -num6);
					}
					notePlayParameter.NoteNameAccidental = (flag3 ? 0 : num5);
					notePlayParameter.Octave = this.c.Octave[this.c.TrackIndex];
					notePlayParameter.KeyShift = this.c.KeyShift[this.c.TrackIndex];
					notePlayParameter.RelativeKeyShift = this.c.RelativeKeyShift[this.c.TrackIndex];
				}
				if (NoteNumber < 1)
				{
					this.c.Message = "o1c を下回りました。";
					Core core = this.c;
					c = this.c.Read1(-1);
					core.ErrorString = c.ToString();
				}
				if (NoteNumber > 95)
				{
					this.c.Message = "o8a+ を上回りました。";
					Core core2 = this.c;
					c = this.c.Read1(-1);
					core2.ErrorString = c.ToString();
				}
			}
			if (this.c.PortamentoNoteSetAndSaveOctave[this.c.TrackIndex] > 0)
			{
				int num7 = 0;
				if (this.c.CheckNextCharacter(','))
				{
					num7 = this.c.NumberRead(true, true);
					if (num7 < 0 || num7 > 1)
					{
						this.c.RangeNotice("ポルタメントノート指定の第 2 パラメータ", 0, 1, "\n0: 直後の 1 音に適用\n1: 以降すべての音符に適用");
						this.c.ErrorParameter("P_…", num7);
						this.c.Octave[this.c.TrackIndex] = this.c.PortamentoNoteSetAndSaveOctave[this.c.TrackIndex];
						this.c.PortamentoNoteSetAndSaveOctave[this.c.TrackIndex] = 0;
						return;
					}
				}
				if (num7 == 1)
				{
					this.c.PortamentoKeyShift[this.c.TrackIndex] = 0;
					this.c.PortamentoContinuousMode[this.c.TrackIndex] = 2;
					this.c.PortamentoContinuousKeyShift[this.c.TrackIndex] = 0;
					this.c.PortamentoContinuousStartNote[this.c.TrackIndex] = NoteNumber;
				}
				else if (this.c.PortamentoCommandSet[this.c.TrackIndex])
				{
					this.c.DataWrite(168);
					this.c.DataWrite(NoteNumber);
					this.ClearPortamentoContinuous();
				}
				else
				{
					this.c.Message = "ポルタメント開始音は P コマンドを有効にした状態で設定してください。";
					Core core3 = this.c;
					ReadOnlySpan<char> readOnlySpan = "P_…";
					char c = this.c.Read1(-1);
					core3.ErrorString = readOnlySpan + new ReadOnlySpan<char>(ref c);
				}
				this.c.Octave[this.c.TrackIndex] = this.c.PortamentoNoteSetAndSaveOctave[this.c.TrackIndex];
				this.c.PortamentoNoteSetAndSaveOctave[this.c.TrackIndex] = 0;
				return;
			}
			if (NoteNumber > 0 && this.c.PortamentoKeyShift[this.c.TrackIndex] != 0)
			{
				int num8 = NoteNumber + this.c.PortamentoKeyShift[this.c.TrackIndex];
				this.c.PortamentoKeyShift[this.c.TrackIndex] = 0;
				if (num8 < 1 || num8 > 95)
				{
					this.c.Message = "P_K コマンドによるポルタメント開始音が o1c ～ o8a+ の範囲を超えています。";
					this.c.ErrorString = "P_K";
					return;
				}
				if (!this.c.PortamentoCommandSet[this.c.TrackIndex])
				{
					this.c.Message = "ポルタメント開始音は P コマンドを有効にした状態で設定してください。";
					this.c.ErrorString = "P_K";
					return;
				}
				this.c.DataWrite(168);
				this.c.DataWrite(num8);
			}
			if (this.c.EnableArpeggio[this.c.TrackIndex])
			{
				this.c.ArpeggioNoteNumber[this.c.TrackIndex].Add(NoteNumber);
				return;
			}
			if (this.c.CheckNextCharacter('_'))
			{
				if (NoteNumber == 0)
				{
					this.c.Message = "ポルタメントの開始音に休符が指定されています。";
					this.c.ErrorString = "R_";
					return;
				}
				this.c.PortamentoStartNote[this.c.TrackIndex] = NoteNumber;
				this.c.DetunePortamentoMode[this.c.TrackIndex] = this.c.CheckNextCharacter('\\');
				return;
			}
			else if (this.c.CheckNextCharacter('\''))
			{
				if (NoteNumber == 0)
				{
					this.c.Message = "グリッサンドの開始音に休符が指定されています。";
					this.c.ErrorString = "R'";
					return;
				}
				this.c.GlissStartNote[this.c.TrackIndex] = NoteNumber;
				return;
			}
			else if (this.c.CheckNextCharacter('~'))
			{
				if (NoteNumber == 0)
				{
					this.c.Message = "ランダムノートの開始音に休符が指定されています。";
					this.c.ErrorString = "R~";
					return;
				}
				this.c.RandomStartNote[this.c.TrackIndex] = NoteNumber;
				return;
			}
			else
			{
				if (NoteNumber > 0 && this.c.PortamentoCommandSet[this.c.TrackIndex])
				{
					if (this.c.PortamentoContinuousMode[this.c.TrackIndex] == 1)
					{
						int num9 = NoteNumber + this.c.PortamentoContinuousKeyShift[this.c.TrackIndex];
						if (num9 < 1 || num9 > 95)
						{
							this.c.Message = "P_K コマンドによるポルタメント開始音が o1c ～ o8a+ の範囲を超えています。";
							this.c.ErrorString = "P_K";
							return;
						}
						this.c.DataWrite(168);
						this.c.DataWrite(num9);
					}
					else if (this.c.PortamentoContinuousMode[this.c.TrackIndex] == 2)
					{
						this.c.DataWrite(168);
						this.c.DataWrite(this.c.PortamentoContinuousStartNote[this.c.TrackIndex]);
					}
				}
				List<byte> list = this.c.SetNoteLength(true, false);
				if (this.c.Message != null)
				{
					return;
				}
				this.SetFirstVoice();
				bool flag4 = false;
				this.c.Index++;
				if (this.c.Read1(-1) != '&')
				{
					if (this.c.Slur[this.c.TrackIndex])
					{
						if (!this.c.Legato[this.c.TrackIndex])
						{
							this.c.DataWrite(132);
						}
						this.c.Slur[this.c.TrackIndex] = false;
					}
					this.c.Index--;
				}
				else if (!this.c.Slur[this.c.TrackIndex])
				{
					if (this.c.GlissStartNote[this.c.TrackIndex] == 0)
					{
						if (!this.c.Legato[this.c.TrackIndex])
						{
							this.c.DataWrite(133);
						}
						this.c.Slur[this.c.TrackIndex] = true;
					}
					else
					{
						flag4 = true;
					}
				}
				bool flag5 = true;
				int num10 = this.c.PortamentoStartNote[this.c.TrackIndex];
				if (num10 > 0 && this.CheckUseHardwareEnvelopeVolumeHarf("ポルタメント", "通常の音符に置換"))
				{
					this.c.FrameAnalyzeNote[this.c.TrackIndex] = num10;
					num10 = 0;
					this.c.PortamentoStartNote[this.c.TrackIndex] = 0;
				}
				if (num10 > 0)
				{
					if (this.c.PortamentoReverse[this.c.TrackIndex])
					{
						int num11 = NoteNumber;
						NoteNumber = num10;
						num10 = num11;
					}
					int i = this.SumLengthData(list);
					if (i <= this.c.PortamentoWait[this.c.TrackIndex])
					{
						NoteNumber = num10;
					}
					else
					{
						if (this.c.PortamentoMode[this.c.TrackIndex] == 1)
						{
							i = i / 8 * this.c.QuantizeBackup[this.c.TrackIndex] - this.c.AtQuantizeBackup[this.c.TrackIndex];
						}
						this.c.FrameAnalyzeLength[this.c.TrackIndex] = i;
						int num12 = Math.Abs(this.c.PitchTable[num10 - 1] - this.c.PitchTable[NoteNumber - 1]);
						i -= this.c.PortamentoWait[this.c.TrackIndex];
						double num13 = (double)num12 / (double)i;
						this.c.AddPianoRollNote(NoteNumber, list, 0, 0, num10);
						if (this.c.DetunePortamentoMode[this.c.TrackIndex])
						{
							string text2 = "疑似ポルタメント展開 : ";
							this.c.DetunePortamentoMode[this.c.TrackIndex] = false;
							flag5 = false;
							if (this.c.PortamentoCommandSet[this.c.TrackIndex])
							{
								this.c.DataWrite(136);
								this.c.DataWrite(0);
								text2 += "p0";
							}
							this.c.DataWrite(143);
							this.c.DataWrite(1);
							if (this.c.PitchTable[num10] > this.c.PitchTable[NoteNumber])
							{
								num13 = -num13;
							}
							if (!this.c.Slur[this.c.TrackIndex] && !this.c.Legato[this.c.TrackIndex])
							{
								this.c.DataWrite(133);
								text2 += " (";
							}
							this.c.DataWrite(this.c.IsCurrentNoteMuted() ? 0 : num10);
							this.c.DataWrite(this.c.PortamentoWait[this.c.TrackIndex]);
							if (this.c.PortamentoWait[this.c.TrackIndex] > 0)
							{
								text2 = string.Concat(new string[]
								{
									text2,
									" ",
									this.GetMMLFromNoteNumber(num10).Trim(),
									"%",
									this.c.PortamentoWait[this.c.TrackIndex].ToString()
								});
							}
							double num14 = 0.0;
							int num15 = 0;
							int num16 = 1;
							for (int j = 0; j < i - 1; j++)
							{
								num14 += num13;
								if (num13 < 0.0)
								{
									if (num14 < (double)(-(double)num12))
									{
										num14 = (double)(-(double)num12);
									}
								}
								else if (num14 > (double)num12)
								{
									num14 = (double)num12;
								}
								if (num15 != (int)num14)
								{
									num15 = (int)num14;
									int num17 = (int)(num14 + (double)this.c.Detune[this.c.TrackIndex]);
									if (num17 < 127 || num17 > 127)
									{
										int num18 = (int)((short)num14) + this.c.Detune[this.c.TrackIndex];
										this.c.DataWrite(142);
										this.c.DataWrite(num18 & 255);
										this.c.DataWrite(num18 >> 8);
										text2 = text2 + " \\" + (-num18).ToString();
									}
									else
									{
										this.c.DataWrite(135);
										this.c.DataWrite(num17);
										text2 = text2 + " \\" + (-num17).ToString();
									}
									this.c.DataWrite(this.c.IsCurrentNoteMuted() ? 0 : num10);
									this.c.DataWrite(num16);
									if (num16 > 0)
									{
										text2 = string.Concat(new string[]
										{
											text2,
											" ",
											this.GetMMLFromNoteNumber(num10).Trim(),
											"%",
											num16.ToString()
										});
									}
									num16 = 1;
								}
								else
								{
									num16++;
								}
							}
							if (!this.c.Slur[this.c.TrackIndex] && !this.c.Legato[this.c.TrackIndex])
							{
								this.c.DataWrite(132);
								text2 += " )";
							}
							if (this.c.Detune[this.c.TrackIndex] < -127 || this.c.Detune[this.c.TrackIndex] > 127)
							{
								short num19 = (short)this.c.Detune[this.c.TrackIndex];
								this.c.DataWrite(142);
								this.c.DataWrite((int)(num19 & 255));
								this.c.DataWrite(num19 >> 8);
								text2 = text2 + " \\" + ((int)(-(int)num19)).ToString();
							}
							else
							{
								this.c.DataWrite(135);
								this.c.DataWrite(this.c.Detune[this.c.TrackIndex]);
								text2 = text2 + " \\" + (-this.c.Detune[this.c.TrackIndex]).ToString();
							}
							this.c.DataWrite(this.c.IsCurrentNoteMuted() ? 0 : NoteNumber);
							this.c.DataWrite(num16);
							text2 = string.Concat(new string[]
							{
								text2,
								" ",
								this.GetMMLFromNoteNumber(NoteNumber).Trim(),
								"%",
								num16.ToString()
							});
							this.c.DataWrite(143);
							this.c.DataWrite(0);
							if (this.c.PortamentoCommandSet[this.c.TrackIndex])
							{
								this.c.DataWrite(136);
								this.c.DataWrite(this.c.Portamento[this.c.TrackIndex]);
								text2 = text2 + " p" + this.c.Portamento[this.c.TrackIndex].ToString();
							}
							this.c.AddInfoText(text2);
						}
						else
						{
							if (this.c.PortamentoWait[this.c.TrackIndex] > 0)
							{
								if (!this.c.Slur[this.c.TrackIndex] && !this.c.Legato[this.c.TrackIndex])
								{
									this.c.DataWrite(133);
								}
								this.c.DataWrite(this.c.IsCurrentNoteMuted() ? 0 : num10);
								this.c.DataWrite(this.c.PortamentoWait[this.c.TrackIndex]);
								if (!this.c.Slur[this.c.TrackIndex] && !this.c.Legato[this.c.TrackIndex])
								{
									this.c.DataWrite(132);
								}
								list = new List<byte>();
								while (i > 254)
								{
									list.Add((byte)i);
									i -= 255;
								}
								list.Add((byte)i);
							}
							this.c.DataWrite(161);
							if (num13 % 1.0 > 0.0)
							{
								num13 += 1.0;
							}
							if (num13 > 255.0)
							{
								num13 = 255.0;
							}
							this.c.DataWrite(0);
							this.c.DataWrite((int)num13);
							this.c.DataWrite(num10);
						}
					}
					this.c.FrameAnalyzeNote[this.c.TrackIndex] = num10;
					this.c.PortamentoStartNote[this.c.TrackIndex] = 0;
				}
				num10 = this.c.GlissStartNote[this.c.TrackIndex];
				if (num10 <= 0)
				{
					this.KeyOnPostProcess(NoteNumber, list, flag5, num);
					return;
				}
				string text3 = "グリッサンド展開 : ";
				this.c.AddPianoRollNote(NoteNumber, list, 0, 0, num10);
				int num20 = this.SumLengthData(list);
				int num21 = Math.Abs(num10 - NoteNumber) + 1;
				decimal num22 = num20 / num21;
				if (num22 > 254m)
				{
					this.c.Message = "グリッサンドで 1 音あたりの長さが 254 フレームを超えています。\n音長を短くする、遷移の高低を広くする、などの対処が必要です。";
					this.c.ErrorString = "'";
					return;
				}
				int num23 = (int)num22;
				decimal num24 = num22 - num23;
				decimal num25 = 0.0m;
				int num26 = 0;
				if (this.c.GlissLegato[this.c.TrackIndex] && !this.c.Slur[this.c.TrackIndex] && !this.c.Legato[this.c.TrackIndex])
				{
					this.c.DataWrite(133);
					text3 += "(";
				}
				for (int k = 0; k < num21; k++)
				{
					if (k == num21 - 1)
					{
						if (flag4)
						{
							if (!this.c.Slur[this.c.TrackIndex] && !this.c.Legato[this.c.TrackIndex])
							{
								this.c.Slur[this.c.TrackIndex] = true;
								this.c.DataWrite(133);
								text3 += "(";
							}
						}
						else if (this.c.GlissLegato[this.c.TrackIndex] && !flag4 && !this.c.Slur[this.c.TrackIndex] && !this.c.Legato[this.c.TrackIndex])
						{
							this.c.DataWrite(132);
							text3 += ")";
						}
						num26 += num23;
						if (num26 < num20)
						{
							num23 += num20 - num26;
							num25 = 0m;
							num24 = 0m;
						}
					}
					num25 += num24;
					if (num25 < 1m)
					{
						num26 += num23;
						if (num23 > 0)
						{
							this.ProcessSequence(num10);
							this.EmitGlissNote(num10, num23);
							text3 = string.Concat(new string[]
							{
								text3,
								this.GetMMLFromNoteNumber(num10).Trim(),
								"%",
								num23.ToString(),
								" "
							});
						}
					}
					else
					{
						num26 += num23 + 1;
						if (this.c.HardwareEnvelopeMode == 0 && this.c.HardwareEnvelopeAuto[this.c.TrackIndex] && num10 > 0)
						{
							int num27 = this.NoteNumberToHardwareEnvelopePeriod(num10 - 1);
							this.SetHardwareEnvelopePeriod(num27, null);
						}
						this.EmitGlissNote(num10, num23 + 1);
						text3 = string.Concat(new string[]
						{
							text3,
							this.GetMMLFromNoteNumber(num10).Trim(),
							"%",
							(num23 + 1).ToString(),
							" "
						});
						num25 -= 1m;
					}
					if (num10 < NoteNumber)
					{
						num10++;
					}
					else if (num10 > NoteNumber)
					{
						num10--;
					}
				}
				this.c.FrameAnalyzeNote[this.c.TrackIndex] = -this.c.GlissStartNote[this.c.TrackIndex];
				this.c.FrameAnalyzeLength[this.c.TrackIndex] = -num20;
				this.c.GlissStartNote[this.c.TrackIndex] = 0;
				this.c.AddInfoText(text3);
				this.SetNotePlayParameterActual(num, NoteNumber, list);
				this.ProcessF3(NoteNumber, list);
				return;
			}
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00007CF0 File Offset: 0x00005EF0
		private void EmitGlissNote(int noteNumber, int frames)
		{
			if (this.QuantizeMode[this.c.TrackIndex] != 1 || this.c.GlissLegato[this.c.TrackIndex] || this.c.Slur[this.c.TrackIndex] || this.c.Legato[this.c.TrackIndex] || this.c.IsCurrentNoteMuted())
			{
				this.c.DataWrite(this.c.IsCurrentNoteMuted() ? 0 : noteNumber);
				this.c.DataWrite(frames);
				return;
			}
			int num = ((this.c.QuantizeCount[this.c.TrackIndex] > 0) ? this.c.QuantizeBackup[this.c.TrackIndex] : this.c.Quantize[this.c.TrackIndex]);
			int num2 = ((this.c.AtQuantizeCount[this.c.TrackIndex] > 0) ? this.c.AtQuantizeBackup[this.c.TrackIndex] : this.c.AtQuantize[this.c.TrackIndex]);
			int num3 = Math.Max(0, Math.Min(frames, frames * num / 8));
			if (num2 > 0 && num2 < 129)
			{
				num3 = Math.Max(1, num3 - num2);
			}
			else if (num2 > 128 && num2 < 256 && num < 8)
			{
				num3 = Math.Min(frames, num3 + num2 - 128);
			}
			else if (num2 > 255)
			{
				num3 = Math.Min(frames, num2 - 257);
			}
			num3 = Math.Max(0, Math.Min(frames, num3));
			this.QuantizeGeneratedRest[this.c.TrackIndex] = false;
			if (num3 > 0)
			{
				this.c.DataWrite(noteNumber);
				this.c.DataWrite(num3);
			}
			if (num3 < frames)
			{
				this.c.PastNoteNumber[this.c.TrackIndex, 3] = this.c.PastNoteNumber[this.c.TrackIndex, 2];
				this.c.PastNoteNumber[this.c.TrackIndex, 2] = this.c.PastNoteNumber[this.c.TrackIndex, 1];
				this.c.PastNoteNumber[this.c.TrackIndex, 1] = this.c.PastNoteNumber[this.c.TrackIndex, 0];
				this.c.PastNoteNumber[this.c.TrackIndex, 0] = noteNumber;
				this.c.SelfDelayContinueCount[this.c.TrackIndex] = this.c.SelfDelayCount[this.c.TrackIndex];
				this.EmitRest(this.ToFrameBytes(frames - num3), true);
				this.QuantizeGeneratedRest[this.c.TrackIndex] = true;
			}
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00007FEC File Offset: 0x000061EC
		private void ProcessSequence(int NoteNumber)
		{
			this.ApplyDetuneSequence();
			this.ApplyHardwareEnvelopeDetuneSequence();
			this.ApplyHardwareEnvelopeKeyShiftSequence();
			this.ApplyHardwareEnvelopeDecaySequence();
			if (this.c.HardwareEnvelopeMode == 0 && this.c.HardwareEnvelopeAuto[this.c.TrackIndex] && NoteNumber > 0)
			{
				int num = this.NoteNumberToHardwareEnvelopePeriod(NoteNumber - 1);
				this.SetHardwareEnvelopePeriod(num, null);
			}
			if (this.c.VolumeSequenceNumber[this.c.TrackIndex] < 255)
			{
				int num2 = this.c.VolumeSequenceData[this.c.VolumeSequenceNumber[this.c.TrackIndex]][this.c.VolumeSequenceCounter[this.c.TrackIndex]];
				if (num2 < 0)
				{
					this.c.VolumeSequenceCounter[this.c.TrackIndex] += num2;
					num2 = this.c.VolumeSequenceData[this.c.VolumeSequenceNumber[this.c.TrackIndex]][this.c.VolumeSequenceCounter[this.c.TrackIndex]];
				}
				if (num2 > 0)
				{
					if (num2 < 16)
					{
						num2 += this.c.VolumeOffset[this.c.TrackIndex];
						this.c.VolumeLimit[this.c.TrackIndex] = ((num2 < 0) ? (-1) : ((num2 > 15) ? 1 : 0));
						if (num2 < 0)
						{
							num2 = 0;
						}
						if (num2 > 15)
						{
							num2 = 15;
						}
						this.c.Volume[this.c.TrackIndex] = num2;
						this.c.DataWrite(111 - num2);
					}
					else if (num2 < 32)
					{
						int num3 = this.c.Volume[this.c.TrackIndex] + num2 - 16;
						this.c.VolumeLimit[this.c.TrackIndex] = ((num3 > 15) ? 1 : 0);
						this.c.Volume[this.c.TrackIndex] = Math.Min(num3, 15);
						this.c.DataWrite(192 + num2 - 16);
					}
					else if (num2 < 48)
					{
						int num4 = this.c.Volume[this.c.TrackIndex] - (num2 - 32);
						this.c.VolumeLimit[this.c.TrackIndex] = ((num4 < 0) ? (-1) : 0);
						this.c.Volume[this.c.TrackIndex] = Math.Max(num4, 0);
						this.c.DataWrite(176 + num2 - 32);
					}
					else
					{
						this.c.VolumeSequenceCounter[this.c.TrackIndex]--;
					}
				}
				this.c.VolumeSequenceCounter[this.c.TrackIndex]++;
			}
			if (this.c.QuantizeSequenceNumber[this.c.TrackIndex] < 255)
			{
				int num5 = this.c.QuantizeSequenceData[this.c.QuantizeSequenceNumber[this.c.TrackIndex]][this.c.QuantizeSequenceCounter[this.c.TrackIndex]];
				if (num5 < 0)
				{
					this.c.QuantizeSequenceCounter[this.c.TrackIndex] += num5;
					num5 = this.c.QuantizeSequenceData[this.c.QuantizeSequenceNumber[this.c.TrackIndex]][this.c.QuantizeSequenceCounter[this.c.TrackIndex]];
				}
				if (num5 > 0 && num5 < 255)
				{
					if (num5 > 8)
					{
						this.c.AddInfoText("Qシーケンスで、8を超える値が代入されました。動作は不定となります。");
					}
					this.WriteQuantize(num5, false);
					this.c.Quantize[this.c.TrackIndex] = num5;
				}
				else
				{
					this.c.QuantizeSequenceCounter[this.c.TrackIndex]--;
				}
				this.c.QuantizeSequenceCounter[this.c.TrackIndex]++;
			}
			if (this.c.AtQuantizeSequenceNumber[this.c.TrackIndex] < 255)
			{
				int num6 = this.c.AtQuantizeSequenceData[this.c.AtQuantizeSequenceNumber[this.c.TrackIndex]][this.c.AtQuantizeSequenceCounter[this.c.TrackIndex]];
				if (num6 < 0)
				{
					this.c.AtQuantizeSequenceCounter[this.c.TrackIndex] += num6;
					num6 = this.c.AtQuantizeSequenceData[this.c.AtQuantizeSequenceNumber[this.c.TrackIndex]][this.c.AtQuantizeSequenceCounter[this.c.TrackIndex]];
				}
				if (num6 >= 0 && num6 < 256)
				{
					this.WriteAtQuantize(num6, false);
					this.c.AtQuantize[this.c.TrackIndex] = num6;
				}
				else if (num6 == 256)
				{
					this.c.AtQuantizeSequenceCounter[this.c.TrackIndex]--;
				}
				else
				{
					this.WriteAtQuantize(num6, false);
					this.c.AtQuantize[this.c.TrackIndex] = num6;
				}
				this.c.AtQuantizeSequenceCounter[this.c.TrackIndex]++;
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00008594 File Offset: 0x00006794
		private List<byte> ToFrameBytes(int frames)
		{
			List<byte> list = new List<byte>();
			while (frames >= 255)
			{
				list.Add(byte.MaxValue);
				frames -= 255;
			}
			list.Add((byte)frames);
			return list;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x000085D0 File Offset: 0x000067D0
		private int EmitRest(List<byte> LengthData, bool advanceSelfDelay = true)
		{
			int num = 0;
			if (this.c.SelfDelayCount[this.c.TrackIndex] < 255 && this.c.SelfDelayContinueCount[this.c.TrackIndex] >= 0 && this.c.SelfDelayContinueCount[this.c.TrackIndex] < 255)
			{
				if (this.c.HardwareEnvelopeNumber[this.c.TrackIndex] > -1 && this.c.DelayVoiceNumber[this.c.TrackIndex] < 0)
				{
					this.c.Message = "ハードエンベロープとセルフディレイを併用する際は、@ コマンドの第 2 パラメータでセルフディレイ時の音色を指定する必要があります。";
					this.c.ErrorString = "R";
					return 0;
				}
				if (this.c.DelayVoiceNumber[this.c.TrackIndex] > -1 && (this.c.HardwareEnvelopeNumber[this.c.TrackIndex] > -1 || this.c.VoiceNumber[this.c.TrackIndex] != this.c.DelayVoiceNumber[this.c.TrackIndex]))
				{
					int num2 = this.c.DelayVoiceNumber[this.c.TrackIndex];
					this.c.DataWrite(112 + num2);
					if (this.c.VoiceDataUsePM[num2] > 0)
					{
						this.c.UsePitchEnvelopeType[this.c.TrackIndex] = this.c.VoiceDataUsePM[num2];
					}
					if (this.c.HardwareEnvelopeAuto[this.c.TrackIndex] && this.c.MixMode[this.c.TrackIndex] != 1)
					{
						this.c.DataWrite(129);
						this.c.DataWrite(1);
					}
				}
				if (this.c.HardwareEnvelopeNumber[this.c.TrackIndex] < 0)
				{
					this.c.DataWrite(191 - this.c.SelfDelayVolume[this.c.TrackIndex]);
				}
				else
				{
					int num3 = this.c.SelfDelayVolume[this.c.TrackIndex] + this.c.VolumeOffset[this.c.TrackIndex];
					if (num3 < 0)
					{
						num3 = 0;
					}
					if (num3 > 15)
					{
						num3 = 15;
					}
					this.c.DataWrite(111 - num3);
				}
				int num4 = this.c.PastNoteNumber[this.c.TrackIndex, this.c.SelfDelayContinueCount[this.c.TrackIndex]];
				if (num4 == 0 && this.c.SelfDelayRestFill[this.c.TrackIndex])
				{
					num4 = this.c.PastNoteNumber[this.c.TrackIndex, 0];
				}
				if (this.c.HardwareEnvelopeAuto[this.c.TrackIndex] && (this.c.HardwareEnvelopeNumber[this.c.TrackIndex] == 10 || this.c.HardwareEnvelopeNumber[this.c.TrackIndex] == 14))
				{
					num4 -= 12;
					if (num4 < 0)
					{
						num4 = 0;
					}
				}
				num = this.c.Address;
				this.c.DataWrite(this.c.IsCurrentNoteMuted() ? 0 : num4);
				foreach (byte b in LengthData)
				{
					this.c.DataWrite((int)b);
				}
				if (this.c.HardwareEnvelopeNumber[this.c.TrackIndex] < 0)
				{
					this.c.DataWrite(207 - this.c.SelfDelayVolume[this.c.TrackIndex]);
				}
				if (this.c.DelayVoiceNumber[this.c.TrackIndex] > -1)
				{
					if (this.c.HardwareEnvelopeNumber[this.c.TrackIndex] > -1)
					{
						if (this.c.HardwareEnvelopeMode == 0 && this.c.HardwareEnvelopeAuto[this.c.TrackIndex])
						{
							this.c.DataWrite(160);
							this.c.DataWrite(this.c.HardwareEnvelopePeriod[this.c.TrackIndex] & 255);
							this.c.DataWrite(this.c.HardwareEnvelopePeriod[this.c.TrackIndex] / 256);
							if (this.c.MixMode[this.c.TrackIndex] != 1)
							{
								this.c.DataWrite(129);
								this.c.DataWrite(this.c.MixMode[this.c.TrackIndex]);
							}
						}
						this.c.DataWrite(144 + this.c.HardwareEnvelopeNumber[this.c.TrackIndex]);
						this.c.DataWrite(this.c.HardwareEnvelopeVolumeDecayFrame[this.c.TrackIndex]);
					}
					else if (this.c.VoiceNumber[this.c.TrackIndex] != this.c.DelayVoiceNumber[this.c.TrackIndex])
					{
						this.c.DataWrite(112 + this.c.VoiceNumber[this.c.TrackIndex]);
					}
				}
				if (advanceSelfDelay)
				{
					this.c.SelfDelayContinueCount[this.c.TrackIndex]--;
				}
			}
			else
			{
				num = this.c.Address;
				this.c.DataWrite(0);
				foreach (byte b2 in LengthData)
				{
					this.c.DataWrite((int)b2);
				}
			}
			return num;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00008BF0 File Offset: 0x00006DF0
		private int KeyOnPostProcess(int NoteNumber, List<byte> LengthData, bool PostProcess = true, int NotePlayParameterIndex = -1)
		{
			int num = 0;
			this.ProcessSequence(NoteNumber);
			if (PostProcess)
			{
				bool flag = false;
				if (this.c.RestNoteSpan[this.c.TrackIndex] > 0)
				{
					this.c.RestNoteCount[this.c.TrackIndex]--;
					if (this.c.RestNoteCount[this.c.TrackIndex] < 0)
					{
						this.c.RestNoteCount[this.c.TrackIndex] = this.c.RestNoteSpan[this.c.TrackIndex];
					}
					else
					{
						flag = true;
					}
				}
				if (NoteNumber > 0)
				{
					int num2 = this.c.RandomStartNote[this.c.TrackIndex];
					if (num2 > 0)
					{
						if (NoteNumber > num2)
						{
							int num3 = num2;
							num2 = NoteNumber;
							NoteNumber = num3;
						}
						NoteNumber = this.c.RandomParam.Next(NoteNumber, num2 + 1);
						this.c.RandomStartNote[this.c.TrackIndex] = 0;
					}
					this.c.PastNoteNumber[this.c.TrackIndex, 3] = this.c.PastNoteNumber[this.c.TrackIndex, 2];
					this.c.PastNoteNumber[this.c.TrackIndex, 2] = this.c.PastNoteNumber[this.c.TrackIndex, 1];
					this.c.PastNoteNumber[this.c.TrackIndex, 1] = this.c.PastNoteNumber[this.c.TrackIndex, 0];
					this.c.PastNoteNumber[this.c.TrackIndex, 0] = NoteNumber;
					this.c.SelfDelayContinueCount[this.c.TrackIndex] = this.c.SelfDelayCount[this.c.TrackIndex];
					num = this.c.Address;
					this.QuantizeGeneratedRest[this.c.TrackIndex] = false;
					int num4 = this.SumLengthData(LengthData);
					int num5 = num4;
					if (this.QuantizeMode[this.c.TrackIndex] == 1 && !this.c.Legato[this.c.TrackIndex] && !this.c.Slur[this.c.TrackIndex] && !flag && !this.c.IsCurrentNoteMuted())
					{
						int num6 = ((this.c.QuantizeCount[this.c.TrackIndex] > 0) ? this.c.QuantizeBackup[this.c.TrackIndex] : this.c.Quantize[this.c.TrackIndex]);
						int num7 = ((this.c.AtQuantizeCount[this.c.TrackIndex] > 0) ? this.c.AtQuantizeBackup[this.c.TrackIndex] : this.c.AtQuantize[this.c.TrackIndex]);
						num5 = Math.Max(0, Math.Min(num4, num4 * num6 / 8));
						if (num7 > 0 && num7 < 129)
						{
							num5 = Math.Max(1, num5 - num7);
						}
						else if (num7 > 128 && num7 < 256 && num6 < 8)
						{
							num5 = Math.Min(num4, num5 + num7 - 128);
						}
						else if (num7 > 255)
						{
							num5 = Math.Min(num4, num7 - 257);
						}
						num5 = Math.Max(0, Math.Min(num4, num5));
					}
					if (num5 > 0 || num5 == num4)
					{
						this.c.DataWrite((this.c.IsCurrentNoteMuted() || flag) ? 0 : NoteNumber);
						foreach (byte b in ((num5 == num4) ? LengthData : this.ToFrameBytes(num5)))
						{
							this.c.DataWrite((int)b);
						}
					}
					if (num5 < num4)
					{
						if (num5 == 0)
						{
							num = this.c.Address;
						}
						this.EmitRest(this.ToFrameBytes(num4 - num5), true);
						this.QuantizeGeneratedRest[this.c.TrackIndex] = true;
					}
					this.c.PutNote[this.c.TrackIndex] = true;
					if (this.c.ArpeggioFirstNoteFrameAddress[this.c.TrackIndex] < 0)
					{
						this.c.ArpeggioFirstNoteFrameAddress[this.c.TrackIndex] = ((num5 == num4) ? (this.c.Address - 1) : ((num5 > 0) ? (num + this.ToFrameBytes(num5).Count) : (this.c.Address - 1)));
					}
					this.c.AddPianoRollNote(NoteNumber, LengthData, 0, 0, 0);
					if (this.c.ReleaseVolumeAfter[this.c.TrackIndex] >= 0)
					{
						this.c.DataWrite(141);
						this.c.DataWrite(this.c.Mute[this.c.TrackIndex] ? 15 : (15 - this.c.ReleaseVolume[this.c.TrackIndex]));
						this.c.ReleaseVolumeAfter[this.c.TrackIndex] = -1;
					}
				}
				else
				{
					num = this.EmitRest(LengthData, !this.QuantizeGeneratedRest[this.c.TrackIndex]);
					this.QuantizeGeneratedRest[this.c.TrackIndex] = false;
				}
			}
			if (this.c.QuantizeSequenceNumber[this.c.TrackIndex] == 255 && this.c.AtQuantizeSequenceNumber[this.c.TrackIndex] == 255)
			{
				if (this.c.QuantizeCount[this.c.TrackIndex] > 0)
				{
					this.c.QuantizeCount[this.c.TrackIndex]--;
					if (this.c.QuantizeCount[this.c.TrackIndex] == 0)
					{
						this.WriteQuantize(this.c.Quantize[this.c.TrackIndex], false);
					}
				}
				if (this.c.AtQuantizeCount[this.c.TrackIndex] > 0)
				{
					this.c.AtQuantizeCount[this.c.TrackIndex]--;
					if (this.c.AtQuantizeCount[this.c.TrackIndex] == 0)
					{
						this.WriteAtQuantize(this.c.AtQuantize[this.c.TrackIndex], false);
					}
				}
			}
			this.SetNotePlayParameterActual(NotePlayParameterIndex, NoteNumber, LengthData);
			this.ProcessF3(NoteNumber, LengthData);
			return num;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x000092CC File Offset: 0x000074CC
		private void ProcessF3(int NoteNumber, List<byte> LengthData)
		{
			if (this.c.FrameAnalyzeCount[this.c.TrackIndex] > 0)
			{
				string text = "\t";
				int num;
				if (this.c.FrameAnalyzeLength[this.c.TrackIndex] > 0)
				{
					num = this.c.FrameAnalyzeLength[this.c.TrackIndex];
					this.c.FrameAnalyzeLength[this.c.TrackIndex] = 0;
				}
				else
				{
					num = this.SumLengthData(LengthData);
				}
				int num2 = this.c.FrameAnalyzeNote[this.c.TrackIndex];
				string text2 = ((num2 < 0) ? "'" : "_");
				if (num2 < 0)
				{
					num2 = -num2;
				}
				if (num2 > 0)
				{
					this.c.FrameAnalyzeNote[this.c.TrackIndex] = 0;
					text = text + this.GetMMLFromNoteNumber(num2) + text2 + " ";
				}
				text += this.GetMMLFromNoteNumber(NoteNumber);
				ref string ptr = ref this.c.FrameAnalyzeText[this.c.TrackIndex];
				ptr = string.Concat(new string[]
				{
					ptr,
					"\t",
					text,
					" %",
					num.ToString(),
					"\r\n"
				});
				this.c.FrameAnalyzeCount[this.c.TrackIndex]++;
				if (this.c.FrameAnalyzeCount[this.c.TrackIndex] > 8)
				{
					this.c.FrameAnalyzeCount[this.c.TrackIndex] = 0;
				}
			}
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00009468 File Offset: 0x00007668
		private int SumLengthData(List<byte> LengthData)
		{
			int num = 0;
			for (int i = 0; i < LengthData.Count; i++)
			{
				num += (int)LengthData[i];
			}
			return num;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00009494 File Offset: 0x00007694
		private string GetMMLFromNoteNumber(int NoteNumber)
		{
			NoteNumber--;
			if (NoteNumber < 0)
			{
				return "\u3000R";
			}
			return "o" + (NoteNumber / 12 + 1).ToString() + "C C+D D+E F F+G G+A A+B ".Substring((NoteNumber - NoteNumber / 12 * 12) * 2, 2);
		}

		// Token: 0x06000066 RID: 102 RVA: 0x000094E4 File Offset: 0x000076E4
		private void SetFirstVoice()
		{
			if (!this.c.SetVoice[this.c.TrackIndex])
			{
				this.c.DataWrite(112);
				this.c.SetVoice[this.c.TrackIndex] = true;
				this.c.UseVoice[0] = true;
			}
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00009540 File Offset: 0x00007740
		private void SetArpeggio()
		{
			if (this.c.EnableArpeggio[this.c.TrackIndex])
			{
				List<byte> list = this.c.SetNoteLength(false, false);
				if (this.c.Message != null)
				{
					this.c.ErrorString = "\" … \"";
					return;
				}
				if (this.c.CheckNextCharacter(','))
				{
					int num = this.c.NumberRead(true, true);
					if (this.c.DefaultOn && num <= -32769)
					{
						num = 0;
					}
					if (num < 0 || num > 6)
					{
						this.c.RangeNotice("アルペジオモード", 0, 6, "\n0: ノーマル\n1: リバース\n2: オルタネート (ノーマル)\n3: オルタネート (リバース)\n4: ランダム\n5: オルタネート2 (ノーマル)\n6: オルタネート2 (リバース)");
						this.c.ErrorParameter("\" … \"", num);
						return;
					}
					this.c.ArpeggioMode[this.c.TrackIndex] = num;
				}
				if (this.c.ArpeggioMode[this.c.TrackIndex] > 0 && (this.c.ArpeggioPoint1[this.c.TrackIndex] >= 0 || this.c.ArpeggioPoint2[this.c.TrackIndex] >= 0))
				{
					this.c.Message = "/ によるアルペジオ内のループポイント設定は、アルペジオモード 0 でのみ可能です。";
					this.c.ErrorString = "\" … / … \"";
					return;
				}
				bool flag = this.c.CheckNextCharacter('&');
				int num2 = this.SumLengthData(list);
				bool flag2 = false;
				decimal num3 = this.c.FrameMod[this.c.TrackIndex];
				this.c.FrameMod[this.c.TrackIndex] = 0m;
				this.SetFirstVoice();
				int num4 = this.c.ArpeggioVolumeSequenceNumber[this.c.TrackIndex];
				if (num4 >= 0 && num4 <= 31)
				{
					this.c.VolumeSequenceNumber[this.c.TrackIndex] = num4;
					this.c.VolumeSequenceCounter[this.c.TrackIndex] = 0;
					if (this.c.ArpeggioEndVolume[this.c.TrackIndex] == 1)
					{
						this.c.DataWrite(171);
						this.c.DataWrite(3);
						this.c.RestoreVolume[this.c.TrackIndex] = this.c.Volume[this.c.TrackIndex];
					}
				}
				int num5 = this.c.ArpeggioQuantizeSequenceNumber[this.c.TrackIndex];
				if (num5 >= 0 && num5 <= 31)
				{
					this.c.QuantizeSequenceNumber[this.c.TrackIndex] = num5;
					this.c.QuantizeSequenceCounter[this.c.TrackIndex] = 0;
				}
				int num6 = this.c.ArpeggioAtQuantizeSequenceNumber[this.c.TrackIndex];
				if (num6 >= 0 && num6 <= 31)
				{
					this.c.AtQuantizeSequenceNumber[this.c.TrackIndex] = num6;
					this.c.AtQuantizeSequenceCounter[this.c.TrackIndex] = 0;
				}
				int num7 = this.c.ArpeggioDetuneSequenceNumber[this.c.TrackIndex];
				if (num7 >= 0 && num7 <= 31)
				{
					this.c.DetuneSequenceNumber[this.c.TrackIndex] = num7;
					this.c.DetuneSequenceCounter[this.c.TrackIndex] = 0;
				}
				int num8 = this.c.ArpeggioHardwareEnvelopeDetuneSequenceNumber[this.c.TrackIndex];
				if (num8 >= 0 && num8 <= 31)
				{
					this.c.HardwareEnvelopeDetuneSequenceNumber[this.c.TrackIndex] = num8;
					this.c.HardwareEnvelopeDetuneSequenceCounter[this.c.TrackIndex] = 0;
				}
				int num9 = this.c.ArpeggioHardwareEnvelopeKeyShiftSequenceNumber[this.c.TrackIndex];
				if (num9 >= 0 && num9 <= 31)
				{
					this.c.HardwareEnvelopeKeyShiftSequenceNumber[this.c.TrackIndex] = num9;
					this.c.HardwareEnvelopeKeyShiftSequenceCounter[this.c.TrackIndex] = 0;
				}
				int num10 = this.c.ArpeggioHardwareEnvelopeDecaySequenceNumber[this.c.TrackIndex];
				if (num10 >= 0 && num10 <= 31)
				{
					this.c.HardwareEnvelopeDecaySequenceNumber[this.c.TrackIndex] = num10;
					this.c.HardwareEnvelopeDecaySequenceCounter[this.c.TrackIndex] = 0;
				}
				if (this.c.ArpeggioLegato[this.c.TrackIndex])
				{
					if (!this.c.Slur[this.c.TrackIndex])
					{
						this.c.DataWrite(133);
					}
				}
				else if (this.c.Slur[this.c.TrackIndex])
				{
					this.c.DataWrite(132);
				}
				this.c.Slur[this.c.TrackIndex] = false;
				List<int> list2 = new List<int>();
				int num11 = 0;
				int num12 = 1;
				bool flag3 = false;
				switch (this.c.ArpeggioMode[this.c.TrackIndex])
				{
				case 0:
				case 2:
				case 5:
					num11 = 0;
					num12 = 1;
					break;
				case 1:
				case 3:
				case 6:
					num11 = this.c.ArpeggioNoteNumber[this.c.TrackIndex].Count - 1;
					num12 = -1;
					break;
				case 4:
					num11 = this.c.RandomParam.Next(0, this.c.ArpeggioNoteNumber[this.c.TrackIndex].Count);
					break;
				}
				for (;;)
				{
					decimal num13 = this.c.ArpeggioDefaultLength[this.c.TrackIndex][this.c.ArpeggioNoteLengthIndex[this.c.TrackIndex]];
					if (num13 == -1m)
					{
						this.c.ArpeggioNoteLengthIndex[this.c.TrackIndex] = 0;
						num13 = this.c.ArpeggioDefaultLength[this.c.TrackIndex][this.c.ArpeggioNoteLengthIndex[this.c.TrackIndex]];
					}
					this.c.ArpeggioNoteLengthIndex[this.c.TrackIndex]++;
					if (this.c.ArpeggioNoteLengthIndex[this.c.TrackIndex] > 15)
					{
						this.c.ArpeggioNoteLengthIndex[this.c.TrackIndex] = 0;
					}
					int num15;
					if (num13 < 1000m)
					{
						decimal num14 = 14400m / this.c.Tempo[this.c.TrackIndex] / num13;
						num15 = (int)num14;
						this.c.FrameMod[this.c.TrackIndex] += num14 - num15;
					}
					else
					{
						num15 = (int)num13 - 1000;
					}
					while (this.c.FrameMod[this.c.TrackIndex] >= 1.0m)
					{
						decimal[] frameMod = this.c.FrameMod;
						int trackIndex = this.c.TrackIndex;
						frameMod[trackIndex] -= 1m;
						num15++;
					}
					num2 -= num15;
					if (num2 < 1)
					{
						num15 += num2;
						if (flag)
						{
							if (!this.c.ArpeggioLegato[this.c.TrackIndex])
							{
								this.c.DataWrite(133);
							}
						}
						else if (this.c.ArpeggioLegato[this.c.TrackIndex] && !this.c.Legato[this.c.TrackIndex])
						{
							this.c.DataWrite(132);
						}
						this.c.Slur[this.c.TrackIndex] = flag;
						flag2 = true;
					}
					if (num15 > 254)
					{
						break;
					}
					if (num15 > 0)
					{
						List<byte> list3 = new List<byte> { (byte)num15 };
						if (flag2 && num15 == 1 && num13 < 1000m && !this.c.PreCompile)
						{
							int num16 = (int)(this.c.BGMData[this.c.ArpeggioFirstNoteFrameAddress[this.c.TrackIndex]] + 1);
							if (num16 > 254)
							{
								goto Block_43;
							}
							this.c.BGMData[this.c.ArpeggioFirstNoteFrameAddress[this.c.TrackIndex]] = (byte)num16;
							if (this.c.BGMData[this.c.Address - 1] == 132)
							{
								this.c.BGMData[this.c.Address - 1] = this.c.BGMData[this.c.Address - 2];
								this.c.BGMData[this.c.Address - 2] = this.c.BGMData[this.c.Address - 3];
								this.c.BGMData[this.c.Address - 3] = 132;
							}
							else if (this.c.BGMData[this.c.Address - 3] == 132)
							{
								this.c.BGMData[this.c.Address - 1] = this.c.BGMData[this.c.Address - 2];
								this.c.BGMData[this.c.Address - 2] = this.c.BGMData[this.c.Address - 3];
								this.c.BGMData[this.c.Address - 3] = this.c.BGMData[this.c.Address - 4];
								this.c.BGMData[this.c.Address - 4] = this.c.BGMData[this.c.Address - 5];
								this.c.BGMData[this.c.Address - 5] = 132;
							}
						}
						else
						{
							if (this.c.ArpeggioNoteNumber[this.c.TrackIndex].Count == 0)
							{
								goto Block_46;
							}
							list2.Add(this.KeyOnPostProcess(this.c.ArpeggioNoteNumber[this.c.TrackIndex][num11], list3, true, -1));
						}
						this.c.AddTrackFrame(num15);
					}
					if (this.c.ArpeggioMode[this.c.TrackIndex] == 0)
					{
						if (num11 == this.c.ArpeggioPoint1[this.c.TrackIndex])
						{
							this.c.ArpeggioCount1[this.c.TrackIndex]--;
							if (this.c.ArpeggioCount1[this.c.TrackIndex] > 0)
							{
								num11 = -1;
							}
						}
						if (num11 == this.c.ArpeggioPoint2[this.c.TrackIndex])
						{
							this.c.ArpeggioCount2[this.c.TrackIndex]--;
							if (this.c.ArpeggioCount2[this.c.TrackIndex] > 0)
							{
								num11 = this.c.ArpeggioPoint1[this.c.TrackIndex];
							}
						}
						if (num11 == this.c.ArpeggioNoteNumber[this.c.TrackIndex].Count - 1)
						{
							if (this.c.ArpeggioPoint2[this.c.TrackIndex] < 0)
							{
								num11 = this.c.ArpeggioPoint1[this.c.TrackIndex];
							}
							else
							{
								num11 = this.c.ArpeggioPoint2[this.c.TrackIndex];
							}
						}
					}
					if (flag2)
					{
						goto IL_0E2A;
					}
					switch (this.c.ArpeggioMode[this.c.TrackIndex])
					{
					case 0:
						num11 += num12;
						if (num11 >= this.c.ArpeggioNoteNumber[this.c.TrackIndex].Count)
						{
							num11 = 0;
						}
						break;
					case 1:
						num11 += num12;
						if (num11 < 0)
						{
							num11 = this.c.ArpeggioNoteNumber[this.c.TrackIndex].Count - 1;
						}
						break;
					case 2:
					case 3:
						num11 += num12;
						if (num11 <= 0)
						{
							num11 = 0;
							num12 = 1;
						}
						if (num11 >= this.c.ArpeggioNoteNumber[this.c.TrackIndex].Count - 1)
						{
							num11 = this.c.ArpeggioNoteNumber[this.c.TrackIndex].Count - 1;
							num12 = -1;
						}
						break;
					case 4:
						num11 = this.c.RandomParam.Next(0, this.c.ArpeggioNoteNumber[this.c.TrackIndex].Count);
						break;
					case 5:
					case 6:
						num11 += num12;
						if (num11 <= 0)
						{
							num11 = 0;
							if (!flag3)
							{
								num12 = 0;
								flag3 = true;
							}
							else
							{
								num12 = 1;
								flag3 = false;
							}
						}
						if (num11 >= this.c.ArpeggioNoteNumber[this.c.TrackIndex].Count - 1)
						{
							num11 = this.c.ArpeggioNoteNumber[this.c.TrackIndex].Count - 1;
							if (!flag3)
							{
								num12 = 0;
								flag3 = true;
							}
							else
							{
								num12 = -1;
								flag3 = false;
							}
						}
						break;
					}
				}
				this.c.Message = "アルペジオの音符 1 個あたりのフレーム数が 255 以上になりました。";
				return;
				Block_43:
				this.c.Message = "アルペジオの最後の音符が 255 フレーム 以上になりました。";
				return;
				Block_46:
				this.c.Message = "アルペジオ内に音符がありません。\n\"～\" が正しく閉じられていない可能性があります。";
				return;
				IL_0E2A:
				this.c.EnableArpeggio[this.c.TrackIndex] = false;
				this.c.FrameMod[this.c.TrackIndex] = num3;
				int num17 = 0;
				if (this.c.ArpeggioRest[this.c.TrackIndex] < 0)
				{
					using (List<int>.Enumerator enumerator = list2.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							int num18 = enumerator.Current;
							num17++;
							if (num17 > list2.Count + this.c.ArpeggioRest[this.c.TrackIndex])
							{
								this.c.BGMData[num18] = 0;
							}
						}
						goto IL_0F5B;
					}
				}
				if (this.c.ArpeggioRest[this.c.TrackIndex] > 0)
				{
					foreach (int num19 in list2)
					{
						num17++;
						if (num17 > this.c.ArpeggioRest[this.c.TrackIndex])
						{
							this.c.BGMData[num19] = 0;
						}
					}
				}
				IL_0F5B:
				int num20 = this.c.ArpeggioEndVolume[this.c.TrackIndex];
				if (num20 >= 0 && num20 <= 15)
				{
					this.VolumeSetAndSequenceOff(num20, true);
				}
				int num21 = this.c.ArpeggioEndQuantize[this.c.TrackIndex];
				if (num21 > 0 && num21 <= 8)
				{
					this.WriteQuantize(num21, false);
					this.c.QuantizeSequenceNumber[this.c.TrackIndex] = 255;
					this.c.Quantize[this.c.TrackIndex] = num21;
				}
				int num22 = this.c.ArpeggioEndAtQuantize[this.c.TrackIndex];
				if (num22 >= 0 && num22 < 255)
				{
					this.WriteAtQuantize(num22, false);
					this.c.AtQuantizeSequenceNumber[this.c.TrackIndex] = 255;
					this.c.AtQuantize[this.c.TrackIndex] = num22;
					return;
				}
			}
			else
			{
				this.c.EnableArpeggio[this.c.TrackIndex] = true;
				this.c.ArpeggioNoteNumber[this.c.TrackIndex].Clear();
				this.c.ArpeggioPoint1[this.c.TrackIndex] = -1;
				this.c.ArpeggioPoint2[this.c.TrackIndex] = -1;
				this.c.ArpeggioCount1[this.c.TrackIndex] = 1;
				this.c.ArpeggioCount2[this.c.TrackIndex] = 1;
				this.c.ArpeggioFirstNoteFrameAddress[this.c.TrackIndex] = -1;
			}
		}

		// Token: 0x06000068 RID: 104 RVA: 0x0000A670 File Offset: 0x00008870
		private void PlaySE()
		{
			if (this.c.CheckNextCharacter('A'))
			{
				this.AlignTracks();
				return;
			}
			if (!this.c.CheckNextCharacter('S'))
			{
				this.c.Message = "! の書式が正しくありません。\n効果音は !S<番号>、トラック同期は !A で指定してください。";
				return;
			}
			int num = this.c.NumberRead(false, true);
			if (this.c.SkipSE)
			{
				return;
			}
			if (this.c.SoundId > 0)
			{
				this.c.Message = "効果音のネストはできません。";
				this.c.ErrorParameter("!S", num);
				return;
			}
			this.c.SEUseAddress.Add(this.c.Address);
			this.c.SEUseLine.Add(this.c.LineNumber);
			this.c.SEUseNumber.Add(num);
			this.c.DataWrite(172);
			this.c.DataWrite(0);
			this.c.DataWrite(0);
		}

		// Token: 0x06000069 RID: 105 RVA: 0x0000A774 File Offset: 0x00008974
		protected string GetAlignSourceKey()
		{
			int num = Math.Max(this.c.Index - 1, 0);
			int num2 = this.c.LineNumber;
			int num3 = this.c.PlayLineColumn + (num - this.c.PlayLineBufferIndex);
			if (this.c.CurrentSourceMap != null && num < this.c.CurrentSourceMap.Count)
			{
				MMLSourcePosition mmlsourcePosition = this.c.CurrentSourceMap[num];
				if (mmlsourcePosition.IsValid)
				{
					num2 = mmlsourcePosition.Line;
					num3 = mmlsourcePosition.Column;
				}
			}
			return num2.ToString("D4") + ":" + num3.ToString("D5");
		}

		// Token: 0x0600006A RID: 106 RVA: 0x0000A828 File Offset: 0x00008A28
		protected void AlignTracks()
		{
			if (this.c.SoundId != 0)
			{
				this.c.Message = "!A コマンドは効果音では使用できません。";
				this.c.ErrorString = "!A";
				return;
			}
			string alignSourceKey = this.GetAlignSourceKey();
			if (this.c.GetTimeCompile && !this.c.AlignPlanReady)
			{
				this.c.RegisterAlignPoint(alignSourceKey);
				return;
			}
			if (!this.c.AlignPlanReady)
			{
				return;
			}
			int trackIndex = this.c.TrackIndex;
			if (this.AlignTrackIndex >= 0)
			{
				this.c.TrackIndex = this.AlignTrackIndex;
			}
			int alignWaitFrame = this.c.GetAlignWaitFrame(alignSourceKey);
			this.c.TrackIndex = trackIndex;
			if (this.c.Message != null)
			{
				return;
			}
			if (alignWaitFrame > 0)
			{
				this.c.DataWrite(0);
				int i;
				for (i = alignWaitFrame; i >= 255; i -= 255)
				{
					this.c.DataWrite(255);
				}
				this.c.DataWrite(i);
				this.c.AddTrackFrame(alignWaitFrame);
			}
			this.c.ResetAlignFrameMod();
		}

		// Token: 0x0600006B RID: 107 RVA: 0x0000A944 File Offset: 0x00008B44
		public void SetTempo()
		{
			if (this.c.CheckNextCharacter('S'))
			{
				int num = this.c.NumberRead(false, true);
				if (num < 0 || num > 7)
				{
					this.c.RangeNotice("スロー設定", 0, 7, null);
					this.c.ErrorParameter("TS", num);
					return;
				}
				this.c.DataWrite(253);
				this.c.DataWrite(Core.SlowParameter[num]);
				this.c.RegisterSlowEvent(num);
				return;
			}
			else
			{
				if (this.c.SetTimebase[this.c.TrackIndex])
				{
					this.c.Message = "このトラックは既にタイムベース指定によってテンポが設定されています。\nどちらか片方で設定してください。\n\nタイムベースは 4 分音符のフレーム数で、設定すると自動的にテンポも決定されます。";
					this.c.ErrorString = "T";
					return;
				}
				decimal num2 = this.c.DecimalNumberRead(false, true);
				if (this.c.DefaultOn && num2 <= -32769m)
				{
					num2 = 120m;
				}
				if (num2 < 1m || num2 > 999m)
				{
					this.c.RangeNotice("テンポ", 1, 999, null);
					this.c.ErrorParameter("T", num2);
					return;
				}
				this.c.Tempo[this.c.TrackIndex] = num2;
				this.c.SetTempo[this.c.TrackIndex] = true;
				return;
			}
		}

		// Token: 0x0600006C RID: 108 RVA: 0x0000AAB8 File Offset: 0x00008CB8
		public void SetLength()
		{
			if (this.c.CheckNextCharacter('A'))
			{
				this.SetArpeggioLength();
				return;
			}
			this.c.NoteLengthIndex[this.c.TrackIndex] = 0;
			if (this.c.CheckNextCharacter('-'))
			{
				return;
			}
			bool flag = true;
			int num = 0;
			while (flag)
			{
				if (num > 15)
				{
					this.c.Message = "ひとつの L コマンドで定義できる音長シーケンスは最大 16 個です。";
					this.c.ErrorString = "L";
					return;
				}
				if (!this.c.CheckFrameMode())
				{
					decimal num2 = this.c.NumberRead(false, true);
					if (this.c.DefaultOn && num2 <= -32769m)
					{
						if (num == 0)
						{
							num2 = 4m;
						}
						else
						{
							num2 = this.c.DefaultLength[this.c.TrackIndex][num - 1];
						}
					}
					if (num2 < 1m)
					{
						this.c.Message = "音長として 0 が指定されています。";
						this.c.ErrorString = "L0";
						return;
					}
					this.c.DefaultLength[this.c.TrackIndex][num] = num2;
					this.c.DefaultLengthAddString[this.c.TrackIndex][num] = this.SetDefaultLengthAddSub();
					if (this.c.Message != null)
					{
						return;
					}
				}
				else
				{
					int num3 = this.c.NumberRead(false, true);
					if (num3 < 1)
					{
						this.c.Message = "音長として 0 フレームが指定されています。";
						this.c.ErrorString = "L";
						return;
					}
					this.c.DefaultLength[this.c.TrackIndex][num] = num3 + 1000;
					this.c.DefaultLengthAddString[this.c.TrackIndex][num] = this.SetDefaultLengthAddSub();
					if (this.c.Message != null)
					{
						return;
					}
				}
				this.c.SkipSpace();
				num++;
				if (!this.c.CheckNextCharacter(','))
				{
					flag = false;
					if (num < 16)
					{
						this.c.DefaultLength[this.c.TrackIndex][num] = -1m;
					}
				}
			}
		}

		// Token: 0x0600006D RID: 109 RVA: 0x0000ACFC File Offset: 0x00008EFC
		public string SetDefaultLengthAddSub()
		{
			bool flag = true;
			string text = "";
			while (flag)
			{
				this.c.Index++;
				char c = this.c.Read1(-1);
				if ("0123456789+-^.%# \t".IndexOf(c) < 0)
				{
					this.c.Index--;
					flag = false;
				}
				else
				{
					ReadOnlySpan<char> readOnlySpan = text;
					char c2 = c;
					text = readOnlySpan + new ReadOnlySpan<char>(ref c2);
				}
			}
			return text;
		}

		// Token: 0x0600006E RID: 110 RVA: 0x0000AD74 File Offset: 0x00008F74
		public void SetArpeggioLength()
		{
			this.c.ArpeggioNoteLengthIndex[this.c.TrackIndex] = 0;
			if (this.c.CheckNextCharacter('-'))
			{
				return;
			}
			bool flag = true;
			int num = 0;
			while (flag)
			{
				if (num > 15)
				{
					this.c.Message = "ひとつの LA コマンドで定義できる音長シーケンスは最大 16 個です。";
					this.c.ErrorString = "LA";
					return;
				}
				if (!this.c.CheckFrameMode())
				{
					decimal num2 = this.c.NumberRead(false, true);
					if (this.c.DefaultOn && num2 <= -32769m)
					{
						if (num == 0)
						{
							num2 = 16m;
						}
						else
						{
							num2 = this.c.ArpeggioDefaultLength[this.c.TrackIndex][num - 1];
						}
					}
					if (num2 < 1m)
					{
						this.c.Message = "音長として 0 が指定されています。";
						this.c.ErrorString = "LA0";
						return;
					}
					bool flag2 = false;
					while (this.c.CheckNextCharacter('.'))
					{
						if (!flag2)
						{
							num2 = 14400m / this.c.Tempo[this.c.TrackIndex] / num2;
						}
						num2 *= 1.5m;
						flag2 = true;
					}
					if (flag2)
					{
						num2 += 1000m;
					}
					this.c.ArpeggioDefaultLength[this.c.TrackIndex][num] = num2;
				}
				else
				{
					int num3 = this.c.NumberRead(false, true);
					this.c.ArpeggioDefaultLength[this.c.TrackIndex][num] = num3 + 1000;
					if (num3 < 1)
					{
						this.c.Message = "0 フレーム以下は指定できません。";
						this.c.ErrorString = "L";
						return;
					}
				}
				this.c.SkipSpace();
				num++;
				if (!this.c.CheckNextCharacter(','))
				{
					flag = false;
					if (num < 16)
					{
						this.c.ArpeggioDefaultLength[this.c.TrackIndex][num] = -1m;
					}
				}
			}
		}

		// Token: 0x0600006F RID: 111 RVA: 0x0000AFB4 File Offset: 0x000091B4
		private void SetOctave()
		{
			int num = this.c.NumberRead(false, true);
			if (this.c.DefaultOn && num <= -32769)
			{
				num = 4;
			}
			if (num < 1 || num > 8)
			{
				this.c.RangeNotice("オクターブ", 1, 8, null);
				this.c.ErrorParameter("0", num);
			}
			this.c.Octave[this.c.TrackIndex] = num;
		}

		// Token: 0x06000070 RID: 112 RVA: 0x0000B02C File Offset: 0x0000922C
		private void SetRelativeOctave(char Octave)
		{
			if (Octave == this.c.Command_OctaveDown)
			{
				this.c.Octave[this.c.TrackIndex]--;
				if (this.c.Octave[this.c.TrackIndex] < 1)
				{
					this.c.Message = this.c.TrackName[this.c.TrackIndex] + "のオクターブ指定が 1 を 下回りました。";
					this.c.ErrorString = this.c.Command_OctaveDown.ToString();
					return;
				}
			}
			else
			{
				this.c.Octave[this.c.TrackIndex]++;
				if (this.c.Octave[this.c.TrackIndex] > 8)
				{
					this.c.Message = this.c.TrackName[this.c.TrackIndex] + "のオクターブ指定が 8 を 上回りました。";
					this.c.ErrorString = this.c.Command_OctaveUp.ToString();
				}
			}
		}

		// Token: 0x06000071 RID: 113 RVA: 0x0000B150 File Offset: 0x00009350
		private void VCommand()
		{
			if (this.c.Index >= this.c.LineBuffer.Length)
			{
				this.c.Message = "音量の値が指定されていません。";
				this.c.ErrorString = "V";
				return;
			}
			if (this.c.CheckNextCharacter('O'))
			{
				this.c.SkipSpace();
				int num = this.c.NumberRead(false, true);
				if (num <= -32769)
				{
					if (!this.c.DefaultOn)
					{
						this.c.Message = "音量オフセット値が設定されていません。";
						this.c.ErrorString = "VO";
						return;
					}
					num = 0;
				}
				if (num < -15 || num > 15)
				{
					this.c.RangeNotice("音量オフセット値", -15, 15, null);
					this.c.ErrorParameter("VO", num);
					return;
				}
				this.c.VolumeOffset[this.c.TrackIndex] = num;
				return;
			}
			else if (this.c.CheckNextCharacter('I'))
			{
				this.c.SkipSpace();
				int num = this.c.NumberRead(false, true);
				if (num <= -32769)
				{
					if (!this.c.DefaultOn)
					{
						this.c.Message = "音量オフセット値が設定されていません。";
						this.c.ErrorString = "VI";
						return;
					}
					num = 0;
				}
				if (num < -127 || num > 127)
				{
					this.c.RangeNotice("音量遷移インターバル値", -127, 127, null);
					this.c.ErrorParameter("VI", num);
					return;
				}
				this.c.DataWrite(165);
				if (num < 0)
				{
					this.c.DataWrite(-num | 128);
				}
				else
				{
					this.c.DataWrite(num);
				}
				this.c.vi[this.c.TrackIndex] = num;
				if (this.c.CheckNextCharacter(','))
				{
					int num2 = this.c.NumberRead(false, true);
					if (this.c.DefaultOn && num2 <= -32769)
					{
						num2 = 0;
					}
					if (num < 0)
					{
						this.c.Message = "VI コマンドで上限の音量を設定できるのは加算時のみです。";
						this.c.ErrorParameter("VI", num, num2);
						return;
					}
					if (num2 < 0 || num2 > 15)
					{
						this.c.RangeNotice("音量遷移インターバルで上限の音量", 0, 15, "0 にすると V コマンドの値になります。");
						this.c.ErrorParameter("VI", num, num2);
						return;
					}
					this.c.DataWrite(167);
					this.c.DataWrite(num2);
					this.c.viMax[this.c.TrackIndex] = num2;
				}
				return;
			}
			else if (this.c.CheckNextCharacter('A'))
			{
				if (this.c.CheckNextCharacter('-'))
				{
					this.VolumeSequenceOff();
					return;
				}
				int num = this.c.NumberRead(false, true);
				if (num <= -32769)
				{
					num = 255;
				}
				int num3;
				if (this.c.CheckNextCharacter(','))
				{
					num3 = this.c.NumberRead(false, true);
					if (num3 <= -32769)
					{
						num3 = 255;
					}
				}
				else
				{
					num3 = 255;
				}
				if (num == 255)
				{
					if (num3 == 255)
					{
						this.VolumeSequenceOff();
						return;
					}
					if (this.c.VolumeSequenceNumber[this.c.TrackIndex] > 31)
					{
						this.c.Message = "VA コマンドの第 2 パラメータのみ指定することはできません。\n第 1 パラメータで音量シーケンス番号を指定してください。";
						this.c.ErrorString = "VA,";
						return;
					}
					num = this.c.VolumeSequenceNumber[this.c.TrackIndex];
				}
				if (num < 0 || ((num > 31) & (num != 255)))
				{
					this.c.RangeNotice("VA コマンドの第 1 パラメータは", 0, 31, null);
					this.c.ErrorParameter("VA", num);
					return;
				}
				if (num < 32 && !this.c.RegisteredVolumeSequence[num])
				{
					this.c.Message = num.ToString() + " 番の音量シーケンスは定義されていません。";
					this.c.ErrorParameter("VA", num);
					return;
				}
				if (num3 < 0 || (num3 > 15 && num3 != 255))
				{
					this.c.RangeNotice("VA コマンドの第 2 パラメータ", 0, 15, "無指定、または 255 でアルペジオ終了時の音量設定を行いません。");
					this.c.ErrorParameter("VA", num, num3);
					return;
				}
				this.c.ArpeggioVolumeSequenceNumber[this.c.TrackIndex] = num;
				this.c.ArpeggioEndVolume[this.c.TrackIndex] = num3;
				return;
			}
			else if (this.c.CheckNextCharacter('S'))
			{
				if (this.c.CheckNextCharacter('-'))
				{
					this.VolumeSequenceOff();
					return;
				}
				int num = this.c.NumberRead(false, true);
				if (this.c.DefaultOn && num <= -32769)
				{
					num = 256;
				}
				this.c.ErrorParameter("VS", num);
				if (this.c.CheckNextCharacter('='))
				{
					this.c.Message = "トラック内で音量シーケンスデータは定義できません。\n改行してトラック外で定義する必要があります。";
					Core core = this.c;
					core.ErrorString += "=";
					return;
				}
				if (num < 0 || (num > 31 && num != 255 && num != 256))
				{
					this.c.Message = "音量シーケンス番号は 0 ～ 31、「 - 」 または 255 です。\n「 - 」 または 255 で解除されます。\n\nまた、「 VS 」 のみ実行するとカウンタを先頭に戻すことができます。";
					return;
				}
				if (num == 255)
				{
					this.VolumeSequenceOff();
					return;
				}
				if (num < 256)
				{
					if (num < 32 && !this.c.RegisteredVolumeSequence[num])
					{
						this.c.Message = num.ToString() + " 番の音量シーケンスは定義されていません。";
						return;
					}
					this.c.VolumeSequenceNumber[this.c.TrackIndex] = num;
				}
				this.c.VolumeSequenceCounter[this.c.TrackIndex] = 0;
				return;
			}
			else
			{
				this.c.SkipSpace();
				this.c.Index++;
				char c = this.c.Read1(-1);
				if (c != '+')
				{
					if (c == '-')
					{
						int num = this.c.NumberRead(true, true);
						if (num < 0)
						{
							num = 1;
							while (this.c.CheckNextCharacter('-'))
							{
								num++;
							}
						}
						if (num > 15)
						{
							this.c.Message = "相対音量は V-15 ～ V+15 で指定してください。";
							this.c.ErrorParameter("V-", num);
							return;
						}
						this.c.DataWrite(176 + num);
						int num4 = this.c.Volume[this.c.TrackIndex] - num;
						this.c.VolumeLimit[this.c.TrackIndex] = ((num4 < 0) ? (-1) : 0);
						this.c.Volume[this.c.TrackIndex] = Math.Max(num4, 0);
						return;
					}
					else
					{
						this.c.Index--;
						int num = this.c.NumberRead(true, true);
						if (this.c.DefaultOn && num <= -32769)
						{
							num = 15;
						}
						if (num < 0 || num > 15)
						{
							this.c.RangeNotice("音量", 0, 15, null);
							this.c.ErrorParameter("V", num);
							return;
						}
						if (this.c.SetVolume[this.c.TrackIndex] && this.c.Volume[this.c.TrackIndex] == num)
						{
							this.c.AddInfoText("V" + num.ToString() + " : 重複指定の可能性があります。");
						}
						this.VolumeSetAndSequenceOff(num, false);
						this.c.SetVolume[this.c.TrackIndex] = true;
						return;
					}
				}
				else
				{
					int num = this.c.NumberRead(true, true);
					if (num < 0)
					{
						num = 1;
						while (this.c.CheckNextCharacter('-'))
						{
							num++;
						}
					}
					if (num > 15)
					{
						this.c.Message = "相対音量は V-15 ～ V+15 で指定してください。";
						this.c.ErrorParameter("V+", num);
						return;
					}
					this.c.DataWrite(192 + num);
					int num5 = this.c.Volume[this.c.TrackIndex] + num;
					this.c.VolumeLimit[this.c.TrackIndex] = ((num5 > 15) ? 1 : 0);
					this.c.Volume[this.c.TrackIndex] = Math.Min(num5, 15);
					return;
				}
			}
		}

		// Token: 0x06000072 RID: 114 RVA: 0x0000B994 File Offset: 0x00009B94
		private void VolumeSetAndSequenceOff(int Volume, bool VolumeRestore = false)
		{
			if (VolumeRestore && Volume == 1)
			{
				this.c.DataWrite(171);
				this.c.DataWrite(131);
				this.c.Volume[this.c.TrackIndex] = this.c.RestoreVolume[this.c.TrackIndex];
				this.c.VolumeLimit[this.c.TrackIndex] = 0;
			}
			else
			{
				Volume += this.c.VolumeOffset[this.c.TrackIndex];
				this.c.VolumeLimit[this.c.TrackIndex] = ((Volume < 0) ? (-1) : ((Volume > 15) ? 1 : 0));
				if (Volume < 0)
				{
					Volume = 0;
				}
				if (Volume > 15)
				{
					Volume = 15;
				}
				this.c.DataWrite(111 - Volume);
				this.c.Volume[this.c.TrackIndex] = Volume;
			}
			this.c.VolumeSequenceNumber[this.c.TrackIndex] = 255;
		}

		// Token: 0x06000073 RID: 115 RVA: 0x0000BAA4 File Offset: 0x00009CA4
		private void AtCommand()
		{
			bool flag = false;
			if (!flag && this.c.CheckNextCharacter('R'))
			{
				flag = this.AtRCommand(false);
			}
			if (!flag && this.c.CheckNextCharacter('P'))
			{
				flag = this.AtPCommand();
			}
			if (!flag && this.c.CheckNextCharacter('M'))
			{
				flag = this.AtMCommand();
			}
			if (!flag && this.c.CheckNextCharacter('N'))
			{
				flag = this.AtNCommand();
			}
			if (!flag && this.c.CheckNextCharacter('A'))
			{
				flag = this.AtACommand();
			}
			if (!flag && this.c.CheckNextCharacter('L'))
			{
				flag = this.SetLoopPoint();
			}
			if (!flag && this.c.CheckNextCharacter('Q'))
			{
				if (this.c.CheckNextCharacter('M'))
				{
					flag = this.SetQuantizeMode();
				}
				else
				{
					flag = this.SetQ2();
				}
			}
			if (!flag && this.c.CheckNextCharacter('D'))
			{
				flag = this.SelfDelay();
			}
			if (!flag && this.c.CheckNextCharacter('F'))
			{
				flag = this.AtFCommand();
			}
			if (!flag && this.c.CheckNextCharacter('I'))
			{
				flag = this.AtICommand();
			}
			if (!flag && this.c.CheckNextCharacter('O'))
			{
				flag = this.SetMute();
			}
			if (!flag && this.c.CheckNextCharacter('T'))
			{
				flag = this.SetTimebase();
			}
			if (!flag && this.c.CheckNextCharacter('C'))
			{
				flag = this.SetInterruptTrack();
			}
			if (!flag && this.c.CheckNextCharacter('G'))
			{
				flag = this.SetGlissMode();
			}
			if (!flag && this.c.CheckNextCharacter('K'))
			{
				flag = this.SetRelativeKeyShift();
			}
			if (!flag && this.c.CheckNextCharacter('V'))
			{
				flag = this.AtVCommand();
			}
			if (flag)
			{
				return;
			}
			if (!flag && this.c.CheckNextCharacter('S'))
			{
				this.c.Message = "@S コマンドは @RR コマンドに変更されました。";
				this.c.ErrorString = "@S";
				return;
			}
			if (this.c.CheckNextCharacter('E'))
			{
				flag = true;
			}
			int num;
			if (this.c.CheckNextCharacter('-'))
			{
				num = 255;
			}
			else if (this.c.CheckNextCharacter(','))
			{
				num = 256;
			}
			else
			{
				num = this.c.NumberRead(false, true);
			}
			if (num > -32769)
			{
				if (num < 256)
				{
					if (num == 255)
					{
						this.c.DelayVoiceNumber[this.c.TrackIndex] = -1;
						return;
					}
					this.c.ErrorParameter(flag ? "@E" : "@", num);
					if (this.c.CheckNextCharacter('='))
					{
						this.c.Message = "トラック内で音色データは定義できません。\n改行してトラック外で定義する必要があります。";
						Core core = this.c;
						core.ErrorString += "=";
						return;
					}
					if (num < 0 || num > 15)
					{
						this.c.RangeNotice("音色番号", 0, 15, null);
						return;
					}
					this.c.DataWrite(112 + num);
					this.c.UseVoice[num] = true;
					this.c.SetVoice[this.c.TrackIndex] = true;
					this.c.VoiceNumber[this.c.TrackIndex] = num;
					this.c.DelayVoiceNumber[this.c.TrackIndex] = -1;
					this.c.HardwareEnvelopeNumber[this.c.TrackIndex] = -1;
					if (this.c.VoiceHardwareEnvelope[num] < 0)
					{
						this.HardwareEnvelopeAutoSetting(false);
					}
					if (this.c.VoiceDataUseHENVHarfVolume[num])
					{
						this.HardwareEnvelopeAutoSetting(true);
					}
				}
				if (num == 256 || this.c.CheckNextCharacter(','))
				{
					num = this.c.NumberRead(false, true);
					if (num < 0 || num > 15)
					{
						this.c.RangeNotice("セルフディレイ中の音色番号", 0, 15, null);
						this.c.ErrorParameter(flag ? "@E" : "@", num);
						return;
					}
					this.c.DelayVoiceNumber[this.c.TrackIndex] = num;
					this.c.UseVoice[num] = true;
				}
				if (this.c.PitchEnvelopeNumber[this.c.TrackIndex] >= 0 && this.CheckUseHardwareEnvelopeVolumeHarf("@M/@P", "音色指定時に解除"))
				{
					this.PitchEnvelopeOff();
				}
				if (this.c.Portamento[this.c.TrackIndex] > 0 && this.CheckUseHardwareEnvelopeVolumeHarf("ポルタメント", "音色指定時に解除"))
				{
					this.c.PortamentoCommandSet[this.c.TrackIndex] = false;
					this.c.Portamento[this.c.TrackIndex] = 0;
					this.c.DataWrite(136);
					this.c.DataWrite(0);
				}
				return;
			}
			int num2 = -(num - -32769);
			if (num2 >= 65 && num2 <= 90)
			{
				Core core2 = this.c;
				ReadOnlySpan<char> readOnlySpan = "@";
				char c = (char)num2;
				core2.ErrorString = readOnlySpan + new ReadOnlySpan<char>(ref c);
				this.c.Message = this.c.ErrorString + "コマンドは存在しません。";
				return;
			}
			this.c.Message = "@ コマンドの文法エラーです。";
			this.c.ErrorString = "@";
		}

		// Token: 0x06000074 RID: 116 RVA: 0x0000BFE0 File Offset: 0x0000A1E0
		private bool SetReleaseRate()
		{
			int num = this.c.NumberRead(false, true);
			if (this.c.DefaultOn && num <= -32769)
			{
				num = 0;
			}
			if (num < 0 || num > 255)
			{
				this.c.RangeNotice("リリース時間", 0, 255, null);
				this.c.ErrorParameter("@RR", num);
				return true;
			}
			this.c.DataWrite(163);
			this.c.DataWrite(num);
			this.c.ReleaseRate[this.c.TrackIndex] = num;
			return true;
		}

		// Token: 0x06000075 RID: 117 RVA: 0x0000C07D File Offset: 0x0000A27D
		private bool AtVCommand()
		{
			this.c.Message = "音量シーケンス関連のコマンドは、\n\n・@V → VS\n・@VA → VA\n\nに変更となりました。\n@ を付けずに上記のコマンドをご使用ください。";
			this.c.ErrorString = "@V";
			return true;
		}

		// Token: 0x06000076 RID: 118 RVA: 0x0000C0A0 File Offset: 0x0000A2A0
		private void VolumeSequenceOff()
		{
			this.c.ArpeggioVolumeSequenceNumber[this.c.TrackIndex] = 255;
			this.c.ArpeggioEndVolume[this.c.TrackIndex] = 255;
			this.c.VolumeSequenceNumber[this.c.TrackIndex] = 255;
			this.c.VolumeSequenceCounter[this.c.TrackIndex] = 0;
		}

		// Token: 0x06000077 RID: 119 RVA: 0x0000C11C File Offset: 0x0000A31C
		private void QuantizeSequenceOff()
		{
			this.c.ArpeggioQuantizeSequenceNumber[this.c.TrackIndex] = 255;
			this.c.ArpeggioEndQuantize[this.c.TrackIndex] = 255;
			this.c.QuantizeSequenceNumber[this.c.TrackIndex] = 255;
			this.c.QuantizeSequenceCounter[this.c.TrackIndex] = 0;
		}

		// Token: 0x06000078 RID: 120 RVA: 0x0000C198 File Offset: 0x0000A398
		private void AtQuantizeSequenceOff()
		{
			this.c.ArpeggioAtQuantizeSequenceNumber[this.c.TrackIndex] = 255;
			this.c.ArpeggioEndAtQuantize[this.c.TrackIndex] = 255;
			this.c.AtQuantizeSequenceNumber[this.c.TrackIndex] = 255;
			this.c.AtQuantizeSequenceCounter[this.c.TrackIndex] = 0;
		}

		// Token: 0x06000079 RID: 121 RVA: 0x0000C214 File Offset: 0x0000A414
		private void HardwareEnvelope()
		{
			if (this.c.CheckNextCharacter('S'))
			{
				this.SetSequenceCommand("SS", this.c.HardwareEnvelopeDecaySequenceNumber, this.c.HardwareEnvelopeDecaySequenceCounter, this.c.RegisteredHardwareEnvelopeDecaySequence, this.c.ArpeggioHardwareEnvelopeDecaySequenceNumber);
				return;
			}
			if (this.c.CheckNextCharacter('A'))
			{
				this.SetArpeggioSequenceCommand("SA", this.c.HardwareEnvelopeDecaySequenceNumber, this.c.HardwareEnvelopeDecaySequenceCounter, this.c.ArpeggioHardwareEnvelopeDecaySequenceNumber, this.c.RegisteredHardwareEnvelopeDecaySequence);
				return;
			}
			this.c.HardwareEnvelopeDecaySequenceNumber[this.c.TrackIndex] = 255;
			this.c.HardwareEnvelopeDecaySequenceCounter[this.c.TrackIndex] = 0;
			int num = this.c.NumberRead(false, true);
			if (this.c.DefaultOn && num <= -32769)
			{
				num = this.c.HardwareEnvelopeNumber[this.c.TrackIndex];
				if (num <= 0)
				{
					num = 0;
				}
			}
			if (num < 0 || num > 15)
			{
				this.c.RangeNotice("ハードエンベロープタイプ", 0, 15, null);
				this.c.ErrorParameter("S", num);
				return;
			}
			this.c.DataWrite(144 + num);
			this.c.HardwareEnvelopeNumber[this.c.TrackIndex] = num;
			int num2 = 0;
			bool flag = false;
			if (this.c.CheckNextCharacter(','))
			{
				if (this.c.CheckNextCharacter('L'))
				{
					List<byte> list = this.c.SetNoteLength(false, true);
					if (list == null)
					{
						return;
					}
					num2 = this.SumLengthData(list);
					this.c.AddInfoText("Sコマンド第2パラメータ置換 : S" + num.ToString() + "," + num2.ToString());
				}
				else
				{
					num2 = this.c.NumberRead(false, true);
				}
				this.c.ErrorParameter("S", num, num2);
				if (num2 < 0 || num2 > 127)
				{
					this.c.RangeNotice("音量半減フレーム数は", 0, 127, null);
					return;
				}
				if (num != 8 && num != 10 && num != 12 && num != 14)
				{
					this.c.Message = "音量半減フレーム数を設定する際は、ループ型のエンベロープを指定してください。\n\n・S8\tノコギリ波\u3000|＼|＼|＼|＼\u3000（最大から）\n\n・S12\tノコギリ波\u3000／|／|／|／|\u3000（最小から）\n\n・S10\t疑似三角波\u3000＼／＼／＼（最大から）\n\n・S14\t疑似三角波\u3000／＼／＼／（最小から）";
					return;
				}
				flag = true;
			}
			this.c.DataWrite(num2);
			if (flag)
			{
				this.HardwareEnvelopeAutoSetting(true);
			}
			this.c.HardwareEnvelopeVolumeDecayFrame[this.c.TrackIndex] = num2;
			this.c.SetVoice[this.c.TrackIndex] = true;
		}

		// Token: 0x0600007A RID: 122 RVA: 0x0000C490 File Offset: 0x0000A690
		private void HardwareEnvelopeAutoSetting(bool Flag)
		{
			bool flag = this.c.HardwareEnvelopeAuto[this.c.TrackIndex] != Flag;
			if (Flag)
			{
				for (int i = 0; i < this.c.HardwareEnvelopeAuto.Length; i++)
				{
					this.c.HardwareEnvelopeAuto[i] = i == this.c.TrackIndex;
				}
			}
			else
			{
				this.c.HardwareEnvelopeAuto[this.c.TrackIndex] = false;
			}
			if (this.c.HardwareEnvelopeMode == 1 && flag)
			{
				this.c.DataWrite(Flag ? 246 : 245);
			}
		}

		// Token: 0x0600007B RID: 123 RVA: 0x0000C538 File Offset: 0x0000A738
		private void HardwareEnvelopePeriod()
		{
			if (this.c.CheckNextCharacter('+'))
			{
				if (this.c.HardwareEnvelopeNumber[this.c.TrackIndex] < 0 && (this.c.VoiceNumber[this.c.TrackIndex] < 0 || this.c.VoiceHardwareEnvelope[this.c.VoiceNumber[this.c.TrackIndex]] < 0))
				{
					this.c.Message = "ハードエンベロープ無効時に M+ が使用されています。";
					this.c.ErrorString = "M+";
					return;
				}
				int[] array = new int[] { 8, 10, 12, 14 };
				bool flag = false;
				if (array.Contains(this.c.HardwareEnvelopeNumber[this.c.TrackIndex]))
				{
					flag = true;
				}
				else if (this.c.VoiceNumber[this.c.TrackIndex] >= 0 && array.Contains(this.c.VoiceHardwareEnvelope[this.c.VoiceNumber[this.c.TrackIndex]]))
				{
					flag = true;
				}
				if (!flag)
				{
					this.c.AddInfoText("ハードエンベロープの非ループ波形で「M+」を指定しています。");
				}
				this.HardwareEnvelopeAutoSetting(true);
				return;
			}
			else
			{
				if (this.c.CheckNextCharacter('-'))
				{
					this.HardwareEnvelopeAutoSetting(false);
					return;
				}
				if (this.c.CheckNextCharacter('O'))
				{
					this.c.ErrorString = "MO";
					int num = this.c.NumberRead(false, true);
					if (num <= -32769)
					{
						if (!this.c.DefaultOn)
						{
							this.c.Message = "ハードエンベロープ周期のオクターブ指定が正しくありません。";
							return;
						}
						num = 4;
					}
					if (num < 1 || num > 8)
					{
						this.c.RangeNotice("ハードエンベロープ周期のオクターブ指定", 1, 8, null);
						this.c.ErrorString = "MO" + num.ToString();
						return;
					}
					this.c.HardwareEnvelopeOctave = num - 1;
					return;
				}
				else if (this.c.CheckNextCharacter('\\'))
				{
					if (this.c.CheckNextCharacter('S'))
					{
						this.SetSequenceCommand("M\\S", this.c.HardwareEnvelopeDetuneSequenceNumber, this.c.HardwareEnvelopeDetuneSequenceCounter, this.c.RegisteredHardwareEnvelopeDetuneSequence, this.c.ArpeggioHardwareEnvelopeDetuneSequenceNumber);
						return;
					}
					if (this.c.CheckNextCharacter('A'))
					{
						this.SetArpeggioSequenceCommand("M\\A", this.c.HardwareEnvelopeDetuneSequenceNumber, this.c.HardwareEnvelopeDetuneSequenceCounter, this.c.ArpeggioHardwareEnvelopeDetuneSequenceNumber, this.c.RegisteredHardwareEnvelopeDetuneSequence);
						return;
					}
					this.c.ErrorString = "M\\";
					int num2 = this.c.NumberRead(false, true);
					if (num2 <= -32769)
					{
						if (!this.c.DefaultOn)
						{
							this.c.Message = "ハードエンベロープ周期のデチューン指定が正しくありません。";
							return;
						}
						num2 = 0;
					}
					int num3 = ((this.c.HardwareEnvelopeMode == 1) ? (-128) : (-1000));
					int num4 = ((this.c.HardwareEnvelopeMode == 1) ? 127 : 1000);
					if (num2 < num3 || num2 > num4)
					{
						this.c.RangeNotice("ハードエンベロープ周期のデチューン指定", num3, num4, null);
						this.c.ErrorString = "M\\" + num2.ToString();
						return;
					}
					this.c.HardwareEnvelopeDetune = num2;
					if (this.c.HardwareEnvelopeMode == 1)
					{
						this.c.DataWrite(248);
						this.c.DataWrite(num2 & 255);
					}
					return;
				}
				else if (this.c.CheckNextCharacter('K'))
				{
					if (this.c.CheckNextCharacter('S'))
					{
						this.SetSequenceCommand("MKS", this.c.HardwareEnvelopeKeyShiftSequenceNumber, this.c.HardwareEnvelopeKeyShiftSequenceCounter, this.c.RegisteredHardwareEnvelopeKeyShiftSequence, this.c.ArpeggioHardwareEnvelopeKeyShiftSequenceNumber);
						return;
					}
					if (this.c.CheckNextCharacter('A'))
					{
						this.SetArpeggioSequenceCommand("MKA", this.c.HardwareEnvelopeKeyShiftSequenceNumber, this.c.HardwareEnvelopeKeyShiftSequenceCounter, this.c.ArpeggioHardwareEnvelopeKeyShiftSequenceNumber, this.c.RegisteredHardwareEnvelopeKeyShiftSequence);
						return;
					}
					int num5 = this.c.NumberRead(false, true);
					if (this.c.DefaultOn && num5 <= -32769)
					{
						num5 = 0;
					}
					if (num5 < -84 || num5 > 84)
					{
						this.c.RangeNotice("ハードエンベロープのトランスポーズ", -84, 84, null);
						this.c.ErrorParameter("MK", num5);
						return;
					}
					this.c.HardwareEnvelopeKeyShift = num5;
					if (this.c.HardwareEnvelopeMode == 1)
					{
						this.c.DataWrite(247);
						this.c.DataWrite(num5 & 255);
					}
					if (this.c.HardwareEnvelopeVolumeDecayFrame[this.c.TrackIndex] > 0 && num5 != 0)
					{
						this.c.AddInfoText("Sの第2パラメータが1以上のときにMKコマンドが実行されました。基音が移調されます。");
					}
					return;
				}
				else
				{
					this.HardwareEnvelopeAutoSetting(false);
					this.c.SkipSpace();
					this.c.Index++;
					if (this.c.Index >= this.c.LineBuffer.Length)
					{
						return;
					}
					int num6 = "CDEFGAB".IndexOf(this.c.LineBuffer[this.c.Index]);
					if (num6 < 0)
					{
						this.c.Index--;
						num6 = this.c.NumberRead(false, true) + this.c.HardwareEnvelopeDetune;
					}
					else
					{
						int num7 = this.c.NoteNumberTable[num6];
						this.c.Index++;
						if (this.c.Index < this.c.LineBuffer.Length)
						{
							char c = this.c.LineBuffer[this.c.Index];
							if (c != '+')
							{
								if (c != '-')
								{
									this.c.Index--;
								}
								else
								{
									num7--;
								}
							}
							else
							{
								num7++;
							}
						}
						num6 = this.NoteNumberToHardwareEnvelopePeriod(num7 + this.c.HardwareEnvelopeOctave * 12 + this.c.KeyShift[this.c.TrackIndex]);
						if (num6 < 0)
						{
							return;
						}
					}
					this.SetHardwareEnvelopePeriod(num6, null);
					return;
				}
			}
		}

		// Token: 0x0600007C RID: 124 RVA: 0x0000CB94 File Offset: 0x0000AD94
		private void SetHardwareEnvelopePeriod(int Period, string errorString = null)
		{
			if (Period < 0 || Period > 65535)
			{
				this.c.Message = "ハードエンベロープ周期が 0 ～ 65535 の範囲外になりました。";
				return;
			}
			this.c.DataWrite(160);
			this.c.DataWrite(Period & 255);
			this.c.DataWrite(Period / 256);
			this.c.HardwareEnvelopePeriod[this.c.TrackIndex] = Period;
		}

		// Token: 0x0600007D RID: 125 RVA: 0x0000CC0C File Offset: 0x0000AE0C
		private int NoteNumberToHardwareEnvelopePeriod(int NoteNumber)
		{
			NoteNumber += this.c.HardwareEnvelopeKeyShift;
			if (NoteNumber < 0 || NoteNumber > 95)
			{
				this.c.Message = "ハードエンベロープ周期のピッチ指定が 「 MO1 MC 」～「 MO8 MB- 」 の範囲を超えています。";
				return -1;
			}
			int num = NoteNumber / 12;
			return (this.c.HardwareEnvelopePitchTable[NoteNumber - num * 12] >> num) - this.c.HardwareEnvelopeDetune;
		}

		// Token: 0x0600007E RID: 126 RVA: 0x0000CC6B File Offset: 0x0000AE6B
		private void WriteQuantize(int value, bool force = false)
		{
			if (!force && this.QuantizeMode[this.c.TrackIndex] == 1)
			{
				return;
			}
			this.c.DataWrite(134);
			this.c.DataWrite(value);
		}

		// Token: 0x0600007F RID: 127 RVA: 0x0000CCA4 File Offset: 0x0000AEA4
		private void WriteAtQuantize(int value, bool force = false)
		{
			if (!force && this.QuantizeMode[this.c.TrackIndex] == 1)
			{
				return;
			}
			if (value > 255)
			{
				this.c.DataWrite(170);
				this.c.DataWrite(value - 257);
				return;
			}
			this.c.DataWrite(162);
			this.c.DataWrite(value);
		}

		// Token: 0x06000080 RID: 128 RVA: 0x0000CD14 File Offset: 0x0000AF14
		private bool SetQuantizeMode()
		{
			int num = this.c.NumberRead(false, true);
			if (num != 0 && num != 1)
			{
				this.c.RangeNotice("クオンタイズモード", 0, 1, null);
				this.c.ErrorParameter("@QM", num);
				return true;
			}
			if (this.QuantizeMode[this.c.TrackIndex] == num)
			{
				return true;
			}
			this.QuantizeMode[this.c.TrackIndex] = num;
			if (num == 1)
			{
				this.WriteQuantize(8, true);
				this.WriteAtQuantize(0, true);
			}
			else
			{
				this.WriteQuantize((this.c.QuantizeCount[this.c.TrackIndex] > 0) ? this.c.QuantizeBackup[this.c.TrackIndex] : this.c.Quantize[this.c.TrackIndex], true);
				this.WriteAtQuantize((this.c.AtQuantizeCount[this.c.TrackIndex] > 0) ? this.c.AtQuantizeBackup[this.c.TrackIndex] : this.c.AtQuantize[this.c.TrackIndex], true);
			}
			return true;
		}

		// Token: 0x06000081 RID: 129 RVA: 0x0000CE44 File Offset: 0x0000B044
		private void SetQ()
		{
			if (this.c.CheckNextCharacter('A'))
			{
				if (this.c.CheckNextCharacter('-'))
				{
					this.QuantizeSequenceOff();
				}
				int num = this.c.NumberRead(false, true);
				if (num <= -32769)
				{
					num = 255;
				}
				int num2;
				if (this.c.CheckNextCharacter(','))
				{
					num2 = this.c.NumberRead(false, true);
					if (num2 <= -32769)
					{
						num2 = 255;
					}
				}
				else
				{
					num2 = 255;
				}
				if (num == 255)
				{
					if (num == 255)
					{
						this.QuantizeSequenceOff();
						return;
					}
					if (this.c.QuantizeSequenceNumber[this.c.TrackIndex] > 31)
					{
						this.c.Message = "QA コマンドの第 2 パラメータのみ指定することはできません。\n第 1 パラメータでシーケンス番号を指定してください。";
						this.c.ErrorString = "QA,";
						return;
					}
					num = this.c.QuantizeSequenceNumber[this.c.TrackIndex];
				}
				if (num < 0 || ((num > 31) & (num != 255)))
				{
					this.c.RangeNotice("QA コマンドの第 1 パラメータは", 0, 31, null);
					this.c.ErrorParameter("QA", num);
					return;
				}
				if (num < 32 && !this.c.RegisteredQuantizeSequence[num])
				{
					this.c.Message = num.ToString() + " 番のQシーケンスは定義されていません。";
					this.c.ErrorParameter("QA", num);
					return;
				}
				if (num2 < 0 || (num2 > 8 && num2 != 255))
				{
					this.c.RangeNotice("QA コマンドの第 2 パラメータ", 1, 8, "無指定、0、255 のいずれかでアルペジオ終了時のゲートタイム設定を行いません。");
					this.c.ErrorParameter("QA", num, num2);
					return;
				}
				this.c.ArpeggioQuantizeSequenceNumber[this.c.TrackIndex] = num;
				this.c.ArpeggioEndQuantize[this.c.TrackIndex] = num2;
				return;
			}
			else if (this.c.CheckNextCharacter('S'))
			{
				if (this.c.CheckNextCharacter('-'))
				{
					this.QuantizeSequenceOff();
					return;
				}
				int num3 = this.c.NumberRead(false, true);
				if (this.c.DefaultOn && num3 <= -32769)
				{
					num3 = 256;
				}
				this.c.ErrorParameter("QS", num3);
				if (this.c.CheckNextCharacter('='))
				{
					this.c.Message = "トラック内でQシーケンスデータは定義できません。\n改行してトラック外で定義する必要があります。";
					Core core = this.c;
					core.ErrorString += "=";
					return;
				}
				if (num3 < 0 || (num3 > 31 && num3 != 255 && num3 != 256))
				{
					this.c.Message = "Qシーケンス番号は 0 ～ 31、「 - 」 または 255 です。\n「 - 」 または 255 で解除されます。\n\nまた、「 QS 」 のみ実行するとカウンタを先頭に戻すことができます。";
					return;
				}
				if (num3 == 255)
				{
					this.QuantizeSequenceOff();
					return;
				}
				if (num3 < 256)
				{
					if (num3 < 32 && !this.c.RegisteredQuantizeSequence[num3])
					{
						this.c.Message = num3.ToString() + " 番のQシーケンスは定義されていません。";
						this.c.ErrorParameter("QS", num3);
						return;
					}
					this.c.QuantizeSequenceNumber[this.c.TrackIndex] = num3;
				}
				this.c.QuantizeSequenceCounter[this.c.TrackIndex] = 0;
				return;
			}
			else
			{
				int num4 = this.c.NumberRead(true, true);
				if (this.c.CheckNextCharacter('+'))
				{
					this.c.Message = "Q コマンドで 「 + 」 は使用できません。\nゲートタイムのフレーム加算には @Q+ をご使用ください。";
					Core core2 = this.c;
					core2.ErrorString += "Q+";
					return;
				}
				if (this.c.CheckNextCharacter('-'))
				{
					this.c.Message = "Q コマンドで 「 - 」 は使用できません。\n固定ゲートタイムには @Q% をご使用ください。";
					Core core3 = this.c;
					core3.ErrorString += "Q-";
					return;
				}
				if (this.c.DefaultOn && num4 <= -32769)
				{
					num4 = 8;
					this.c.QuantizeNumber[this.c.TrackIndex] = 0;
					this.c.QuantizeCount[this.c.TrackIndex] = 0;
					this.c.AtQuantizeNumber[this.c.TrackIndex] = 0;
					this.c.AtQuantizeCount[this.c.TrackIndex] = 0;
					this.c.AtQuantize[this.c.TrackIndex] = 0;
					this.WriteAtQuantize(0, false);
				}
				if (num4 < 0 || num4 > 8)
				{
					this.c.RangeNotice("ゲートタイム（クオンタイズ）", 0, 8, null);
					this.c.ErrorParameter("Q", num4);
					return;
				}
				if (this.c.SetQuantize[this.c.TrackIndex] && this.c.Quantize[this.c.TrackIndex] == num4)
				{
					this.c.AddInfoText("Q" + num4.ToString() + " : 重複指定の可能性があります。");
				}
				this.WriteQuantize(num4, false);
				this.c.QuantizeBackup[this.c.TrackIndex] = num4;
				this.c.SetQuantize[this.c.TrackIndex] = true;
				if (this.c.CheckNextCharacter(','))
				{
					int num5 = this.c.NumberRead(false, true);
					if (num5 < 0 || num5 > 10)
					{
						this.c.RangeNotice("ゲートタイム（クオンタイズ）を有効にする音数", 0, 10, null);
						this.c.ErrorParameter("Q", num4, num5);
					}
					this.c.QuantizeNumber[this.c.TrackIndex] = num5;
					this.c.QuantizeCount[this.c.TrackIndex] = num5;
				}
				else
				{
					this.c.QuantizeCount[this.c.TrackIndex] = this.c.QuantizeNumber[this.c.TrackIndex];
					if (this.c.QuantizeCount[this.c.TrackIndex] == 0)
					{
						this.c.Quantize[this.c.TrackIndex] = num4;
					}
				}
				this.QuantizeSequenceOff();
				return;
			}
		}

		// Token: 0x06000082 RID: 130 RVA: 0x0000D430 File Offset: 0x0000B630
		private bool SetQ2()
		{
			if (this.c.CheckNextCharacter('-'))
			{
				this.c.Message = "固定ゲートタイムは 「 @Q- 」 から 「 @Q% 」 に変更となりました。";
				this.c.ErrorString = "@Q-";
				return true;
			}
			if (this.c.CheckNextCharacter('A'))
			{
				if (this.c.CheckNextCharacter('-'))
				{
					this.AtQuantizeSequenceOff();
				}
				int num = this.c.NumberRead(false, true);
				if (num <= -32769)
				{
					num = 255;
				}
				int num2;
				if (this.c.CheckNextCharacter(','))
				{
					num2 = this.c.NumberRead(false, true);
					if (num2 <= -32769)
					{
						num2 = 255;
					}
				}
				else
				{
					num2 = 255;
				}
				if (num == 255)
				{
					if (num == 255)
					{
						this.AtQuantizeSequenceOff();
						return true;
					}
					if (this.c.AtQuantizeSequenceNumber[this.c.TrackIndex] > 31)
					{
						this.c.Message = "@QA コマンドの第 2 パラメータのみ指定することはできません。\n第 1 パラメータでシーケンス番号を指定してください。";
						this.c.ErrorString = "@QA,";
						return true;
					}
					num = this.c.AtQuantizeSequenceNumber[this.c.TrackIndex];
				}
				if (num < 0 || ((num > 31) & (num != 255)))
				{
					this.c.RangeNotice("@QA コマンドの第 1 パラメータは", 0, 31, null);
					this.c.ErrorParameter("@QA", num);
					return true;
				}
				if (num < 32 && !this.c.RegisteredAtQuantizeSequence[num])
				{
					this.c.Message = num.ToString() + " 番の@Qシーケンスは定義されていません。";
					this.c.ErrorParameter("@QA", num);
					return true;
				}
				if (num2 < 0 || num2 > 255)
				{
					this.c.RangeNotice("@QA コマンドの第 2 パラメータ", 0, 254, "無指定、または 255 でアルペジオ終了時のゲートタイム設定を行いません。");
					this.c.ErrorParameter("@QA", num, num2);
					return true;
				}
				this.c.ArpeggioAtQuantizeSequenceNumber[this.c.TrackIndex] = num;
				this.c.ArpeggioEndAtQuantize[this.c.TrackIndex] = num2;
				return true;
			}
			else
			{
				if (!this.c.CheckNextCharacter('S'))
				{
					int num3;
					if (this.c.CheckNextCharacter('%'))
					{
						num3 = this.c.NumberRead(false, true);
						if (this.c.DefaultOn && num3 <= -32769)
						{
							num3 = 0;
							this.c.QuantizeNumber[this.c.TrackIndex] = 0;
						}
						if (num3 < 0 || num3 > 128)
						{
							this.c.RangeNotice("固定ゲートタイム", 0, 128, "0 や「@Q%」のみの記述、またはQ/@Qコマンドを実行すると解除されます。");
							this.c.ErrorParameter("@Q%", num3);
							return true;
						}
						if (this.c.SetAtQuantize[this.c.TrackIndex] && this.c.AtQuantize[this.c.TrackIndex] - 257 == num3)
						{
							this.c.AddInfoText("@Q%" + num3.ToString() + " : 重複指定の可能性があります。");
						}
						this.WriteAtQuantize(num3 + 257, false);
						this.c.AtQuantizeBackup[this.c.TrackIndex] = num3 + 257;
						this.c.SetAtQuantize[this.c.TrackIndex] = true;
					}
					else
					{
						if (this.c.CheckNextCharacter('+'))
						{
							num3 = this.c.NumberRead(true, true);
							if (this.c.DefaultOn && num3 <= -32769)
							{
								num3 = 0;
								this.c.AtQuantizeNumber[this.c.TrackIndex] = 0;
							}
							if (num3 < 0 || num3 > 126)
							{
								this.c.RangeNotice("フレーム加算ゲートタイム", 0, 126, null);
								this.c.ErrorParameter("@Q+", num3);
								return true;
							}
							num3 += 129;
							if (this.c.SetAtQuantize[this.c.TrackIndex] && this.c.AtQuantize[this.c.TrackIndex] - 129 == num3)
							{
								this.c.AddInfoText("@Q+" + num3.ToString() + " : 重複指定の可能性があります。");
							}
						}
						else
						{
							num3 = this.c.NumberRead(false, true);
							if (this.c.DefaultOn && num3 <= -32769)
							{
								num3 = 0;
								this.c.AtQuantizeNumber[this.c.TrackIndex] = 0;
							}
							if (num3 < 0 || num3 > 128)
							{
								this.c.RangeNotice("フレーム減算ゲートタイム", 0, 128, null);
								this.c.ErrorParameter("@Q", num3);
								return true;
							}
							if (this.c.SetAtQuantize[this.c.TrackIndex] && this.c.AtQuantize[this.c.TrackIndex] == num3)
							{
								this.c.AddInfoText("@Q" + num3.ToString() + " : 重複指定の可能性があります。");
							}
						}
						this.WriteAtQuantize(num3, false);
						this.c.AtQuantizeBackup[this.c.TrackIndex] = num3;
						this.c.SetAtQuantize[this.c.TrackIndex] = true;
					}
					if (this.c.CheckNextCharacter(','))
					{
						int num4 = this.c.NumberRead(false, true);
						if (num4 < 0 || num4 > 10)
						{
							this.c.RangeNotice("フレーム加減算ゲートタイムを有効にする音数", 0, 10, null);
							this.c.ErrorParameter("Q", num3, num4);
						}
						this.c.AtQuantizeNumber[this.c.TrackIndex] = num4;
						this.c.AtQuantizeCount[this.c.TrackIndex] = num4;
					}
					else
					{
						this.c.AtQuantizeCount[this.c.TrackIndex] = this.c.AtQuantizeNumber[this.c.TrackIndex];
						if (this.c.AtQuantizeCount[this.c.TrackIndex] == 0)
						{
							this.c.AtQuantize[this.c.TrackIndex] = num3;
						}
					}
					this.AtQuantizeSequenceOff();
					return true;
				}
				if (this.c.CheckNextCharacter('-'))
				{
					this.AtQuantizeSequenceOff();
					return true;
				}
				int num5 = this.c.NumberRead(false, true);
				if (this.c.DefaultOn && num5 <= -32769)
				{
					num5 = 256;
				}
				this.c.ErrorParameter("@QS", num5);
				if (this.c.CheckNextCharacter('='))
				{
					this.c.Message = "トラック内で@Qシーケンスデータは定義できません。\n改行してトラック外で定義する必要があります。";
					Core core = this.c;
					core.ErrorString += "=";
					return true;
				}
				if (num5 < 0 || (num5 > 31 && num5 != 255 && num5 != 256))
				{
					this.c.Message = "@Qシーケンス番号は 0 ～ 31、「 - 」 または 255 です。\n「 - 」 または 255 で解除されます。\n\nまた、「 @QS 」 のみ実行するとカウンタを先頭に戻すことができます。";
					return true;
				}
				if (num5 == 255)
				{
					this.AtQuantizeSequenceOff();
				}
				else
				{
					if (num5 < 256)
					{
						if (num5 < 32 && !this.c.RegisteredAtQuantizeSequence[num5])
						{
							this.c.Message = num5.ToString() + " 番の@Qシーケンスは定義されていません。";
							this.c.ErrorParameter("QS", num5);
							return true;
						}
						this.c.AtQuantizeSequenceNumber[this.c.TrackIndex] = num5;
					}
					this.c.AtQuantizeSequenceCounter[this.c.TrackIndex] = 0;
				}
				return true;
			}
		}

		// Token: 0x06000083 RID: 131 RVA: 0x0000DB98 File Offset: 0x0000BD98
		public bool AtFCommand()
		{
			if (this.c.CheckNextCharacter('O'))
			{
				int num = this.c.NumberRead(false, true);
				if (this.c.DefaultOn && num <= -32769)
				{
					num = 20;
				}
				if (num < 1 || num > 255)
				{
					this.c.RangeNotice("フェードアウト", 1, 255, null);
					this.c.ErrorParameter("@FO", num);
					return true;
				}
				if (this.c.FadeOutFrame > 0)
				{
					this.c.AddInfoText("複数の@FOを検出したため、2個目以降はスキップします。複数のトラックで使用していないかをご確認ください。");
				}
				else
				{
					this.c.DataWrite(240);
					this.c.DataWrite(num);
					this.c.FadeOutFrame = num;
				}
				int num2;
				if (this.c.CheckNextCharacter(','))
				{
					num2 = this.c.NumberRead(false, true);
					if (num2 < 1 || num2 > 255)
					{
						this.c.RangeNotice("フェードアウトまでのループ数", 1, 255, null);
						this.c.ErrorParameter("@FO", num, num2);
						return true;
					}
				}
				else
				{
					num2 = 1;
				}
				if (this.c.FadeOutLoopCount == 0)
				{
					this.c.DataWrite(num2);
					this.c.FadeOutLoopCount = num2;
				}
				return true;
			}
			else
			{
				int num3 = this.c.NumberRead(false, true);
				if (this.c.SkipF || this.c.PreCompile)
				{
					return true;
				}
				if (num3 <= -32769)
				{
					num3 = 2;
				}
				if (num3 < 0 || num3 > 3)
				{
					this.c.RangeNotice("フレーム数表示コマンド", 0, 3, "\nトラックごとに @F0 ～ @F1 の範囲の合計フレーム数を表示できます。\n@F（値を省略）または @F2 はトラックの先頭からの総フレーム数となります。\n\nまた、@F3 でそこから10個分の各音符のフレーム数を表示できます。");
					this.c.ErrorParameter("@F", num3);
					return true;
				}
				if (num3 < 3)
				{
					if (this.c.AtF_Frame[num3][this.c.TrackIndex] >= 0)
					{
						this.c.Message = "重複指定です。\nこのトラックでは既に @F" + ((num3 == 2) ? " / @F2" : num3.ToString()) + " コマンドが使用されています。";
						this.c.ErrorParameter("@F", num3);
						return true;
					}
					this.c.AtF_Frame[num3][this.c.TrackIndex] = this.c.GetFrame();
				}
				else
				{
					this.c.FrameAnalyzeCount[this.c.TrackIndex] = 1;
				}
				return true;
			}
		}

		// Token: 0x06000084 RID: 132 RVA: 0x0000DDE0 File Offset: 0x0000BFE0
		private void SaveAtINoteMonitorParameter()
		{
			int trackIndex = this.c.TrackIndex;
			if (trackIndex < 0 || trackIndex >= this.AtINoteMonitorParameters.Length)
			{
				return;
			}
			int num = ((this.c.BeforeLoopFrame[trackIndex, this.c.SoundId] < 0) ? 0 : this.c.BeforeLoopFrame[trackIndex, this.c.SoundId]) + this.c.TrackFrame[trackIndex, this.c.SoundId];
			int interruptTrackAtFrame = this.c.GetInterruptTrackAtFrame(this.c.GetFrame());
			this.AtINoteMonitorParameters[trackIndex] = new NotePlayParameter
			{
				TrackIndex = trackIndex,
				LineNumber = this.c.LineNumber,
				Note = 'I',
				NoteName = "@I",
				WrittenNoteName = "@I",
				NoteNameAccidental = 0,
				Octave = this.c.Octave[trackIndex],
				KeyShift = this.c.KeyShift[trackIndex],
				RelativeKeyShift = this.c.RelativeKeyShift[trackIndex],
				Volume = this.c.Volume[trackIndex],
				VolumeOffset = this.c.VolumeOffset[trackIndex],
				VolumeIncrease = this.c.vi[trackIndex],
				VolumeIncreaseMax = this.c.viMax[trackIndex],
				VoiceNumber = this.c.VoiceNumber[trackIndex],
				DelayVoiceNumber = this.c.DelayVoiceNumber[trackIndex],
				Quantize = this.c.Quantize[trackIndex],
				AtQuantize = this.c.AtQuantize[trackIndex],
				Detune = this.c.Detune[trackIndex],
				InterruptTrack = interruptTrackAtFrame,
				RepeatMode = this.c.RepeatMode[trackIndex],
				GlissLegato = this.c.GlissLegato[trackIndex],
				Portamento = this.c.Portamento[trackIndex],
				Tempo = this.c.Tempo[trackIndex],
				LengthText = "",
				FrameLength = 0,
				GateFrameLength = 0,
				ReleaseVolume = this.c.ReleaseVolume[trackIndex],
				ReleaseRate = this.c.ReleaseRate[trackIndex],
				ReleaseDelay = this.c.ReleaseDelay[trackIndex],
				ReleaseDelayFrame = this.c.ReleaseDelayFrame[trackIndex],
				PitchEnvelopeNumber = this.c.PitchEnvelopeNumber[trackIndex],
				PitchEnvelopeType = this.c.UsePitchEnvelopeType[trackIndex],
				HardwareEnvelopeNumber = this.c.HardwareEnvelopeNumber[trackIndex],
				HardwareEnvelopeVolumeDecayFrame = this.c.HardwareEnvelopeVolumeDecayFrame[trackIndex],
				NoteEnvelopeNumber = this.c.NoteEnvelopeNumber[trackIndex],
				SelfDelayCount = this.c.SelfDelayCount[trackIndex],
				SelfDelayVolume = this.c.SelfDelayVolume[trackIndex],
				CurrentFrame = num,
				VolumeLimit = this.c.VolumeLimit[trackIndex],
				RhythmCodes = ((trackIndex == 0) ? this.c.StoreRhythmCode : ""),
				RhythmActiveCodes = ((trackIndex == 0 && !string.IsNullOrEmpty(this.c.StoreRhythmCode)) ? new string('-', this.c.StoreRhythmCode.Length) : "")
			};
			this.AtINoteMonitorColumns[trackIndex] = -1;
			if (this.c.CurrentSourceMap == null || this.c.CurrentSourceMap.Count == 0)
			{
				return;
			}
			for (int i = Math.Min(Math.Max(this.c.Index, 0), this.c.CurrentSourceMap.Count - 1); i >= 0; i--)
			{
				MMLSourcePosition mmlsourcePosition = this.c.CurrentSourceMap[i];
				if (mmlsourcePosition.IsValid)
				{
					if (mmlsourcePosition.Line == this.c.LineNumber)
					{
						this.AtINoteMonitorColumns[trackIndex] = mmlsourcePosition.Column;
						return;
					}
					if (mmlsourcePosition.Line < this.c.LineNumber)
					{
						break;
					}
				}
			}
		}

		// Token: 0x06000085 RID: 133 RVA: 0x0000E20C File Offset: 0x0000C40C
		public bool AtICommand()
		{
			if (this.c.CheckNextCharacter('1'))
			{
				return this.AtI1Command();
			}
			this.c.CheckNextCharacter('0');
			if (this.c.PreCompile && !this.c.GetTimeCompile)
			{
				this.c.InfoCompile = true;
				return true;
			}
			if (!this.c.InfoCompile || this.c.GetTimeCompile)
			{
				return true;
			}
			int trackIndex = this.c.TrackIndex;
			string text = this.c.TrackCode[trackIndex];
			this.SaveAtINoteMonitorParameter();
			this.c.ParameterText.Add(string.Concat(new string[]
			{
				"\n;====== ",
				this.c.TrackName[trackIndex],
				" (",
				(((this.c.BeforeLoopFrame[trackIndex, this.c.SoundId] < 0) ? 0 : this.c.BeforeLoopFrame[trackIndex, this.c.SoundId]) + this.c.TrackFrame[trackIndex, this.c.SoundId]).ToString(),
				"フレーム目) ======\n"
			}));
			string text2 = text + "\tt" + this.c.Tempo[trackIndex].ToString();
			if (trackIndex > 0)
			{
				this.c.ParameterText.Add(";\t【基本】");
				int num = 0;
				string text3 = "";
				string text4 = "";
				foreach (int num2 in this.c.KeySign[trackIndex])
				{
					if (num2 < 0)
					{
						text3 += " ccddeffggaabb".Substring(num, 1);
					}
					if (num2 > 0)
					{
						text4 += " ccddeffggaabb".Substring(num, 1);
					}
					num++;
				}
				if (text3 != "")
				{
					text2 = text2 + " ks-" + text3;
				}
				if (text4 != "")
				{
					text2 = text2 + " ks+" + text4;
				}
			}
			this.c.ParameterText.Add(text2);
			if (trackIndex > 0)
			{
				string text5 = text + "\to" + this.c.Octave[trackIndex].ToString();
				text5 += " q";
				if (this.c.QuantizeBackup[trackIndex] > 0 && this.c.QuantizeCount[trackIndex] > 0)
				{
					text5 = text5 + this.c.QuantizeBackup[trackIndex].ToString() + "," + this.c.QuantizeNumber[trackIndex].ToString();
				}
				else
				{
					text5 += this.c.Quantize[trackIndex].ToString();
				}
				text5 += " @q";
				if (this.c.AtQuantizeCount[trackIndex] > 0)
				{
					if (this.c.AtQuantizeBackup[trackIndex] < 129)
					{
						text5 += this.c.AtQuantizeBackup[trackIndex].ToString();
					}
					else if (this.c.AtQuantizeBackup[trackIndex] < 256)
					{
						text5 = text5 + "+" + (this.c.AtQuantizeBackup[trackIndex] - 129).ToString();
					}
					else
					{
						text5 = text5 + "%" + (this.c.AtQuantizeBackup[trackIndex] - 257).ToString();
					}
					text5 = text5 + "," + this.c.AtQuantizeNumber[trackIndex].ToString();
				}
				else if (this.c.AtQuantize[trackIndex] < 129)
				{
					text5 += this.c.AtQuantize[trackIndex].ToString();
				}
				else if (this.c.AtQuantize[trackIndex] < 256)
				{
					text5 = text5 + "+" + (this.c.AtQuantize[trackIndex] - 129).ToString();
				}
				else
				{
					text5 = text5 + "%" + (this.c.AtQuantize[trackIndex] - 257).ToString();
				}
				text5 = text5 + " \\" + (-this.c.Detune[trackIndex]).ToString();
				string text6 = ((this.c.VoiceNumber[trackIndex] < 0) ? "" : this.c.VoiceNumber[trackIndex].ToString()) + ((this.c.DelayVoiceNumber[trackIndex] < 0) ? "" : ("," + this.c.DelayVoiceNumber[trackIndex].ToString()));
				if (text6 != "")
				{
					text5 = text5 + " @" + text6;
				}
				if (this.c.HardwareEnvelopeNumber[trackIndex] >= 0)
				{
					text5 = string.Concat(new string[]
					{
						text5,
						" s",
						this.c.HardwareEnvelopeNumber[trackIndex].ToString(),
						",",
						this.c.HardwareEnvelopeVolumeDecayFrame[trackIndex].ToString()
					});
				}
				else
				{
					text5 = text5 + " v" + this.c.Volume[trackIndex].ToString();
				}
				text5 = string.Concat(new string[]
				{
					text5,
					" vo",
					(this.c.VolumeOffset[trackIndex] > 0) ? "+" : "",
					this.c.VolumeOffset[trackIndex].ToString(),
					" vi",
					(this.c.vi[trackIndex] > 0) ? "+" : "",
					this.c.vi[trackIndex].ToString(),
					(this.c.viMax[trackIndex] < 0) ? "" : ("," + this.c.viMax[trackIndex].ToString())
				});
				this.c.ParameterText.Add(text5);
			}
			int interruptTrackAtFrame = this.c.GetInterruptTrackAtFrame(this.c.GetFrame());
			this.c.ParameterText.Add(string.Concat(new string[]
			{
				text,
				"\t@c",
				interruptTrackAtFrame.ToString(),
				" @rm",
				this.c.RepeatMode[trackIndex].ToString(),
				(trackIndex > 0) ? string.Concat(new string[]
				{
					" @g",
					this.c.GlissLegato[trackIndex] ? "1" : "0",
					" k",
					this.c.KeyShift[trackIndex].ToString(),
					" @k",
					this.c.RelativeKeyShift[trackIndex].ToString(),
					" p",
					this.c.Portamento[trackIndex].ToString()
				}) : ""
			}));
			this.c.ParameterText.Add(this.GenerateLengthMML(text + "\tl") + "\n");
			if (trackIndex == 0)
			{
				string text7 = "";
				int num3 = 0;
				foreach (int num4 in this.c.RhythmVolume)
				{
					text7 = string.Concat(new string[]
					{
						text7,
						"V",
						this.c.StoreRhythmCode[num3].ToString(),
						num4.ToString(),
						" "
					});
					num3++;
					if (num3 % 7 == 0)
					{
						text7 += "\n";
					}
				}
				foreach (string text8 in text7.Split('\n', StringSplitOptions.None))
				{
					this.c.ParameterText.Add("R\t" + text8);
				}
				return true;
			}
			this.c.ParameterText.Add(";\t【エフェクト】");
			string text9 = text + "\t" + ((this.c.PitchEnvelopeNumber[trackIndex] < 0) ? "@p- @m-" : ("@" + ((this.c.PitchEnvelopeType[this.c.PitchEnvelopeNumber[trackIndex]] == 1) ? "p" : "m") + this.c.PitchEnvelopeNumber[trackIndex].ToString()));
			text9 = text9 + " @n" + ((this.c.NoteEnvelopeNumber[trackIndex] < 0) ? "-" : this.c.NoteEnvelopeNumber[trackIndex].ToString());
			text9 = text9 + " @rd" + ((this.c.ReleaseDelay[trackIndex] < 1) ? "-" : (this.c.ReleaseDelay[trackIndex].ToString() + ((this.c.ReleaseDelayFrame[trackIndex] < 0) ? "" : ("," + this.c.ReleaseDelayFrame[trackIndex].ToString()))));
			text9 = text9 + " @d" + ((this.c.SelfDelayCount[trackIndex] < 255) ? (this.c.SelfDelayCount[trackIndex].ToString() + "," + this.c.SelfDelayVolume[trackIndex].ToString()) : "-");
			this.c.ParameterText.Add(text9);
			this.c.ParameterText.Add(string.Concat(new string[]
			{
				text,
				"\t@r",
				this.c.ReleaseVolume[trackIndex].ToString(),
				",",
				this.c.ReleaseRate[trackIndex].ToString(),
				"; (@rv",
				this.c.ReleaseVolume[trackIndex].ToString(),
				" @rr",
				this.c.ReleaseRate[trackIndex].ToString(),
				")"
			}));
			this.c.ParameterText.Add("\n;\t【シーケンス】");
			this.c.ParameterText.Add(string.Concat(new string[]
			{
				text,
				"\tvs",
				(this.c.VolumeSequenceNumber[trackIndex] < 255) ? this.c.VolumeSequenceNumber[trackIndex].ToString() : "-",
				" qs",
				(this.c.QuantizeSequenceNumber[trackIndex] < 255) ? this.c.QuantizeSequenceNumber[trackIndex].ToString() : "-",
				" @qs",
				(this.c.AtQuantizeSequenceNumber[trackIndex] < 255) ? this.c.AtQuantizeSequenceNumber[trackIndex].ToString() : "-",
				" \\s",
				(this.c.DetuneSequenceNumber[trackIndex] < 255) ? this.c.DetuneSequenceNumber[trackIndex].ToString() : "-",
				" m\\s",
				(this.c.HardwareEnvelopeDetuneSequenceNumber[trackIndex] < 255) ? this.c.HardwareEnvelopeDetuneSequenceNumber[trackIndex].ToString() : "-",
				" mks",
				(this.c.HardwareEnvelopeKeyShiftSequenceNumber[trackIndex] < 255) ? this.c.HardwareEnvelopeKeyShiftSequenceNumber[trackIndex].ToString() : "-",
				" ss",
				(this.c.HardwareEnvelopeDecaySequenceNumber[trackIndex] < 255) ? this.c.HardwareEnvelopeDecaySequenceNumber[trackIndex].ToString() : "-"
			}));
			this.c.ParameterText.Add("\n;\t【アルペジオ】");
			string text10 = text + "\tla";
			foreach (decimal num5 in this.c.ArpeggioDefaultLength[trackIndex])
			{
				if (num5 == -1m)
				{
					break;
				}
				text10 = text10 + ((num5 < 1000m) ? num5.ToString() : ("%" + (num5 - 1000m).ToString())) + ",";
			}
			this.c.ParameterText.Add(text10.Substring(0, text10.Length - 1));
			this.c.ParameterText.Add(string.Concat(new string[]
			{
				text,
				"\t@as",
				this.c.ArpeggioLegato[trackIndex] ? "1" : "0",
				" @ar",
				this.c.ArpeggioRest[trackIndex].ToString()
			}));
			this.c.ParameterText.Add(string.Concat(new string[]
			{
				text,
				"\tva",
				(this.c.ArpeggioVolumeSequenceNumber[trackIndex] < 255) ? this.c.ArpeggioVolumeSequenceNumber[trackIndex].ToString() : "-",
				" qa",
				(this.c.ArpeggioQuantizeSequenceNumber[trackIndex] < 255) ? this.c.ArpeggioQuantizeSequenceNumber[trackIndex].ToString() : "-",
				" @qa",
				(this.c.ArpeggioAtQuantizeSequenceNumber[trackIndex] < 255) ? (this.c.ArpeggioAtQuantizeSequenceNumber[trackIndex].ToString() + ((this.c.ArpeggioEndAtQuantize[this.c.TrackIndex] < 255) ? ("," + this.c.ArpeggioEndAtQuantize[this.c.TrackIndex].ToString()) : "")) : "-",
				" \\a",
				(this.c.ArpeggioDetuneSequenceNumber[trackIndex] < 255) ? this.c.ArpeggioDetuneSequenceNumber[trackIndex].ToString() : "-",
				" m\\a",
				(this.c.ArpeggioHardwareEnvelopeDetuneSequenceNumber[trackIndex] < 255) ? this.c.ArpeggioHardwareEnvelopeDetuneSequenceNumber[trackIndex].ToString() : "-",
				" mka",
				(this.c.ArpeggioHardwareEnvelopeKeyShiftSequenceNumber[trackIndex] < 255) ? this.c.ArpeggioHardwareEnvelopeKeyShiftSequenceNumber[trackIndex].ToString() : "-",
				" sa",
				(this.c.ArpeggioHardwareEnvelopeDecaySequenceNumber[trackIndex] < 255) ? this.c.ArpeggioHardwareEnvelopeDecaySequenceNumber[trackIndex].ToString() : "-"
			}));
			return true;
		}

		// Token: 0x06000086 RID: 134 RVA: 0x0000F230 File Offset: 0x0000D430
		private bool AtI1Command()
		{
			if (this.c.PreCompile && !this.c.GetTimeCompile)
			{
				this.c.InfoCompile = true;
				return true;
			}
			if (!this.c.InfoCompile || this.c.GetTimeCompile)
			{
				return true;
			}
			int frame = this.c.GetFrame();
			this.c.ParameterText.Add("\n;====== @I1 全トラック情報 (" + frame.ToString() + "フレーム目) ======\n");
			int num = 0;
			while (num < this.c.NotePlayParameters.Count && num < 4)
			{
				NotePlayParameter notePlayParameter = null;
				List<NotePlayParameter> list = this.c.NotePlayParameters[num];
				for (int i = list.Count - 1; i >= 0; i--)
				{
					if (list[i].CurrentFrame <= frame)
					{
						notePlayParameter = list[i];
						break;
					}
				}
				if (notePlayParameter == null)
				{
					this.c.ParameterText.Add("\n;------ " + this.c.TrackName[num] + " ------");
					this.c.ParameterText.Add(";\tこの時点以前のデータはありません。");
				}
				else
				{
					this.OutputAtI1Parameter(num, notePlayParameter);
					if (num < this.AtINoteMonitorParameters.Length)
					{
						this.AtINoteMonitorParameters[num] = notePlayParameter;
						this.AtINoteMonitorColumns[num] = ((num < this.c.NotePlayColumn.Count && this.c.NotePlayColumn[num].Count > 0) ? this.GetAtI1ParameterColumn(num, notePlayParameter) : (-1));
					}
				}
				num++;
			}
			return true;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x0000F3C0 File Offset: 0x0000D5C0
		private int GetAtI1ParameterColumn(int ti, NotePlayParameter Parameter)
		{
			if (ti < 0 || ti >= this.c.NotePlayParameters.Count || ti >= this.c.NotePlayColumn.Count)
			{
				return -1;
			}
			List<NotePlayParameter> list = this.c.NotePlayParameters[ti];
			List<int> list2 = this.c.NotePlayColumn[ti];
			for (int i = Math.Min(list.Count, list2.Count) - 1; i >= 0; i--)
			{
				if (list[i] == Parameter)
				{
					return list2[i];
				}
			}
			return -1;
		}

		// Token: 0x06000088 RID: 136 RVA: 0x0000F44E File Offset: 0x0000D64E
		private static string SequenceNumberText(int Number)
		{
			if (Number >= 255)
			{
				return "-";
			}
			return Number.ToString();
		}

		// Token: 0x06000089 RID: 137 RVA: 0x0000F468 File Offset: 0x0000D668
		private string GenerateLengthMML(NotePlayParameter Parameter, string Prefix)
		{
			string text = Prefix;
			if (Parameter.DefaultLength == null || Parameter.DefaultLength.Length == 0)
			{
				return text;
			}
			for (int i = 0; i < Parameter.DefaultLength.Length; i++)
			{
				decimal num = Parameter.DefaultLength[i];
				if (num == -1m)
				{
					break;
				}
				text += ((num < 1000m) ? num.ToString() : ("%" + (num - 1000m).ToString()));
				if (Parameter.DefaultLengthAddString != null && i < Parameter.DefaultLengthAddString.Length)
				{
					text += Parameter.DefaultLengthAddString[i];
				}
				text += ",";
			}
			if (!text.EndsWith(","))
			{
				return text;
			}
			return text.Substring(0, text.Length - 1);
		}

		// Token: 0x0600008A RID: 138 RVA: 0x0000F550 File Offset: 0x0000D750
		private string GenerateArpeggioLengthMML(NotePlayParameter Parameter, string Prefix)
		{
			string text = Prefix;
			if (Parameter.ArpeggioDefaultLength == null || Parameter.ArpeggioDefaultLength.Length == 0)
			{
				return text;
			}
			for (int i = 0; i < Parameter.ArpeggioDefaultLength.Length; i++)
			{
				decimal num = Parameter.ArpeggioDefaultLength[i];
				if (num == -1m)
				{
					break;
				}
				text = text + ((num < 1000m) ? num.ToString() : ("%" + (num - 1000m).ToString())) + ",";
			}
			if (!text.EndsWith(","))
			{
				return text;
			}
			return text.Substring(0, text.Length - 1);
		}

		// Token: 0x0600008B RID: 139 RVA: 0x0000F608 File Offset: 0x0000D808
		private void OutputAtI1Parameter(int ti, NotePlayParameter Parameter)
		{
			string text = this.c.TrackCode[ti];
			this.c.ParameterText.Add(string.Concat(new string[]
			{
				"\n;------ ",
				this.c.TrackName[ti],
				" (定点 ",
				Parameter.CurrentFrame.ToString(),
				"フレーム目) ------"
			}));
			string text2 = text + "\tt" + Parameter.Tempo.ToString();
			if (ti > 0 && Parameter.KeySign != null)
			{
				string text3 = "";
				string text4 = "";
				for (int i = 0; i < Parameter.KeySign.Length; i++)
				{
					int num = Parameter.KeySign[i];
					if (i < " ccddeffggaabb".Length)
					{
						if (num < 0)
						{
							text3 += " ccddeffggaabb".Substring(i, 1);
						}
						if (num > 0)
						{
							text4 += " ccddeffggaabb".Substring(i, 1);
						}
					}
				}
				if (text3 != "")
				{
					text2 = text2 + " ks-" + text3;
				}
				if (text4 != "")
				{
					text2 = text2 + " ks+" + text4;
				}
			}
			this.c.ParameterText.Add(text2);
			if (ti > 0)
			{
				this.c.ParameterText.Add(";\t【基本】");
				string text5 = text + "\to" + Parameter.Octave.ToString() + " q";
				text5 += ((Parameter.QuantizeBackup > 0 && Parameter.QuantizeCount > 0) ? (Parameter.QuantizeBackup.ToString() + "," + Parameter.QuantizeNumber.ToString()) : Parameter.Quantize.ToString());
				text5 += " @q";
				int num2 = ((Parameter.AtQuantizeCount > 0) ? Parameter.AtQuantizeBackup : Parameter.AtQuantize);
				if (num2 < 129)
				{
					text5 += num2.ToString();
				}
				else if (num2 < 256)
				{
					text5 = text5 + "+" + (num2 - 129).ToString();
				}
				else
				{
					text5 = text5 + "%" + (num2 - 257).ToString();
				}
				if (Parameter.AtQuantizeCount > 0)
				{
					text5 = text5 + "," + Parameter.AtQuantizeNumber.ToString();
				}
				text5 = text5 + " \\" + (-Parameter.Detune).ToString();
				string text6 = ((Parameter.VoiceNumber < 0) ? "" : Parameter.VoiceNumber.ToString()) + ((Parameter.DelayVoiceNumber < 0) ? "" : ("," + Parameter.DelayVoiceNumber.ToString()));
				if (text6 != "")
				{
					text5 = text5 + " @" + text6;
				}
				if (Parameter.HardwareEnvelopeNumber >= 0)
				{
					text5 = string.Concat(new string[]
					{
						text5,
						" s",
						Parameter.HardwareEnvelopeNumber.ToString(),
						",",
						Parameter.HardwareEnvelopeVolumeDecayFrame.ToString()
					});
				}
				else
				{
					text5 = text5 + " v" + Parameter.Volume.ToString();
				}
				text5 = string.Concat(new string[]
				{
					text5,
					" vo",
					(Parameter.VolumeOffset > 0) ? "+" : "",
					Parameter.VolumeOffset.ToString(),
					" vi",
					(Parameter.VolumeIncrease > 0) ? "+" : "",
					Parameter.VolumeIncrease.ToString(),
					(Parameter.VolumeIncreaseMax < 0) ? "" : ("," + Parameter.VolumeIncreaseMax.ToString())
				});
				this.c.ParameterText.Add(text5);
			}
			this.c.ParameterText.Add(string.Concat(new string[]
			{
				text,
				"\t@c",
				Parameter.InterruptTrack.ToString(),
				" @rm",
				Parameter.RepeatMode.ToString(),
				(ti > 0) ? string.Concat(new string[]
				{
					" @g",
					Parameter.GlissLegato ? "1" : "0",
					" k",
					Parameter.KeyShift.ToString(),
					" @k",
					Parameter.RelativeKeyShift.ToString(),
					" p",
					Parameter.Portamento.ToString()
				}) : ""
			}));
			this.c.ParameterText.Add(this.GenerateLengthMML(Parameter, text + "\tl") + "\n");
			if (ti == 0)
			{
				if (Parameter.RhythmVolume != null && !string.IsNullOrEmpty(Parameter.RhythmCodes))
				{
					string text7 = "";
					int num3 = Math.Min(Parameter.RhythmVolume.Length, Parameter.RhythmCodes.Length);
					for (int j = 0; j < num3; j++)
					{
						text7 = string.Concat(new string[]
						{
							text7,
							"V",
							Parameter.RhythmCodes[j].ToString(),
							Parameter.RhythmVolume[j].ToString(),
							" "
						});
						if ((j + 1) % 7 == 0)
						{
							text7 += "\n";
						}
					}
					foreach (string text8 in text7.Split('\n', StringSplitOptions.None))
					{
						if (text8.Length > 0)
						{
							this.c.ParameterText.Add("R\t" + text8);
						}
					}
				}
				return;
			}
			this.c.ParameterText.Add(";\t【エフェクト】");
			string text9 = text + "\t" + ((Parameter.PitchEnvelopeNumber < 0) ? "@p- @m-" : ("@" + ((Parameter.PitchEnvelopeType == 1) ? "p" : "m") + Parameter.PitchEnvelopeNumber.ToString()));
			text9 = text9 + " @n" + ((Parameter.NoteEnvelopeNumber < 0) ? "-" : Parameter.NoteEnvelopeNumber.ToString());
			text9 = text9 + " @rd" + ((Parameter.ReleaseDelay < 1) ? "-" : (Parameter.ReleaseDelay.ToString() + ((Parameter.ReleaseDelayFrame < 0) ? "" : ("," + Parameter.ReleaseDelayFrame.ToString()))));
			text9 = text9 + " @d" + ((Parameter.SelfDelayCount < 255) ? (Parameter.SelfDelayCount.ToString() + "," + Parameter.SelfDelayVolume.ToString()) : "-");
			this.c.ParameterText.Add(text9);
			this.c.ParameterText.Add(string.Concat(new string[]
			{
				text,
				"\t@r",
				Parameter.ReleaseVolume.ToString(),
				",",
				Parameter.ReleaseRate.ToString(),
				"; (@rv",
				Parameter.ReleaseVolume.ToString(),
				" @rr",
				Parameter.ReleaseRate.ToString(),
				")"
			}));
			this.c.ParameterText.Add("\n;\t【シーケンス】");
			this.c.ParameterText.Add(string.Concat(new string[]
			{
				text,
				"\tvs",
				CompileMML.SequenceNumberText(Parameter.VolumeSequenceNumber),
				" qs",
				CompileMML.SequenceNumberText(Parameter.QuantizeSequenceNumber),
				" @qs",
				CompileMML.SequenceNumberText(Parameter.AtQuantizeSequenceNumber),
				" \\s",
				CompileMML.SequenceNumberText(Parameter.DetuneSequenceNumber),
				" m\\s",
				CompileMML.SequenceNumberText(Parameter.HardwareEnvelopeDetuneSequenceNumber),
				" mks",
				CompileMML.SequenceNumberText(Parameter.HardwareEnvelopeKeyShiftSequenceNumber),
				" ss",
				CompileMML.SequenceNumberText(Parameter.HardwareEnvelopeDecaySequenceNumber)
			}));
			this.c.ParameterText.Add("\n;\t【アルペジオ】");
			this.c.ParameterText.Add(this.GenerateArpeggioLengthMML(Parameter, text + "\tla"));
			this.c.ParameterText.Add(string.Concat(new string[]
			{
				text,
				"\t@as",
				Parameter.ArpeggioLegato ? "1" : "0",
				" @ar",
				Parameter.ArpeggioRest.ToString()
			}));
			this.c.ParameterText.Add(string.Concat(new string[]
			{
				text,
				"\tva",
				CompileMML.SequenceNumberText(Parameter.ArpeggioVolumeSequenceNumber),
				" qa",
				CompileMML.SequenceNumberText(Parameter.ArpeggioQuantizeSequenceNumber),
				" @qa",
				(Parameter.ArpeggioAtQuantizeSequenceNumber < 255) ? (Parameter.ArpeggioAtQuantizeSequenceNumber.ToString() + ((Parameter.ArpeggioEndAtQuantize < 255) ? ("," + Parameter.ArpeggioEndAtQuantize.ToString()) : "")) : "-",
				" \\a",
				CompileMML.SequenceNumberText(Parameter.ArpeggioDetuneSequenceNumber),
				" m\\a",
				CompileMML.SequenceNumberText(Parameter.ArpeggioHardwareEnvelopeDetuneSequenceNumber),
				" mka",
				CompileMML.SequenceNumberText(Parameter.ArpeggioHardwareEnvelopeKeyShiftSequenceNumber),
				" sa",
				CompileMML.SequenceNumberText(Parameter.ArpeggioHardwareEnvelopeDecaySequenceNumber)
			}));
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00010010 File Offset: 0x0000E210
		public string GenerateLengthMML(string LengthMML = "L")
		{
			int num = 0;
			foreach (decimal num2 in this.c.DefaultLength[this.c.TrackIndex])
			{
				if (num2 == -1m)
				{
					break;
				}
				LengthMML = LengthMML + ((num2 < 1000m) ? num2.ToString() : ("%" + (num2 - 1000m).ToString())) + this.c.DefaultLengthAddString[this.c.TrackIndex][num] + ",";
				num++;
			}
			return LengthMML.Substring(0, LengthMML.Length - 1);
		}

		// Token: 0x0600008D RID: 141 RVA: 0x000100DC File Offset: 0x0000E2DC
		private void SetSequenceCommand(string Command, int[] SequenceNumber, int[] SequenceCounter, bool[] RegisteredSequence, int[] ArpeggioSequenceNumber = null)
		{
			if (this.c.CheckNextCharacter('-'))
			{
				SequenceNumber[this.c.TrackIndex] = 255;
				SequenceCounter[this.c.TrackIndex] = 0;
				if (ArpeggioSequenceNumber != null)
				{
					ArpeggioSequenceNumber[this.c.TrackIndex] = 255;
				}
				return;
			}
			int num = this.c.NumberRead(false, true);
			if (this.c.DefaultOn && num <= -32769)
			{
				num = 256;
			}
			this.c.ErrorParameter(Command, num);
			if (this.c.CheckNextCharacter('='))
			{
				this.c.Message = "トラック内でシーケンスデータは定義できません。\n改行してトラック外で定義する必要があります。";
				Core core = this.c;
				core.ErrorString += "=";
				return;
			}
			if (num < 0 || (num > 31 && num != 255 && num != 256))
			{
				this.c.Message = Command + " のシーケンス番号は 0 ～ 31、「 - 」または 255 です。\n「 - 」または 255 で解除されます。\n\nまた、番号を省略するとカウンタを先頭に戻します。";
				return;
			}
			if (num == 255)
			{
				SequenceNumber[this.c.TrackIndex] = 255;
				SequenceCounter[this.c.TrackIndex] = 0;
				if (ArpeggioSequenceNumber != null)
				{
					ArpeggioSequenceNumber[this.c.TrackIndex] = 255;
				}
				return;
			}
			if (num < 256)
			{
				if (!RegisteredSequence[num])
				{
					this.c.Message = num.ToString() + " 番の " + Command + " シーケンスは定義されていません。";
					return;
				}
				SequenceNumber[this.c.TrackIndex] = num;
			}
			SequenceCounter[this.c.TrackIndex] = 0;
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00010260 File Offset: 0x0000E460
		private void SetArpeggioSequenceCommand(string Command, int[] SequenceNumber, int[] SequenceCounter, int[] ArpeggioSequenceNumber, bool[] RegisteredSequence)
		{
			if (this.c.CheckNextCharacter('-'))
			{
				SequenceNumber[this.c.TrackIndex] = 255;
				SequenceCounter[this.c.TrackIndex] = 0;
				ArpeggioSequenceNumber[this.c.TrackIndex] = 255;
				return;
			}
			int num = this.c.NumberRead(false, true);
			if (num <= -32769)
			{
				num = 255;
			}
			this.c.ErrorParameter(Command, num);
			if (this.c.CheckNextCharacter('='))
			{
				this.c.Message = "トラック内でシーケンスデータは定義できません。\n改行してトラック外で定義する必要があります。";
				Core core = this.c;
				core.ErrorString += "=";
				return;
			}
			if (num < 0 || (num > 31 && num != 255))
			{
				this.c.Message = Command + " のシーケンス番号は 0 ～ 31、「 - 」または 255 です。\n「 - 」または 255 で解除されます。";
				return;
			}
			if (num == 255)
			{
				SequenceNumber[this.c.TrackIndex] = 255;
				SequenceCounter[this.c.TrackIndex] = 0;
				ArpeggioSequenceNumber[this.c.TrackIndex] = 255;
				return;
			}
			if (!RegisteredSequence[num])
			{
				this.c.Message = num.ToString() + " 番の " + Command + " シーケンスは定義されていません。";
				return;
			}
			ArpeggioSequenceNumber[this.c.TrackIndex] = num;
		}

		// Token: 0x0600008F RID: 143 RVA: 0x000103B0 File Offset: 0x0000E5B0
		private bool TryGetSignedSequenceValue(int[] SequenceNumber, int[] SequenceCounter, List<List<int>> SequenceData, out int Value)
		{
			Value = 0;
			int trackIndex = this.c.TrackIndex;
			if (SequenceNumber[trackIndex] >= 255)
			{
				return false;
			}
			List<int> list = SequenceData[SequenceNumber[trackIndex]];
			if (list == null || list.Count == 0)
			{
				return false;
			}
			if (SequenceCounter[trackIndex] < 0 || SequenceCounter[trackIndex] >= list.Count)
			{
				SequenceCounter[trackIndex] = 0;
			}
			int num = list[SequenceCounter[trackIndex]];
			if (num == 2147483647)
			{
				return false;
			}
			if (num <= -2147483393)
			{
				int num2 = num - int.MinValue;
				SequenceCounter[trackIndex] -= num2;
				if (SequenceCounter[trackIndex] < 0)
				{
					SequenceCounter[trackIndex] = 0;
				}
				num = list[SequenceCounter[trackIndex]];
			}
			Value = num;
			SequenceCounter[trackIndex]++;
			return true;
		}

		// Token: 0x06000090 RID: 144 RVA: 0x0001045C File Offset: 0x0000E65C
		private void ApplyDetuneSequence()
		{
			int num;
			if (!this.TryGetSignedSequenceValue(this.c.DetuneSequenceNumber, this.c.DetuneSequenceCounter, this.c.DetuneSequenceData, out num))
			{
				return;
			}
			this.SetDetuneAbsolute(num);
		}

		// Token: 0x06000091 RID: 145 RVA: 0x0001049C File Offset: 0x0000E69C
		private void ApplyHardwareEnvelopeDetuneSequence()
		{
			int num;
			if (!this.TryGetSignedSequenceValue(this.c.HardwareEnvelopeDetuneSequenceNumber, this.c.HardwareEnvelopeDetuneSequenceCounter, this.c.HardwareEnvelopeDetuneSequenceData, out num))
			{
				return;
			}
			int num2 = ((this.c.HardwareEnvelopeMode == 1) ? (-128) : (-1000));
			int num3 = ((this.c.HardwareEnvelopeMode == 1) ? 127 : 1000);
			if (num < num2 || num > num3)
			{
				this.c.RangeNotice("ハードエンベロープ周期のデチューンシーケンス", num2, num3, null);
				this.c.ErrorString = "M\\S";
				return;
			}
			this.c.HardwareEnvelopeDetune = num;
			if (this.c.HardwareEnvelopeMode == 1)
			{
				this.c.DataWrite(248);
				this.c.DataWrite(num & 255);
			}
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00010570 File Offset: 0x0000E770
		private void ApplyHardwareEnvelopeKeyShiftSequence()
		{
			int num;
			if (!this.TryGetSignedSequenceValue(this.c.HardwareEnvelopeKeyShiftSequenceNumber, this.c.HardwareEnvelopeKeyShiftSequenceCounter, this.c.HardwareEnvelopeKeyShiftSequenceData, out num))
			{
				return;
			}
			if (num < -84 || num > 84)
			{
				this.c.RangeNotice("ハードエンベロープのトランスポーズシーケンス", -84, 84, null);
				this.c.ErrorString = "MKS";
				return;
			}
			this.c.HardwareEnvelopeKeyShift = num;
			if (this.c.HardwareEnvelopeMode == 1)
			{
				this.c.DataWrite(247);
				this.c.DataWrite(num & 255);
			}
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00010618 File Offset: 0x0000E818
		private void ApplyHardwareEnvelopeDecaySequence()
		{
			int num;
			if (!this.TryGetSignedSequenceValue(this.c.HardwareEnvelopeDecaySequenceNumber, this.c.HardwareEnvelopeDecaySequenceCounter, this.c.HardwareEnvelopeDecaySequenceData, out num))
			{
				return;
			}
			int num2 = this.c.HardwareEnvelopeNumber[this.c.TrackIndex];
			if (num2 != 8 && num2 != 10 && num2 != 12 && num2 != 14)
			{
				this.c.Message = "SS シーケンスは S8、S10、S12、S14 のループ型ハードエンベロープで使用してください。";
				this.c.ErrorString = "SS";
				return;
			}
			this.c.DataWrite(144 + num2);
			this.c.DataWrite(num);
			this.HardwareEnvelopeAutoSetting(true);
			this.c.HardwareEnvelopeVolumeDecayFrame[this.c.TrackIndex] = num;
			this.c.SetVoice[this.c.TrackIndex] = true;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x000106F4 File Offset: 0x0000E8F4
		private void SetDetuneAbsolute(int a)
		{
			if (a < -127 || a > 128)
			{
				short num = (short)(-(short)a);
				this.c.Detune[this.c.TrackIndex] = -a;
				this.c.DataWrite(142);
				this.c.DataWrite((int)(num & 255));
				this.c.DataWrite(num >> 8);
				return;
			}
			this.c.DataWrite(135);
			this.c.DataWrite(-a);
			this.c.Detune[this.c.TrackIndex] = -a;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00010794 File Offset: 0x0000E994
		private void SetDetune()
		{
			if (this.c.CheckNextCharacter('S'))
			{
				this.SetSequenceCommand("\\S", this.c.DetuneSequenceNumber, this.c.DetuneSequenceCounter, this.c.RegisteredDetuneSequence, this.c.ArpeggioDetuneSequenceNumber);
				return;
			}
			if (this.c.CheckNextCharacter('A'))
			{
				this.SetArpeggioSequenceCommand("\\A", this.c.DetuneSequenceNumber, this.c.DetuneSequenceCounter, this.c.ArpeggioDetuneSequenceNumber, this.c.RegisteredDetuneSequence);
				return;
			}
			int num = this.c.NumberRead(false, true);
			if (this.c.DefaultOn && num <= -32769)
			{
				num = 0;
			}
			else if (this.c.CheckNextCharacter('-'))
			{
				if (num < 0)
				{
					this.c.Message = "相対デチューンは、負数の場合もパラメータの後には 「 - 」 ではなく 「 + 」 を付けてください。";
					this.c.ErrorParameter("\\", num);
					num = -num;
				}
				else
				{
					this.c.Message = "負数の相対デチューンは、パラメータの前に 「 - 」、後に 「 + 」 を付けてください。";
					this.c.ErrorParameter("\\", num);
				}
				Core core = this.c;
				core.Message = string.Concat(new string[]
				{
					core.Message,
					"\n「 \\-",
					num.ToString(),
					" 」 で通常のデチューン、「 \\-",
					num.ToString(),
					"+ 」で前回のデチューン値から ",
					num.ToString(),
					" を減算した状態となります。"
				});
				return;
			}
			if (this.c.CheckNextCharacter('+'))
			{
				if (num < -1000 || num > 1000)
				{
					this.c.RangeNotice("相対デチューン", -1000, 1000, null);
					this.c.ErrorParameter("\\", num);
					return;
				}
				short num2 = (short)(-(short)num);
				this.c.Detune[this.c.TrackIndex] -= num;
				this.c.DataWrite(137);
				this.c.DataWrite((int)(num2 & 255));
				this.c.DataWrite(num2 >> 8);
				return;
			}
			else
			{
				if (num < -4000 || num > 4000)
				{
					this.c.RangeNotice("デチューン", -4000, 4000, null);
					this.c.ErrorParameter("\\", num);
					return;
				}
				this.SetDetuneAbsolute(num);
				return;
			}
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00010A00 File Offset: 0x0000EC00
		private void ClearPortamentoContinuous()
		{
			this.c.PortamentoContinuousMode[this.c.TrackIndex] = 0;
			this.c.PortamentoContinuousKeyShift[this.c.TrackIndex] = 0;
			this.c.PortamentoContinuousStartNote[this.c.TrackIndex] = 0;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00010A58 File Offset: 0x0000EC58
		private void SetPortamento()
		{
			if (this.c.CheckNextCharacter('_'))
			{
				if (this.c.CheckNextCharacter('K'))
				{
					int num = this.c.NumberRead(false, true);
					if (this.c.DefaultOn && num <= -32769)
					{
						num = 0;
					}
					if (num < -84 || num > 84)
					{
						this.c.RangeNotice("P_K コマンド", -84, 84, "K/@K・調号・臨時記号反映後の次音を基準に、ポルタメント開始音を半音単位でずらします。");
						this.c.ErrorParameter("P_K", num);
						return;
					}
					int num2 = 0;
					if (this.c.CheckNextCharacter(','))
					{
						num2 = this.c.NumberRead(true, true);
						if (num2 < 0 || num2 > 1)
						{
							this.c.RangeNotice("P_K コマンドの第 2 パラメータ", 0, 1, "\n0: 直後の 1 音に適用\n1: 以降すべての音符に適用");
							this.c.ErrorParameter("P_K…", num2);
							return;
						}
					}
					if (num2 == 1)
					{
						this.c.PortamentoKeyShift[this.c.TrackIndex] = 0;
						this.c.PortamentoContinuousMode[this.c.TrackIndex] = 1;
						this.c.PortamentoContinuousKeyShift[this.c.TrackIndex] = num;
						this.c.PortamentoContinuousStartNote[this.c.TrackIndex] = 0;
						return;
					}
					this.ClearPortamentoContinuous();
					this.c.PortamentoKeyShift[this.c.TrackIndex] = num;
					return;
				}
				else if (this.c.CheckNextCharacter('Q'))
				{
					int num3;
					if (this.c.CheckNextCharacter('-'))
					{
						num3 = 0;
					}
					else if (this.c.CheckNextCharacter('+'))
					{
						num3 = 1;
					}
					else
					{
						num3 = this.c.NumberRead(false, true);
					}
					if (this.c.DefaultOn && num3 <= -32769)
					{
						num3 = 0;
					}
					if (num3 < 0 || num3 > 1)
					{
						this.c.RangeNotice("P_Q コマンド", 0, 1, "1 または「 + 」を設定すると、「 _ 」によるポルタメント時に Q コマンドや @Q コマンドの影響を受けるようになります。");
						this.c.ErrorParameter("P_Q", num3);
						return;
					}
					this.c.PortamentoMode[this.c.TrackIndex] = num3;
					return;
				}
				else if (this.c.CheckNextCharacter('R'))
				{
					int num4;
					if (this.c.CheckNextCharacter('-'))
					{
						num4 = 0;
					}
					else if (this.c.CheckNextCharacter('+'))
					{
						num4 = 1;
					}
					else
					{
						num4 = this.c.NumberRead(false, true);
					}
					if (this.c.DefaultOn && num4 <= -32769)
					{
						num4 = 0;
					}
					if (num4 < 0 || num4 > 1)
					{
						this.c.RangeNotice("P_R コマンド", 0, 1, "1 または「 + 」を設定すると、「 _ 」によるポルタメントの始点と終点が反転します。");
						this.c.ErrorParameter("P_R", num4);
						return;
					}
					this.c.PortamentoReverse[this.c.TrackIndex] = num4 == 1;
					return;
				}
				else
				{
					if (!this.c.CheckNextCharacter('W'))
					{
						this.c.PortamentoNoteSetAndSaveOctave[this.c.TrackIndex] = this.c.Octave[this.c.TrackIndex];
						return;
					}
					int num5;
					if (this.c.CheckNextCharacter('-'))
					{
						num5 = 0;
					}
					else
					{
						num5 = this.c.NumberRead(false, true);
					}
					if (this.c.DefaultOn && num5 <= -32769)
					{
						num5 = 0;
					}
					if (num5 < 0 || num5 > 254)
					{
						this.c.RangeNotice("P_W コマンド", 0, 254, null);
						this.c.ErrorParameter("P_W", num5);
						return;
					}
					this.c.PortamentoWait[this.c.TrackIndex] = num5;
					return;
				}
			}
			else
			{
				int num6;
				if (this.c.CheckNextCharacter('-'))
				{
					num6 = 0;
				}
				else
				{
					num6 = this.c.NumberRead(false, true);
				}
				if (this.c.DefaultOn && num6 <= -32769)
				{
					num6 = 0;
				}
				if (num6 < 0 || num6 > 255)
				{
					this.c.RangeNotice("ポルタメント", 0, 255, null);
					this.c.ErrorParameter("P", num6);
					return;
				}
				if (this.CheckUseHardwareEnvelopeVolumeHarf("Pコマンド", "スキップ"))
				{
					return;
				}
				if (num6 == 0)
				{
					this.ClearPortamentoContinuous();
					this.c.PortamentoKeyShift[this.c.TrackIndex] = 0;
					this.c.PortamentoStartNote[this.c.TrackIndex] = 0;
					this.c.PortamentoNoteSetAndSaveOctave[this.c.TrackIndex] = 0;
				}
				this.c.PortamentoCommandSet[this.c.TrackIndex] = num6 > 0;
				this.c.Portamento[this.c.TrackIndex] = num6;
				this.c.DataWrite(136);
				this.c.DataWrite(num6);
				return;
			}
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00010F00 File Offset: 0x0000F100
		public void SetJump(int Flag)
		{
			if (this.c.CheckNextCharacter('=') && this.c.CheckNextCharacter('"'))
			{
				string text = "";
				while (this.c.Index < this.c.LineBuffer.Length)
				{
					this.c.Index++;
					char c = this.c.Read1(-1);
					if (c == '"')
					{
						break;
					}
					if (c != '\0')
					{
						ReadOnlySpan<char> readOnlySpan = text;
						char c2 = c;
						text = readOnlySpan + new ReadOnlySpan<char>(ref c2);
					}
				}
				if (text == "")
				{
					this.c.Message = "ジャンプマーカーの文字列が正しく設定されていません。";
					this.c.ErrorString = "J=\"...";
					return;
				}
				int repeatStorageMode = this.GetRepeatStorageMode();
				int num = 0;
				if (repeatStorageMode == 1)
				{
					int num2 = this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode];
					if (num2 > 0)
					{
						num2--;
						num = this.c.RepeatFrame1[this.c.TrackIndex, num2, repeatStorageMode] + this.c.RepeatFrame2[this.c.TrackIndex, num2, repeatStorageMode];
					}
				}
				double num3 = (double)(this.c.TrackFrame[this.c.TrackIndex, 0] + this.c.BeforeLoopFrame[this.c.TrackIndex, 0] + num) / 59.94080511738504;
				if (this.c.SeekMarker.ContainsKey(text))
				{
					this.c.SeekMarker[text] = num3;
					this.c.SeekMarkerLine[text] = this.c.LineNumber;
					return;
				}
				this.c.SeekMarker.Add(text, num3);
				this.c.SeekMarkerLine.Add(text, this.c.LineNumber);
				return;
			}
			else
			{
				int num4 = this.c.NumberRead(false, true);
				if (this.c.FastForwardMode)
				{
					if (num4 > -32769)
					{
						this.c.Status = "#FAST-FORWARD の影響で J" + num4.ToString() + " は無効化されています。";
						if (!this.c.UseJ[this.c.TrackIndex])
						{
							this.c.FastForwardTrack[this.c.TrackIndex] = false;
						}
						return;
					}
					this.c.DataWrite(254);
					this.c.DataWrite(Flag);
					this.c.UseJ[this.c.TrackIndex] = true;
					this.c.FastForwardTrack[this.c.TrackIndex] = true;
					return;
				}
				else
				{
					if (this.c.DefaultOn && num4 <= -32769)
					{
						num4 = 0;
					}
					if (num4 < 0 || num4 > 5)
					{
						this.c.RangeNotice("J コマンドの値", 0, 5, "J0 ～ J5 が F5 ～ F10 キーに対応します。");
						this.c.ErrorParameter("J", num4);
						return;
					}
					this.c.UseJump = true;
					this.c.SetJumpNumber[this.c.TrackIndex, num4] = true;
					this.c.JumpStartTime[num4] = (double)this.c.GetCurrentPlaybackFrame() / 59.94080511738504;
					this.c.JumpStartLine[num4] = this.c.LineNumber;
					return;
				}
			}
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00011268 File Offset: 0x0000F468
		public void SetYCommand()
		{
			this.c.DataWrite(252);
			int num = this.c.NumberRead(false, true);
			if (num < 0 || num > 15)
			{
				this.c.RangeNotice("Y コマンドのレジスタ番号", 0, 15, null);
				this.c.ErrorParameter("Y", num);
				return;
			}
			this.c.DataWrite(num);
			this.c.ErrorString = "Y" + num.ToString();
			if (!this.c.CheckNextCharacter(','))
			{
				this.c.Message = "Y コマンドのデータ値がありません。";
				return;
			}
			int num2 = this.c.NumberRead(false, true);
			if (num2 < 0 || num2 > 255)
			{
				this.c.RangeNotice("Y コマンドのデータ値", 0, 255, null);
				Core core = this.c;
				Core core2 = this.c;
				core.ErrorParameter(core2.ErrorString += ", ", num2);
				return;
			}
			this.c.DataWrite(num2);
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00011374 File Offset: 0x0000F574
		public bool SelfDelay()
		{
			if (this.c.CheckNextCharacter('R'))
			{
				int num;
				if (this.c.CheckNextCharacter('+'))
				{
					num = 1;
				}
				else if (this.c.CheckNextCharacter('-'))
				{
					num = 0;
				}
				else
				{
					num = this.c.NumberRead(false, true);
				}
				if (num < 0 || num > 1)
				{
					this.c.Message = "セルフディレイの休符を埋めるかどうかの設定指定は 0 で OFF、1 で ON となります。";
					this.c.ErrorParameter("@DR", num);
					return true;
				}
				this.c.SelfDelayRestFill[this.c.TrackIndex] = num == 1;
				return true;
			}
			else if (this.c.CheckNextCharacter('+') || this.c.CheckNextCharacter('>'))
			{
				int num2 = this.c.NumberRead(false, true);
				if (num2 <= -32769)
				{
					num2 = 1;
				}
				if (num2 < 1 || num2 > 3)
				{
					this.c.RangeNotice("セルフディレイのバッファを進める数", 1, 3, null);
					this.c.ErrorParameter("@D>", num2);
					return true;
				}
				for (int i = 0; i < num2; i++)
				{
					this.c.PastNoteNumber[this.c.TrackIndex, 3] = this.c.PastNoteNumber[this.c.TrackIndex, 2];
					this.c.PastNoteNumber[this.c.TrackIndex, 2] = this.c.PastNoteNumber[this.c.TrackIndex, 1];
					this.c.PastNoteNumber[this.c.TrackIndex, 1] = this.c.PastNoteNumber[this.c.TrackIndex, 0];
					this.c.PastNoteNumber[this.c.TrackIndex, 0] = 0;
				}
				return true;
			}
			else if (this.c.CheckNextCharacter('<'))
			{
				int num2 = this.c.NumberRead(false, true);
				if (num2 <= -32769)
				{
					num2 = 1;
				}
				if (num2 < 1 || num2 > 3)
				{
					this.c.RangeNotice("セルフディレイのバッファを戻す数", 1, 3, null);
					this.c.ErrorParameter("@D<", num2);
					return true;
				}
				this.c.SelfDelayContinueCount[this.c.TrackIndex] += num2;
				if (this.c.SelfDelayContinueCount[this.c.TrackIndex] > 3)
				{
					this.c.SelfDelayContinueCount[this.c.TrackIndex] = 3;
				}
				return true;
			}
			else
			{
				int num2;
				if (this.c.CheckNextCharacter('-'))
				{
					num2 = 255;
				}
				else
				{
					num2 = this.c.NumberRead(false, true);
				}
				if (this.c.DefaultOn && num2 <= -32769)
				{
					num2 = 255;
				}
				if (num2 < 255 && (num2 < 0 || num2 > 2))
				{
					this.c.Message = "セルフディレイで戻す音符の数は 0 ～ 2、「 - 」 または 255 です。\n「 - 」 または 255 でセルフディレイが解除されます。";
					this.c.ErrorParameter("@D", num2);
					return true;
				}
				this.c.SelfDelayCount[this.c.TrackIndex] = num2;
				if (num2 == 255)
				{
					return true;
				}
				this.c.ErrorString = "@D" + num2.ToString();
				if (!this.c.CheckNextCharacter(','))
				{
					if (this.c.DefaultOn)
					{
						this.c.SelfDelayVolume[this.c.TrackIndex] = 12;
					}
					else
					{
						this.c.Message = "セルフディレイの音量が設定されていません。\n第 2 パラメータで設定してください。";
					}
					return true;
				}
				int num3 = this.c.NumberRead(false, true);
				if (this.c.DefaultOn && num3 <= -32769)
				{
					num3 = 12;
				}
				if (num3 < 0 || num3 > 15)
				{
					this.c.RangeNotice("セルフディレイの音量は", 0, 15, "15 で現在と同じ音量、1 で最小音量、0 でセルフディレイ解除となります。");
					Core core = this.c;
					Core core2 = this.c;
					core.ErrorParameter(core2.ErrorString += ", ", num3);
					return true;
				}
				if (num3 == 0)
				{
					this.c.SelfDelayCount[this.c.TrackIndex] = 255;
					return true;
				}
				this.c.SelfDelayVolume[this.c.TrackIndex] = num3;
				return true;
			}
		}

		// Token: 0x0600009B RID: 155 RVA: 0x000117A4 File Offset: 0x0000F9A4
		public bool AtRCommand(bool RhythmTrack = false)
		{
			if (this.c.CheckNextCharacter('M'))
			{
				if (this.c.SoundId > 0)
				{
					this.c.Message = "効果音のデータ内では @RM コマンドは使用できません。";
					this.c.ErrorString = "@RM";
					return true;
				}
				int num = this.c.NumberRead(false, true);
				if (this.c.DefaultOn && num <= -32769)
				{
					num = 4;
				}
				if (num < 0 || num > 4)
				{
					this.c.RangeNotice("リピートモードの値", 0, 4, "\n0: コンパイル時に展開し、ドライバのリピート機能は使用しない\n1: ドライバの機能を使用（リピート終了直後の音符にフレーム加算）\n2: ドライバの機能を使用（1 フレーム未満の端数は切り捨て）\n3: ドライバの機能を使用（フレーム補正せずにエラー検出）\n4: 展開後の同一バイナリを自動検出し、ドライバのリピート機能で圧縮");
					this.c.ErrorParameter("@RM", num);
					return true;
				}
				if (!this.c.PreCompile || this.c.GetTimeCompile)
				{
					this.c.RepeatMode[this.c.TrackIndex] = num;
				}
				return true;
			}
			else if (this.c.CheckNextCharacter('S'))
			{
				int num2 = this.c.NumberRead(false, true);
				if (this.c.DefaultOn && num2 <= -32769)
				{
					num2 = 0;
				}
				if (num2 < 0 || num2 > 9999)
				{
					this.c.RangeNotice("乱数のシード値", 0, 9999, "0 でコンパイルのたびに異なる値となります。");
					this.c.ErrorParameter("@RS", num2);
					return true;
				}
				this.c.SetRandomSeed(num2);
				return true;
			}
			else
			{
				if (RhythmTrack)
				{
					this.c.ErrorString = "@R";
					return false;
				}
				if (this.c.CheckNextCharacter('D'))
				{
					this.SetReleaseDelay();
					return true;
				}
				if (this.c.CheckNextCharacter('V'))
				{
					int num3;
					if (this.c.CheckNextCharacter('+'))
					{
						num3 = this.c.NumberRead(false, true);
						this.c.ErrorParameter("@RV+", num3);
						if (num3 < 0 || num3 > 15)
						{
							this.c.RangeNotice("@RV+", 0, 15, null);
							return true;
						}
						num3 = this.c.ReleaseVolume[this.c.TrackIndex] + num3;
					}
					else if (this.c.CheckNextCharacter('-'))
					{
						num3 = this.c.NumberRead(false, true);
						this.c.ErrorParameter("@RV-", num3);
						if (num3 < 0 || num3 > 15)
						{
							this.c.RangeNotice("@RV-", 0, 15, null);
							return true;
						}
						num3 = this.c.ReleaseVolume[this.c.TrackIndex] - num3;
					}
					else
					{
						num3 = this.c.NumberRead(false, true);
						this.c.ErrorParameter("@RV", num3);
						if (this.c.DefaultOn && num3 <= -32769)
						{
							num3 = 0;
						}
						if (num3 < 0 || num3 > 15)
						{
							this.c.RangeNotice("リリース音量", 0, 15, null);
							return true;
						}
						num3 += this.c.ReleaseVolumeOffset[this.c.TrackIndex];
					}
					if (num3 < 0)
					{
						num3 = 0;
					}
					if (num3 > 15)
					{
						num3 = 15;
					}
					this.c.ReleaseVolume[this.c.TrackIndex] = num3;
					this.c.DataWrite(141);
					this.c.DataWrite(this.c.Mute[this.c.TrackIndex] ? 15 : (15 - num3));
					return true;
				}
				if (this.c.CheckNextCharacter('R'))
				{
					this.SetReleaseRate();
					return true;
				}
				if (this.c.CheckNextCharacter('-'))
				{
					this.c.DataWrite(141);
					this.c.DataWrite(16);
					this.c.ReleaseVolume[this.c.TrackIndex] = 0;
					this.c.DataWrite(163);
					this.c.DataWrite(0);
					this.c.ReleaseRate[this.c.TrackIndex] = 0;
					return true;
				}
				if (this.c.CheckNextCharacter('N'))
				{
					this.SetRestNote();
					return true;
				}
				if (this.c.CheckNextCharacter('A'))
				{
					this.SetReleaseAfter();
					return true;
				}
				if (this.c.CheckNextCharacter('L'))
				{
					this.SetReleaseLevel();
					return true;
				}
				int num4 = this.c.NumberRead(false, true);
				if (num4 < 0 || num4 > 15)
				{
					this.c.RangeNotice("@R コマンドのリリース音量", 0, 15, null);
					this.c.ErrorParameter("@R", num4 + 1);
					return true;
				}
				num4 += this.c.ReleaseVolumeOffset[this.c.TrackIndex];
				if (num4 < 0)
				{
					num4 = 0;
				}
				if (num4 > 15)
				{
					num4 = 15;
				}
				this.c.DataWrite(141);
				this.c.DataWrite(this.c.Mute[this.c.TrackIndex] ? 15 : (15 - num4));
				this.c.ReleaseVolume[this.c.TrackIndex] = num4;
				this.c.ErrorString = "@R" + num4.ToString();
				if (!this.c.CheckNextCharacter(','))
				{
					this.c.Message = "@R コマンドのサスティン（リリースレート）が設定されていません。";
					return true;
				}
				int num5 = this.c.NumberRead(false, true);
				if (num5 < 0 || num5 > 255)
				{
					this.c.RangeNotice("@R コマンドのサスティン (リリースレート) ", 0, 255, null);
					Core core = this.c;
					Core core2 = this.c;
					core.ErrorParameter(core2.ErrorString += ", ", num5);
					return true;
				}
				this.c.DataWrite(163);
				this.c.DataWrite(num5);
				this.c.ReleaseRate[this.c.TrackIndex] = num5;
				return true;
			}
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00011D60 File Offset: 0x0000FF60
		private void SetReleaseAfter()
		{
			this.c.RangeNotice("@RA コマンド", 0, 15, "-1～15、+1～15 で @R コマンド や @RV コマンドからの相対値となります。」");
			int num;
			if (this.c.CheckNextCharacter('+'))
			{
				num = this.c.NumberRead(true, true);
				if (num < 0 || num > 15)
				{
					this.c.ErrorParameter("@RA+", num);
					return;
				}
				num += this.c.ReleaseVolume[this.c.TrackIndex] + this.c.ReleaseVolumeOffset[this.c.TrackIndex];
				if (num < 0)
				{
					num = 0;
				}
				if (num > 15)
				{
					num = 15;
				}
			}
			else if (this.c.CheckNextCharacter('-'))
			{
				num = this.c.NumberRead(true, true);
				if (num < 0 || num > 15)
				{
					this.c.ErrorParameter("@RA-", num);
					return;
				}
				num = this.c.ReleaseVolume[this.c.TrackIndex] + this.c.ReleaseVolumeOffset[this.c.TrackIndex] - num;
				if (num < 0)
				{
					num = 0;
				}
				if (num > 15)
				{
					num = 15;
				}
			}
			else
			{
				num = this.c.NumberRead(true, true);
				if (num < 0 || num > 15)
				{
					this.c.ErrorParameter("@RA", num);
					return;
				}
			}
			this.c.ReleaseVolumeAfter[this.c.TrackIndex] = num;
			this.c.DataWrite(141);
			this.c.DataWrite(this.c.Mute[this.c.TrackIndex] ? 15 : (15 - num));
			this.c.Message = null;
			this.c.ErrorString = null;
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00011F14 File Offset: 0x00010114
		private void SetReleaseLevel()
		{
			this.c.RangeNotice("@RL コマンド", 0, 15, "-1～15、+1～15 で @R コマンド や @RV コマンドからの相対値となります。」");
			int num;
			if (this.c.CheckNextCharacter('+'))
			{
				num = this.c.NumberRead(true, true);
				if (num < 0 || num > 15)
				{
					this.c.ErrorParameter("@RL+", num);
					return;
				}
				num += this.c.ReleaseVolume[this.c.TrackIndex] + this.c.ReleaseVolumeOffset[this.c.TrackIndex];
				if (num < 0)
				{
					num = 0;
				}
				if (num > 15)
				{
					num = 15;
				}
			}
			else if (this.c.CheckNextCharacter('-'))
			{
				num = this.c.NumberRead(true, true);
				if (num < 0 || num > 15)
				{
					this.c.ErrorParameter("@RL-", num);
					return;
				}
				num = this.c.ReleaseVolume[this.c.TrackIndex] + this.c.ReleaseVolumeOffset[this.c.TrackIndex] - num;
				if (num < 0)
				{
					num = 0;
				}
				if (num > 15)
				{
					num = 15;
				}
			}
			else
			{
				num = this.c.NumberRead(true, true);
				if (num < 0 || num > 15)
				{
					this.c.ErrorParameter("@RL", num);
					return;
				}
			}
			this.c.DataWrite(141);
			this.c.DataWrite(this.c.Mute[this.c.TrackIndex] ? 15 : (num + 128));
			this.c.Message = null;
			this.c.ErrorString = null;
		}

		// Token: 0x0600009E RID: 158 RVA: 0x000120B4 File Offset: 0x000102B4
		private bool SetReleaseDelay()
		{
			int num;
			if (this.c.CheckNextCharacter('+'))
			{
				num = 1;
			}
			else if (this.c.CheckNextCharacter('-'))
			{
				int index = this.c.Index;
				int num2 = this.c.NumberRead(false, true);
				if (num2 <= -32769)
				{
					num = 0;
					this.c.Index = index;
				}
				else
				{
					if (num2 != 1)
					{
						this.c.Message = "リリースディレイのピッチをリセットする際は @RD-1 にしてください。";
						this.c.ErrorParameter("@RD-", num2);
						return true;
					}
					if (this.c.Slur[this.c.TrackIndex] || this.c.Legato[this.c.TrackIndex])
					{
						this.c.Message = "スラ―やレガートの途中で @RD-1 は実行できません。";
						this.c.ErrorParameter("@RD-", num2);
						return true;
					}
					this.c.DataWrite(169);
					if (this.c.CheckNextCharacter(','))
					{
						this.c.Message = "@RD-1 の場合、第 2 パラメータは設定できません。";
					}
					return true;
				}
			}
			else
			{
				num = this.c.NumberRead(false, true);
			}
			if (num < 0 || num > 1)
			{
				this.c.Message = "リリースディレイ指定は 0 で OFF、1 で ON となります。";
				this.c.ErrorParameter("@RD", num);
				return true;
			}
			if (!this.c.CheckNextCharacter(','))
			{
				this.c.DataWrite(138 + num);
				this.c.ReleaseDelay[this.c.TrackIndex] = num;
				this.c.ReleaseDelayFrame[this.c.TrackIndex] = -1;
				return true;
			}
			int num3;
			if (this.c.CheckNextCharacter('L'))
			{
				List<byte> list = this.c.SetNoteLength(false, true);
				if (list == null)
				{
					return true;
				}
				num3 = this.SumLengthData(list);
				this.c.AddInfoText("@RDコマンド第2パラメータ置換 : @RD" + num.ToString() + "," + num3.ToString());
			}
			else
			{
				num3 = this.c.NumberRead(false, true);
				this.c.ErrorParameter("@RD", num, num3);
				if (num == 0)
				{
					this.c.Message = "第 2 パラメータはリリースディレイを有効にする際に指定してください。";
					return true;
				}
			}
			if (num3 < 1 || num3 > 254)
			{
				this.c.RangeNotice("リリースディレイのフレーム数", 1, 254, "省略時は、ピッチを進ませない設定（常に 1 音前のピッチ）になります。\n");
				return true;
			}
			this.c.DataWrite(140);
			this.c.DataWrite(num3);
			this.c.ReleaseDelay[this.c.TrackIndex] = 1;
			this.c.ReleaseDelayFrame[this.c.TrackIndex] = num3;
			return true;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00012360 File Offset: 0x00010560
		public void SetRestNote()
		{
			if (this.c.CheckNextCharacter('-'))
			{
				this.c.RestNoteSpan[this.c.TrackIndex] = 0;
				return;
			}
			int num = this.c.NumberRead(false, true);
			if (num < 0 || num > 7)
			{
				this.c.RangeNotice("@RN コマンドの間隔", 0, 7, "\n何音おきに発音するかを設定します。\n「 @RN0 」 または 「 @RN- 」 で効果オフです。");
				this.c.ErrorParameter("@RN", num);
				return;
			}
			this.c.RestNoteSpan[this.c.TrackIndex] = num;
			this.c.ErrorString = "@RN" + num.ToString();
			if (!this.c.CheckNextCharacter(','))
			{
				return;
			}
			int num2 = this.c.NumberRead(false, true);
			if (num2 < 0 || num2 > 7)
			{
				this.c.RangeNotice("@RN コマンドの開始音数", 0, 7, "\nコマンド実行後、何音目から発音するかを設定します。\n省略、または 0 を指定すると設定されません（間隔のみ変更するときに使用）。");
				Core core = this.c;
				Core core2 = this.c;
				core.ErrorParameter(core2.ErrorString += ", ", num2);
				return;
			}
			if (num2 > 0)
			{
				this.c.RestNoteCount[this.c.TrackIndex] = num2 - 1;
			}
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00012490 File Offset: 0x00010690
		private void SetMixMode()
		{
			this.c.DataWrite(129);
			int num = this.c.NumberRead(false, true);
			if (this.c.DefaultOn && num <= -32769)
			{
				num = 1;
			}
			if (num < 0 || num > 3)
			{
				this.c.RangeNotice("ミックスモード", 0, 3, "\n0 : Off\n1 : Tone\n2 : Noise\n3 : Tone + Noise");
				this.c.ErrorParameter("*", num);
				return;
			}
			this.c.MixMode[this.c.TrackIndex] = num;
			this.c.DataWrite(num);
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00012528 File Offset: 0x00010728
		private void SetNoiseFrequency()
		{
			this.c.DataWrite(131);
			int num = this.c.NumberRead(true, true);
			if (this.c.DefaultOn && num <= -32769)
			{
				num = 0;
			}
			if (num < 0 || num > 31)
			{
				this.c.RangeNotice("ノイズ周波数", 0, 31, null);
				this.c.ErrorParameter("N", num);
				return;
			}
			this.c.DataWrite(num + 32);
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x000125A8 File Offset: 0x000107A8
		public bool SetMute()
		{
			if (this.c.CheckNextCharacter('N'))
			{
				if (this.c.TrackEnable[this.c.TrackIndex])
				{
					this.c.Mute[this.c.TrackIndex] = false;
				}
			}
			else if (this.c.CheckNextCharacter('F'))
			{
				if (this.c.CheckNextCharacter('F'))
				{
					this.c.Mute[this.c.TrackIndex] = true;
				}
				else
				{
					this.c.Message = "トラックミュートには @OFF コマンドを使用してください。";
				}
			}
			return true;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00012640 File Offset: 0x00010840
		public bool SetTimebase()
		{
			if (this.c.SetTempo[this.c.TrackIndex])
			{
				this.c.Message = "このトラックは既にテンポコマンドによってテンポが設定されています。\nどちらか片方で設定してください。\n\nタイムベースは 4 分音符のフレーム数で、設定すると自動的にテンポも決定されます。";
				this.c.ErrorString = "@T";
				return true;
			}
			decimal num = this.c.DecimalNumberRead(false, true);
			if (this.c.CheckNextCharacter(','))
			{
				decimal num2 = this.c.DecimalNumberRead(false, true);
				if (num < 1m || num > 64m)
				{
					this.c.RangeNotice("@T<n>,<m> の n は音長です。範囲", 1, 64, "\nパラメータを 2 つ指定することにより、任意の音長に対するフレーム数を定義できます。この値によって自動的にテンポが決まり、たとえば @T4,32 なら T112.5 を設定したことになります。");
					this.c.ErrorParameter("@T" + num.ToString() + ", ", num2);
					return true;
				}
				if (num2 < 1m || num2 > 100m)
				{
					this.c.RangeNotice("@T<n>,<m> の m はタイムベース値です。範囲", 1, 100, "\nパラメータを 2 つ指定することにより、任意の音長に対するフレーム数を定義できます。この値によって自動的にテンポが決まり、たとえば @T4,32 なら T112.5 を設定したことになります。");
					this.c.ErrorParameter("@T" + num.ToString() + ", ", num2);
					return true;
				}
				this.c.Tempo[this.c.TrackIndex] = 14400m / num / num2;
			}
			else
			{
				if (num < 4m || num > 100m)
				{
					this.c.RangeNotice("@T<n> はタイムベース指定です。範囲", 4, 100, "\nこの指定では、4 分音符のフレーム数を定義できます。この値によって自動的にテンポが決まり、たとえば @T32 なら T112.5 を設定したことになります。");
					this.c.ErrorParameter("@T", num);
					return true;
				}
				this.c.Tempo[this.c.TrackIndex] = 3600m / num;
			}
			this.c.SetTimebase[this.c.TrackIndex] = true;
			return true;
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00012825 File Offset: 0x00010A25
		public void PitchEnvelopeOff()
		{
			this.c.DataWrite(130);
			this.c.DataWrite(0);
			this.c.PitchEnvelopeNumber[this.c.TrackIndex] = -1;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x0001285C File Offset: 0x00010A5C
		public bool SetPitchEnvelope(byte Type)
		{
			string text = ((Type == 1) ? "ピッチエンベロープ" : "ビブラート");
			int num;
			if (this.c.CheckNextCharacter('-'))
			{
				num = 255;
			}
			else
			{
				num = this.c.NumberRead(false, true);
				if (this.c.DefaultOn && num <= -32769)
				{
					num = 255;
				}
			}
			if (num == 255)
			{
				if ((int)Type != this.c.UsePitchEnvelopeType[this.c.TrackIndex] && this.c.UsePitchEnvelopeType[this.c.TrackIndex] >= 0)
				{
					this.c.Status = ((this.c.UsePitchEnvelopeType[this.c.TrackIndex] == 1) ? "@P" : "@M") + " の使用中に " + ((Type == 1) ? "@P" : "@M") + "- を検出、無効化しました。";
					return true;
				}
				this.PitchEnvelopeOff();
				return true;
			}
			else
			{
				this.c.ErrorParameter((Type == 1) ? "@P" : "@M", num);
				if (this.c.CheckNextCharacter('='))
				{
					this.c.Message = "トラック内で" + ((Type != 1) ? "ビブラート" : "ピッチエンベロープ") + "は定義できません。\n改行してトラック外で定義する必要があります。";
					Core core = this.c;
					core.ErrorString += "=";
					return true;
				}
				this.c.DataWrite(130);
				if (num < 0 || (num > 15 && num < 255))
				{
					this.c.Message = text + "番号は 0 ～ 15、「 - 」 または 255 です。\n「 - 」 または 255 で" + text + "が解除されます。";
					return true;
				}
				if (this.c.PitchEnvelopeType[num] == 0)
				{
					this.c.Message = num.ToString() + " 番の" + text + "は定義されていません。";
					return true;
				}
				if (this.c.PitchEnvelopeType[num] != Type)
				{
					this.c.Message = num.ToString() + " 番は " + text + "ではなく、";
					if (this.c.PitchEnvelopeRegisterLine[num] > 0)
					{
						Core core2 = this.c;
						core2.Message = string.Concat(new string[]
						{
							core2.Message,
							this.c.PitchEnvelopeRegisterLine[num].ToString(),
							" 行目の ",
							(Type == 1) ? "@M" : "@P",
							" コマンドで"
						});
					}
					else
					{
						Core core3 = this.c;
						core3.Message += "プリセットで";
					}
					Core core4 = this.c;
					core4.Message = core4.Message + ((Type != 1) ? "ピッチエンベロープ" : "ビブラート") + "として定義されています。";
					return true;
				}
				int num2;
				if (Type != 1)
				{
					if (Type != 2)
					{
						num2 = 0;
					}
					else
					{
						num2 = this.c.DefaultVibrato;
					}
				}
				else
				{
					num2 = this.c.DefaultPitchEnvelope;
				}
				if (this.c.CheckNextCharacter(','))
				{
					num2 = this.c.NumberRead(false, true);
					if (num2 < 0 || num2 > 1)
					{
						this.c.Message = text + "の第 2 パラメータには 0 か 1 を指定してください。\n\nキーオフ時にピッチを戻すかどうかの指定となります。\n(0: 戻さない 1: 戻す)";
						Core core5 = this.c;
						Core core6 = this.c;
						core5.ErrorParameter(core6.ErrorString += ", ", num2);
						return true;
					}
				}
				this.c.UsePitchEnvelope[num] = true;
				this.c.UsePitchEnvelopeType[this.c.TrackIndex] = (int)Type;
				this.c.DataWrite(num + 1 + ((num2 == 1) ? 128 : 0) + this.c.PitchEnvelopeShapeType[num] * 32);
				this.c.PitchEnvelopeNumber[this.c.TrackIndex] = num;
				return true;
			}
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00012C20 File Offset: 0x00010E20
		private bool AtMCommand()
		{
			if (!this.c.CheckNextCharacter('D'))
			{
				return this.SetPitchEnvelope(2);
			}
			int num = this.c.NumberRead(false, true);
			if (num < 2 || num > 255)
			{
				this.c.RangeNotice("@MD コマンドのディレイ値", 2, 255, null);
				this.c.ErrorParameter("@MD", num);
				return true;
			}
			this.c.DataWrite(128);
			this.c.DataWrite(num);
			return true;
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00012CA5 File Offset: 0x00010EA5
		private bool AtPCommand()
		{
			return this.SetPitchEnvelope(1);
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00012CB0 File Offset: 0x00010EB0
		private bool AtACommand()
		{
			if (this.c.CheckNextCharacter('S'))
			{
				int num = this.c.NumberRead(false, true);
				if (num > 1 || num < 0)
				{
					this.c.Message = "@AS コマンドによるレガートモードの指定は 0 で OFF、1 で ON となります。";
					this.c.ErrorParameter("@AS", num);
					return true;
				}
				this.c.ArpeggioLegato[this.c.TrackIndex] = num == 1;
				return true;
			}
			else
			{
				if (!this.c.CheckNextCharacter('R'))
				{
					this.SetNoteEnvelope("A");
					return true;
				}
				int num2 = this.c.NumberRead(false, true);
				if (num2 > 100 || num2 < -100)
				{
					this.c.RangeNotice("@AR コマンド", -100, 100, null);
					this.c.ErrorParameter("@AR", num2);
					return true;
				}
				this.c.ArpeggioRest[this.c.TrackIndex] = num2;
				return true;
			}
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00012D98 File Offset: 0x00010F98
		private bool AtNCommand()
		{
			if (!this.c.CheckNextCharacter('O'))
			{
				return this.SetNoteEnvelope("N");
			}
			int num = this.c.NumberRead(false, true);
			if (num < 1 || num > 8)
			{
				this.c.RangeNotice("@NO コマンドのオクターブ値", 1, 8, null);
				this.c.ErrorParameter("@NO", num);
				return true;
			}
			this.c.Index++;
			char c = this.c.Read1(-1);
			int num2 = " C D EF G A B".IndexOf(c);
			if (num2 < 0)
			{
				this.c.Message = "@NO コマンドの音名が正しくありません。";
				this.c.ErrorParameter("@NO", num);
				Core core = this.c;
				ReadOnlySpan<char> readOnlySpan = core.ErrorString;
				char c2 = c;
				core.ErrorString = readOnlySpan + new ReadOnlySpan<char>(ref c2);
				return true;
			}
			if (this.c.CheckNextCharacter('+') || this.c.CheckNextCharacter('#'))
			{
				num2++;
			}
			else if (this.c.CheckNextCharacter('-'))
			{
				num2--;
			}
			num2 += (num - 1) * 12 - 1;
			if (num2 < 0 || num2 > 94)
			{
				this.c.Message = "@NO コマンドのピッチ指定は o1c ～ o8a+ です。";
				this.c.ErrorParameter("@NO", num);
				Core core2 = this.c;
				ReadOnlySpan<char> readOnlySpan2 = core2.ErrorString;
				char c3 = c;
				core2.ErrorString = readOnlySpan2 + new ReadOnlySpan<char>(ref c3);
				return true;
			}
			this.c.SkipSpace();
			if (!this.c.CheckNextCharacter('='))
			{
				this.c.Message = "@NO コマンドに 「 = 」 がありません。";
				this.c.ErrorParameter("@NO", num);
				Core core3 = this.c;
				ReadOnlySpan<char> readOnlySpan3 = core3.ErrorString;
				char c4 = c;
				core3.ErrorString = readOnlySpan3 + new ReadOnlySpan<char>(ref c4);
				return true;
			}
			bool flag = true;
			while (flag)
			{
				int num3;
				if (this.c.CheckNextCharacter('+'))
				{
					num3 = this.c.PitchTable[num2] + this.c.NumberRead(true, true);
				}
				else if (this.c.CheckNextCharacter('-'))
				{
					num3 = this.c.PitchTable[num2] - this.c.NumberRead(true, true);
				}
				else
				{
					num3 = this.c.NumberRead(true, true);
				}
				if (num3 > -32769)
				{
					if (num3 < 0 || num3 > 4095)
					{
						this.c.RangeNotice("@NO コマンドで設定できる周波数レジスタの値", 0, 4095, null);
						this.c.ErrorParameter("@NO…", num3);
						return true;
					}
					this.c.PitchTable[num2] = num3;
					this.c.DataWrite(166);
					this.c.DataWrite(num2 * 2);
					this.c.DataWrite(num3 & 255);
					this.c.DataWrite(num3 / 256);
				}
				flag = this.c.CheckNextCharacter(',');
				num2++;
				if (num2 > 95)
				{
					this.c.Message = "複数指定により o8a+ を超えました。\n@NO コマンドのピッチ指定は o1c ～ o8a+ です。";
					break;
				}
			}
			return true;
		}

		// Token: 0x060000AA RID: 170 RVA: 0x000130A8 File Offset: 0x000112A8
		private bool SetNoteEnvelope(string Name = "N")
		{
			this.c.DataWrite(164);
			int num;
			if (this.c.CheckNextCharacter('-'))
			{
				num = 255;
			}
			else
			{
				num = this.c.NumberRead(false, true);
				if (this.c.DefaultOn && num <= -32769)
				{
					num = 255;
				}
			}
			if (num == 255)
			{
				this.c.DataWrite(0);
				this.c.NoteEnvelopeNumber[this.c.TrackIndex] = -1;
				return true;
			}
			this.c.ErrorParameter("@" + Name, num);
			if (this.c.CheckNextCharacter('='))
			{
				this.c.Message = "トラック内でノートエンベロープは定義できません。\n改行してトラック外で定義する必要があります。";
				Core core = this.c;
				core.ErrorString += "=";
				return true;
			}
			if (num < 0 || (num > 15 && num < 255))
			{
				this.c.Message = "ノートエンベロープ番号は 0 ～ 15、「 - 」 または 255 です。\n「 - 」 または 255 でノートエンベロープが解除されます。";
				return true;
			}
			if (this.c.NoteEnvelopeData[num].Length == 0)
			{
				this.c.Message = num.ToString() + " 番のノートエンベロープは定義されていません。";
				return true;
			}
			this.c.UseNoteEnvelope[num] = true;
			this.c.DataWrite(num + 1);
			this.c.NoteEnvelopeNumber[this.c.TrackIndex] = num;
			return true;
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00013210 File Offset: 0x00011410
		public bool SetLoopPoint()
		{
			if (this.c.RepeatNest[this.c.TrackIndex, 0] > 0 || this.c.RepeatNest[this.c.TrackIndex, 1] > 0)
			{
				this.c.Message = "リピート内に無限ループポイントは指定できません。";
				return true;
			}
			if (this.c.BeforeLoopFrame[this.c.TrackIndex, this.c.SoundId] >= 0)
			{
				this.c.Message = "同一トラック内に @L コマンドを複数指定することはできません。";
				this.c.ErrorString = "@L";
				return true;
			}
			this.c.BeforeLoopFrame[this.c.TrackIndex, this.c.SoundId] = this.c.TrackFrame[this.c.TrackIndex, this.c.SoundId];
			this.c.TrackFrame[this.c.TrackIndex, this.c.SoundId] = 0;
			this.c.LoopAddress[this.c.TrackIndex] = this.c.Address;
			this.c.LoopSlur[this.c.TrackIndex] = this.c.Slur[this.c.TrackIndex];
			this.c.FrameMod[this.c.TrackIndex] = 0m;
			this.c.BGMData[11] = this.c.BGMData[11] & 254;
			return true;
		}

		// Token: 0x060000AC RID: 172 RVA: 0x000133C0 File Offset: 0x000115C0
		private bool SetGlissMode()
		{
			int num = this.c.NumberRead(false, true);
			if (this.c.DefaultOn && num <= -32769)
			{
				num = 0;
			}
			if (num < 0 || num > 1)
			{
				this.c.RangeNotice("グリッサンドモード", 0, 1, "0 で通常、1 でレガートになります。");
				this.c.ErrorParameter("@G", num);
				return true;
			}
			this.c.GlissLegato[this.c.TrackIndex] = num == 1;
			return true;
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00013440 File Offset: 0x00011640
		private void SetKCommand()
		{
			if (this.c.CheckNextCharacter('S'))
			{
				this.c.KeySign[this.c.TrackIndex] = this.c.SetKeySignSub();
				this.c.ErrorString = "KS";
				return;
			}
			int num = this.c.NumberRead(false, true);
			if (this.c.DefaultOn && num <= -32769)
			{
				num = 0;
			}
			if (num < -84 || num > 84)
			{
				this.c.RangeNotice("トランスポーズの値", -84, 84, null);
				this.c.ErrorParameter("K", num);
				return;
			}
			this.c.KeyShift[this.c.TrackIndex] = num;
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00013500 File Offset: 0x00011700
		private bool SetRelativeKeyShift()
		{
			int num = this.c.NumberRead(false, true);
			if (this.c.DefaultOn && num <= -32769)
			{
				num = 0;
			}
			if (num < -24 || num > 24)
			{
				this.c.RangeNotice("相対トランスポーズの値", -24, 24, "0 で初期値に戻ります。");
				this.c.ErrorParameter("@K", num);
				return true;
			}
			if (num == 0)
			{
				this.c.RelativeKeyShift[this.c.TrackIndex] = 0;
			}
			else
			{
				this.c.RelativeKeyShift[this.c.TrackIndex] += num;
			}
			return true;
		}

		// Token: 0x060000AF RID: 175 RVA: 0x000135A8 File Offset: 0x000117A8
		public bool SetInterruptTrack()
		{
			if (this.c.SoundId > 0)
			{
				this.c.Message = "効果音のデータ内では @C コマンドは使用できません。";
				this.c.ErrorString = "@C";
				return true;
			}
			int num = this.c.NumberRead(false, true);
			if (this.c.DefaultOn && num <= -32769)
			{
				num = ((this.c.TrackIndex > 0) ? this.c.TrackIndex : 3);
			}
			if (num < 1 || num > 3)
			{
				this.c.RangeNotice("リズム割り込みトラック", 1, 3, null);
				this.c.ErrorParameter("@C", num);
				return true;
			}
			this.c.UseRhythmTrackAtC = num - 1;
			this.c.AddInterruptTrackPoint(num);
			if (this.c.Mute[num])
			{
				return true;
			}
			this.c.DataWrite(244);
			this.c.DataWrite(4 - num);
			return true;
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x000136A0 File Offset: 0x000118A0
		public void FlushPendingRepeatFrameModAsRest(bool PreserveFractionForOuterRepeat = false)
		{
			if (!this.c.RepeatFrameModPending[this.c.TrackIndex])
			{
				return;
			}
			if (this.c.PreCompile || this.c.GetTimeCompile || this.c.InfoCompile || this.c.SoundId != 0)
			{
				return;
			}
			int num = 0;
			decimal num2 = (PreserveFractionForOuterRepeat ? 1.0m : 0.9m);
			while (this.c.FrameMod[this.c.TrackIndex] >= num2)
			{
				decimal[] frameMod = this.c.FrameMod;
				int trackIndex = this.c.TrackIndex;
				frameMod[trackIndex] -= 1m;
				num++;
			}
			this.c.RepeatFrameModPending[this.c.TrackIndex] = false;
			if (num <= 0)
			{
				return;
			}
			this.c.DataWrite(0);
			int i;
			for (i = num; i >= 255; i -= 255)
			{
				this.c.DataWrite(255);
			}
			this.c.DataWrite(i);
			this.c.AddTrackFrame(num);
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x000137D3 File Offset: 0x000119D3
		private int GetRepeatStorageMode()
		{
			if (this.c.RepeatMode[this.c.TrackIndex] != 0 && this.c.RepeatMode[this.c.TrackIndex] != 4)
			{
				return 1;
			}
			return 0;
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x0001380C File Offset: 0x00011A0C
		public void RepeatStart(bool Output = true, bool LengthInsert = false)
		{
			if (Output && this.c.RepeatMode[this.c.TrackIndex] == 1)
			{
				int repeatStorageMode = this.GetRepeatStorageMode();
				this.FlushPendingRepeatFrameModAsRest(this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode] > 0);
			}
			int num = this.c.NumberRead(true, false);
			if (num <= -32769)
			{
				num = -1;
			}
			else if (num < 0 || num > 255)
			{
				this.c.RangeNotice("リピート回数", 0, 255, null);
				this.c.ErrorString = "[" + num.ToString();
				return;
			}
			if (Output)
			{
				this.c.DataWrite(241);
			}
			if (LengthInsert)
			{
				string text = " " + this.GenerateLengthMML("L");
				this.c.LineBuffer = this.c.LineBuffer.Remove(this.c.Index, 1).Insert(this.c.Index, text);
				if (this.c.CurrentSourceMap != null)
				{
					if (this.c.Index < this.c.CurrentSourceMap.Count)
					{
						this.c.CurrentSourceMap.RemoveAt(this.c.Index);
					}
					this.c.CurrentSourceMap.InsertRange(this.c.Index, Enumerable.Repeat<MMLSourcePosition>(default(MMLSourcePosition), text.Length));
				}
			}
			int repeatStorageMode2 = this.GetRepeatStorageMode();
			if (this.c.RepeatNest[this.c.TrackIndex, 0] > 7)
			{
				this.c.Message = "リピートモード 0 のネストは最大 8 重です。";
				this.c.ErrorString = "[";
				return;
			}
			if (this.c.RepeatNest[this.c.TrackIndex, 1] > 3)
			{
				this.c.Message = "リピートモード 0 以外のネストは最大 4 重です。";
				this.c.ErrorString = "[";
				return;
			}
			this.c.RepeatCount[this.c.TrackIndex, this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode2], repeatStorageMode2] = num;
			this.c.RepeatEscapeOctave[this.c.TrackIndex, this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode2]] = 0;
			this.c.RepeatEscapeLength[this.c.TrackIndex, this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode2]] = null;
			this.c.RepeatEscapeLengthIndex[this.c.TrackIndex, this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode2]] = 0;
			this.c.RepeatFrame1[this.c.TrackIndex, this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode2], repeatStorageMode2] = 0;
			this.c.RepeatFrame2[this.c.TrackIndex, this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode2], repeatStorageMode2] = 0;
			if (this.c.RepeatMode[this.c.TrackIndex] == 1)
			{
				int num2 = this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode2];
				this.c.RepeatFrameMod[this.c.TrackIndex, num2] = this.c.FrameMod[this.c.TrackIndex];
				this.c.RepeatEscapeFrameMod[this.c.TrackIndex, num2] = 0m;
				this.c.FrameMod[this.c.TrackIndex] = 0m;
			}
			this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode2]++;
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00013C4C File Offset: 0x00011E4C
		public void RepeatEscape(bool Output = true)
		{
			int repeatStorageMode = this.GetRepeatStorageMode();
			if (this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode] < 1)
			{
				this.c.Message = "リピート外で / が使用されました。";
				this.c.ErrorString = "/";
				return;
			}
			if (Output)
			{
				this.c.DataWrite(242);
			}
			this.c.RepeatEscapeOctave[this.c.TrackIndex, this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode] - 1] = this.c.Octave[this.c.TrackIndex];
			this.c.RepeatEscapeLength[this.c.TrackIndex, this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode] - 1] = new decimal[16];
			Array.Copy(this.c.DefaultLength[this.c.TrackIndex], this.c.RepeatEscapeLength[this.c.TrackIndex, this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode] - 1], 16);
			this.c.RepeatEscapeLengthIndex[this.c.TrackIndex, this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode] - 1] = this.c.NoteLengthIndex[this.c.TrackIndex];
			if (this.c.RepeatMode[this.c.TrackIndex] == 1)
			{
				int num = this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode] - 1;
				this.c.RepeatEscapeFrameMod[this.c.TrackIndex, num] = this.c.FrameMod[this.c.TrackIndex];
				this.c.FrameMod[this.c.TrackIndex] = 0m;
			}
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00013E7C File Offset: 0x0001207C
		public void RepeatEnd(bool Output = true)
		{
			if (Output && this.c.RepeatMode[this.c.TrackIndex] == 1)
			{
				int repeatStorageMode = this.GetRepeatStorageMode();
				this.FlushPendingRepeatFrameModAsRest(this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode] > 0);
			}
			int repeatStorageMode2 = this.GetRepeatStorageMode();
			this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode2]--;
			if (this.c.RepeatNest[this.c.TrackIndex, 0] < 0 || this.c.RepeatNest[this.c.TrackIndex, 1] < 0)
			{
				this.c.Message = "リピート外で ] が使用されました。";
				this.c.ErrorString = "]";
				return;
			}
			int num = this.c.NumberRead(true, false);
			if (num <= -32769)
			{
				if (this.c.RepeatCount[this.c.TrackIndex, this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode2], repeatStorageMode2] > -1)
				{
					num = this.c.RepeatCount[this.c.TrackIndex, this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode2], repeatStorageMode2];
				}
				else
				{
					num = 2;
				}
			}
			else
			{
				this.c.ErrorString = "]";
				if (this.c.RepeatCount[this.c.TrackIndex, this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode2], repeatStorageMode2] > -1)
				{
					this.c.Message = "リピート回数が重複指定されています。";
					return;
				}
				if (num < 0 || num > 255)
				{
					this.c.RangeNotice("リピート回数", 0, 255, null);
					return;
				}
			}
			if (this.c.RepeatMode[this.c.TrackIndex] > 0 && this.c.RepeatMode[this.c.TrackIndex] < 4 && num == 0)
			{
				this.c.Message = "リピートモード 0 以外ではリピート回数に 0 を指定できません。";
				this.c.ErrorString = "]";
				return;
			}
			this.c.RepeatCount[this.c.TrackIndex, this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode2], repeatStorageMode2] = num;
			int num2 = (this.c.RepeatFrame1[this.c.TrackIndex, this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode2], repeatStorageMode2] + this.c.RepeatFrame2[this.c.TrackIndex, this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode2], repeatStorageMode2]) * (num - 1) + this.c.RepeatFrame1[this.c.TrackIndex, this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode2], repeatStorageMode2];
			if (Output)
			{
				if (!this.c.PreCompile)
				{
					if (this.c.RepeatMode[this.c.TrackIndex] == 3 && this.c.FrameMod[this.c.TrackIndex] != 0m)
					{
						this.c.Message = "リピート中のフレーム数が割り切れていません。\nリピート内の合計音長を調整するか、リピートモード 0 を使用してください。\n#TIMEBASE、@T コマンドを使用すると、割り切れる音長を設定しやすくなります。";
						this.c.ErrorString = "]";
						return;
					}
					if (this.c.RepeatMode[this.c.TrackIndex] == 1)
					{
						int num3 = this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode2];
						decimal num4 = this.c.RepeatFrameMod[this.c.TrackIndex, num3];
						if (this.c.RepeatEscapeOctave[this.c.TrackIndex, num3] > 0)
						{
							decimal num5 = this.c.RepeatEscapeFrameMod[this.c.TrackIndex, num3];
							decimal num6 = this.c.FrameMod[this.c.TrackIndex];
							this.c.FrameMod[this.c.TrackIndex] = num4 + num5 * num + num6 * (num - 1);
						}
						else
						{
							this.c.FrameMod[this.c.TrackIndex] = num4 + this.c.FrameMod[this.c.TrackIndex] * num;
						}
						this.c.RepeatFrameModPending[this.c.TrackIndex] = true;
					}
				}
				if (this.c.Slur[this.c.TrackIndex] && !this.c.Legato[this.c.TrackIndex])
				{
					this.c.DataWrite(132);
					this.c.Slur[this.c.TrackIndex] = false;
				}
				this.c.DataWrite(243);
				this.c.DataWrite(num);
			}
			if (this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode2] > 0)
			{
				if (this.c.RepeatEscapeOctave[this.c.TrackIndex, this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode2] - 1] > 0)
				{
					this.c.RepeatFrame2[this.c.TrackIndex, this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode2] - 1, repeatStorageMode2] += num2;
				}
				else
				{
					this.c.RepeatFrame1[this.c.TrackIndex, this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode2] - 1, repeatStorageMode2] += num2;
					this.c.RepeatFrame2[this.c.TrackIndex, this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode2] - 1, repeatStorageMode2] = 0;
				}
			}
			else if (Output)
			{
				this.c.TrackFrame[this.c.TrackIndex, this.c.SoundId] += num2;
			}
			if (this.c.RepeatEscapeOctave[this.c.TrackIndex, this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode2]] > 0)
			{
				this.c.Octave[this.c.TrackIndex] = this.c.RepeatEscapeOctave[this.c.TrackIndex, this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode2]];
			}
			if (this.c.RepeatEscapeLength[this.c.TrackIndex, this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode2]] != null)
			{
				this.c.DefaultLength[this.c.TrackIndex] = this.c.RepeatEscapeLength[this.c.TrackIndex, this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode2]];
			}
			this.c.NoteLengthIndex[this.c.TrackIndex] = this.c.RepeatEscapeLengthIndex[this.c.TrackIndex, this.c.RepeatNest[this.c.TrackIndex, repeatStorageMode2]];
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x000146A8 File Offset: 0x000128A8
		public void RepeatDeploy()
		{
			int index = this.c.Index;
			int repeatStorageMode = this.GetRepeatStorageMode();
			bool flag = this.c.RepeatLengthReturn || ((this.c.GetTimeCompile || this.c.InfoCompile) && this.c.RepeatMode[this.c.TrackIndex] > 0 && this.c.RepeatMode[this.c.TrackIndex] < 4);
			string text = "";
			if (repeatStorageMode > 0 && this.c.TrackIndex > 0)
			{
				text = text + "O" + this.c.Octave[this.c.TrackIndex].ToString() + " ";
			}
			if (flag)
			{
				text = text + this.GenerateLengthMML("L") + " ";
			}
			int num = this.c.NumberRead(true, false);
			if (num <= -32769)
			{
				num = -1;
			}
			else if (num < 0 || num > 255)
			{
				this.c.RangeNotice("リピート回数", 0, 255, null);
				this.c.ErrorString = "[" + num.ToString();
				return;
			}
			string[] array = new string[] { text, "" };
			List<MMLSourcePosition>[] array2 = new List<MMLSourcePosition>[]
			{
				new List<MMLSourcePosition>(),
				new List<MMLSourcePosition>()
			};
			if (text.Length > 0)
			{
				array2[0].AddRange(Enumerable.Repeat<MMLSourcePosition>(default(MMLSourcePosition), text.Length));
			}
			int num2 = 0;
			int num3 = 0;
			bool flag2 = true;
			this.c.Index++;
			while (this.c.Index < this.c.LineBuffer.Length)
			{
				char c = this.c.LineBuffer[this.c.Index];
				if (c == '[')
				{
					num3++;
				}
				else if (c == ']')
				{
					if (num3 > 0)
					{
						num3--;
					}
					else
					{
						int num4 = this.c.NumberRead(true, false);
						int num5;
						if (num4 <= -32769)
						{
							num5 = ((num > -1) ? num : 2);
						}
						else
						{
							this.c.ErrorString = "]";
							if (num > -1)
							{
								this.c.Message = "リピート回数が重複指定されています。";
								return;
							}
							if (num4 < 0 || num4 > 255)
							{
								this.c.RangeNotice("リピート回数", 0, 255, null);
								return;
							}
							num5 = num4;
						}
						if (this.c.RepeatMode[this.c.TrackIndex] > 0 && this.c.RepeatMode[this.c.TrackIndex] < 4 && num5 == 0)
						{
							this.c.Message = "リピートモード 0 以外ではリピート回数に 0 を指定できません。";
							this.c.ErrorString = "]";
							return;
						}
						string text2 = "";
						List<MMLSourcePosition> list = new List<MMLSourcePosition>();
						int num6 = num5 - num2;
						for (int i = 0; i < num6; i++)
						{
							if (i > 0 && repeatStorageMode > 0 && this.c.GetTimeCompile)
							{
								text2 += "\v";
								list.Add(default(MMLSourcePosition));
							}
							text2 = text2 + array[0] + array[1];
							list.AddRange(array2[0]);
							list.AddRange(array2[1]);
						}
						if (num2 == 1)
						{
							if (num6 > 0 && repeatStorageMode > 0 && this.c.GetTimeCompile)
							{
								text2 += "\v";
								list.Add(default(MMLSourcePosition));
							}
							text2 += array[0];
							list.AddRange(array2[0]);
						}
						this.c.Index++;
						if (this.c.LineBuffer.Length <= this.c.Index)
						{
							Core core = this.c;
							core.LineBuffer += " ";
							if (this.c.CurrentSourceMap != null)
							{
								this.c.CurrentSourceMap.Add(default(MMLSourcePosition));
							}
							this.c.Index = this.c.LineBuffer.Length - 1;
						}
						int num7 = this.c.Index - index;
						int length = this.c.LineBuffer.Length;
						if (this.c.CurrentSourceMap != null && this.c.CurrentSourceMap.Count < length)
						{
							this.c.CurrentSourceMap.AddRange(Enumerable.Repeat<MMLSourcePosition>(default(MMLSourcePosition), length - this.c.CurrentSourceMap.Count));
						}
						this.c.LineBuffer = this.c.LineBuffer.Remove(index, num7).Insert(index, text2);
						if (this.c.CurrentSourceMap != null)
						{
							this.c.CurrentSourceMap.RemoveRange(index, num7);
							this.c.CurrentSourceMap.InsertRange(index, list);
						}
						flag2 = false;
						break;
					}
				}
				else if (c == '/' && num3 == 0)
				{
					num2 = 1;
					this.c.Index++;
					continue;
				}
				string[] array3 = array;
				int num8 = num2;
				ReadOnlySpan<char> readOnlySpan = array3[num8];
				char c2 = c;
				array3[num8] = readOnlySpan + new ReadOnlySpan<char>(ref c2);
				if (this.c.CurrentSourceMap != null && this.c.Index < this.c.CurrentSourceMap.Count)
				{
					array2[num2].Add(this.c.CurrentSourceMap[this.c.Index]);
				}
				else
				{
					array2[num2].Add(default(MMLSourcePosition));
				}
				this.c.Index++;
			}
			this.c.Index = index;
			if (flag2)
			{
				this.c.RepeatNotClose[this.c.TrackIndex] = true;
				return;
			}
			this.c.Index--;
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00014CC0 File Offset: 0x00012EC0
		public void SelectStepMacro()
		{
			this.c.SelectStepMacroName = new string[4, 3];
			for (int i = 0; i < 3; i++)
			{
				this.c.StepMacroCount[this.c.TrackIndex, i] = 0;
				string text = "";
				this.c.SkipSpace();
				this.c.Index++;
				for (;;)
				{
					char c = this.c.Read1(-1);
					this.c.Index++;
					if (c < '0' || (c > '9' && c < 'A') || c > 'Z' || this.c.Index > this.c.LineBuffer.Length)
					{
						break;
					}
					ReadOnlySpan<char> readOnlySpan = text;
					char c2 = c;
					text = readOnlySpan + new ReadOnlySpan<char>(ref c2);
				}
				this.c.Index--;
				if (text != "" && !this.c.StepMacroMML.ContainsKey(text))
				{
					this.c.Message = "ステップマクロ名 「 " + text + " 」 は定義されていません。";
					this.c.ErrorString = "?" + text;
					return;
				}
				this.c.SelectStepMacroName[this.c.TrackIndex, i] = ((text == "") ? null : text);
				this.c.Index--;
				this.c.SkipSpace();
				if (!this.c.CheckNextCharacter(','))
				{
					break;
				}
				if (i == 2)
				{
					this.c.Message = "同時に指定できるステップマクロは最大 3 個です。";
					this.c.ErrorString = "?" + text;
					return;
				}
			}
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00014E84 File Offset: 0x00013084
		public void StepMacro()
		{
			bool flag = this.c.CurrentSourceMap != null && this.c.Index >= 0 && this.c.Index < this.c.CurrentSourceMap.Count && this.c.CurrentSourceMap[this.c.Index].SelectionPlayTarget;
			for (int i = 0; i < 3; i++)
			{
				int num = this.c.NumberRead(false, true);
				if (num > -32769)
				{
					string text = this.c.SelectStepMacroName[this.c.TrackIndex, i];
					if (text == null)
					{
						this.c.Message = (i + 1).ToString() + " 番目のステップマクロは呼び出されていません。\nXコマンドを実行する前に 「 ?＜マクロ名＞ 」 でステップマクロを選択してください。";
						this.c.ErrorParameter("X" + ((i > 0) ? "…," : ""), num);
						return;
					}
					if (num < 0 || num >= this.c.StepMacroMML[text].Count)
					{
						this.c.Message = "ステップマクロ 「 " + text + " 」 のステップ番号は" + ((this.c.StepMacroMML[text].Count > 1) ? (" 0 ～ " + (this.c.StepMacroMML[text].Count - 1).ToString() + " です。") : " 0 のみです。");
						this.c.ErrorParameter("X" + ((i > 0) ? "…," : ""), num);
						return;
					}
					this.c.StepMacroCount[this.c.TrackIndex, i] = num;
				}
				if (!this.c.CheckNextCharacter(','))
				{
					break;
				}
			}
			int num2 = 0;
			for (int j = 2; j >= 0; j--)
			{
				if (this.c.SelectStepMacroName[this.c.TrackIndex, j] == null)
				{
					num2++;
				}
				else
				{
					string text2 = this.c.SelectStepMacroName[this.c.TrackIndex, j];
					if (!this.c.StepMacroMML.ContainsKey(text2))
					{
						num2++;
					}
					else
					{
						int index = this.c.Index;
						int num3 = this.c.Index - index;
						if (this.c.Index < this.c.LineBuffer.Length)
						{
							num3++;
						}
						if (num3 > 0)
						{
							this.c.LineBuffer = this.c.LineBuffer.Remove(index, num3);
							if (this.c.CurrentSourceMap != null && index < this.c.CurrentSourceMap.Count)
							{
								this.c.CurrentSourceMap.RemoveRange(index, Math.Min(num3, this.c.CurrentSourceMap.Count - index));
							}
						}
						int num4 = this.c.StepMacroCount[this.c.TrackIndex, j];
						string text3 = this.c.StepMacroMML[text2][num4];
						this.c.LineBuffer = this.c.LineBuffer.Insert(index, text3);
						if (this.c.CurrentSourceMap != null)
						{
							this.c.CurrentSourceMap.InsertRange(index, Enumerable.Repeat<MMLSourcePosition>(new MMLSourcePosition(0, 0, '\0', flag, false), text3.Length));
						}
						num4++;
						if (num4 >= this.c.StepMacroMML[text2].Count)
						{
							num4 = 0;
						}
						this.c.StepMacroCount[this.c.TrackIndex, j] = num4;
						this.c.Index = index - 1;
					}
				}
			}
			if (num2 > 2)
			{
				this.c.Message = "ステップマクロが呼び出されていません。\nXコマンドを実行する前に 「 ?＜マクロ名＞ 」 でステップマクロを選択してください。";
				this.c.ErrorString = "X";
			}
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00015298 File Offset: 0x00013498
		public void AddBar()
		{
			int num = 0;
			int num2 = 0;
			int num3 = this.c.Index + 1;
			while (num3 < this.c.LineBuffer.Length && (this.c.LineBuffer[num3] == ' ' || this.c.LineBuffer[num3] == '\t'))
			{
				num3++;
			}
			int num4 = num3;
			while (num3 < this.c.LineBuffer.Length && char.IsDigit(this.c.LineBuffer[num3]))
			{
				num3++;
			}
			if (num3 > num4)
			{
				int.TryParse(this.c.LineBuffer.Substring(num4, num3 - num4), out num);
				while (num3 < this.c.LineBuffer.Length && (this.c.LineBuffer[num3] == ' ' || this.c.LineBuffer[num3] == '\t'))
				{
					num3++;
				}
				if (num3 < this.c.LineBuffer.Length && this.c.LineBuffer[num3] == '/')
				{
					num3++;
					while (num3 < this.c.LineBuffer.Length && (this.c.LineBuffer[num3] == ' ' || this.c.LineBuffer[num3] == '\t'))
					{
						num3++;
					}
					int num5 = num3;
					while (num3 < this.c.LineBuffer.Length && char.IsDigit(this.c.LineBuffer[num3]))
					{
						num3++;
					}
					if (num3 > num5)
					{
						int.TryParse(this.c.LineBuffer.Substring(num5, num3 - num5), out num2);
					}
					else
					{
						num = 0;
					}
				}
				else
				{
					num = 0;
				}
			}
			if (num > 0 && num2 > 0)
			{
				if (num > 32 || num2 > 64 || (num2 & (num2 - 1)) != 0)
				{
					this.c.Message = "拍子は |1/1 ～ |32/64 の範囲で、分母を 1, 2, 4, 8, 16, 32, 64 のいずれかで指定してください。";
					this.c.ErrorString = "|" + num.ToString() + "/" + num2.ToString();
					return;
				}
				this.c.Index = num3 - 1;
			}
			if (this.c.GetTimeCompile)
			{
				PianoRollNote pianoRollNote = new PianoRollNote();
				pianoRollNote.Track = this.c.TrackIndex;
				pianoRollNote.StartPoint = (double)this.c.GetFrame();
				this.c.PianoRollBars.Add(pianoRollNote);
			}
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00015514 File Offset: 0x00013714
		public bool CheckUseHardwareEnvelopeVolumeHarf(string Name, string ProcessText = "スキップ")
		{
			int num = ((this.c.VoiceNumber[this.c.TrackIndex] < 0) ? 0 : this.c.VoiceNumber[this.c.TrackIndex]);
			if (this.c.VoiceDataUseHENVHarfVolume[num])
			{
				this.c.AddInfoText(string.Concat(new string[] { "H-を含む音色と", Name, "は併用できないため、", ProcessText, "しました。" }));
				return true;
			}
			return false;
		}

		// Token: 0x0400002D RID: 45
		public Core c;

		// Token: 0x0400002E RID: 46
		public readonly NotePlayParameter[] AtINoteMonitorParameters = new NotePlayParameter[4];

		// Token: 0x0400002F RID: 47
		public readonly int[] AtINoteMonitorColumns = new int[] { -1, -1, -1, -1 };

		// Token: 0x04000030 RID: 48
		public int AlignTrackIndex = -1;

		// Token: 0x04000031 RID: 49
		private readonly int[] QuantizeMode = new int[4];

		// Token: 0x04000032 RID: 50
		private readonly bool[] QuantizeGeneratedRest = new bool[4];
	}
}
