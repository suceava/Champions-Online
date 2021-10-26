using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageFixer
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void browseButton_Click(object sender, EventArgs e)
		{
			folderBrowserDialog.SelectedPath = string.IsNullOrEmpty(pathTextBox.Text) ? Application.ExecutablePath : pathTextBox.Text;
			if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
			{
				pathTextBox.Text = folderBrowserDialog.SelectedPath;
			}
		}

		private void browseFixedButton_Click(object sender, EventArgs e)
		{
			folderBrowserDialog.SelectedPath = string.IsNullOrEmpty(fixedTextBox.Text) ? Application.ExecutablePath : fixedTextBox.Text;
			if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
			{
				fixedTextBox.Text = folderBrowserDialog.SelectedPath;
			}
		}

		private void processButton_Click(object sender, EventArgs e)
		{
			ProcessFolder(pathTextBox.Text, ignoreTextBox.Text.Split(','), fixedTextBox.Text, extendedBottomCheckBox.Checked, doubleExceptionCheckBox.Checked);
			MessageBox.Show("Folder processed");
		}

		private void ProcessFolder(string folder, string[] ignores, string fixedFolder, bool extendedBottom, bool doubleException)
		{
			if (!Directory.Exists(fixedFolder))
			{
				Directory.CreateDirectory(fixedFolder);
			}

			bool ignoreFile = false;
			string fileName;
			foreach (string file in Directory.GetFiles(folder, "*.png"))
			{
				fileName = Path.GetFileName(file);
				ignoreFile = false;
				foreach (string ignore in ignores)
				{
					if (fileName.StartsWith(ignore))
					{
						ignoreFile = true;
						break;
					}
				}
				if (ignoreFile)
					continue;

				using (Bitmap bmp = new Bitmap(file, false))
				{
					if (bmp == null)
						continue;

					// top left
					ClearPoints(bmp, new Point[] { 
						new Point(0, 0),
						new Point(1, 0),
						new Point(2, 0),
						new Point(0, 1),
						new Point(1, 1),
						new Point(0, 2)
					});

					// top right
					ClearPoints(bmp, new Point[] { 
						new Point(bmp.Width - 3, 0),
						new Point(bmp.Width - 2, 0),
						new Point(bmp.Width - 1, 0),
						new Point(bmp.Width - 2, 1),
						new Point(bmp.Width - 1, 1),
						new Point(bmp.Width - 1, 2)
					});

					int adjustCorner = 0;
					if (extendedBottom)
					{
						adjustCorner = 2;

						// remove bottom 2 pixels
						for (int x = 0; x < bmp.Width; x++)
						{
							if (doubleException)
							{
								if (x < 8 || (x > 15 && x < 22) || x > 29)
									bmp.SetPixel(x, bmp.Height - 2, Color.Transparent);
								if (x < 10 || (x > 13 && x < 24) || x > 27)
									bmp.SetPixel(x, bmp.Height - 1, Color.Transparent);
							}
							else
							{
								if (x < 16 || x > 23)
									bmp.SetPixel(x, bmp.Height - 2, Color.Transparent);
								if (x < 18 || x > 21)
									bmp.SetPixel(x, bmp.Height - 1, Color.Transparent);
							}
						}
					}

					// bottom left
					ClearPoints(bmp, new Point[] { 
						new Point(0, bmp.Height - 1 - adjustCorner),
						new Point(1, bmp.Height - 1 - adjustCorner),
						new Point(2, bmp.Height - 1 - adjustCorner),
						new Point(0, bmp.Height - 2 - adjustCorner),
						new Point(1, bmp.Height - 2 - adjustCorner),
						new Point(0, bmp.Height - 3 - adjustCorner)
					});

					// bottom left
					ClearPoints(bmp, new Point[] { 
						new Point(bmp.Width - 3, bmp.Height - 1 - adjustCorner),
						new Point(bmp.Width - 2, bmp.Height - 1 - adjustCorner),
						new Point(bmp.Width - 1, bmp.Height - 1 - adjustCorner),
						new Point(bmp.Width - 2, bmp.Height - 2 - adjustCorner),
						new Point(bmp.Width - 1, bmp.Height - 2 - adjustCorner),
						new Point(bmp.Width - 1, bmp.Height - 3 - adjustCorner)
					});

					//bmp.Save(Path.Combine(fixedFolder, Path.GetFileNameWithoutExtension(file)) + "_co.png");
					bmp.Save(Path.Combine(fixedFolder, fileName));
				}
			}
		}

		private void ClearPoints(Bitmap bmp, Point[] points)
		{
			foreach (Point point in points)
			{
				bmp.SetPixel(point.X, point.Y, Color.Transparent);
			}
		}
	}
}
