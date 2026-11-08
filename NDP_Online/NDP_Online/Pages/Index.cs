using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.CompilerServices;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using ndp;

namespace NDP_Online.Pages
{
	// Token: 0x02000009 RID: 9
	[Route("/")]
	public class Index : ComponentBase, IAsyncDisposable
	{
		// Token: 0x06000025 RID: 37 RVA: 0x000027E4 File Offset: 0x000009E4
		protected override void BuildRenderTree(RenderTreeBuilder __builder)
		{
			if (this.ShowSharedPlayDialog)
			{
				__builder.OpenElement(0, "div");
				__builder.AddAttribute(1, "class", "shared-play-backdrop");
				__builder.AddAttribute(2, "role", "presentation");
				__builder.AddAttribute(3, "style", "position:fixed;inset:0;z-index:10000;display:flex;align-items:center;justify-content:center;padding:20px;box-sizing:border-box;background:rgba(0,0,0,.72);");
				__builder.OpenElement(4, "div");
				__builder.AddAttribute(5, "class", "shared-play-dialog");
				__builder.AddAttribute(6, "role", "dialog");
				__builder.AddAttribute(7, "aria-modal", "true");
				__builder.AddAttribute(8, "aria-labelledby", "ndp-shared-title");
				__builder.AddAttribute(9, "style", "box-sizing:border-box;width:100%;max-width:480px;padding:24px;border:1px solid #5b708e;border-radius:12px;background:#222c3a;box-shadow:0 16px 50px rgba(0,0,0,.5);text-align:center;");
				__builder.AddMarkupContent(10, "<div class=\"shared-play-heading\" style=\"color:#b7c5d9;font-size:14px;margin-bottom:12px;\">共有されたMML</div>\n    ");
				__builder.OpenElement(11, "div");
				__builder.AddAttribute(12, "id", "ndp-shared-title");
				__builder.AddAttribute(13, "class", "shared-play-title");
				__builder.AddAttribute(14, "style", "font-size:22px;font-weight:700;color:#fff;overflow-wrap:anywhere;white-space:pre-wrap;margin:0 0 22px;max-height:36vh;overflow:auto;");
				__builder.AddContent(15, this.SharedTitle);
				__builder.CloseElement();
				__builder.AddMarkupContent(16, "\n    ");
				__builder.OpenElement(17, "div");
				__builder.AddAttribute(18, "class", "shared-play-actions");
				__builder.AddAttribute(19, "style", "display:flex;gap:12px;justify-content:center;");
				__builder.OpenElement(20, "button");
				__builder.AddAttribute<MouseEventArgs>(21, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, new Action(this.CloseSharedPlayDialog)));
				__builder.AddMarkupContent(22, "閉じる");
				__builder.CloseElement();
				__builder.AddMarkupContent(23, "\n      ");
				__builder.OpenElement(24, "button");
				__builder.AddAttribute(25, "class", "primary");
				__builder.AddAttribute<MouseEventArgs>(26, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, new Func<Task>(this.PlaySharedMml)));
				__builder.AddAttribute(27, "disabled", this.Busy);
				__builder.AddContent(28, "PLAY");
				__builder.CloseElement();
				__builder.CloseElement();
				__builder.CloseElement();
				__builder.CloseElement();
			}
			if (this.ShowMmlSaveDialog)
			{
				__builder.OpenElement(29, "div");
				__builder.AddAttribute(30, "class", "ndp-save-backdrop");
				__builder.AddAttribute(31, "role", "presentation");
				__builder.AddAttribute(32, "style", "position:fixed;inset:0;z-index:10010;display:flex;align-items:center;justify-content:center;padding:18px;box-sizing:border-box;background:rgba(0,0,0,.68);");
				__builder.AddAttribute<MouseEventArgs>(33, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, new Action(this.CancelMmlSave)));
				__builder.OpenElement(34, "div");
				__builder.AddAttribute(35, "class", "ndp-save-dialog");
				__builder.AddAttribute(36, "role", "dialog");
				__builder.AddAttribute(37, "aria-modal", "true");
				__builder.AddAttribute(38, "aria-labelledby", "ndp-save-title");
				__builder.AddAttribute(39, "style", "width:100%;max-width:420px;box-sizing:border-box;padding:22px;border:1px solid #5b708e;border-radius:10px;background:#222c3a;color:#f5f8ff;box-shadow:0 16px 50px rgba(0,0,0,.5);");
				__builder.AddEventStopPropagationAttribute(40, "onclick", true);
				__builder.AddMarkupContent(41, "<h2 id=\"ndp-save-title\">MMLファイルを保存</h2>\n    ");
				__builder.AddMarkupContent(42, "<label for=\"ndp-save-name\">ファイル名</label>\n    ");
				__builder.OpenElement(43, "input");
				__builder.AddAttribute(44, "id", "ndp-save-name");
				__builder.AddAttribute(45, "type", "text");
				__builder.AddAttribute(46, "maxlength", "255");
				__builder.AddAttribute<KeyboardEventArgs>(47, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, new Func<KeyboardEventArgs, Task>(this.SaveNameKeyDown)));
				__builder.AddAttribute(48, "value", BindConverter.FormatValue(this.SaveMmlFileName, null));
				__builder.AddAttribute<ChangeEventArgs>(49, "oninput", EventCallback.Factory.CreateBinder(this, delegate(string __value)
				{
					this.SaveMmlFileName = __value;
				}, this.SaveMmlFileName, null));
				__builder.SetUpdatesAttributeName("value");
				__builder.CloseElement();
				__builder.AddMarkupContent(50, "\n    ");
				__builder.OpenElement(51, "div");
				__builder.AddAttribute(52, "class", "ndp-save-actions");
				__builder.OpenElement(53, "button");
				__builder.AddAttribute(54, "type", "button");
				__builder.AddAttribute<MouseEventArgs>(55, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, new Action(this.CancelMmlSave)));
				__builder.AddMarkupContent(56, "キャンセル");
				__builder.CloseElement();
				__builder.AddMarkupContent(57, "\n      ");
				__builder.OpenElement(58, "button");
				__builder.AddAttribute(59, "type", "button");
				__builder.AddAttribute(60, "class", "primary");
				__builder.AddAttribute<MouseEventArgs>(61, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, new Func<Task>(this.ConfirmMmlSave)));
				__builder.AddAttribute(62, "disabled", string.IsNullOrWhiteSpace(this.SaveMmlFileName));
				__builder.AddMarkupContent(63, "保存");
				__builder.CloseElement();
				__builder.CloseElement();
				__builder.CloseElement();
				__builder.CloseElement();
			}
			__builder.OpenElement(64, "div");
			__builder.AddAttribute(65, "class", "app");
			__builder.AddMarkupContent(66, "<header><div><h1>NDP Online — MML Editor</h1><p>ブラウザでMML編集・PSG再生</p></div><small>0.5.13</small></header>\n");
			__builder.OpenElement(67, "div");
			__builder.AddAttribute(68, "class", "editor-shell " + (this.WrapEditor ? "ndp-wrap" : ""));
			__builder.AddMarkupContent(69, "<pre id=\"ndp-highlight\" class=\"editor-highlight\" aria-hidden=\"true\"></pre>");
			__builder.OpenElement(70, "textarea");
			__builder.AddAttribute(71, "id", "ndp-editor");
			__builder.AddAttribute(72, "class", "editor");
			__builder.AddAttribute(73, "aria-label", "MMLエディタ");
			__builder.AddAttribute(74, "spellcheck", "false");
			__builder.AddAttribute(75, "value", BindConverter.FormatValue(this.Mml, null));
			__builder.AddAttribute<ChangeEventArgs>(76, "oninput", EventCallback.Factory.CreateBinder(this, delegate(string __value)
			{
				this.Mml = __value;
			}, this.Mml, null));
			__builder.SetUpdatesAttributeName("value");
			__builder.CloseElement();
			__builder.CloseElement();
			__builder.AddMarkupContent(77, "\n");
			__builder.OpenElement(78, "div");
			__builder.AddAttribute(79, "class", "bar transport");
			__builder.AddMarkupContent(80, "<label for=\"ndp-seek\">再生位置</label>\n  ");
			__builder.OpenElement(81, "div");
			__builder.AddAttribute(82, "class", "ndp-seek-bar");
			__builder.AddMarkupContent(83, "<input id=\"ndp-seek\" aria-label=\"再生位置\" type=\"range\" value=\"0\" min=\"0\" max=\"100\" step=\"0.1\" onchange=\"ndpEditor.seekFromInput(this.value)\">");
			using (List<Index.JumpMarker>.Enumerator enumerator = this.JumpMarkers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Index.JumpMarker marker2 = enumerator.Current;
					__builder.OpenElement(84, "button");
					__builder.AddAttribute(85, "class", "ndp-seek-marker");
					__builder.AddAttribute(86, "title", marker2.Label + " (" + Index.FormatMarkerTime(marker2.Seconds) + ")");
					__builder.AddAttribute(87, "aria-label", marker2.Label + " にジャンプ");
					__builder.AddAttribute(88, "style", "left:" + marker2.Percent.ToString(CultureInfo.InvariantCulture) + "%");
					__builder.AddAttribute<MouseEventArgs>(89, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => this.SeekToMarker(marker2)));
					__builder.AddAttribute(90, "disabled", this.Busy);
					__builder.CloseElement();
				}
			}
			__builder.CloseElement();
			__builder.AddMarkupContent(91, "\n  ");
			__builder.OpenElement(92, "span");
			__builder.AddAttribute(93, "id", "ndp-playtime");
			__builder.AddAttribute(94, "class", "time");
			__builder.AddContent(95, this.Duration);
			__builder.CloseElement();
			__builder.CloseElement();
			if (this.JumpMarkers.Count > 0)
			{
				__builder.OpenElement(96, "div");
				__builder.AddAttribute(97, "class", "ndp-marker-region");
				__builder.AddAttribute(98, "aria-label", "ジャンプマーカー");
				__builder.OpenElement(99, "div");
				__builder.AddAttribute(100, "class", "ndp-marker-buttons");
				using (List<Index.JumpMarker>.Enumerator enumerator = this.JumpMarkers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Index.JumpMarker marker = enumerator.Current;
						__builder.OpenElement(101, "button");
						__builder.AddAttribute(102, "title", marker.Label + " (" + Index.FormatMarkerTime(marker.Seconds) + ")");
						__builder.AddAttribute<MouseEventArgs>(103, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => this.SeekToMarker(marker)));
						__builder.AddAttribute(104, "disabled", this.Busy);
						__builder.OpenElement(105, "span");
						__builder.AddAttribute(106, "class", "ndp-marker-name");
						__builder.AddContent(107, marker.Label);
						__builder.CloseElement();
						__builder.AddMarkupContent(108, "\n          ");
						__builder.OpenElement(109, "span");
						__builder.AddAttribute(110, "class", "ndp-marker-time");
						__builder.AddContent(111, Index.FormatMarkerTime(marker.Seconds));
						__builder.CloseElement();
						__builder.CloseElement();
					}
				}
				__builder.CloseElement();
				__builder.CloseElement();
			}
			__builder.OpenElement(112, "div");
			__builder.AddAttribute(113, "class", "bar controls");
			__builder.OpenElement(114, "button");
			__builder.AddAttribute(115, "class", "primary");
			__builder.AddAttribute<MouseEventArgs>(116, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => this.CompileMml(0, false)));
			__builder.AddAttribute(117, "disabled", this.Busy);
			__builder.AddContent(118, this.Busy ? "コンパイル中…" : "コンパイル・再生");
			__builder.AddMarkupContent(119, "<span class=\"ndp-desktop-shortcut\"> (F5)</span>");
			__builder.CloseElement();
			__builder.AddMarkupContent(120, "\n");
			__builder.OpenElement(121, "button");
			__builder.AddAttribute<MouseEventArgs>(122, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => this.PlayFromCursor(false)));
			__builder.AddAttribute(123, "disabled", this.Busy);
			__builder.AddMarkupContent(124, "カーソルから再生");
			__builder.AddMarkupContent(125, "<span class=\"ndp-desktop-shortcut\"> (Shift+F5)</span>");
			__builder.CloseElement();
			__builder.AddMarkupContent(126, "\n");
			__builder.OpenElement(127, "button");
			__builder.AddAttribute<MouseEventArgs>(128, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, new Func<Task>(this.PlayFromBeginning)));
			__builder.AddAttribute(129, "disabled", this.Kss == null || this.Busy);
			__builder.AddMarkupContent(130, "先頭から再生");
			__builder.CloseElement();
			__builder.AddMarkupContent(131, "\n");
			__builder.OpenElement(132, "button");
			__builder.AddAttribute<MouseEventArgs>(133, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, new Func<Task>(this.TogglePause)));
			__builder.AddAttribute(134, "disabled", this.Kss == null);
			__builder.AddContent(135, this.IsPaused ? "再開" : "一時停止");
			__builder.CloseElement();
			__builder.AddMarkupContent(136, "\n");
			__builder.OpenElement(137, "button");
			__builder.AddAttribute<MouseEventArgs>(138, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, new Func<Task>(this.Stop)));
			__builder.AddMarkupContent(139, "停止");
			__builder.AddMarkupContent(140, "<span class=\"ndp-desktop-shortcut\"> (Esc / F3)</span>");
			__builder.CloseElement();
			__builder.AddMarkupContent(141, "\n");
			__builder.AddMarkupContent(142, "<button type=\"button\" onclick=\"window.ndpEditor.openCaretHelp()\">ヘルプ<span class=\"ndp-desktop-shortcut\"> (F1)</span></button>\n");
			__builder.OpenElement(143, "label");
			__builder.AddAttribute(144, "class", "ndp-file-open");
			__builder.AddAttribute(145, "title", "MML・テキストファイルを開く");
			__builder.AddMarkupContent(146, "MMLファイルを開く");
			__builder.OpenComponent<InputFile>(147);
			__builder.AddComponentParameter(148, "OnChange", RuntimeHelpers.TypeCheck<EventCallback<InputFileChangeEventArgs>>(EventCallback.Factory.Create<InputFileChangeEventArgs>(this, new Func<InputFileChangeEventArgs, Task>(this.OpenMmlFile))));
			__builder.AddComponentParameter(149, "accept", ".mml,.txt,text/plain");
			__builder.AddComponentParameter(150, "disabled", this.Busy);
			__builder.CloseComponent();
			__builder.CloseElement();
			__builder.AddMarkupContent(151, "\n");
			__builder.OpenElement(152, "button");
			__builder.AddAttribute<MouseEventArgs>(153, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, new Action(this.DownloadMml)));
			__builder.AddAttribute(154, "disabled", this.Busy);
			__builder.AddMarkupContent(155, "MMLファイルを保存");
			__builder.CloseElement();
			__builder.AddMarkupContent(156, "\n");
			__builder.OpenElement(157, "button");
			__builder.AddAttribute<MouseEventArgs>(158, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, new Func<Task>(this.ShareMml)));
			__builder.AddAttribute(159, "disabled", this.Busy);
			__builder.AddMarkupContent(160, "共有URLを作成");
			__builder.CloseElement();
			__builder.AddMarkupContent(161, "\n");
			__builder.OpenElement(162, "button");
			__builder.AddAttribute<MouseEventArgs>(163, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, new Func<Task>(this.DownloadKss)));
			__builder.AddAttribute(164, "disabled", this.Kss == null);
			__builder.AddMarkupContent(165, "KSS保存");
			__builder.CloseElement();
			__builder.AddMarkupContent(166, "\n");
			__builder.OpenElement(167, "button");
			__builder.AddAttribute<MouseEventArgs>(168, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, new Func<Task>(this.DownloadBinary)));
			__builder.AddAttribute(169, "disabled", this.Binary == null);
			__builder.AddMarkupContent(170, "NDP保存");
			__builder.CloseElement();
			__builder.AddMarkupContent(171, "\n");
			__builder.OpenElement(172, "button");
			__builder.AddAttribute(173, "type", "button");
			__builder.AddAttribute<MouseEventArgs>(174, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, new Func<Task>(this.ResetMml)));
			__builder.AddAttribute(175, "disabled", this.Busy);
			__builder.AddAttribute(176, "style", "background:#b42318;color:#fff;border-color:#b42318;");
			__builder.AddMarkupContent(177, "MMLを初期化");
			__builder.CloseElement();
			__builder.CloseElement();
			if (this.Busy && this.Compiling)
			{
				__builder.AddMarkupContent(178, "<div class=\"ndp-compile-indicator\" role=\"status\" aria-live=\"polite\"><span class=\"ndp-compile-spinner\" aria-hidden=\"true\"></span>\n  <span>コンパイル中です。完了するまでお待ちください…</span></div>");
			}
			__builder.OpenElement(179, "div");
			__builder.AddAttribute(180, "id", "ndp-status");
			__builder.AddAttribute(181, "class", "info " + (this.Error ? "error" : "ok"));
			__builder.AddAttribute(182, "role", "status");
			__builder.AddContent(183, this.Status);
			__builder.CloseElement();
			__builder.AddMarkupContent(184, "\n");
			__builder.AddMarkupContent(185, "<details class=\"ndp-shortcut-guide\" aria-label=\"キーボードショートカット\"><summary class=\"ndp-shortcut-heading\">キーボードショートカット</summary>\n  <table class=\"ndp-shortcut-table\"><thead><tr><th scope=\"col\">キー</th><th scope=\"col\">動作</th></tr></thead>\n    <tbody><tr><td><kbd>F1</kbd></td><td>カーソル位置のヘルプ</td></tr>\n      <tr><td><kbd>F2</kbd> / <kbd>F4</kbd></td><td>1秒戻し／1秒送り（長押し可）</td></tr>\n      <tr><td><kbd>F5</kbd></td><td>コンパイル・再生（J0）</td></tr>\n      <tr><td><kbd>Shift+F5</kbd></td><td>カーソルから再生</td></tr>\n      <tr><td><kbd>Ctrl+F5</kbd></td><td>カーソルからソロ再生</td></tr>\n      <tr><td><kbd>F6～F10</kbd></td><td>J1～J5から再生</td></tr>\n      <tr><td><kbd>Ctrl+F6～F10</kbd></td><td>J1～J5からソロ再生</td></tr>\n      <tr><td><kbd>Esc</kbd> / <kbd>F3</kbd></td><td>演奏停止</td></tr>\n      <tr><td><kbd>Tab</kbd></td><td>タブ入力（エディタ内）</td></tr></tbody></table></details>\n");
			__builder.OpenElement(186, "div");
			__builder.AddAttribute(187, "class", "bar controls");
			__builder.OpenElement(188, "label");
			__builder.OpenElement(189, "input");
			__builder.AddAttribute(190, "type", "checkbox");
			__builder.AddAttribute(191, "checked", BindConverter.FormatValue(this.OutputMetaData, null));
			__builder.AddAttribute<ChangeEventArgs>(192, "onchange", EventCallback.Factory.CreateBinder(this, delegate(bool __value)
			{
				this.OutputMetaData = __value;
			}, this.OutputMetaData, null));
			__builder.SetUpdatesAttributeName("checked");
			__builder.CloseElement();
			__builder.AddMarkupContent(193, " メタデータ出力");
			__builder.CloseElement();
			__builder.AddMarkupContent(194, "\n");
			__builder.OpenElement(195, "label");
			__builder.OpenElement(196, "input");
			__builder.AddAttribute(197, "type", "checkbox");
			__builder.AddAttribute(198, "checked", this.TrackHighlight);
			__builder.AddAttribute<ChangeEventArgs>(199, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, new Func<ChangeEventArgs, Task>(this.ToggleTrackHighlight)));
			__builder.CloseElement();
			__builder.AddMarkupContent(200, " 再生ハイライト");
			__builder.CloseElement();
			__builder.AddMarkupContent(201, "\n");
			__builder.OpenElement(202, "label");
			__builder.OpenElement(203, "input");
			__builder.AddAttribute(204, "type", "checkbox");
			__builder.AddAttribute(205, "checked", this.AutoScroll);
			__builder.AddAttribute<ChangeEventArgs>(206, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, new Func<ChangeEventArgs, Task>(this.ToggleAutoScroll)));
			__builder.CloseElement();
			__builder.AddMarkupContent(207, " 自動スクロール");
			__builder.CloseElement();
			__builder.AddMarkupContent(208, "\n");
			__builder.OpenElement(209, "label");
			__builder.OpenElement(210, "input");
			__builder.AddAttribute(211, "type", "checkbox");
			__builder.AddAttribute(212, "checked", this.WrapEditor);
			__builder.AddAttribute<ChangeEventArgs>(213, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, new Func<ChangeEventArgs, Task>(this.ToggleWrapEditor)));
			__builder.CloseElement();
			__builder.AddMarkupContent(214, " 行を折り返す");
			__builder.CloseElement();
			__builder.CloseElement();
			if (!string.IsNullOrEmpty(this.ShareUrl))
			{
				__builder.OpenElement(215, "div");
				__builder.AddAttribute(216, "id", "ndp-share-result");
				__builder.AddAttribute(217, "class", "share-result");
				__builder.AddMarkupContent(218, "<label for=\"ndp-share-url\">共有URL</label>");
				__builder.OpenElement(219, "input");
				__builder.AddAttribute(220, "id", "ndp-share-url");
				__builder.AddAttribute(221, "readonly");
				__builder.AddAttribute(222, "value", this.ShareUrl);
				__builder.AddAttribute(223, "aria-label", "共有URL");
				__builder.CloseElement();
				__builder.OpenElement(224, "button");
				__builder.AddAttribute<MouseEventArgs>(225, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, new Func<Task>(this.CopyShareUrl)));
				__builder.AddMarkupContent(226, "URLをコピー");
				__builder.CloseElement();
				__builder.OpenElement(227, "button");
				__builder.AddAttribute<MouseEventArgs>(228, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, new Func<Task>(this.TweetShare)));
				__builder.AddMarkupContent(229, "Xに投稿");
				__builder.CloseElement();
				__builder.CloseElement();
			}
			__builder.AddMarkupContent(230, "<p>コンパイルに成功すると自動再生します。エラーの場合は該当行を選択します（正確な列位置が不明な場合は行全体を選択）。</p>");
			__builder.CloseElement();
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00003CA0 File Offset: 0x00001EA0
		private void UpdateJumpMarkers()
		{
			this.JumpMarkers.Clear();
			this.HasJ0 = false;
			this.J0Seconds = 0.0;
			Compiler compiler = this.Compiler;
			Index.<>c__DisplayClass30_0 CS$<>8__locals1;
			CS$<>8__locals1.c = ((compiler != null) ? compiler.c : null);
			if (CS$<>8__locals1.c == null)
			{
				return;
			}
			this.AudioDuration = Math.Max(0.0, CS$<>8__locals1.c.MusicToRealTime((double)CS$<>8__locals1.c.TotalFrame / 59.94080511738504));
			this.HasJ0 = Index.<UpdateJumpMarkers>g__IsDefined|30_0(0, ref CS$<>8__locals1);
			if (this.HasJ0)
			{
				this.J0Seconds = this.ConvertJumpTime(CS$<>8__locals1.c.JumpStartTime[0]);
			}
			for (int i = 1; i <= 5; i++)
			{
				if (Index.<UpdateJumpMarkers>g__IsDefined|30_0(i, ref CS$<>8__locals1))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 1);
					defaultInterpolatedStringHandler.AppendLiteral("J");
					defaultInterpolatedStringHandler.AppendFormatted<int>(i);
					this.AddJumpMarker(defaultInterpolatedStringHandler.ToStringAndClear(), CS$<>8__locals1.c.JumpStartTime[i]);
				}
			}
			foreach (KeyValuePair<string, double> keyValuePair in CS$<>8__locals1.c.SeekMarker.OrderBy<KeyValuePair<string, double>, double>((KeyValuePair<string, double> entry) => entry.Value))
			{
				this.AddJumpMarker(keyValuePair.Key, keyValuePair.Value);
			}
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00003E18 File Offset: 0x00002018
		private double ConvertJumpTime(double musicSeconds)
		{
			Compiler compiler = this.Compiler;
			Core core = ((compiler != null) ? compiler.c : null);
			if (core == null)
			{
				return 0.0;
			}
			return Math.Max(0.0, Math.Min(this.AudioDuration, core.MusicToRealTime(Math.Max(0.0, musicSeconds + core.JumpOffset))));
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00003E7C File Offset: 0x0000207C
		private void AddJumpMarker(string label, double musicSeconds)
		{
			double num = this.ConvertJumpTime(musicSeconds);
			this.JumpMarkers.Add(new Index.JumpMarker
			{
				Label = label,
				Seconds = num,
				Percent = ((this.AudioDuration > 0.0) ? (100.0 * num / this.AudioDuration) : 0.0)
			});
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00003EE4 File Offset: 0x000020E4
		private static string FormatMarkerTime(double seconds)
		{
			if (double.IsNaN(seconds) || double.IsInfinity(seconds))
			{
				seconds = 0.0;
			}
			seconds = Math.Max(0.0, seconds);
			int num = (int)Math.Floor(seconds / 60.0);
			double num2 = seconds - (double)(num * 60);
			return string.Format(CultureInfo.InvariantCulture, "{0:00}:{1:00.0}", num, num2);
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00003F54 File Offset: 0x00002154
		private async Task SeekToMarker(Index.JumpMarker marker)
		{
			if (!this.Busy && this.Kss != null)
			{
				try
				{
					ValueTaskAwaiter<JsonElement> valueTaskAwaiter = this.JS.InvokeAsync<JsonElement>("ndpPlayer.getState", Array.Empty<object>()).GetAwaiter();
					if (!valueTaskAwaiter.IsCompleted)
					{
						await valueTaskAwaiter;
						ValueTaskAwaiter<JsonElement> valueTaskAwaiter2;
						valueTaskAwaiter = valueTaskAwaiter2;
						valueTaskAwaiter2 = default(ValueTaskAwaiter<JsonElement>);
					}
					JsonElement jsonElement;
					bool hasPlayer = valueTaskAwaiter.GetResult().TryGetProperty("loaded", out jsonElement) && jsonElement.GetBoolean();
					await this.JS.InvokeVoidAsync("ndpEditor.resetPlaybackTimeline", new object[] { marker.Seconds });
					if (hasPlayer)
					{
						await this.JS.InvokeVoidAsync("ndpPlayer.seek", new object[] { marker.Seconds });
					}
					else
					{
						await this.JS.InvokeVoidAsync("ndpPlayer.unlock", Array.Empty<object>());
						await this.JS.InvokeVoidAsync("ndpPlayer.playFrom", new object[]
						{
							Convert.ToBase64String(this.Kss),
							this.AudioDuration,
							this.Compiler.c.InfinityLoop,
							marker.Seconds
						});
					}
					this.IsPaused = false;
					this.Error = false;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 2);
					defaultInterpolatedStringHandler.AppendFormatted(marker.Label);
					defaultInterpolatedStringHandler.AppendLiteral(" にジャンプ：");
					defaultInterpolatedStringHandler.AppendFormatted<double>(marker.Seconds, "F2");
					defaultInterpolatedStringHandler.AppendLiteral("秒");
					this.Status = defaultInterpolatedStringHandler.ToStringAndClear();
				}
				catch (Exception ex)
				{
					this.Error = true;
					this.Status = "ジャンプエラー: " + ex.Message;
				}
			}
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00003FA0 File Offset: 0x000021A0
		protected override async Task OnInitializedAsync()
		{
			try
			{
				byte[] array = await this.Http.GetByteArrayAsync("NDP.BIN");
				this.Driver = array;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(17, 1);
				defaultInterpolatedStringHandler.AppendLiteral("準備完了：ドライバ ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(this.Driver.Length);
				defaultInterpolatedStringHandler.AppendLiteral(" bytes。");
				this.Status = defaultInterpolatedStringHandler.ToStringAndClear();
			}
			catch (Exception ex)
			{
				this.Error = true;
				this.Status = "ドライバの読み込みに失敗: " + ex.Message;
			}
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00003FE4 File Offset: 0x000021E4
		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if (this.Error && !string.IsNullOrEmpty(this.Status) && this.LastScrolledError != this.Status)
			{
				this.LastScrolledError = this.Status;
				await this.JS.InvokeVoidAsync("ndpEditor.scrollToError", Array.Empty<object>());
			}
			else if (!this.Error)
			{
				this.LastScrolledError = null;
			}
			if (firstRender)
			{
				await this.JS.InvokeVoidAsync("ndpEditor.init", Array.Empty<object>());
				this.WrapEditor = await this.JS.InvokeAsync<bool>("ndpEditor.getWordWrap", Array.Empty<object>());
				await base.InvokeAsync(new Action(base.StateHasChanged));
				this.shortcutReference = DotNetObjectReference.Create<Index>(this);
				await this.JS.InvokeVoidAsync("ndpEditor.bindPlaybackShortcuts", new object[] { this.shortcutReference });
				string text = new Uri(this.Navigation.Uri).Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault<string>((string part) => part.StartsWith("id=", StringComparison.OrdinalIgnoreCase));
				if (text != null)
				{
					await this.LoadSharedMml(Uri.UnescapeDataString(text.Substring(3).Replace('+', ' ')));
				}
			}
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00004030 File Offset: 0x00002230
		[JSInvokable]
		public async Task HandlePlaybackShortcut(string action)
		{
			object obj = null;
			int num = 0;
			try
			{
				if (action == "stop")
				{
					if (this.ShowSharedPlayDialog)
					{
						this.ShowSharedPlayDialog = false;
					}
					else
					{
						await this.Stop();
					}
				}
				else if (!this.Busy && !this.ShowSharedPlayDialog)
				{
					int num2;
					int num3;
					if (action == "compile")
					{
						await this.CompileMml(0, false);
					}
					else if (action == "cursor")
					{
						await this.PlayFromCursor(false);
					}
					else if (action == "solo")
					{
						await this.PlayFromCursor(true);
					}
					else if (action.StartsWith("jump-solo:", StringComparison.Ordinal) && int.TryParse(action.Substring(10), out num2) && num2 >= 1 && num2 <= 5)
					{
						await this.CompileMml(num2, true);
					}
					else if (action.StartsWith("jump:", StringComparison.Ordinal) && int.TryParse(action.Substring(5), out num3) && num3 >= 1 && num3 <= 5)
					{
						await this.CompileMml(num3, false);
					}
					goto IL_0379;
				}
				num = 1;
			}
			catch (object obj)
			{
			}
			IL_0379:
			await base.InvokeAsync(new Action(base.StateHasChanged));
			object obj2 = obj;
			if (obj2 != null)
			{
				Exception ex = obj2 as Exception;
				if (ex == null)
				{
					throw obj2;
				}
				ExceptionDispatchInfo.Capture(ex).Throw();
			}
			if (num != 1)
			{
				obj = null;
			}
		}

		// Token: 0x0600002E RID: 46 RVA: 0x0000407C File Offset: 0x0000227C
		public async ValueTask DisposeAsync()
		{
			try
			{
				await this.JS.InvokeVoidAsync("ndpEditor.unbindPlaybackShortcuts", Array.Empty<object>());
			}
			catch (JSDisconnectedException)
			{
			}
			IDisposable disposable = this.shortcutReference;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}

		// Token: 0x0600002F RID: 47 RVA: 0x000040C0 File Offset: 0x000022C0
		private async Task LoadSharedMml(string id)
		{
			if (id.Length == 16)
			{
				if (id.All<char>((char ch) => char.IsLetterOrDigit(ch) || ch == '-' || ch == '_'))
				{
					try
					{
						HttpResponseMessage httpResponseMessage = await this.Http.GetAsync("api/load.php?id=" + Uri.EscapeDataString(id));
						if (!httpResponseMessage.IsSuccessStatusCode)
						{
							this.Error = true;
							this.Status = "共有MMLを取得できません（HTTP " + ((int)httpResponseMessage.StatusCode).ToString() + "）。";
						}
						else
						{
							string content = await httpResponseMessage.Content.ReadAsStringAsync();
							await this.JS.InvokeVoidAsync("ndpEditor.loadSharedMml", new object[] { content });
							this.Mml = content;
							this.ShareUrl = null;
							this.SharedTitle = Index.GetSharedTitle(content);
							this.ShowSharedPlayDialog = true;
							this.Error = false;
							this.Status = "共有MMLを読み込みました。PLAYを押すとコンパイル・再生します。";
							await base.InvokeAsync(new Action(base.StateHasChanged));
							content = null;
						}
					}
					catch (Exception ex)
					{
						this.Error = true;
						this.Status = "共有MMLの読み込みに失敗: " + ex.Message;
					}
					return;
				}
			}
			this.Error = true;
			this.Status = "共有IDが正しくありません。";
		}

		// Token: 0x06000030 RID: 48 RVA: 0x0000410C File Offset: 0x0000230C
		private static string GetSharedTitle(string mml)
		{
			string[] array = mml.Split('\n', StringSplitOptions.None);
			int i = 0;
			while (i < array.Length)
			{
				string text = array[i].TrimStart(new char[] { ' ', '\t' }).TrimEnd('\r');
				if (text.StartsWith("#TITLE", StringComparison.OrdinalIgnoreCase) && (text.Length <= 6 || char.IsWhiteSpace(text[6]) || text[6] == '"'))
				{
					string text2 = text.Substring(6).Trim();
					if (text2.Length > 0 && text2[0] == '"')
					{
						int num = text2.IndexOf('"', 1);
						text2 = ((num >= 0) ? text2.Substring(1, num - 1) : text2.Substring(1));
					}
					if (!string.IsNullOrWhiteSpace(text2))
					{
						return text2;
					}
					return "NO TITLE";
				}
				else
				{
					i++;
				}
			}
			return "NO TITLE";
		}

		// Token: 0x06000031 RID: 49 RVA: 0x000041E2 File Offset: 0x000023E2
		private void CloseSharedPlayDialog()
		{
			this.ShowSharedPlayDialog = false;
		}

		// Token: 0x06000032 RID: 50 RVA: 0x000041EC File Offset: 0x000023EC
		private async Task PlaySharedMml()
		{
			this.ShowSharedPlayDialog = false;
			await this.CompileMml(0, false);
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00004230 File Offset: 0x00002430
		private async Task ShareMml()
		{
			if (!this.Busy)
			{
				this.Busy = true;
				try
				{
					string text = await this.JS.InvokeAsync<string>("ndpShare.create", new object[] { this.Mml });
					this.ShareUrl = text;
					this.Status = "共有URLを発行しました。このURLから曲の再生やMMLの閲覧ができます。";
					this.Error = false;
					await base.InvokeAsync(new Action(base.StateHasChanged));
					await this.JS.InvokeVoidAsync("ndpWeb.scrollToId", new object[] { "ndp-share-result" });
				}
				catch (Exception ex)
				{
					this.Error = true;
					this.Status = "共有に失敗: " + ex.Message;
				}
				finally
				{
					this.Busy = false;
				}
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00004274 File Offset: 0x00002474
		private Task CopyShareUrl()
		{
			return this.JS.InvokeVoidAsync("ndpShare.copy", new object[] { this.ShareUrl }).AsTask();
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000042A8 File Offset: 0x000024A8
		private Task TweetShare()
		{
			return this.JS.InvokeVoidAsync("ndpShare.tweet", new object[]
			{
				this.ShareUrl,
				Index.GetSharedTitle(this.Mml)
			}).AsTask();
		}

		// Token: 0x06000036 RID: 54 RVA: 0x000042EC File Offset: 0x000024EC
		private string GetTrackFrameSummary()
		{
			Compiler compiler = this.Compiler;
			Core core = ((compiler != null) ? compiler.c : null);
			if (core == null)
			{
				return "";
			}
			List<string> list = new List<string>();
			string[] array = new string[] { "R", "1", "2", "3" };
			for (int i = 0; i < array.Length; i++)
			{
				int num = core.BeforeLoopFrame[i, 0];
				int num2 = core.TrackFrame[i, 0];
				if (num >= 0 && num2 > 0)
				{
					List<string> list2 = list;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 4);
					defaultInterpolatedStringHandler.AppendLiteral("トラック");
					defaultInterpolatedStringHandler.AppendFormatted(array[i]);
					defaultInterpolatedStringHandler.AppendLiteral("：");
					defaultInterpolatedStringHandler.AppendFormatted<int>(num + num2);
					defaultInterpolatedStringHandler.AppendLiteral(" frames（ループ開始 ");
					defaultInterpolatedStringHandler.AppendFormatted<int>(num);
					defaultInterpolatedStringHandler.AppendLiteral(" / ループ長 ");
					defaultInterpolatedStringHandler.AppendFormatted<int>(num2);
					defaultInterpolatedStringHandler.AppendLiteral("）");
					list2.Add(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				else
				{
					List<string> list3 = list;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(12, 2);
					defaultInterpolatedStringHandler2.AppendLiteral("トラック");
					defaultInterpolatedStringHandler2.AppendFormatted(array[i]);
					defaultInterpolatedStringHandler2.AppendLiteral("：");
					defaultInterpolatedStringHandler2.AppendFormatted<int>(Math.Max(num2, 0));
					defaultInterpolatedStringHandler2.AppendLiteral(" frames");
					list3.Add(defaultInterpolatedStringHandler2.ToStringAndClear());
				}
			}
			return string.Join("\n", list);
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00004460 File Offset: 0x00002660
		private async Task CompileMml(int jumpPoint = 0, bool solo = false)
		{
			if (!this.Busy)
			{
				this.Busy = true;
				this.Compiling = true;
				this.Error = false;
				this.Status = "コンパイルを開始しています…";
				this.Kss = null;
				this.Binary = null;
				this.JumpMarkers.Clear();
				this.HasJ0 = false;
				await base.InvokeAsync(new Action(base.StateHasChanged));
				try
				{
					await this.JS.InvokeVoidAsync("ndpEditor.clearTracking", Array.Empty<object>());
					await this.JS.InvokeVoidAsync("ndpPlayer.unlock", Array.Empty<object>());
					await this.JS.InvokeVoidAsync("ndpPlayer.stop", Array.Empty<object>());
					this.IsPaused = false;
					await Task.Delay(80);
					this.Compiler = new Compiler();
					CompileOptions compileOptions = new CompileOptions(PresetBank.Voices, PresetBank.Rhythm, this.OutputMetaData);
					if (!((jumpPoint > 0 && solo) ? this.Compiler.Compile(this.Mml, compileOptions, -1 - jumpPoint, true, false, null, -1, -1, -1, -1) : this.Compiler.Compile(this.Mml, compileOptions, -1, false, false, null, -1, -1, -1, -1)) || this.Compiler.c.Message != null)
					{
						this.Error = true;
						int errorLine = this.Compiler.c.ErrorLine;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 2);
						defaultInterpolatedStringHandler.AppendLiteral("コンパイルエラー（行 ");
						defaultInterpolatedStringHandler.AppendFormatted<int>(errorLine);
						defaultInterpolatedStringHandler.AppendLiteral("）：");
						defaultInterpolatedStringHandler.AppendFormatted(this.Compiler.c.Message);
						this.Status = defaultInterpolatedStringHandler.ToStringAndClear();
						if (!string.IsNullOrEmpty(this.Compiler.c.ErrorString))
						{
							this.Status = this.Status + "\nエラー対象「" + this.Compiler.c.ErrorString + "」";
						}
						if (errorLine > 0)
						{
							await this.JS.InvokeVoidAsync("ndpPlayer.focusError", new object[]
							{
								errorLine,
								this.Compiler.c.ErrorString
							});
						}
					}
					else
					{
						this.Kss = KssBuilder.Build(this.Driver, this.Compiler.c);
						int length = this.Compiler.c.Address + ((this.Compiler.c.SEData != null && this.Compiler.c.SEAddressAbsolute == 0) ? this.Compiler.c.SEData.Length : 0);
						this.Binary = new byte[length];
						Array.Copy(this.Compiler.c.BGMData, this.Binary, length);
						this.Duration = "00:00 / " + TimeSpan.FromSeconds((double)this.Compiler.c.TotalFrame / 59.94080511738504).ToString("mm\\:ss");
						this.UpdateJumpMarkers();
						try
						{
							await this.SetupPlaybackTracking();
						}
						catch (Exception ex)
						{
							Console.Error.WriteLine("Playback tracking setup failed: " + ((ex != null) ? ex.ToString() : null));
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(32, 4);
						defaultInterpolatedStringHandler2.AppendLiteral("コンパイル成功：演奏データ ");
						defaultInterpolatedStringHandler2.AppendFormatted<int>(length);
						defaultInterpolatedStringHandler2.AppendLiteral(" bytes / ");
						defaultInterpolatedStringHandler2.AppendFormatted<int>(this.Compiler.c.TotalFrame);
						defaultInterpolatedStringHandler2.AppendLiteral(" frames。");
						defaultInterpolatedStringHandler2.AppendFormatted(this.Compiler.c.InfinityLoop ? "無限ループ再生中。" : "再生中。");
						defaultInterpolatedStringHandler2.AppendLiteral("\n");
						defaultInterpolatedStringHandler2.AppendFormatted(this.GetTrackFrameSummary());
						this.Status = defaultInterpolatedStringHandler2.ToStringAndClear();
						if (jumpPoint > 0)
						{
							Index.JumpMarker marker = this.JumpMarkers.FirstOrDefault<Index.JumpMarker>(delegate(Index.JumpMarker m)
							{
								string label = m.Label;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(1, 1);
								defaultInterpolatedStringHandler5.AppendLiteral("J");
								defaultInterpolatedStringHandler5.AppendFormatted<int>(jumpPoint);
								return label == defaultInterpolatedStringHandler5.ToStringAndClear();
							});
							if (marker == null)
							{
								this.Error = true;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(17, 1);
								defaultInterpolatedStringHandler3.AppendLiteral("J");
								defaultInterpolatedStringHandler3.AppendFormatted<int>(jumpPoint);
								defaultInterpolatedStringHandler3.AppendLiteral(" がMMLに定義されていません。");
								this.Status = defaultInterpolatedStringHandler3.ToStringAndClear();
							}
							else
							{
								await this.JS.InvokeVoidAsync("ndpEditor.resetPlaybackTimeline", new object[] { marker.Seconds });
								await this.JS.InvokeVoidAsync("ndpPlayer.playFrom", new object[]
								{
									Convert.ToBase64String(this.Kss),
									this.AudioDuration,
									this.Compiler.c.InfinityLoop,
									marker.Seconds
								});
								this.IsPaused = false;
								if (solo)
								{
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(11, 2);
									defaultInterpolatedStringHandler4.AppendLiteral("J");
									defaultInterpolatedStringHandler4.AppendFormatted<int>(jumpPoint);
									defaultInterpolatedStringHandler4.AppendLiteral(" からソロ再生中。\n");
									defaultInterpolatedStringHandler4.AppendFormatted(this.GetTrackFrameSummary());
									this.Status = defaultInterpolatedStringHandler4.ToStringAndClear();
								}
								marker = null;
							}
						}
						else
						{
							await this.Play(true);
						}
					}
				}
				catch (Exception ex2)
				{
					this.Error = true;
					this.Status = "エラー: " + ex2.Message;
				}
				finally
				{
					this.Compiling = false;
					this.Busy = false;
				}
			}
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000044B4 File Offset: 0x000026B4
		private async Task SetupPlaybackTracking()
		{
			Index.<>c__DisplayClass48_0 CS$<>8__locals1 = new Index.<>c__DisplayClass48_0();
			Index.<>c__DisplayClass48_0 CS$<>8__locals2 = CS$<>8__locals1;
			Compiler compiler = this.Compiler;
			CS$<>8__locals2.c = ((compiler != null) ? compiler.c : null);
			if (CS$<>8__locals1.c != null)
			{
				List<object> list = new List<object>();
				for (int i = 0; i < 4; i++)
				{
					List<object> list2 = new List<object>();
					for (int j = 0; j < Math.Min(CS$<>8__locals1.c.LinePlayTime[i].Count, CS$<>8__locals1.c.LinePlayLineNumber[i].Count); j++)
					{
						list2.Add(new
						{
							time = CS$<>8__locals1.c.LinePlayTime[i][j],
							line = CS$<>8__locals1.c.LinePlayLineNumber[i][j]
						});
					}
					List<object> list3 = new List<object>();
					int num = Math.Min(CS$<>8__locals1.c.NotePlayTime[i].Count, Math.Min(CS$<>8__locals1.c.NotePlayLineNumber[i].Count, CS$<>8__locals1.c.NotePlayColumn[i].Count));
					for (int k = 0; k < num; k++)
					{
						list3.Add(new
						{
							time = CS$<>8__locals1.c.NotePlayTime[i][k],
							line = CS$<>8__locals1.c.NotePlayLineNumber[i][k],
							column = CS$<>8__locals1.c.NotePlayColumn[i][k]
						});
					}
					list.Add(new
					{
						id = i,
						lines = list2,
						notes = list3,
						loopStart = (double)CS$<>8__locals1.c.BeforeLoopFrame[i, 0] / 59.94080511738504,
						loopLength = (double)CS$<>8__locals1.c.TrackFrame[i, 0] / 59.94080511738504,
						hasLoop = (CS$<>8__locals1.c.BeforeLoopFrame[i, 0] >= 0 && CS$<>8__locals1.c.TrackFrame[i, 0] > 0)
					});
				}
				double num2 = (double)CS$<>8__locals1.c.TotalFrame / 59.94080511738504;
				double[] array = null;
				if (CS$<>8__locals1.c.HasSlowMotion && CS$<>8__locals1.c.TotalFrame > 0 && CS$<>8__locals1.c.TotalFrame <= 200000)
				{
					array = (from frame in Enumerable.Range(0, CS$<>8__locals1.c.TotalFrame + 1)
						select CS$<>8__locals1.c.MusicToRealTime((double)frame / 59.94080511738504)).ToArray<double>();
				}
				await this.JS.InvokeVoidAsync("ndpEditor.setTracking", new object[]
				{
					list,
					CS$<>8__locals1.c.InfinityLoop,
					59.94080511738504,
					array,
					this.AudioDuration
				});
				await this.JS.InvokeVoidAsync("ndpEditor.setTrackingOptions", new object[] { this.TrackHighlight, this.AutoScroll });
			}
		}

		// Token: 0x06000039 RID: 57 RVA: 0x000044F8 File Offset: 0x000026F8
		private async Task ToggleTrackHighlight(ChangeEventArgs e)
		{
			object value = e.Value;
			bool flag;
			bool flag2;
			if (value is bool)
			{
				flag = (bool)value;
				flag2 = true;
			}
			else
			{
				flag2 = false;
			}
			this.TrackHighlight = flag2 && flag;
			await this.JS.InvokeVoidAsync("ndpEditor.setTrackingOptions", new object[] { this.TrackHighlight, this.AutoScroll });
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00004544 File Offset: 0x00002744
		private async Task ToggleAutoScroll(ChangeEventArgs e)
		{
			object value = e.Value;
			bool flag;
			bool flag2;
			if (value is bool)
			{
				flag = (bool)value;
				flag2 = true;
			}
			else
			{
				flag2 = false;
			}
			this.AutoScroll = flag2 && flag;
			await this.JS.InvokeVoidAsync("ndpEditor.setTrackingOptions", new object[] { this.TrackHighlight, this.AutoScroll });
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00004590 File Offset: 0x00002790
		private async Task PlayFromCursor(bool solo = false)
		{
			if (!this.Busy && this.Driver != null)
			{
				this.Busy = true;
				this.Compiling = true;
				this.Error = false;
				this.Status = "コンパイルを開始しています…";
				this.JumpMarkers.Clear();
				this.HasJ0 = false;
				await base.InvokeAsync(new Action(base.StateHasChanged));
				try
				{
					await this.JS.InvokeVoidAsync("ndpPlayer.unlock", Array.Empty<object>());
					await this.JS.InvokeVoidAsync("ndpPlayer.stop", Array.Empty<object>());
					this.IsPaused = false;
					int caret = await this.JS.InvokeAsync<int>("ndpEditor.getCaretOffset", Array.Empty<object>());
					await Task.Delay(80);
					Compiler compiler = new Compiler();
					CompileOptions compileOptions = new CompileOptions(PresetBank.Voices, PresetBank.Rhythm, this.OutputMetaData);
					if (!compiler.Compile(this.Mml, compileOptions, caret, solo, false, null, -1, -1, -1, -1) || compiler.c.Message != null)
					{
						this.Error = true;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 2);
						defaultInterpolatedStringHandler.AppendLiteral("コンパイルエラー（行 ");
						defaultInterpolatedStringHandler.AppendFormatted<int>(compiler.c.ErrorLine);
						defaultInterpolatedStringHandler.AppendLiteral("）：");
						defaultInterpolatedStringHandler.AppendFormatted(compiler.c.Message);
						this.Status = defaultInterpolatedStringHandler.ToStringAndClear();
						if (!string.IsNullOrEmpty(compiler.c.ErrorString))
						{
							this.Status = this.Status + "\nエラー対象「" + compiler.c.ErrorString + "」";
						}
					}
					else
					{
						this.Compiler = compiler;
						this.Kss = KssBuilder.Build(this.Driver, this.Compiler.c);
						int num = this.Compiler.c.Address + ((this.Compiler.c.SEData != null && this.Compiler.c.SEAddressAbsolute == 0) ? this.Compiler.c.SEData.Length : 0);
						this.Binary = new byte[num];
						Array.Copy(this.Compiler.c.BGMData, this.Binary, num);
						this.Duration = "00:00 / " + TimeSpan.FromSeconds((double)this.Compiler.c.TotalFrame / 59.94080511738504).ToString("mm\\:ss");
						this.UpdateJumpMarkers();
						try
						{
							await this.SetupPlaybackTracking();
						}
						catch (Exception ex)
						{
							Console.Error.WriteLine("Playback tracking setup failed: " + ((ex != null) ? ex.ToString() : null));
						}
						double start = Math.Max(0.0, this.Compiler.c.JumpStartTime[6] + this.Compiler.c.JumpOffset);
						start = this.Compiler.c.MusicToRealTime(start);
						await this.JS.InvokeVoidAsync("ndpEditor.resetPlaybackTimeline", new object[] { start });
						await this.JS.InvokeVoidAsync("ndpPlayer.playFrom", new object[]
						{
							Convert.ToBase64String(this.Kss),
							this.AudioDuration,
							this.Compiler.c.InfinityLoop,
							start
						});
						this.IsPaused = false;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(22, 4);
						defaultInterpolatedStringHandler2.AppendLiteral("カーソル位置から");
						defaultInterpolatedStringHandler2.AppendFormatted(solo ? "ソロ再生" : "再生");
						defaultInterpolatedStringHandler2.AppendLiteral("：");
						defaultInterpolatedStringHandler2.AppendFormatted<double>(start, "F2");
						defaultInterpolatedStringHandler2.AppendLiteral(" 秒 / ");
						defaultInterpolatedStringHandler2.AppendFormatted<int>(this.Compiler.c.TotalFrame);
						defaultInterpolatedStringHandler2.AppendLiteral(" frames\n");
						defaultInterpolatedStringHandler2.AppendFormatted(this.GetTrackFrameSummary());
						this.Status = defaultInterpolatedStringHandler2.ToStringAndClear();
					}
				}
				catch (Exception ex2)
				{
					this.Error = true;
					this.Status = "カーソル再生エラー: " + ex2.Message;
				}
				finally
				{
					this.Compiling = false;
					this.Busy = false;
				}
			}
		}

		// Token: 0x0600003C RID: 60 RVA: 0x000045DC File Offset: 0x000027DC
		private async Task ToggleWrapEditor(ChangeEventArgs e)
		{
			object value = e.Value;
			bool flag2;
			if (value is bool)
			{
				bool flag = (bool)value;
				flag2 = flag;
			}
			else
			{
				object value2 = e.Value;
				flag2 = string.Equals((value2 != null) ? value2.ToString() : null, "true", StringComparison.OrdinalIgnoreCase);
			}
			this.WrapEditor = flag2;
			await this.JS.InvokeVoidAsync("ndpEditor.setWordWrap", new object[] { this.WrapEditor });
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00004627 File Offset: 0x00002827
		private Task PlayFromBeginning()
		{
			return this.Play(false);
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00004630 File Offset: 0x00002830
		private async Task Play(bool useJ0)
		{
			if (this.Kss != null)
			{
				try
				{
					if (useJ0 && this.HasJ0 && this.J0Seconds > 0.0)
					{
						await this.JS.InvokeVoidAsync("ndpEditor.resetPlaybackTimeline", new object[] { this.J0Seconds });
						await this.JS.InvokeVoidAsync("ndpPlayer.playFrom", new object[]
						{
							Convert.ToBase64String(this.Kss),
							this.AudioDuration,
							this.Compiler.c.InfinityLoop,
							this.J0Seconds
						});
					}
					else
					{
						await this.JS.InvokeVoidAsync("ndpEditor.resetPlaybackTimeline", new object[] { 0 });
						await this.JS.InvokeVoidAsync("ndpPlayer.play", new object[]
						{
							Convert.ToBase64String(this.Kss),
							this.AudioDuration,
							this.Compiler.c.InfinityLoop
						});
					}
					this.IsPaused = false;
					this.Error = false;
				}
				catch (Exception ex)
				{
					this.Error = true;
					this.Status = "再生エラー: " + ex.Message;
				}
			}
		}

		// Token: 0x0600003F RID: 63 RVA: 0x0000467C File Offset: 0x0000287C
		private async Task TogglePause()
		{
			if (this.Kss != null)
			{
				try
				{
					if (this.IsPaused)
					{
						await this.JS.InvokeVoidAsync("ndpPlayer.resume", Array.Empty<object>());
					}
					else
					{
						await this.JS.InvokeVoidAsync("ndpPlayer.pause", Array.Empty<object>());
					}
					this.IsPaused = !this.IsPaused;
					this.Error = false;
				}
				catch (Exception ex)
				{
					this.Error = true;
					this.Status = (this.IsPaused ? "再開" : "一時停止") + "エラー: " + ex.Message;
				}
			}
		}

		// Token: 0x06000040 RID: 64 RVA: 0x000046C0 File Offset: 0x000028C0
		private async Task Stop()
		{
			try
			{
				await this.JS.InvokeVoidAsync("ndpPlayer.stop", Array.Empty<object>());
				await this.JS.InvokeVoidAsync("ndpEditor.resetPlaybackTimeline", new object[] { 0 });
				this.IsPaused = false;
			}
			catch (Exception ex)
			{
				this.Error = true;
				this.Status = "停止エラー: " + ex.Message;
			}
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00004704 File Offset: 0x00002904
		private async Task OpenMmlFile(InputFileChangeEventArgs e)
		{
			if (!this.Busy && e.File != null)
			{
				try
				{
					if (e.File.Size > 2097152L)
					{
						this.Error = true;
						this.Status = "ファイルが大きすぎます（上限2MB）。";
					}
					else
					{
						ValueTaskAwaiter<bool> valueTaskAwaiter = this.JS.InvokeAsync<bool>("confirm", new object[] { "現在のMMLを置き換えてファイルを開きますか？" }).GetAwaiter();
						if (!valueTaskAwaiter.IsCompleted)
						{
							await valueTaskAwaiter;
							ValueTaskAwaiter<bool> valueTaskAwaiter2;
							valueTaskAwaiter = valueTaskAwaiter2;
							valueTaskAwaiter2 = default(ValueTaskAwaiter<bool>);
						}
						if (valueTaskAwaiter.GetResult())
						{
							string content;
							using (Stream stream = e.File.OpenReadStream(2097152L, default(CancellationToken)))
							{
								using (StreamReader reader = new StreamReader(stream, new UTF8Encoding(false), true))
								{
									content = await reader.ReadToEndAsync();
									await this.Stop();
									this.Mml = content;
									this.MmlFileName = e.File.Name;
									object obj = null;
									this.Binary = obj;
									this.Kss = obj;
									this.ShareUrl = null;
									this.JumpMarkers.Clear();
									this.AudioDuration = 0.0;
									this.Duration = "00:00 / 00:00";
									await this.JS.InvokeVoidAsync("ndpEditor.loadLocalMml", new object[] { content });
									this.Error = false;
									this.Status = e.File.Name + " を読み込みました。";
								}
							}
							Stream stream = null;
							StreamReader reader = null;
							content = null;
						}
					}
				}
				catch (Exception ex)
				{
					this.Error = true;
					this.Status = "ファイル読み込みエラー: " + ex.Message;
				}
			}
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00004750 File Offset: 0x00002950
		private async Task ResetMml()
		{
			if (!this.Busy)
			{
				ValueTaskAwaiter<bool> valueTaskAwaiter = this.JS.InvokeAsync<bool>("confirm", new object[] { "現在編集中のMMLを破棄して、初期MMLに戻します。\nこの操作は元に戻せません。よろしいですか？" }).GetAwaiter();
				if (!valueTaskAwaiter.IsCompleted)
				{
					await valueTaskAwaiter;
					ValueTaskAwaiter<bool> valueTaskAwaiter2;
					valueTaskAwaiter = valueTaskAwaiter2;
					valueTaskAwaiter2 = default(ValueTaskAwaiter<bool>);
				}
				if (valueTaskAwaiter.GetResult())
				{
					await this.Stop();
					this.Mml = "#TITLE\t\"動作確認用MML\"\n#TEMPO\t150\n\n;これはサンプルMMLです。自由に書き換えてください。\n;書き換えた内容はローカル環境にのみ保存されます。\n;勝手にオンラインで共有されることはありません。\n\n123R\t@L\n\n12\t@0 q6 @r12,8 @rd1,30 l8\n1\tv15 o5 cdefgab>c c<bagfedc\n2\tv13 o4 efgab>cde edc<bagfe\n\n3\ts8,3 l8 q7\n3\to2 [ cc>c<c ]4\n\nR\tv15 vh12 l8\nR\tdhsh bbsh bhsb hbsb\n";
					this.MmlFileName = "NDP-ONLINE.mml";
					object obj = null;
					this.Binary = obj;
					this.Kss = obj;
					this.ShareUrl = null;
					this.JumpMarkers.Clear();
					this.AudioDuration = 0.0;
					this.Duration = "00:00 / 00:00";
					await this.JS.InvokeVoidAsync("ndpEditor.loadLocalMml", new object[] { this.Mml });
					this.Error = false;
					this.Status = "MMLを初期化しました。";
				}
			}
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00004793 File Offset: 0x00002993
		private void DownloadMml()
		{
			this.SaveMmlFileName = this.MmlFileName;
			this.ShowMmlSaveDialog = true;
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000047A8 File Offset: 0x000029A8
		private void CancelMmlSave()
		{
			this.ShowMmlSaveDialog = false;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x000047B4 File Offset: 0x000029B4
		private async Task SaveNameKeyDown(KeyboardEventArgs e)
		{
			if (e.Key == "Escape")
			{
				this.CancelMmlSave();
			}
			else if (e.Key == "Enter")
			{
				await this.ConfirmMmlSave();
			}
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00004800 File Offset: 0x00002A00
		private async Task ConfirmMmlSave()
		{
			if (this.ShowMmlSaveDialog)
			{
				string name = (this.SaveMmlFileName ?? "").Trim();
				if (!string.IsNullOrEmpty(name))
				{
					name = string.Concat<char>(name.Select<char, char>(delegate(char c)
					{
						if (!Path.GetInvalidFileNameChars().Contains(c) && !"<>:\\|?*".Contains(c))
						{
							return c;
						}
						return '_';
					}));
					name = name.TrimEnd(new char[] { '.', ' ' });
					if (!string.IsNullOrEmpty(name))
					{
						if (string.IsNullOrEmpty(Path.GetExtension(name)))
						{
							name += ".mml";
						}
						await this.Save(name, Encoding.UTF8.GetBytes(this.Mml ?? ""));
						this.MmlFileName = name;
						this.ShowMmlSaveDialog = false;
					}
				}
			}
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00004843 File Offset: 0x00002A43
		private Task DownloadKss()
		{
			return this.Save("NDP-WEB.kss", this.Kss);
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00004856 File Offset: 0x00002A56
		private Task DownloadBinary()
		{
			return this.Save("NDP-WEB.ndp", this.Binary);
		}

		// Token: 0x06000049 RID: 73 RVA: 0x0000486C File Offset: 0x00002A6C
		private async Task Save(string name, byte[] bytes)
		{
			if (bytes != null)
			{
				await this.JS.InvokeVoidAsync("ndpWeb.saveBase64", new object[]
				{
					name,
					Convert.ToBase64String(bytes)
				});
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600004A RID: 74 RVA: 0x000048BF File Offset: 0x00002ABF
		// (set) Token: 0x0600004B RID: 75 RVA: 0x000048C7 File Offset: 0x00002AC7
		[Inject]
		private NavigationManager Navigation { get; set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600004C RID: 76 RVA: 0x000048D0 File Offset: 0x00002AD0
		// (set) Token: 0x0600004D RID: 77 RVA: 0x000048D8 File Offset: 0x00002AD8
		[Inject]
		private IJSRuntime JS { get; set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600004E RID: 78 RVA: 0x000048E1 File Offset: 0x00002AE1
		// (set) Token: 0x0600004F RID: 79 RVA: 0x000048E9 File Offset: 0x00002AE9
		[Inject]
		private HttpClient Http { get; set; }

		// Token: 0x06000056 RID: 86 RVA: 0x00004990 File Offset: 0x00002B90
		[CompilerGenerated]
		internal static bool <UpdateJumpMarkers>g__IsDefined|30_0(int number, ref Index.<>c__DisplayClass30_0 A_1)
		{
			for (int i = 0; i < 4; i++)
			{
				if (A_1.c.SetJumpNumber[i, number])
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0400000E RID: 14
		private const string DefaultMml = "#TITLE\t\"動作確認用MML\"\n#TEMPO\t150\n\n;これはサンプルMMLです。自由に書き換えてください。\n;書き換えた内容はローカル環境にのみ保存されます。\n;勝手にオンラインで共有されることはありません。\n\n123R\t@L\n\n12\t@0 q6 @r12,8 @rd1,30 l8\n1\tv15 o5 cdefgab>c c<bagfedc\n2\tv13 o4 efgab>cde edc<bagfe\n\n3\ts8,3 l8 q7\n3\to2 [ cc>c<c ]4\n\nR\tv15 vh12 l8\nR\tdhsh bbsh bhsb hbsb\n";

		// Token: 0x0400000F RID: 15
		private string Mml = "#TITLE\t\"動作確認用MML\"\n#TEMPO\t150\n\n;これはサンプルMMLです。自由に書き換えてください。\n;書き換えた内容はローカル環境にのみ保存されます。\n;勝手にオンラインで共有されることはありません。\n\n123R\t@L\n\n12\t@0 q6 @r12,8 @rd1,30 l8\n1\tv15 o5 cdefgab>c c<bagfedc\n2\tv13 o4 efgab>cde edc<bagfe\n\n3\ts8,3 l8 q7\n3\to2 [ cc>c<c ]4\n\nR\tv15 vh12 l8\nR\tdhsh bbsh bhsb hbsb\n";

		// Token: 0x04000010 RID: 16
		private string Status = "初期化中…";

		// Token: 0x04000011 RID: 17
		private string Duration = "00:00 / 00:00";

		// Token: 0x04000012 RID: 18
		private string ShareUrl;

		// Token: 0x04000013 RID: 19
		private string SharedTitle = "NO TITLE";

		// Token: 0x04000014 RID: 20
		private bool ShowSharedPlayDialog;

		// Token: 0x04000015 RID: 21
		private string LastScrolledError;

		// Token: 0x04000016 RID: 22
		private bool Compiling;

		// Token: 0x04000017 RID: 23
		private bool Error;

		// Token: 0x04000018 RID: 24
		private bool Busy;

		// Token: 0x04000019 RID: 25
		private bool OutputMetaData;

		// Token: 0x0400001A RID: 26
		private bool TrackHighlight = true;

		// Token: 0x0400001B RID: 27
		private bool AutoScroll = true;

		// Token: 0x0400001C RID: 28
		private bool WrapEditor;

		// Token: 0x0400001D RID: 29
		private bool IsPaused;

		// Token: 0x0400001E RID: 30
		private byte[] Driver;

		// Token: 0x0400001F RID: 31
		private byte[] Kss;

		// Token: 0x04000020 RID: 32
		private byte[] Binary;

		// Token: 0x04000021 RID: 33
		private string MmlFileName = "NDP-ONLINE.mml";

		// Token: 0x04000022 RID: 34
		private string SaveMmlFileName = "NDP-ONLINE.mml";

		// Token: 0x04000023 RID: 35
		private bool ShowMmlSaveDialog;

		// Token: 0x04000024 RID: 36
		private Compiler Compiler;

		// Token: 0x04000025 RID: 37
		private readonly List<Index.JumpMarker> JumpMarkers = new List<Index.JumpMarker>();

		// Token: 0x04000026 RID: 38
		private IDisposable shortcutReference;

		// Token: 0x04000027 RID: 39
		private bool HasJ0;

		// Token: 0x04000028 RID: 40
		private double J0Seconds;

		// Token: 0x04000029 RID: 41
		private double AudioDuration;

		// Token: 0x0200001E RID: 30
		private sealed class JumpMarker
		{
			// Token: 0x17000018 RID: 24
			// (get) Token: 0x06000142 RID: 322 RVA: 0x00026B71 File Offset: 0x00024D71
			// (set) Token: 0x06000143 RID: 323 RVA: 0x00026B79 File Offset: 0x00024D79
			public string Label { get; set; }

			// Token: 0x17000019 RID: 25
			// (get) Token: 0x06000144 RID: 324 RVA: 0x00026B82 File Offset: 0x00024D82
			// (set) Token: 0x06000145 RID: 325 RVA: 0x00026B8A File Offset: 0x00024D8A
			public double Seconds { get; set; }

			// Token: 0x1700001A RID: 26
			// (get) Token: 0x06000146 RID: 326 RVA: 0x00026B93 File Offset: 0x00024D93
			// (set) Token: 0x06000147 RID: 327 RVA: 0x00026B9B File Offset: 0x00024D9B
			public double Percent { get; set; }
		}
	}
}
