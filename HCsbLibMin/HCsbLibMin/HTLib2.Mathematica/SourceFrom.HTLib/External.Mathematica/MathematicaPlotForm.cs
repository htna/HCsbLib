/*
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HTLib
{
	public partial class MathematicaPlotForm : Form
	{
		string resultText;
		string messagesText;
		string printText;
		string inputText;

		public MathematicaPlotForm(string inputText)
		{
			this.inputText = inputText;
			InitializeComponent();
			Plot();
		}
		protected virtual void Plot()
		{
			if(mathKernel.IsComputing)
			{
				mathKernel.Abort();
			}
			else
			{
				// Clear out any results from previous computation.
				resultText = "";
				messagesText = "";
				printText = "";
				//mathPictureBox.Image = null;

				// This could be done in the initialization code.
				mathKernel.GraphicsHeight = mathPictureBox.Height;
				mathKernel.GraphicsWidth = mathPictureBox.Width;

//				computeButton.Text = "Abort";
				// Perform the computation. Compute() will not return until the result has arrived.
				mathKernel.Compute(inputText);
//				computeButton.Text = "Compute";

				// Populate the various boxes with results.
				resultText = (string)mathKernel.Result;
				foreach(string msg in mathKernel.Messages)
					messagesText += (msg + "\r\n");
				foreach(string p in mathKernel.PrintOutput)
					printText += p;
				// The Graphics property returns an array of images, so it can accommodate
				// more than one graphic produced, but we only have room for one image.
				if(mathKernel.Graphics.Length > 0)
					mathPictureBox.Image = mathKernel.Graphics[0];
				mathPictureBox.Link = mathKernel.Link;
			}
		}

		private void mathPictureBox_Resize(object sender, EventArgs e)
		{
			Plot();
		}
		public new DialogResult ShowDialog()
		{
			DialogResult result = base.ShowDialog();
			mathKernel.Dispose();
			return result;
		}
	}
}
*/