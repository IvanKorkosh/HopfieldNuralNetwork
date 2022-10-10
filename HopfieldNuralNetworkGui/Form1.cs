using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HopfieldNetworkCore;

namespace HopfieldNuralNetworkGui
{
    public partial class Form1 : Form
    {
        private readonly PatternRecognizer recognizer = new PatternRecognizer(100);
        private int imageToShowNumber = 0;

        public Form1()
        {
            InitializeComponent();
            for (int i = 0; i < 10; ++i)
            {
                dataGridView1.Rows.Add();
            }

            foreach (var set in GetSet(Properties.Resources.Symbols))
            {
                recognizer.LoadNewImage(set);
            }
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            imageToShowNumber = 0;
            if (dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor == Color.Black)
            {
                dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.White;
                dataGridView1[e.ColumnIndex, e.RowIndex].Style.SelectionBackColor = Color.White;
            }
            else
            {
                dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.Black;
                dataGridView1[e.ColumnIndex, e.RowIndex].Style.SelectionBackColor = Color.Black;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            imageToShowNumber = 0;
            ClearField();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            imageToShowNumber = 0;
            var image = GetImage();
            recognizer.LoadNewImage(image);
            ClearField();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (recognizer.Count == 0)
            {
                MessageBox.Show("No image has been added");
                return;
            }

            var image = recognizer.GetImage(imageToShowNumber);
            SetImage(image);
            imageToShowNumber = imageToShowNumber == recognizer.Count - 1 ? 0 : imageToShowNumber + 1;
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            if (recognizer.Count == 0)
            {
                MessageBox.Show("No image has been added");
                return;
            }

            imageToShowNumber = 0;
            var image = GetImage();
            try
            {
                var newImage = recognizer.FindImage(image, 100);
                SetImage(newImage);
            }
            catch (ImageNotFoundException)
            {
                MessageBox.Show("Image not recognized");
                ClearField();
            }
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            imageToShowNumber = 0;
            recognizer.EraseAllImages();
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            imageToShowNumber = 0;
            var indexes = GetShuffledArray(100).Take(trackBar1.Value);
            foreach (var index in indexes)
            {
                var column = index % dataGridView1.ColumnCount;
                var row = index / dataGridView1.ColumnCount;
                if (dataGridView1[column, row].Style.BackColor == Color.Black)
                {
                    dataGridView1[column, row].Style.BackColor = Color.White;
                    dataGridView1[column, row].Style.SelectionBackColor = Color.White;
                }
                else
                {
                    dataGridView1[column, row].Style.BackColor = Color.Black;
                    dataGridView1[column, row].Style.SelectionBackColor = Color.Black;
                }
            }

        }

        private List<BinaryState> GetImage()
        {
            var result = new List<BinaryState>();
            for (int row = 0; row < dataGridView1.RowCount; ++row)
            {
                for (int column = 0; column < dataGridView1.ColumnCount; ++column)
                {
                    var item = dataGridView1[column, row].Style.BackColor == Color.Black ? BinaryState.High : BinaryState.Low;
                    result.Add(item);
                }
            }

            return result;
        }
        
        private void SetImage(IEnumerable<BinaryState> image)
        {
            var source = image.ToArray();
            for (int row = 0; row < dataGridView1.RowCount; ++row)
            {
                for (int column = 0; column < dataGridView1.ColumnCount; ++column)
                {
                    if (source[row * dataGridView1.ColumnCount + column] == BinaryState.High)
                    {
                        dataGridView1[column, row].Style.BackColor = Color.Black;
                        dataGridView1[column, row].Style.SelectionBackColor = Color.Black;
                    }
                    else
                    {
                        dataGridView1[column, row].Style.BackColor = Color.White;
                        dataGridView1[column, row].Style.SelectionBackColor = Color.White;
                    }
                }
            }
        }

        private void ClearField()
        {
            for (int row = 0; row < dataGridView1.RowCount; ++row)
            {
                for (int column = 0; column < dataGridView1.ColumnCount; ++column)
                {
                    dataGridView1[column, row].Style.BackColor = Color.White;
                    dataGridView1[column, row].Style.SelectionBackColor = Color.White;
                }
            }
        }

        private List<BinaryState[]> GetSet(string source)
        {
            var result = new List<BinaryState[]>();
            foreach (var item in source.Split(Environment.NewLine))
            {
                result.Add(ToBinaryArray(item));
            }

            return result;
        }

        private BinaryState[] ToBinaryArray(string source)
        {
            var result = new BinaryState[source.Length];
            for (int i = 0; i < source.Length; ++i)
            {
                result[i] = source[i] == '1' ? BinaryState.High : BinaryState.Low;
            }

            return result;
        }

        private int[] GetShuffledArray(int length)
        {
            var result = Enumerable.Range(0, length).ToArray();
            var random = new Random();
            for (int i = result.Length - 1; i >= 1; i--)
            {
                int j = random.Next(i + 1);
                var temp = result[j];
                result[j] = result[i];
                result[i] = temp;
            }

            return result;
        }
    }
}
