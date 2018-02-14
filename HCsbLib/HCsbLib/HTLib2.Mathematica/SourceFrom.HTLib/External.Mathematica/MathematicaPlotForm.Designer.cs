//namespace HTLib
//{
//    partial class MathematicaPlotForm
//    {
//        /// <summary>
//        /// Required designer variable.
//        /// </summary>
//        private System.ComponentModel.IContainer components = null;
//
//        /// <summary>
//        /// Clean up any resources being used.
//        /// </summary>
//        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
//        protected override void Dispose(bool disposing)
//        {
//            if(disposing && (components != null))
//            {
//                components.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//
//        #region Windows Form Designer generated code
//
//        /// <summary>
//        /// Required method for Designer support - do not modify
//        /// the contents of this method with the code editor.
//        /// </summary>
//        private void InitializeComponent()
//        {
//            this.mathKernel = new Wolfram.NETLink.MathKernel();
//            this.mathPictureBox = new Wolfram.NETLink.UI.MathPictureBox();
//            ((System.ComponentModel.ISupportInitialize)(this.mathPictureBox)).BeginInit();
//            this.SuspendLayout();
//            // 
//            // mathKernel
//            // 
//            this.mathKernel.AutoCloseLink = true;
//            this.mathKernel.CaptureGraphics = true;
//            this.mathKernel.CaptureMessages = true;
//            this.mathKernel.CapturePrint = true;
//            this.mathKernel.GraphicsFormat = "Automatic";
//            this.mathKernel.GraphicsHeight = 0;
//            this.mathKernel.GraphicsResolution = 0;
//            this.mathKernel.GraphicsWidth = 0;
//            this.mathKernel.HandleEvents = true;
//            this.mathKernel.Input = null;
//            this.mathKernel.Link = null;
//            this.mathKernel.LinkArguments = null;
//            this.mathKernel.PageWidth = 60;
//            this.mathKernel.ResultFormat = Wolfram.NETLink.MathKernel.ResultFormatType.OutputForm;
//            this.mathKernel.UseFrontEnd = true;
//            // 
//            // mathPictureBox
//            // 
//            this.mathPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
//            this.mathPictureBox.Link = null;
//            this.mathPictureBox.Location = new System.Drawing.Point(0, 0);
//            this.mathPictureBox.MathCommand = null;
//            this.mathPictureBox.Name = "mathPictureBox";
//            this.mathPictureBox.PictureType = "Automatic";
//            this.mathPictureBox.Size = new System.Drawing.Size(784, 564);
//            this.mathPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
//            this.mathPictureBox.TabIndex = 0;
//            this.mathPictureBox.TabStop = false;
//            this.mathPictureBox.UseFrontEnd = true;
//            this.mathPictureBox.Resize += new System.EventHandler(this.mathPictureBox_Resize);
//            // 
//            // PlotForm
//            // 
//            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
//            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
//            this.ClientSize = new System.Drawing.Size(784, 564);
//            this.Controls.Add(this.mathPictureBox);
//            this.Name = "PlotForm";
//            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
//            this.Text = "PlotForm";
//            ((System.ComponentModel.ISupportInitialize)(this.mathPictureBox)).EndInit();
//            this.ResumeLayout(false);
//
//        }
//
//        #endregion
//
//        private Wolfram.NETLink.MathKernel mathKernel;
//        private Wolfram.NETLink.UI.MathPictureBox mathPictureBox;
//    }
//}