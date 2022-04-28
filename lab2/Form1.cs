using System;
using System.Windows.Forms;
using System.IO;
using System.Runtime;
using System.Collections;
using System.Text;

namespace lab2
{
    public partial class Form1 : Form
    {

        private BitArray defaultBits;
        private BitArray key;
        private string fileName;
        byte[] bytes;

        public Form1()
        {
            InitializeComponent();
        }



        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private byte[] ReadtBytes(String file)
        {
            using (FileStream fsSource = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                byte[] bytes = new byte[fsSource.Length];
                int numBytesToRead = (int)fsSource.Length;
                int numBytesRead = 0;
                while (numBytesToRead > 0)
                {
                    int n = fsSource.Read(bytes, numBytesRead, numBytesToRead);
                    if (n == 0)
                        break;

                    numBytesRead += n;
                    numBytesToRead -= n;
                }
                fsSource.Close();
                return bytes;
            }
        }

        private BitArray GetBitArray(byte[] bytes)
        {
            BitArray bits = new BitArray(bytes.Length * 8);
            int index = 0;
            foreach (byte b in bytes)
            {
                byte[] a = new byte[] { b };
                BitArray singleByte = new BitArray(a);
                for (int i = singleByte.Length - 1; i >= 0; i--)
                {
                    bits[index++] = singleByte[i];
                }
            }
            return bits;
        }

        private void PrintBits(BitArray bits, RichTextBox textBox)
        {
            int max = bits.Length > 100 ? 100 : bits.Length;
            for (int i = 0; i < max; i++)
            {
                textBox.Text = textBox.Text + Convert.ToByte(bits[i]);
            }
        }

        private void GenerateKey(int length)
        {
            key = new BitArray(length);
            while (textBoxRegister.Text.Length < 39)
                textBoxRegister.Text = textBoxRegister.Text + '0';
            BitArray register = new BitArray(39, false);
            for (int i = 0; i < register.Length; i++)
            {
                if (textBoxRegister.Text[i].Equals('1'))
                    register[i] = true;
            }
            for (int i = 0; i < length; i++)
            {
                key[i] = register[0];
                bool xor = register[0] ^ register[35];
                for (int j = 0; j < 38; j++)
                {
                    register[j] = register[j + 1];
                }
                register[38] = xor;
            }
        }

        public BitArray xor(String a, String b)
        {
            BitArray bits = new BitArray(a.Length);
            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < a.Length; k++)
                sb.Append(a[k] ^ b[k + (Math.Abs(a.Length - b.Length))]);
            for (int i = 0; i < a.Length; i++)
            {
                bits[i] = sb[i].Equals("1");
            }
            return bits;
        }

        private byte[] BitsToBytes(BitArray bits)
        {
            byte[] bytes = new byte[bits.Length / 8];
            int index = 0;
            int pow = 7;
            foreach (bool bit in bits)
            {
                bytes[index / 8] += Convert.ToByte(bit ? Math.Pow(2, pow) : 0);
                //Console.WriteLine(bytes[index / 8]);
                index++;
                pow = pow == 0 ? 7 : pow - 1;
            }
            return bytes;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1.Enabled = false;
            GenerateKey(defaultBits.Length);
            PrintBits(key, richTextBox1);
           // defaultBits = xor(defaultBits.ToString(), key.ToString());
            defaultBits.Xor(key);
            PrintBits(defaultBits, richTextBox3);
            bytes = BitsToBytes(defaultBits);
            using (FileStream fsNew = new FileStream("result.txt", FileMode.Create, FileAccess.Write))
            {
                fsNew.Write(bytes, 0, bytes.Length);
            }
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            richTextBox2.Text = "";
            richTextBox3.Text = "";
            openFileDialog1.ShowDialog();
            fileName = openFileDialog1.FileName;
            button1.Enabled = true;
            bytes = ReadtBytes(fileName);
            defaultBits = GetBitArray(bytes);
            PrintBits(defaultBits, richTextBox2);
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
