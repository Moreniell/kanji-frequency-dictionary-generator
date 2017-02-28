using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Moreniell.KanjiFrequencyDictionaryGenerator
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private void inputBtn_Click(object sender, EventArgs e)
		{
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				textBoxIn.Text = openFileDialog.FileName;
			}
		}

		private void outputBtn_Click(object sender, EventArgs e)
		{
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				textBoxOut.Text = saveFileDialog.FileName;
			}
		}

		private void convertButton_Click(object sender, EventArgs e)
		{
			if (File.Exists(textBoxIn.Text))
			{
				string content = File.ReadAllText(textBoxIn.Text);
				var frequencyDic = new Dictionary<char, int>();

				foreach (char c in content)
				{
					if (!char.IsDigit(c)
						/* Latina   */ && !"abcdefghijklmnopqrstuvwxyz".Contains(char.ToLower(c))
						/* Romanji  */ && !"ＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ".Contains(char.ToUpper(c))
						/* Hiragana */ && !"あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやゆよらりるれろわゐゑをん".Contains(c)
						/* Katakana */ && !"アイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヰヱヲン".Contains(c)
						/* Hiragana 半濁点 */ && !"がぎぐげござじずぜぞだぢづでどばびぶべぼぱぴぷぺぽゃゅょっ".Contains(c)
						/* Katakana 半濁点 */ && !"ガギグゲゴザジズゼゾダヂヅデドバビブベボパピプペポャュョィッァェォ".Contains(c)
						/* Kanji Numbers */ && !"〇零一二三四五六七八九十百万億挑".Contains(c)
						/* Symbols       */ && !" */!.,()[]?\"\n\r'{}:%$#@~`-+=＝_×・&、。°→！■？|>／【】「」〜～◇◆▲▼－〈〉（）『』“　”％…♪★©ー々".Contains(c))
					{
						if (!frequencyDic.Keys.Contains(c)) frequencyDic[c] = 0;
						frequencyDic[c]++;
					}
				}
				if (!string.IsNullOrWhiteSpace(textBoxOut.Text))
				{
					bool allowMerging = false;
					
					if (File.Exists(textBoxOut.Text))
					{
						if (File.ReadAllLines(textBoxOut.Text)[0].Contains("[allow-merge]"))
						{
							allowMerging = true;
							frequencyDic = MergeWithFile(frequencyDic, textBoxOut.Text);
						}
						File.Delete(textBoxOut.Text);
					}

					// Save dic in file
					using (var sw = new StreamWriter(File.OpenWrite(textBoxOut.Text), Encoding.Unicode))
					{
						if (allowMerging) sw.WriteLine("[allow-merge]");
						foreach (var kv in frequencyDic.OrderByDescending(x => x.Value))
						{
							sw.WriteLine(kv.Value + ": " + kv.Key);
						}
					}

					MessageBox.Show(allowMerging ? "Done! (merged)" : "Done!", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				else
				{
					MessageBox.Show("Error, output path is empty!");
				}
			}
			else
			{
				MessageBox.Show("Error, input file is not found!");
			}
		}

		private Dictionary<char, int> MergeWithFile(Dictionary<char, int> dic, string fileName)
		{
			string[] lines = File.ReadAllLines(fileName);

			foreach (string line in lines)
			{
				int index = line.IndexOf(':');
				if (index == -1) continue;

				int value = int.Parse(line.Substring(0, index));
				//MessageBox.Show($"До: {line}\nПосле: {value}");
				char key = line.Substring(index + 2)[0];

				if (!dic.Keys.Contains(key)) dic[key] = 0;
				dic[key] += value;
			}
			return dic;
		}
	}
}
