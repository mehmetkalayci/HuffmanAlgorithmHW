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
        // sürüklenen dosya isimlerini listede tutuyoruz
        List<string> files = new List<string>();

        public frmMain()
        {
            InitializeComponent();
            // formun drag drop özelliğini açtık ve drag drop işlemi için gereken eventleri oluşturduk
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
        }

        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            // drag drop efektini ayarladık
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Move;
                label1.Visible = false;
            }
        }

        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            // her sürükle bırak işleminde tüm işlemleri resetlemek için, grafiğimizdeki serileri temizledik ve yeniden oluşturduk
            chart1.Series.Clear();
            chart1.Series.Add("Running Time");
            chart1.Series.Add("Compression Ratio");
            
            // seri tiplerini line olarak ayarladık
            chart1.Series["Running Time"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series["Compression Ratio"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;

            // serideki lineların kalınlıklarını ayarlardık
            chart1.Series["Running Time"].BorderWidth = 2;
            chart1.Series["Compression Ratio"].BorderWidth = 2;

            // grafiğimizin x ve y koordinatlarında gösterilecek verilerin aralıklarını ayarladık
            chart1.ChartAreas[0].AxisX.Interval = 1;
            chart1.ChartAreas[0].AxisY.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
            

            // chart nesnesinin zoomable özelliğini açtık
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;

            // chart nesnesinin AutoScroll özelliğini açtık
            chart1.ChartAreas[0].CursorX.AutoScroll = true;
            chart1.ChartAreas[0].CursorY.AutoScroll = true;

            // chart nesnesinde kullanıcı grafiğe zoom yapabilmesi için userselection açtık
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;

            // her sürükle bırak işleminde önce eski dosyaları temizledik ardından drag drop ile gelen dosya yollarını files değişkenine attık
            this.files.Clear();
            this.files.AddRange((string[])e.Data.GetData(DataFormats.FileDrop));


            // gelen tüm dosyaları tek tek aldık
            for (int i = 0; i < this.files.Count; i++)
            {
                // dosya yolunu file değişkenine ata, grafikte göstermek için dosya adını al ve fileName değişkenine ata
                string file = this.files[i];
                string fileName = Path.GetFileNameWithoutExtension(file);

                // gelen dosya içereğini oku
                string fileContent = File.ReadAllText(file);


                // HuffmanTree classını oluşturduk ve classın build metodunu kullarak gelen dosya verisi için huffman ağacı oluşturduk
                HuffmanTree huffmanTree = new HuffmanTree();
                huffmanTree.Build(fileContent);

                // Stopwatch nesnesi oluşturduk, running time hesabı için
                Stopwatch sw = new Stopwatch();

                // stopwatch'ı başlattık ve encode işlemini yaptık, stopwatch'ı durdurduk
                sw.Start();
                BitArray encoded = huffmanTree.Encode(fileContent);
                sw.Stop();

                // stopwatch nesnesinin ElapsedMilliseconds özelliği ile gelen süreyi aldık ve sn birimine çevirdik
                long runningTime = sw.ElapsedMilliseconds / 1000;


                // gelen dosya boyutunu ve huffman ile sıkıştırılan dosya boyutunu alıp byte birimine çevirdik, ardından comp. ratio değerini hesapladık
                double originalFileSize = fileContent.Length; // Byte
                double compressedFileSize = encoded.Count / 8; // Byte

                double compressionRatio = compressedFileSize / originalFileSize * 100;

                // grafikte göstermek üzere Compression Ratio serisinin x ve y pointlerine ilgili değerleri girdik ardından bu seriye eklenen değer için label değerini yazdık
                chart1.Series["Compression Ratio"].Points.AddXY(fileName, compressionRatio);
                chart1.Series["Compression Ratio"].Points[i].Label = string.Format("% {0:0.00}", compressionRatio);


                // grafikte göstermek üzere Running Time serisinin x ve y pointlerine ilgili değerleri girdik ardından bu seriye eklenen değer için label değerini yazdık
                chart1.Series["Running Time"].Points.AddXY(fileName, runningTime);
                chart1.Series["Running Time"].Points[i].Label = runningTime + " sn";
            }

        }
    }
}
