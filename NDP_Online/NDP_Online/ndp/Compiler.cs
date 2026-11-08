using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ndp
{
	// Token: 0x0200000C RID: 12
	public class Compiler
	{
		// Token: 0x060000BF RID: 191 RVA: 0x000156F4 File Offset: 0x000138F4
		public bool Compile(string Text, CompileOptions Options, int CaretOffset = -1, bool SoloPlay = false, bool MutePlay = false, string MMLFilePath = null, int SelectionStartLine = -1, int SelectionStartColumn = -1, int SelectionEndLine = -1, int SelectionEndColumn = -1)
		{
			if (Options == null)
			{
				throw new ArgumentNullException("Options");
			}
			this.c = new Core();
			this.TrackReverseApplied = new bool[100];
			this.TrackSourceIndex = new int[100, 4];
			for (int i = 0; i < 100; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					this.TrackSourceIndex[i, j] = j;
				}
			}
			this.c.SelectionPlay = SelectionStartLine > 0 && SelectionStartColumn >= 0 && SelectionEndLine > 0 && SelectionEndColumn >= 0;
			this.c.SelectionPlayStartLine = SelectionStartLine;
			this.c.SelectionPlayStartColumn = SelectionStartColumn;
			this.c.SelectionPlayEndLine = SelectionEndLine;
			this.c.SelectionPlayEndColumn = SelectionEndColumn;
			this.cv = new CompileVoice(this.c);
			this.cm = new CompileMML(this.c);
			this.cr = new CompileRhythmMML(this.c);
			this.c.MMLFilePath = MMLFilePath;
			for (int k = 0; k < 16; k++)
			{
				this.c.VoiceData.Add(new byte[0]);
				this.c.VoiceDataUsePM.Add(0);
				this.c.VoiceDataUseHENVHarfVolume.Add(false);
				string text = Options.PresetVoices[k];
				this.cv.Registration(text, -1, '@', false);
				if (this.ErrorCheck())
				{
					return false;
				}
			}
			foreach (KeyValuePair<char, string> keyValuePair in Options.RhythmVoices)
			{
				CompileVoice compileVoice = this.cv;
				ReadOnlySpan<char> readOnlySpan = "@R:";
				char key = keyValuePair.Key;
				compileVoice.Registration(readOnlySpan + new ReadOnlySpan<char>(ref key) + "=" + keyValuePair.Value, -1, '@', false);
				if (this.ErrorCheck())
				{
					return false;
				}
			}
			StringReader stringReader = new StringReader(Text);
			List<string> list = new List<string>();
			this.c.LinesIndex = new List<int>();
			int num = 0;
			while (stringReader.Peek() > -1)
			{
				string text2 = stringReader.ReadLine();
				list.Add(text2);
				this.c.LinesIndex.Add(text2.Length);
				num += text2.Length + 1;
			}
			stringReader.Close();
			int num2 = -1;
			int num3 = 0;
			if (CaretOffset < Text.Length)
			{
				int num4 = 0;
				for (int l = 0; l < CaretOffset; l++)
				{
					if (Text[l] == '\n')
					{
						num3++;
						num4 = l;
					}
				}
				if (CaretOffset - num4 >= 0)
				{
					num2 = Text.Substring(num4, CaretOffset - num4).Length - 1;
				}
			}
			this.TrackSeparete(list, num2, num3, SoloPlay, MutePlay);
			if (this.ErrorCheck())
			{
				return false;
			}
			if (this.c.VoiceMMLFromFile.Count > 0)
			{
				for (int m = 0; m < this.c.VoiceMMLFromFile.Count; m++)
				{
					if (!this.c.RegisteredVoice[m])
					{
						this.cv.Registration(this.c.VoiceMMLFromFile[m], -1, '@', false);
					}
				}
			}
			if (this.c.RhythmVoiceMMLFromFile.Count > 0)
			{
				for (int n = 0; n < this.c.RhythmVoiceMMLFromFile.Count; n++)
				{
					if (this.c.RhythmVoiceMMLFromFile[n].Length > 2 && this.c.RegisteredRhythmCode.IndexOf(this.c.RhythmVoiceMMLFromFile[n][3]) < 0)
					{
						this.cv.Registration(this.c.RhythmVoiceMMLFromFile[n], -1, '@', false);
					}
				}
			}
			this.c.RhythmVolume = new int[this.c.StoreRhythmCode.Length];
			for (int num5 = 0; num5 < this.c.RhythmVolume.Length; num5++)
			{
				this.c.RhythmVolume[num5] = 15;
			}
			this.c.PreCompile = true;
			this.c.GetTimeCompile = true;
			this.c.AlignPlanReady = false;
			this.c.ResetInterruptTrackPoints();
			this.CompileTracks(Options.OutputMetaData);
			for (int num6 = 0; num6 < this.c.RepeatNotClose.Length; num6++)
			{
				if (this.c.RepeatNotClose[num6])
				{
					this.c.InfoText.Add(this.c.TrackName[num6] + "のリピートが閉じられずにデータが終了しています。");
					if (this.c.RepeatMode[num6] > 0)
					{
						this.c.Message = this.c.TrackName[num6] + "のリピートが閉じられずにデータが終了しています。";
						this.c.ErrorString = null;
						return false;
					}
				}
			}
			if (!this.c.FinalizeAlignPoints())
			{
				return false;
			}
			if (this.c.AlignPlanReady)
			{
				this.c.ResetTimeCompileOutput();
				this.c.ResetInterruptTrackPoints();
				this.CompileTracks(Options.OutputMetaData);
				if (this.ErrorCheck())
				{
					return false;
				}
			}
			this.c.ApplyInterruptTrackPointsToNotePlayParameters();
			this.c.GetTimeCompile = false;
			this.CompileTracks(Options.OutputMetaData);
			if (this.ErrorCheck())
			{
				return false;
			}
			if (this.c.InfoCompile)
			{
				this.CompileTracks(Options.OutputMetaData);
				if (this.ErrorCheck())
				{
					return false;
				}
			}
			if (this.c.AutoSetHeader.Count > 0)
			{
				string text3 = "#EXPAND-MEMORYヘッダにより、次の設定が自動で適用されました。\n";
				foreach (KeyValuePair<string, string> keyValuePair2 in this.c.AutoSetHeader)
				{
					text3 = string.Concat(new string[] { text3, keyValuePair2.Key, " ", keyValuePair2.Value, "\n" });
				}
				this.c.InfoText.Add(text3);
			}
			if (CaretOffset < -1)
			{
				int num7 = -CaretOffset - 1;
				if (SoloPlay)
				{
					this.c.TrackEnable = new bool[4];
					this.c.Mute = new bool[] { true, true, true, true };
					for (int num8 = 0; num8 < 4; num8++)
					{
						if (this.c.SetJumpNumber[num8, num7])
						{
							this.c.TrackEnable[num8] = true;
							this.c.Mute[num8] = false;
						}
					}
				}
				if (MutePlay)
				{
					this.c.TrackEnable = new bool[] { true, true, true, true };
					this.c.Mute = new bool[4];
					for (int num9 = 0; num9 < 4; num9++)
					{
						if (this.c.SetJumpNumber[num9, num7])
						{
							this.c.TrackEnable[num9] = false;
							this.c.Mute[num9] = true;
						}
					}
				}
			}
			for (int num10 = 0; num10 < this.c.RhythmVolume.Length; num10++)
			{
				this.c.RhythmVolume[num10] = 15;
			}
			this.c.FadeOutLoopCount = 0;
			this.c.FadeOutFrame = 0;
			this.c.PreCompile = false;
			this.CompileTracks(Options.OutputMetaData);
			if (this.ErrorCheck())
			{
				return false;
			}
			if (this.c.DefineAddressAbsolute > 0)
			{
				byte[] bgmdata = this.c.BGMData;
				int num11 = 11;
				bgmdata[num11] |= 4;
			}
			if (this.c.SEAddressAbsolute > 0)
			{
				byte[] bgmdata2 = this.c.BGMData;
				int num12 = 11;
				bgmdata2[num12] |= 8;
			}
			this.c.TotalFrame = 0;
			this.c.InfinityLoop = false;
			for (int num13 = 0; num13 < 4; num13++)
			{
				if (this.c.BeforeLoopFrame[num13, 0] > -1)
				{
					this.c.InfinityLoop = true;
				}
				int num14 = this.c.TrackFrame[num13, 0] + this.c.BeforeLoopFrame[num13, 0];
				if (this.c.TotalFrame < num14)
				{
					this.c.TotalFrame = num14;
				}
			}
			if (this.c.FadeOutLoopCount > 0)
			{
				this.c.InfinityLoop = false;
				int num15 = 0;
				for (int num16 = 0; num16 < 4; num16++)
				{
					if (this.c.TrackFrame[num16, 0] > num15)
					{
						num15 = this.c.TrackFrame[num16, 0];
					}
				}
				this.c.TotalFrame += num15 * (this.c.FadeOutLoopCount - 1) + this.c.FadeOutFrame * 16;
			}
			else
			{
				this.c.TotalFrame *= this.c.PianoRollLoop;
				int num17 = 0;
				int[] loopAddress = this.c.LoopAddress;
				for (int num18 = 0; num18 < loopAddress.Length; num18++)
				{
					if (loopAddress[num18] == 0)
					{
						num17++;
					}
				}
				if (num17 > 3 && this.c.PianoRollLoop > 1)
				{
					this.c.Message = "#LOOP-PIANOROLL ヘッダは @L コマンドを含む曲でご指定ください。";
					return false;
				}
			}
			this.c.BuildSlowTimeMap();
			return true;
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00016078 File Offset: 0x00014278
		private bool ErrorCheck()
		{
			if (this.c.Message == null)
			{
				this.c.ErrorString = "";
				return false;
			}
			return true;
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x0001609C File Offset: 0x0001429C
		private string CommentProcess(string Symbol, string Text)
		{
			int num = Text.IndexOf(Symbol);
			if (num >= 0)
			{
				string[] array = Text.Split('"', StringSplitOptions.None);
				if (array.Count<string>() > 0)
				{
					if (num < array[0].Length)
					{
						Text = Text.Substring(0, num);
					}
					else if (array.Count<string>() > 1 && num > array[0].Length + array[1].Length)
					{
						Text = Text.Substring(0, num);
					}
				}
				else
				{
					Text = Text.Substring(0, num);
				}
			}
			return Text;
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00016114 File Offset: 0x00014314
		private static string StripMmlComments(string line, ref bool inBlock)
		{
			char[] array = line.ToCharArray();
			bool flag = false;
			for (int i = 0; i < line.Length; i++)
			{
				char c = line[i];
				char c2 = ((i + 1 < line.Length) ? line[i + 1] : '\0');
				if (inBlock)
				{
					array[i] = ' ';
					if (c == '*' && c2 == '/')
					{
						array[++i] = ' ';
						inBlock = false;
					}
				}
				else if (c == '"')
				{
					flag = !flag;
				}
				else if (!flag)
				{
					if (c == '/' && c2 == '*')
					{
						array[i] = (array[++i] = ' ');
						inBlock = true;
					}
					else if (c == ';' || (c == '/' && c2 == '/'))
					{
						for (int j = i; j < array.Length; j++)
						{
							array[j] = ' ';
						}
						break;
					}
				}
			}
			return new string(array);
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x000161E4 File Offset: 0x000143E4
		private static string ReplaceCharAt(string Text, int Index, char Replacement)
		{
			if (Index < 0 || Index >= Text.Length)
			{
				return Text;
			}
			char[] array = Text.ToCharArray();
			array[Index] = Replacement;
			return new string(array);
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00016204 File Offset: 0x00014404
		private static string CreateLineBuffer(string Line, int StartIndex)
		{
			if (Line.Length <= StartIndex)
			{
				return "";
			}
			StringBuilder stringBuilder = new StringBuilder(Line.Length - StartIndex);
			for (int i = StartIndex; i < Line.Length; i++)
			{
				char c = Line[i];
				stringBuilder.Append((c == ':' || c == '\t') ? ' ' : c);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00016264 File Offset: 0x00014464
		private static string AddLineNumberMarkers(string LineBuffer, string LineNumberText, int ColumnOffset, List<MMLSourcePosition> SourceMap = null)
		{
			if (LineBuffer.Length == 0)
			{
				return LineBuffer;
			}
			StringBuilder stringBuilder = null;
			int i = 0;
			while (i < LineBuffer.Length)
			{
				if ("0123456789+-.^%# \t".IndexOf(LineBuffer[i]) == -1)
				{
					stringBuilder = new StringBuilder(LineBuffer.Length + 20);
					stringBuilder.Append(LineBuffer);
					stringBuilder.Insert(i, "\0" + LineNumberText + (ColumnOffset + i).ToString("D5"));
					if (SourceMap != null)
					{
						SourceMap.InsertRange(i, Enumerable.Repeat<MMLSourcePosition>(default(MMLSourcePosition), 10));
						break;
					}
					break;
				}
				else
				{
					i++;
				}
			}
			if (stringBuilder == null)
			{
				return LineBuffer;
			}
			for (int j = 0; j < stringBuilder.Length; j++)
			{
				if (stringBuilder[j] == '[')
				{
					j++;
					int num = j;
					while (num < stringBuilder.Length && "0123456789\f".IndexOf(stringBuilder[num]) != -1)
					{
						num++;
					}
					if (num < stringBuilder.Length)
					{
						stringBuilder.Insert(num, "\0" + LineNumberText + (ColumnOffset + num - 10).ToString("D5") + " ");
						if (SourceMap != null)
						{
							SourceMap.InsertRange(num, Enumerable.Repeat<MMLSourcePosition>(default(MMLSourcePosition), 11));
							break;
						}
						break;
					}
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x000163B8 File Offset: 0x000145B8
		private void TrackSeparete(List<string> Lines, int CursorColumn, int CursorLine, bool SoloPlay, bool MutePlay)
		{
			int[] array = new int[4];
			this.c.TrackBuffer = new string[100, 4];
			StringBuilder[,] array2 = new StringBuilder[100, 4];
			this.c.RegisterdSoundNumber = new List<int>();
			this.c.RegisterdSoundNumber.Add(0);
			this.c.RegisterSoundPriority = new Dictionary<int, int>();
			this.c.RegisterSoundPriority.Add(0, 255);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = true;
			bool flag4 = false;
			StringBuilder stringBuilder = new StringBuilder();
			List<MMLSourcePosition> list = new List<MMLSourcePosition>();
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			bool flag5 = false;
			bool flag6 = false;
			string text = "";
			foreach (var anon in Lines.Select((string Value, int Index) => new { Value, Index }))
			{
				if (anon.Index > 9998)
				{
					this.c.Message = "MML が 10,000 行を上回りました。";
					break;
				}
				string text2 = anon.Value.TrimStart(new char[] { ' ', '\t' });
				int num4 = anon.Value.Length - text2.Length;
				text2 = Compiler.StripMmlComments(text2, ref flag);
				if (!string.IsNullOrWhiteSpace(text2))
				{
					string text3 = text2.TrimStart(new char[] { ' ', '\t' });
					num4 += text2.Length - text3.Length;
					text2 = text3;
					if (!flag2)
					{
						text = text2.ToUpper().Split(new char[] { ' ', '\t', '=' })[0];
					}
					bool flag7 = true;
					foreach (string text4 in this.c.TrackCode)
					{
						flag7 = text.Contains(text4);
						if (!flag7)
						{
							break;
						}
					}
					List<MMLSourcePosition> list2 = null;
					int num5 = text2.IndexOf('{');
					if (num5 > -1)
					{
						text2 = Compiler.ReplaceCharAt(text2, num5, ' ');
						if (flag2 || text2.IndexOf('{') > -1)
						{
							this.c.Message = "「 { 」のネストはできません。";
							this.c.ErrorLine = anon.Index + 1;
							this.c.ErrorString = "{";
							return;
						}
						flag2 = true;
						flag3 = true;
						stringBuilder.Clear();
						list.Clear();
						num = anon.Index;
						num2 = num4;
						num3 = 0;
					}
					num5 = text2.IndexOf('}');
					if (num5 > -1)
					{
						if (!flag2)
						{
							this.c.Message = "「 { 」の外で「 } 」が使用されています。";
							this.c.ErrorLine = anon.Index + 1;
							this.c.ErrorString = "}";
							return;
						}
						string text5 = Compiler.ReplaceCharAt(text2, num5, ' ');
						list2 = new List<MMLSourcePosition>(list);
						for (int j = 0; j < text5.Length; j++)
						{
							int num6 = anon.Index + 1;
							int num7 = num4 + j;
							list2.Add(new MMLSourcePosition(num6, num7, char.ToUpperInvariant(text5[j]), this.c.IsSelectionPlayPosition(num6, num7), false));
						}
						text2 = stringBuilder.ToString() + text5;
						if (!flag7)
						{
							this.c.MultiLineStartLine.Add(anon.Index + 1, num + 1);
						}
						flag2 = false;
						flag4 = true;
					}
					else if (flag2 && !flag7)
					{
						string text6 = text2.ToUpper();
						stringBuilder.Append(text6);
						for (int k = 0; k < text6.Length; k++)
						{
							int num8 = anon.Index + 1;
							int num9 = num4 + k;
							list.Add(new MMLSourcePosition(num8, num9, text6[k], this.c.IsSelectionPlayPosition(num8, num9), false));
						}
						if (anon.Index < CursorLine)
						{
							num3 += text2.Length;
							continue;
						}
						continue;
					}
					if (!flag7 && text2.Length > 0)
					{
						char c = text2[0];
						if (c <= 'S')
						{
							if (c <= '@')
							{
								switch (c)
								{
								case '!':
								{
									this.c.ErrorString = text2;
									if (text2.Length < 2 || !text2.StartsWith("!S", StringComparison.OrdinalIgnoreCase))
									{
										this.c.Message = "効果音定義の書式が正しくありません。\n!S＜番号＞[,優先度] で指定してください。";
										return;
									}
									string[] array3 = text2.Substring(2).Split(',', StringSplitOptions.None);
									int num10 = -1;
									int num11 = 7;
									if (!int.TryParse(array3[0], out num10) || num10 < 0 || num10 > 99)
									{
										this.c.RangeNotice("定義する効果音の番号", 1, 99, null);
										return;
									}
									if (array3.Length >= 2 && (!int.TryParse(array3[1], out num11) || num11 < 0 || num11 > 7))
									{
										this.c.RangeNotice("効果音の優先度", 0, 7, null);
										return;
									}
									this.c.RegisterdSoundNumber.Add(num10);
									this.c.RegisterSoundPriority[num10] = num11;
									continue;
								}
								case '"':
									goto IL_0659;
								case '#':
									this.c.RegisterHeader(text2, anon.Index);
									if (this.c.Message != null)
									{
										return;
									}
									continue;
								case '$':
									this.c.RegisterMacro(text2.ToUpper(), anon.Index, num4);
									if (this.c.Message != null)
									{
										return;
									}
									continue;
								default:
									if (c != '?')
									{
										if (c != '@')
										{
											goto IL_0659;
										}
										this.cv.Registration(text2.ToUpper(), anon.Index, '@', true);
										if (this.c.Message != null)
										{
											return;
										}
										continue;
									}
									else
									{
										this.c.RegisterStepMacro(text2.ToUpper(), anon.Index);
										if (this.c.Message != null)
										{
											return;
										}
										continue;
									}
									break;
								}
							}
							else if (c != 'M' && c != 'Q' && c != 'S')
							{
								goto IL_0659;
							}
						}
						else if (c <= 'm')
						{
							if (c != 'V' && c != '\\' && c != 'm')
							{
								goto IL_0659;
							}
						}
						else if (c != 'q' && c != 's' && c != 'v')
						{
							goto IL_0659;
						}
						this.cv.Registration(text2.ToUpper(), anon.Index, text2[0], false);
						if (this.c.Message != null)
						{
							return;
						}
						continue;
					}
					IL_0659:
					this.cv.EnablePitchEnvelopePreset();
					text2 = text2.ToUpper() + " ";
					if (!flag6 && CursorColumn > -1 && CursorLine == anon.Index)
					{
						if (text.Length > 0 && text2.Trim().Length > text.Length)
						{
							flag6 = true;
						}
						else
						{
							CursorColumn = 0;
							CursorLine++;
						}
					}
					for (int l = 0; l < text.Length; l++)
					{
						int num12 = -1;
						int num13 = -1;
						int num14 = Array.IndexOf<string>(this.c.TrackCode, text[l].ToString());
						if (num14 < 0)
						{
							this.c.Message = "トラック名が正しくありません。";
							this.c.ErrorString = text[l].ToString();
							this.c.ErrorLine = anon.Index + 1;
							return;
						}
						if (!flag5)
						{
							if (flag4 && num <= CursorLine && anon.Index >= CursorLine)
							{
								CursorLine = anon.Index;
								CursorColumn += num3;
							}
							if (CursorColumn > -1 && text2.Length > 0 && anon.Index == CursorLine)
							{
								if (SoloPlay)
								{
									this.c.TrackEnable = new bool[4];
									this.c.Mute = new bool[] { true, true, true, true };
									for (int m = 0; m < text.Length; m++)
									{
										int num15 = Array.IndexOf<string>(this.c.TrackCode, text[m].ToString());
										this.c.TrackEnable[num15] = true;
										this.c.Mute[num15] = false;
									}
								}
								int num16 = Math.Max(CursorColumn - num4, 1) + 1;
								num16 = Math.Min(Math.Max(num16, 3), text2.Length - 1);
								if (num16 <= text.Length)
								{
									num16 = text.Length + 1;
								}
								Dictionary<int, int> dictionary = new Dictionary<int, int>();
								int num17 = 0;
								for (;;)
								{
									int num18 = text2.IndexOf("$", num17);
									if (num18 < 0)
									{
										break;
									}
									num18++;
									for (int n = num18; n < text2.Length; n++)
									{
										num5 = (int)text2[n];
										if (text2[n] < '0' || (text2[n] > '9' && text2[n] < 'A') || text2[n] > 'Z' || n > text2.Length - 2)
										{
											num17 = n;
											dictionary.Add(num18 - 1, n);
											break;
										}
									}
									if (!dictionary.ContainsKey(num18 - 1))
									{
										dictionary.Add(num18 - 1, text2.Length - 1);
									}
								}
								foreach (KeyValuePair<int, int> keyValuePair in dictionary)
								{
									if (num16 >= keyValuePair.Key && num16 <= keyValuePair.Value)
									{
										num16 = keyValuePair.Value;
									}
								}
								if (text2[num16] == '%')
								{
									num16--;
								}
								num12 = num16;
								text2 = text2.Insert(num16, "\f");
								flag5 = true;
								if (MutePlay)
								{
									num13 = text.Length + 1;
									text2 = text2.Insert(num13, "@OFF");
									if (num13 <= num12)
									{
										num12 += 4;
									}
								}
							}
						}
						bool flag8 = false;
						int num19 = 0;
						if ((!flag3 && flag4) || (flag2 && !flag3))
						{
							this.c.LineBuffer = " " + text2;
							flag8 = true;
						}
						else
						{
							this.c.LineBuffer = Compiler.CreateLineBuffer(text2, text.Length);
							num19 = text.Length;
						}
						List<Tuple<int, int>> list3 = new List<Tuple<int, int>>();
						if (num12 >= 0)
						{
							int num20 = (flag8 ? (num12 + 1) : (num12 - text.Length));
							if (num20 >= 0 && num20 < this.c.LineBuffer.Length)
							{
								list3.Add(Tuple.Create<int, int>(num20, 1));
							}
						}
						if (num13 >= 0)
						{
							int num21 = (flag8 ? (num13 + 1) : (num13 - text.Length));
							if (num21 >= 0 && num21 + 4 <= this.c.LineBuffer.Length)
							{
								list3.Add(Tuple.Create<int, int>(num21, 4));
							}
						}
						if (this.c.LineBuffer.Contains('J') && !this.c.LineBuffer.Contains("J=") && this.c.FastForwardMode)
						{
							this.c.FastForward = true;
							this.c.FastForwardTrack[num14] = true;
						}
						int num22 = num4 + text.Length;
						List<MMLSourcePosition> list4 = new List<MMLSourcePosition>(this.c.LineBuffer.Length);
						if (list2 != null)
						{
							list2.Add(default(MMLSourcePosition));
							for (int num23 = 0; num23 < this.c.LineBuffer.Length; num23++)
							{
								if (flag8 && num23 == 0)
								{
									list4.Add(default(MMLSourcePosition));
								}
								else
								{
									int num24 = 0;
									bool flag9 = false;
									foreach (Tuple<int, int> tuple in list3)
									{
										if (num23 >= tuple.Item1 && num23 < tuple.Item1 + tuple.Item2)
										{
											flag9 = true;
											break;
										}
										if (tuple.Item1 + tuple.Item2 <= num23)
										{
											num24 += tuple.Item2;
										}
									}
									if (flag9)
									{
										list4.Add(default(MMLSourcePosition));
									}
									else
									{
										int num25 = num23 - num24 - ((flag8 > false) ? 1 : 0) + num19;
										if (num25 < 0 || num25 >= list2.Count)
										{
											list4.Add(default(MMLSourcePosition));
										}
										else
										{
											MMLSourcePosition mmlsourcePosition = list2[num25];
											if (!mmlsourcePosition.IsValid)
											{
												list4.Add(mmlsourcePosition);
											}
											else
											{
												list4.Add(new MMLSourcePosition(mmlsourcePosition.Line, mmlsourcePosition.Column, this.c.LineBuffer[num23], mmlsourcePosition.SelectionPlayTarget, false));
											}
										}
									}
								}
							}
						}
						else
						{
							for (int num26 = 0; num26 < this.c.LineBuffer.Length; num26++)
							{
								int num27 = 0;
								bool flag10 = false;
								foreach (Tuple<int, int> tuple2 in list3)
								{
									if (num26 >= tuple2.Item1 && num26 < tuple2.Item1 + tuple2.Item2)
									{
										flag10 = true;
										break;
									}
									if (tuple2.Item1 + tuple2.Item2 <= num26)
									{
										num27 += tuple2.Item2;
									}
								}
								if (flag10)
								{
									list4.Add(default(MMLSourcePosition));
								}
								else
								{
									int num28 = anon.Index + 1;
									int num29 = num22 + num26 - num27;
									list4.Add(new MMLSourcePosition(num28, num29, this.c.LineBuffer[num26], this.c.IsSelectionPlayPosition(num28, num29), false));
								}
							}
						}
						int num30 = (flag4 ? (num + 1) : (anon.Index + 1));
						int num31 = (flag4 ? (num2 + text.Length) : num22);
						this.c.LineBuffer = Compiler.AddLineNumberMarkers(this.c.LineBuffer, num30.ToString("D4"), num31, list4);
						this.c.Index = 0;
						while (this.c.Index < this.c.LineBuffer.Length)
						{
							if (this.c.Read1(-1) == '$')
							{
								int index = this.c.Index;
								this.MacroDeploy(num14, array[num14], list4);
								this.c.Index = index - 1;
							}
							if (this.c.Message != null)
							{
								this.c.ErrorLine = anon.Index + 1;
								return;
							}
							this.c.Index++;
						}
						int num32 = this.c.RegisterdSoundNumber[this.c.RegisterdSoundNumber.Count - 1];
						if (array2[num32, num14] == null)
						{
							array2[num32, num14] = new StringBuilder();
						}
						array2[num32, num14].Append(this.c.LineBuffer);
						if (this.c.TrackSourceMap[num32, num14] == null)
						{
							this.c.TrackSourceMap[num32, num14] = new List<MMLSourcePosition>();
						}
						this.c.TrackSourceMap[num32, num14].AddRange(list4);
						array[num14] += this.c.LineBuffer.Length;
					}
					if (flag4)
					{
						this.c.TrackNameLength[num] = text.Length;
						this.c.TrackNameLength[anon.Index] = 0;
					}
					else
					{
						this.c.TrackNameLength[anon.Index] = text.Length;
					}
					flag3 = false;
					flag4 = false;
				}
			}
			for (int num33 = 0; num33 < 100; num33++)
			{
				for (int num34 = 0; num34 < 4; num34++)
				{
					if (array2[num33, num34] != null)
					{
						this.c.TrackBuffer[num33, num34] = array2[num33, num34].ToString();
					}
				}
			}
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00017420 File Offset: 0x00015620
		private int JindexSkipSearch(string Command, string SkipMML, string Line, int Jindex)
		{
			int num = Line.IndexOf(Command);
			if (num < 0 || Jindex < num)
			{
				return Jindex;
			}
			if (Line.Split(new char[] { ' ', '\t' })[0].IndexOf('R') > -1)
			{
				return num;
			}
			int num2 = num + Command.Length;
			int num3 = num;
			for (int i = num2; i < Line.Length; i++)
			{
				bool flag = false;
				for (int j = 0; j < SkipMML.Length; j++)
				{
					if (Line[i] == SkipMML[j])
					{
						flag = true;
						break;
					}
				}
				num3 = i;
				if (!flag)
				{
					break;
				}
			}
			if (Jindex <= num3)
			{
				return num3 + 1;
			}
			return Jindex;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x000174BC File Offset: 0x000156BC
		private static ulong AutoRepeatHashBytes(byte[] Data, List<Compiler.AutoRepeatSEReference> References)
		{
			ulong num = 1469598103934665603UL;
			for (int i = 0; i < Data.Length; i++)
			{
				num ^= (ulong)Data[i];
				num *= 1099511628211UL;
			}
			foreach (Compiler.AutoRepeatSEReference autoRepeatSEReference in References)
			{
				num ^= (ulong)((long)(autoRepeatSEReference.RelativeOffset + 1));
				num *= 1099511628211UL;
				num ^= (ulong)((long)(autoRepeatSEReference.SoundNumber + 1));
				num *= 1099511628211UL;
			}
			return num;
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00017560 File Offset: 0x00015760
		private static bool AutoRepeatUnitEquals(Compiler.AutoRepeatUnit A, Compiler.AutoRepeatUnit B)
		{
			if (A.Hash != B.Hash || A.Data.Length != B.Data.Length || A.SEReferences.Count != B.SEReferences.Count)
			{
				return false;
			}
			for (int i = 0; i < A.Data.Length; i++)
			{
				if (A.Data[i] != B.Data[i])
				{
					return false;
				}
			}
			for (int j = 0; j < A.SEReferences.Count; j++)
			{
				if (A.SEReferences[j].RelativeOffset != B.SEReferences[j].RelativeOffset)
				{
					return false;
				}
				if (A.SEReferences[j].SoundNumber != B.SEReferences[j].SoundNumber)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00017634 File Offset: 0x00015834
		private static Compiler.AutoRepeatCandidate FindAutoRepeatCandidate(Compiler.AutoRepeatRunContext Context, int Start, int End, int Depth)
		{
			if (Depth >= 4 || End - Start < 2)
			{
				return null;
			}
			List<Compiler.AutoRepeatUnit> units = Context.Units;
			Compiler.AutoRepeatCandidate autoRepeatCandidate = null;
			for (int i = Start; i < End - 1; i++)
			{
				int num = (End - i) / 2;
				for (int j = 1; j <= num; j++)
				{
					if (units[i].Hash == units[i + j].Hash && Context.RangeHash(i, j) == Context.RangeHash(i + j, j))
					{
						int num2 = Context.LongestCommonPrefix(i, i + j, End - (i + j));
						if (num2 >= j)
						{
							int num3 = Math.Min(255, 1 + num2 / j);
							if (num3 >= 2)
							{
								int num4 = Context.BytePrefix[i + j] - Context.BytePrefix[i];
								int num5 = (num3 - 1) * num4 - 3;
								if (num5 > 0)
								{
									int num6 = num4 * num3;
									if (autoRepeatCandidate == null || num5 > autoRepeatCandidate.Saving || (num5 == autoRepeatCandidate.Saving && num4 > autoRepeatCandidate.BodyBytes) || (num5 == autoRepeatCandidate.Saving && num4 == autoRepeatCandidate.BodyBytes && num6 > autoRepeatCandidate.CoverageBytes))
									{
										autoRepeatCandidate = new Compiler.AutoRepeatCandidate
										{
											Start = i,
											BodyUnitCount = j,
											RepeatCount = num3,
											BodyBytes = num4,
											Saving = num5,
											CoverageBytes = num6
										};
									}
								}
							}
						}
					}
				}
			}
			if (autoRepeatCandidate != null)
			{
				for (int k = 1; k < autoRepeatCandidate.RepeatCount; k++)
				{
					for (int l = 0; l < autoRepeatCandidate.BodyUnitCount; l++)
					{
						if (!Compiler.AutoRepeatUnitEquals(units[autoRepeatCandidate.Start + l], units[autoRepeatCandidate.Start + k * autoRepeatCandidate.BodyUnitCount + l]))
						{
							return null;
						}
					}
				}
			}
			return autoRepeatCandidate;
		}

		// Token: 0x060000CB RID: 203 RVA: 0x000177F8 File Offset: 0x000159F8
		private static void EmitAutoRepeatLiteral(Compiler.AutoRepeatUnit Unit, List<byte> Output, List<Compiler.AutoRepeatSEReference> References)
		{
			int count = Output.Count;
			Output.AddRange(Unit.Data);
			foreach (Compiler.AutoRepeatSEReference autoRepeatSEReference in Unit.SEReferences)
			{
				References.Add(new Compiler.AutoRepeatSEReference
				{
					RelativeOffset = count + autoRepeatSEReference.RelativeOffset,
					LineNumber = autoRepeatSEReference.LineNumber,
					SoundNumber = autoRepeatSEReference.SoundNumber
				});
			}
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00017888 File Offset: 0x00015A88
		private static void EmitAutoRepeatRange(Compiler.AutoRepeatRunContext Context, int Start, int End, int Depth, List<byte> Output, List<Compiler.AutoRepeatSEReference> References)
		{
			if (Start >= End)
			{
				return;
			}
			List<Compiler.AutoRepeatUnit> units = Context.Units;
			Compiler.AutoRepeatCandidate autoRepeatCandidate = Compiler.FindAutoRepeatCandidate(Context, Start, End, Depth);
			if (autoRepeatCandidate == null)
			{
				for (int i = Start; i < End; i++)
				{
					Compiler.EmitAutoRepeatLiteral(units[i], Output, References);
				}
				return;
			}
			Compiler.EmitAutoRepeatRange(Context, Start, autoRepeatCandidate.Start, Depth, Output, References);
			Output.Add(241);
			Compiler.EmitAutoRepeatRange(Context, autoRepeatCandidate.Start, autoRepeatCandidate.Start + autoRepeatCandidate.BodyUnitCount, Depth + 1, Output, References);
			Output.Add(243);
			Output.Add((byte)autoRepeatCandidate.RepeatCount);
			Compiler.EmitAutoRepeatRange(Context, autoRepeatCandidate.Start + autoRepeatCandidate.BodyUnitCount * autoRepeatCandidate.RepeatCount, End, Depth, Output, References);
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00017944 File Offset: 0x00015B44
		private void ReplaceTrackSEReferences(int TrackStart, int TrackEnd, List<Compiler.AutoRepeatSEReference> NewReferences)
		{
			List<Tuple<int, int, int>> list = new List<Tuple<int, int, int>>();
			for (int i = 0; i < this.c.SEUseAddress.Count; i++)
			{
				int num = this.c.SEUseAddress[i];
				if (num < TrackStart || num >= TrackEnd)
				{
					list.Add(Tuple.Create<int, int, int>(num, this.c.SEUseLine[i], this.c.SEUseNumber[i]));
				}
			}
			foreach (Compiler.AutoRepeatSEReference autoRepeatSEReference in NewReferences)
			{
				list.Add(Tuple.Create<int, int, int>(TrackStart + autoRepeatSEReference.RelativeOffset, autoRepeatSEReference.LineNumber, autoRepeatSEReference.SoundNumber));
			}
			list.Sort((Tuple<int, int, int> A, Tuple<int, int, int> B) => A.Item1.CompareTo(B.Item1));
			this.c.SEUseAddress = list.Select<Tuple<int, int, int>, int>((Tuple<int, int, int> Item) => Item.Item1).ToList<int>();
			this.c.SEUseLine = list.Select<Tuple<int, int, int>, int>((Tuple<int, int, int> Item) => Item.Item2).ToList<int>();
			this.c.SEUseNumber = list.Select<Tuple<int, int, int>, int>((Tuple<int, int, int> Item) => Item.Item3).ToList<int>();
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00017ADC File Offset: 0x00015CDC
		private void ApplyAutoRepeatCompression(int TrackIndex, int TrackStart, int TrackDataEnd, int TrackEnd)
		{
			if (this.c.PreCompile || this.c.GetTimeCompile || this.c.InfoCompile || this.c.SoundId != 0)
			{
				return;
			}
			if (TrackDataEnd <= TrackStart || this.c.AutoRepeatBoundaries == null)
			{
				return;
			}
			List<AutoRepeatBoundaryData> list = new List<AutoRepeatBoundaryData>(this.c.AutoRepeatBoundaries);
			list.Add(new AutoRepeatBoundaryData(TrackStart, this.c.RepeatMode[TrackIndex]));
			list.Add(new AutoRepeatBoundaryData(TrackDataEnd, this.c.RepeatMode[TrackIndex]));
			if (this.c.LoopAddress[TrackIndex] >= TrackStart && this.c.LoopAddress[TrackIndex] <= TrackDataEnd)
			{
				list.Add(new AutoRepeatBoundaryData(this.c.LoopAddress[TrackIndex], this.c.RepeatMode[TrackIndex]));
			}
			list = (from Item in list
				where Item.Address >= TrackStart && Item.Address <= TrackDataEnd
				orderby Item.Address, this.c.AutoRepeatBoundaries.IndexOf(Item)
				select Item).ToList<AutoRepeatBoundaryData>();
			List<AutoRepeatBoundaryData> list2 = new List<AutoRepeatBoundaryData>();
			foreach (AutoRepeatBoundaryData autoRepeatBoundaryData in list)
			{
				if (list2.Count > 0 && list2[list2.Count - 1].Address == autoRepeatBoundaryData.Address)
				{
					list2[list2.Count - 1] = autoRepeatBoundaryData;
				}
				else
				{
					list2.Add(autoRepeatBoundaryData);
				}
			}
			if (list2.Count < 2)
			{
				return;
			}
			List<Compiler.AutoRepeatSEReference> list3 = new List<Compiler.AutoRepeatSEReference>();
			for (int i = 0; i < this.c.SEUseAddress.Count; i++)
			{
				if (this.c.SEUseAddress[i] >= TrackStart && this.c.SEUseAddress[i] < TrackDataEnd)
				{
					list3.Add(new Compiler.AutoRepeatSEReference
					{
						RelativeOffset = this.c.SEUseAddress[i] - TrackStart,
						LineNumber = this.c.SEUseLine[i],
						SoundNumber = this.c.SEUseNumber[i]
					});
				}
			}
			List<Compiler.AutoRepeatUnit> list4 = new List<Compiler.AutoRepeatUnit>();
			for (int j = 0; j < list2.Count - 1; j++)
			{
				int address = list2[j].Address;
				int address2 = list2[j + 1].Address;
				if (address2 > address)
				{
					byte[] array = new byte[address2 - address];
					Buffer.BlockCopy(this.c.BGMData, address, array, 0, array.Length);
					Compiler.AutoRepeatUnit autoRepeatUnit = new Compiler.AutoRepeatUnit
					{
						Data = array,
						RepeatMode = list2[j].RepeatMode
					};
					foreach (Compiler.AutoRepeatSEReference autoRepeatSEReference in list3)
					{
						int num = TrackStart + autoRepeatSEReference.RelativeOffset;
						if (num >= address && num < address2)
						{
							autoRepeatUnit.SEReferences.Add(new Compiler.AutoRepeatSEReference
							{
								RelativeOffset = num - address,
								LineNumber = autoRepeatSEReference.LineNumber,
								SoundNumber = autoRepeatSEReference.SoundNumber
							});
						}
					}
					autoRepeatUnit.Hash = Compiler.AutoRepeatHashBytes(autoRepeatUnit.Data, autoRepeatUnit.SEReferences);
					list4.Add(autoRepeatUnit);
				}
			}
			if (list4.Count == 0)
			{
				return;
			}
			Compiler.AutoRepeatRunContext autoRepeatRunContext = new Compiler.AutoRepeatRunContext(list4);
			List<byte> list5 = new List<byte>(TrackDataEnd - TrackStart);
			List<Compiler.AutoRepeatSEReference> list6 = new List<Compiler.AutoRepeatSEReference>();
			int num2 = this.c.LoopAddress[TrackIndex];
			int num3 = -1;
			int num4 = TrackStart;
			int num5;
			for (int k = 0; k < list4.Count; k = num5)
			{
				int repeatMode = list4[k].RepeatMode;
				num5 = k + 1;
				int num6 = num4 + list4[k].Data.Length;
				while (num5 < list4.Count && num6 != num2 && list4[num5].RepeatMode == repeatMode)
				{
					num6 += list4[num5].Data.Length;
					num5++;
				}
				if (num4 == num2)
				{
					num3 = list5.Count;
				}
				if (repeatMode == 4)
				{
					Compiler.EmitAutoRepeatRange(autoRepeatRunContext, k, num5, 0, list5, list6);
				}
				else
				{
					for (int l = k; l < num5; l++)
					{
						Compiler.EmitAutoRepeatLiteral(list4[l], list5, list6);
					}
				}
				for (int m = k; m < num5; m++)
				{
					num4 += list4[m].Data.Length;
				}
				if (num4 == num2)
				{
					num3 = list5.Count;
				}
			}
			if (num2 == TrackDataEnd)
			{
				num3 = list5.Count;
			}
			int num7 = TrackEnd - TrackDataEnd;
			byte[] array2 = new byte[num7];
			if (num7 > 0)
			{
				Buffer.BlockCopy(this.c.BGMData, TrackDataEnd, array2, 0, num7);
			}
			if (list5.Count >= TrackDataEnd - TrackStart)
			{
				return;
			}
			Buffer.BlockCopy(list5.ToArray(), 0, this.c.BGMData, TrackStart, list5.Count);
			if (num7 > 0)
			{
				Buffer.BlockCopy(array2, 0, this.c.BGMData, TrackStart + list5.Count, num7);
			}
			int num8 = TrackStart + list5.Count + num7;
			Array.Clear(this.c.BGMData, num8, TrackEnd - num8);
			this.c.Address = num8;
			if (num7 >= 3 && ((int)array2[num7 - 2] | ((int)array2[num7 - 1] << 8)) != 0 && num3 >= 0)
			{
				int num9 = TrackStart + num3;
				this.c.BGMData[num8 - 2] = (byte)(num9 & 255);
				this.c.BGMData[num8 - 1] = (byte)(num9 >> 8);
				this.c.LoopAddress[TrackIndex] = num9;
			}
			this.ReplaceTrackSEReferences(TrackStart, TrackDataEnd, list6);
		}

		// Token: 0x060000CF RID: 207 RVA: 0x0001816C File Offset: 0x0001636C
		private void CompileTracks(bool OutputMetaData)
		{
			this.c.BeginAlignPass();
			this.c.SoundId = 0;
			int num = 0;
			int num2 = 0;
			foreach (int num3 in this.c.RegisterdSoundNumber)
			{
				this.c.SoundId = num3;
				int[] array = new int[]
				{
					this.TrackSourceIndex[this.c.SoundId, 0],
					this.TrackSourceIndex[this.c.SoundId, 1],
					this.TrackSourceIndex[this.c.SoundId, 2],
					this.TrackSourceIndex[this.c.SoundId, 3]
				};
				if (this.c.PreCompile && !this.c.GetTimeCompile && this.c.ReverseTrack[0] > -1 && !this.TrackReverseApplied[this.c.SoundId])
				{
					for (int i = 0; i < 2; i++)
					{
						if (this.c.TrackBuffer[this.c.SoundId, this.c.ReverseTrack[i]] == null)
						{
							this.c.Message = "#TRACK-REV で空のトラックが指定されています。";
							this.c.ErrorString = this.c.TrackCode[this.c.ReverseTrack[i]];
							return;
						}
					}
					ref string ptr = ref this.c.TrackBuffer[this.c.SoundId, this.c.ReverseTrack[0]];
					ref string ptr2 = ref this.c.TrackBuffer[this.c.SoundId, this.c.ReverseTrack[1]];
					string text = this.c.TrackBuffer[this.c.SoundId, this.c.ReverseTrack[1]];
					string text2 = this.c.TrackBuffer[this.c.SoundId, this.c.ReverseTrack[0]];
					ptr = text;
					ptr2 = text2;
					List<MMLSourcePosition>[,] trackSourceMap = this.c.TrackSourceMap;
					int soundId = this.c.SoundId;
					int num4 = this.c.ReverseTrack[0];
					List<MMLSourcePosition>[,] trackSourceMap2 = this.c.TrackSourceMap;
					int j = this.c.SoundId;
					int num5 = this.c.ReverseTrack[1];
					List<MMLSourcePosition> list = this.c.TrackSourceMap[this.c.SoundId, this.c.ReverseTrack[1]];
					List<MMLSourcePosition> list2 = this.c.TrackSourceMap[this.c.SoundId, this.c.ReverseTrack[0]];
					trackSourceMap[soundId, num4] = list;
					trackSourceMap2[j, num5] = list2;
					ref bool ptr3 = ref this.c.TrackEnable[this.c.ReverseTrack[0]];
					bool[] trackEnable = this.c.TrackEnable;
					int num6 = this.c.ReverseTrack[1];
					bool flag = this.c.TrackEnable[this.c.ReverseTrack[1]];
					bool flag2 = this.c.TrackEnable[this.c.ReverseTrack[0]];
					ptr3 = flag;
					trackEnable[num6] = flag2;
					ptr3 = ref this.c.Mute[this.c.ReverseTrack[0]];
					bool[] mute = this.c.Mute;
					int num7 = this.c.ReverseTrack[1];
					flag2 = this.c.Mute[this.c.ReverseTrack[1]];
					flag = this.c.Mute[this.c.ReverseTrack[0]];
					ptr3 = flag2;
					mute[num7] = flag;
					ref int ptr4 = ref array[this.c.ReverseTrack[0]];
					int[] array2 = array;
					int num8 = this.c.ReverseTrack[1];
					num5 = array[this.c.ReverseTrack[1]];
					j = array[this.c.ReverseTrack[0]];
					ptr4 = num5;
					array2[num8] = j;
					this.TrackSourceIndex[this.c.SoundId, this.c.ReverseTrack[0]] = array[this.c.ReverseTrack[0]];
					this.TrackSourceIndex[this.c.SoundId, this.c.ReverseTrack[1]] = array[this.c.ReverseTrack[1]];
					this.TrackReverseApplied[this.c.SoundId] = true;
				}
				this.c.ParamInit();
				this.c.SetHeaderParameter();
				if (OutputMetaData && num3 == 0)
				{
					this.c.SetMetaData();
				}
				for (int k = ((num2 > 0) ? 1 : 0); k < 4; k++)
				{
					this.c.TrackIndex = k;
					this.c.LineBuffer = this.c.TrackBuffer[this.c.SoundId, k];
					this.c.CurrentSourceMap = ((this.c.TrackSourceMap[this.c.SoundId, k] == null) ? null : new List<MMLSourcePosition>(this.c.TrackSourceMap[this.c.SoundId, k]));
					this.c.ResetAutoRepeatBoundaries();
					this.c.StartTrack(this.c.LineBuffer != null);
					int address = this.c.Address;
					this.c.MarkAutoRepeatBoundary();
					if (this.c.LineBuffer != null)
					{
						this.c.LineBuffer = this.c.LineBuffer.TrimEnd();
						if (this.c.FastForwardTrack[k] && num2 == 0)
						{
							this.c.DataWrite(254);
							this.c.DataWrite(1);
						}
						if (k == 0)
						{
							if (num2 > 0)
							{
								this.c.Message = "効果音ではリズムトラックは使用できません。";
								this.c.ErrorString = "!S" + num2.ToString();
								return;
							}
							this.cr.Compile();
						}
						else
						{
							this.cm.AlignTrackIndex = array[k];
							this.cm.Compile();
							this.cm.AlignTrackIndex = -1;
						}
						if (this.c.Message != null && this.c.Message != "")
						{
							this.c.ErrorLine = this.c.LineNumber;
							break;
						}
						this.cm.FlushPendingRepeatFrameModAsRest(false);
						this.c.MarkAutoRepeatBoundary();
						int address2 = this.c.Address;
						this.c.EndTrack(true);
						int address3 = this.c.Address;
						this.ApplyAutoRepeatCompression(k, address, address2, address3);
					}
				}
				if (this.c.Message != null && this.c.Message != "")
				{
					return;
				}
				if (num2 == 0)
				{
					num = this.c.Address;
					this.c.BackupBGMData = new byte[28672];
					Array.Copy(this.c.BGMData, this.c.BackupBGMData, 28672);
					this.c.BGMData = new byte[28672];
					this.c.Address = 0;
				}
				else
				{
					this.c.StartAddress = this.c.Address;
				}
				num2++;
			}
			if (num2 > 1)
			{
				this.c.SEData = new byte[this.c.Address];
				Array.Copy(this.c.BGMData, this.c.SEData, this.c.Address);
				num2 = -1;
				foreach (int num9 in this.c.SEAddress)
				{
					num2++;
					if (num9 >= 0)
					{
						int num10 = num9 + 7;
						if (num10 < this.c.SEData.Length)
						{
							this.c.SEData[num10] = (byte)this.c.RegisterSoundPriority[num2];
						}
					}
				}
			}
			this.c.StartAddress = 0;
			this.c.BGMData = this.c.BackupBGMData;
			this.c.Address = num;
			this.cv.GenerateBinaryVoice();
			if (this.c.DefineAddressAbsolute > 0)
			{
				this.c.VoiceBinData = new byte[this.c.Address - num];
				Buffer.BlockCopy(this.c.BGMData, num, this.c.VoiceBinData, 0, this.c.Address - num);
				this.c.Address = num;
			}
			if (this.c.SEData != null && this.c.SEAddressAbsolute == 0)
			{
				if (this.c.Address + this.c.SEData.Length > this.c.BGMDataSize)
				{
					this.c.Message = "曲データと音色・効果音データの合計容量が " + this.c.BGMDataSize.ToString() + " バイトを超えました。";
					this.c.ErrorString = null;
					return;
				}
				Buffer.BlockCopy(this.c.SEData, 0, this.c.BGMData, this.c.Address, this.c.SEData.Length);
			}
			this.WriteSEAddress((this.c.SEAddressAbsolute > 0) ? this.c.SEAddressAbsolute : this.c.Address);
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00018B74 File Offset: 0x00016D74
		private void WriteSEAddress(int SEAddressOffset = 0)
		{
			if (this.c.SEUseAddress == null || this.c.SEUseAddress.Count == 0)
			{
				return;
			}
			int num = 0;
			foreach (int num2 in this.c.SEUseAddress)
			{
				this.c.LineNumber = this.c.SEUseLine[num];
				int num3 = this.c.SEUseNumber[num];
				this.c.ErrorParameter("!S", num3);
				if (num3 < 1 || num3 > 99)
				{
					this.c.RangeNotice("効果音の番号", 1, 99, null);
					break;
				}
				if (this.c.SEAddress[num3] < 0)
				{
					this.c.Message = num3.ToString() + " 番の効果音は定義されていません。";
					break;
				}
				int num4 = this.c.SEAddress[num3] + SEAddressOffset;
				this.c.BGMData[num2 + 1] = (byte)(num4 & 255);
				this.c.BGMData[num2 + 2] = (byte)(num4 / 256);
				num++;
			}
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00018CC4 File Offset: 0x00016EC4
		public void MacroDeploy(int TrackNumber = -1, int IndexOffset = 0, List<MMLSourcePosition> SourceMap = null)
		{
			string text = "";
			if (this.c.CheckNextCharacter(' ') || this.c.CheckNextCharacter('\t'))
			{
				this.c.Message = "$ と マクロ名との間に空白を含めることはできません。";
				this.c.ErrorString = "$";
				return;
			}
			int index = this.c.Index;
			for (;;)
			{
				this.c.Index++;
				char c = this.c.Read1(-1);
				if (this.c.Index >= this.c.LineBuffer.Length || c < '0' || (c > '9' && c < 'A') || c > 'Z')
				{
					break;
				}
				ReadOnlySpan<char> readOnlySpan = text;
				char c2 = c;
				text = readOnlySpan + new ReadOnlySpan<char>(ref c2);
			}
			this.c.UseMacroCount++;
			if (this.c.UseMacroCount > 5000)
			{
				this.c.Message = "1 曲中で使用できるマクロの回数は最大 5,000 回です。\nマクロ同士の相互呼び出しにより、無限ループしている恐れがあります。";
				this.c.ErrorString = "$";
				return;
			}
			if (text == "")
			{
				this.c.Message = "マクロ名が指定されていません。";
				this.c.ErrorString = "$";
				return;
			}
			string text2;
			this.c.MacroMML.TryGetValue(text, out text2);
			if (text2 == null)
			{
				this.c.Message = "マクロ名 「 " + text + " 」 は定義されていません。";
				this.c.ErrorString = "$" + text;
				return;
			}
			foreach (string text3 in this.ContainsMacroName(text2))
			{
				try
				{
					if (this.ContainsMacroName(this.c.MacroMML[text3]).Contains(text))
					{
						this.c.Message = string.Concat(new string[] { "マクロ $", text, " と $", text3, " の相互呼び出しにより、無限ループしています。" });
						this.c.ErrorString = "$" + text;
						break;
					}
				}
				catch
				{
				}
			}
			if (this.c.Message != null)
			{
				return;
			}
			int num = index + IndexOffset;
			int num2 = text.Length + 1;
			int length = text2.Length;
			int num3 = length - num2;
			this.c.LineBuffer = this.c.LineBuffer.Remove(index, num2).Insert(index, text2);
			if (SourceMap != null)
			{
				Compiler.<>c__DisplayClass29_0 CS$<>8__locals1 = new Compiler.<>c__DisplayClass29_0();
				Compiler.<>c__DisplayClass29_0 CS$<>8__locals2 = CS$<>8__locals1;
				bool flag;
				if (index >= 0 && index < SourceMap.Count)
				{
					flag = SourceMap.Skip<MMLSourcePosition>(index).Take<MMLSourcePosition>(Math.Min(num2, SourceMap.Count - index)).Any<MMLSourcePosition>((MMLSourcePosition SourcePosition) => SourcePosition.SelectionPlayTarget);
				}
				else
				{
					flag = false;
				}
				CS$<>8__locals2.SelectionPlayTarget = flag;
				SourceMap.RemoveRange(index, num2);
				List<MMLSourcePosition> list;
				if (this.c.MacroSourceMap.TryGetValue(text, out list))
				{
					List<MMLSourcePosition> list2 = list.Select<MMLSourcePosition, MMLSourcePosition>(delegate(MMLSourcePosition SourcePosition)
					{
						SourcePosition.SelectionPlayTarget = CS$<>8__locals1.SelectionPlayTarget;
						SourcePosition.MacroExpanded = true;
						return SourcePosition;
					}).ToList<MMLSourcePosition>();
					SourceMap.InsertRange(index, list2);
				}
				else
				{
					SourceMap.InsertRange(index, Enumerable.Repeat<MMLSourcePosition>(new MMLSourcePosition(0, 0, '\0', CS$<>8__locals1.SelectionPlayTarget, true), length));
				}
			}
			foreach (MacroPointData macroPointData in this.c.MacroPoint)
			{
				if (macroPointData.Contains(TrackNumber, num))
				{
					macroPointData.EndIndex += num3;
				}
			}
			MacroPointData macroPointData2 = new MacroPointData();
			macroPointData2.Name = text;
			macroPointData2.StartIndex = num;
			macroPointData2.EndIndex = num + length;
			macroPointData2.TrackNumber = TrackNumber;
			int num4;
			this.c.MacroLine.TryGetValue(text, out num4);
			macroPointData2.MacroLine = num4;
			this.c.MacroPoint.Add(macroPointData2);
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x000190E0 File Offset: 0x000172E0
		private List<string> ContainsMacroName(string MML)
		{
			MML += " ";
			List<string> list = new List<string>();
			string text = "";
			bool flag = false;
			for (int i = 0; i < MML.Length; i++)
			{
				char c = MML[i];
				if (c == '$' && !flag)
				{
					flag = true;
				}
				else
				{
					if (flag && (i > MML.Length - 1 || c < '0' || (c > '9' && c < 'A') || c > 'Z'))
					{
						list.Add(text);
						flag = false;
						text = "";
						if (c == '$')
						{
							i--;
							goto IL_0090;
						}
					}
					if (flag)
					{
						ReadOnlySpan<char> readOnlySpan = text;
						char c2 = c;
						text = readOnlySpan + new ReadOnlySpan<char>(ref c2);
					}
				}
				IL_0090:;
			}
			return list;
		}

		// Token: 0x04000037 RID: 55
		public Core c;

		// Token: 0x04000038 RID: 56
		public CompileVoice cv;

		// Token: 0x04000039 RID: 57
		public CompileMML cm;

		// Token: 0x0400003A RID: 58
		public CompileRhythmMML cr;

		// Token: 0x0400003B RID: 59
		private bool[] TrackReverseApplied = new bool[100];

		// Token: 0x0400003C RID: 60
		private int[,] TrackSourceIndex = new int[100, 4];

		// Token: 0x0200003C RID: 60
		private sealed class AutoRepeatSEReference
		{
			// Token: 0x040002A0 RID: 672
			public int RelativeOffset;

			// Token: 0x040002A1 RID: 673
			public int LineNumber;

			// Token: 0x040002A2 RID: 674
			public int SoundNumber;
		}

		// Token: 0x0200003D RID: 61
		private sealed class AutoRepeatUnit
		{
			// Token: 0x040002A3 RID: 675
			public byte[] Data;

			// Token: 0x040002A4 RID: 676
			public int RepeatMode;

			// Token: 0x040002A5 RID: 677
			public ulong Hash;

			// Token: 0x040002A6 RID: 678
			public List<Compiler.AutoRepeatSEReference> SEReferences = new List<Compiler.AutoRepeatSEReference>();
		}

		// Token: 0x0200003E RID: 62
		private sealed class AutoRepeatCandidate
		{
			// Token: 0x040002A7 RID: 679
			public int Start;

			// Token: 0x040002A8 RID: 680
			public int BodyUnitCount;

			// Token: 0x040002A9 RID: 681
			public int RepeatCount;

			// Token: 0x040002AA RID: 682
			public int BodyBytes;

			// Token: 0x040002AB RID: 683
			public int Saving;

			// Token: 0x040002AC RID: 684
			public int CoverageBytes;
		}

		// Token: 0x0200003F RID: 63
		private sealed class AutoRepeatRunContext
		{
			// Token: 0x0600018B RID: 395 RVA: 0x0002ADB8 File Offset: 0x00028FB8
			public AutoRepeatRunContext(List<Compiler.AutoRepeatUnit> Units)
			{
				this.Units = Units;
				this.BytePrefix = new int[Units.Count + 1];
				this.HashPrefix = new ulong[Units.Count + 1];
				this.HashPower = new ulong[Units.Count + 1];
				this.HashPower[0] = 1UL;
				for (int i = 0; i < Units.Count; i++)
				{
					this.BytePrefix[i + 1] = this.BytePrefix[i] + Units[i].Data.Length;
					this.HashPrefix[i + 1] = this.HashPrefix[i] * 11400714819323198485UL + Units[i].Hash + 11400714819323198485UL;
					this.HashPower[i + 1] = this.HashPower[i] * 11400714819323198485UL;
				}
			}

			// Token: 0x0600018C RID: 396 RVA: 0x0002AE96 File Offset: 0x00029096
			public ulong RangeHash(int Start, int Count)
			{
				return this.HashPrefix[Start + Count] - this.HashPrefix[Start] * this.HashPower[Count];
			}

			// Token: 0x0600018D RID: 397 RVA: 0x0002AEB4 File Offset: 0x000290B4
			public int LongestCommonPrefix(int A, int B, int MaxCount)
			{
				int i = 0;
				int num = MaxCount;
				while (i < num)
				{
					int num2 = i + (num - i + 1) / 2;
					if (this.RangeHash(A, num2) == this.RangeHash(B, num2))
					{
						i = num2;
					}
					else
					{
						num = num2 - 1;
					}
				}
				return i;
			}

			// Token: 0x040002AD RID: 685
			private const ulong HashBase = 11400714819323198485UL;

			// Token: 0x040002AE RID: 686
			public readonly List<Compiler.AutoRepeatUnit> Units;

			// Token: 0x040002AF RID: 687
			public readonly int[] BytePrefix;

			// Token: 0x040002B0 RID: 688
			private readonly ulong[] HashPrefix;

			// Token: 0x040002B1 RID: 689
			private readonly ulong[] HashPower;
		}
	}
}
