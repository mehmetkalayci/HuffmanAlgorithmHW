using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HuffmanAlgorithmHW
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
        }

        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Move;
        }

        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            chart1.Series.Clear();

            chart1.Series.Add("Running Time");
            chart1.Series.Add("Compression Ratio");
            

            chart1.Series["Running Time"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series["Compression Ratio"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;


            chart1.Series["Running Time"].BorderWidth = 2;
            chart1.Series["Compression Ratio"].BorderWidth = 2;

            chart1.ChartAreas[0].AxisX.Interval = 1;
            chart1.ChartAreas[0].AxisY.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
            


            // Set automatic zooming
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;

            // Set automatic scrolling 
            chart1.ChartAreas[0].CursorX.AutoScroll = true;
            chart1.ChartAreas[0].CursorY.AutoScroll = true;

            // Allow user selection for Zoom
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;

            this.files.Clear();
            this.files.AddRange((string[])e.Data.GetData(DataFormats.FileDrop));



            for (int i = 0; i < this.files.Count; i++)
            {
                string file = this.files[i];
                string fileName = Path.GetFileNameWithoutExtension(file);

                string fileContent = File.ReadAllText(file);



                HuffmanTree huffmanTree = new HuffmanTree();
                // Build
                huffmanTree.Build(fileContent);


                Stopwatch sw = new Stopwatch();

                // Encode
                sw.Start();
                BitArray encoded = huffmanTree.Encode(fileContent);
                sw.Stop();

                long runningTime = sw.ElapsedMilliseconds / 1000;



                double originalFileSize = fileContent.Length; // Byte
                double compressedFileSize = encoded.Count / 8; // Byte

                double compressionRatio = compressedFileSize / originalFileSize * 100;

                // Decode
                //string decoded = huffmanTree.Decode(encoded);

                chart1.Series["Compression Ratio"].Points.AddXY(fileName, compressionRatio);
                chart1.Series["Compression Ratio"].Points[i].Label = string.Format("% {0:0.00}", compressionRatio);



                chart1.Series["Running Time"].Points.AddXY(fileName, runningTime);
                chart1.Series["Running Time"].Points[i].Label = runningTime + " sn";


            }

        }


        List<string> files = new List<string>();

        private void frmMain_Load(object sender, EventArgs e)
        {
            
        }
    }
}
