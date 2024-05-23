using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Drawing;
namespace 任务状态
{
    class FontNum
    {
        class NumDot
        {
            public string Name { get; set; }
            public byte[] Array { get; set; }
            public NumDot(string name, byte[] array)
            {
                Name = name;
                Array = array;
            }
        }
        NumDot[] numDot;
        public byte[] Array(string name)
        {
            NumDot dot = numDot.FirstOrDefault(n => n.Name == name);
            return dot != null ? dot.Array : null;
        }
        public void DrawNum(Bitmap bitmap, String name,int x,int y,Color color,Color bcolor)
        {
            byte[] array = Array(name);
            Console.WriteLine("Set Name: " + name + " Array: " + toString(array));
            for (int i=0;i<array.Length;i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (((x + j) < bitmap.Width) & ((y + i) < bitmap.Height))
                    {
                        if ((array[i] & (byte)(1 << j)) > 0)
                        {
                            bitmap.SetPixel(x + j, y + i, color);
                        }
                        else
                        {
                            bitmap.SetPixel(x + j, y + i, bcolor);
                        }
                    }
                }
            }
        }
        public void DrawString(Bitmap bitmap, String text, int x, int y, Color color, Color bcolor)
        {
            for(int i=0;i<text.Length;i++)
            {
                DrawNum(bitmap, text[i].ToString(), x+i*8, y,color, bcolor);
            }
        }
        public String toString(byte[] array)
        {
            string strTemp = "";
            for (int i = 0; i < array.Length; i++)
            {
                strTemp += array[i].ToString("X2");
                strTemp += " ";
            }
            return strTemp;
        }
        public FontNum(String path)
        {
            int line_count = 0,offset=0;
            String[] texts = File.ReadAllLines(path);
            for (int i = 0; i < texts.Length; i++)
            {
                if (texts[i].IndexOf("{") > -1)
                {
                    Console.WriteLine("Line: " + texts[i]);
                    line_count++;
                }
            }
            numDot = new NumDot[line_count];
            for (int i= 0;i< texts.Length;i++)
            {
                if (texts[i].IndexOf("{") > -1)
                {
                    int startIndex = texts[i].IndexOf('{');
                    int endIndex = texts[i].LastIndexOf('}');
                    string content = texts[i].Substring(startIndex + 1, endIndex - startIndex - 1);
                    string[] contents = content.Split(',');
                    Regex regex = new Regex("\"([^\"]*)\"");
                    Match match = regex.Match(texts[i]);
                    String name = "";
                    if (match.Success)
                    {
                        name = match.Groups[1].Value;
                    }
                    byte[] array = new byte[contents.Length]; 
                    for (int n = 0; n < contents.Length; n++)
                    {
                        array[n] = Convert.ToByte(contents[n].Trim(), 16);
                    }
                    numDot[offset] = new NumDot(name, array);
                    //Console.WriteLine("Name: " + numDot[offset].Name+","+i);
                    //Console.WriteLine("Array: " + toString(numDot[offset].Array));
                    offset++;
                }
               
            }
            foreach (NumDot num in numDot)
            {
                Console.WriteLine("Name: " + num.Name+ " Array: " + toString(num.Array));
            }
        }
    }
}
