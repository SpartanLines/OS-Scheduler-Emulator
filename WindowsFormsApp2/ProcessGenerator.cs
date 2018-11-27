using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathNet.Numerics.Distributions;
using MathNet.Numerics;
using System.IO;

namespace WindowsFormsApp2
{
    public class ProcessGenerator
    {
       

        public void Generate()
        {
            String[] line = new String[6];

            StreamReader sr = new StreamReader(@"C:\\Users\\egypt2\\source\\repos\\WindowsFormsApp2\\input.txt");

            line[0] = sr.ReadLine();
            int i = 1;

            while (line != null)
            {
                if (i == 4) break;
                line[i] = sr.ReadLine();
                i++;
            }
            string[] ArrivalParams = line[1].Split(null);
            string[] BurstParams = line[2].Split(null);
            sr.Close();

            var poisson = new Poisson(Convert.ToDouble(line[3]));
            var arrivalTimeNormal = new Normal(Convert.ToDouble(ArrivalParams[0]), Convert.ToDouble(ArrivalParams[1]));
            var burstTimeNormal = new Normal(Convert.ToDouble(BurstParams[0]), Convert.ToDouble(BurstParams[1]));


            string[] text = new string[Convert.ToInt32(line[0]) + 1];
            text[0] = Convert.ToString(line[0]);

            for (i = 1; i <= Convert.ToInt32(line[0]); i++)
            {
                var arrivalT = Math.Abs(arrivalTimeNormal.Sample());
                var burstT = Math.Abs(burstTimeNormal.Sample());
                var prior = poisson.Sample();

                text[i] = Convert.ToString(i) + " " + Convert.ToString(arrivalT) + " " + Convert.ToString(burstT) + " " + Convert.ToString(prior);
            }
            System.IO.File.WriteAllLines(@"C:\\Users\\egypt2\\source\\repos\\WindowsFormsApp2\\output.txt", text);
            
        }
    }
}