using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        double currentTime;
         List<Process> proccesses;
        List<RunTime> runtimes;
        ProcessGenerator processgen;
        double averageTurnaround;
        double averageWeightedTurnaround;
        public Form1()
        {
            processgen = new ProcessGenerator();
            proccesses = new List<Process>();
            runtimes = new List<RunTime>();
            currentTime = 0;
            averageTurnaround = 0;
            averageWeightedTurnaround = 0;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        

        }
        private void FCFS()
        {
      
        }
        private void SRTN()
        {
            //sorting proccesses based on their arrival time
            proccesses.Sort(delegate (Process p1, Process p2)
            {
                return p1.arrivalTime.CompareTo(p2.arrivalTime);
            });
            for (int i = 0; i < proccesses.Count; i++)
            {
                RunTime Temp=new RunTime();
                Temp.processId = proccesses[i].processId;
               // Temp.runTime = proccesses[i].burstTime;
               // currentTime += proccesses[i].burstTime;
               // proccesses[i]->finishTime = currentTime;
              //  runtimes.Add(Temp);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            processgen.Generate();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader(@"C:\\Users\\egypt2\\source\\repos\\WindowsFormsApp2\\output.txt");
            int processCount = Convert.ToInt32(sr.ReadLine());
            string[] processString;
            proccesses = new List<Process>();
            for (int i = 0; i < processCount; i++)
            {
                processString = sr.ReadLine().Split(null);
                Process Temp = new Process();
                Temp.arrivalTime = Convert.ToDouble(processString[1]);
                Temp.burstTime= Convert.ToDouble(processString[2]);
                Temp.remainingTime = Convert.ToDouble(processString[2]);
                Temp.processId= Convert.ToInt32(processString[0]);
                Temp.priority= Convert.ToInt32(processString[3]);
                proccesses.Add(Temp);
            }

        }
        //First come First Serve
        private void button1_Click(object sender, EventArgs e)
        {
            this.chart1.Series["Series1"].Points.Clear();
            //sorting proccesses based on their arrival time
            currentTime = 0;
            double contextSwitching = Convert.ToDouble(textBox2.Text);
            proccesses.Sort(delegate (Process p1, Process p2)
            {
                return p1.arrivalTime.CompareTo(p2.arrivalTime);
            });
            currentTime = proccesses[0].arrivalTime + contextSwitching;
            for (int i = 0; i < proccesses.Count; i++)
            {   
                RunTime Temp=new RunTime();
                Temp.processId = proccesses[i].processId;
                Temp.runTime = proccesses[i].burstTime;
                Temp.start = Math.Max(proccesses[i].arrivalTime, currentTime);
                proccesses[i].startTime = Temp.start;
                Temp.end = Temp.start + proccesses[i].burstTime;
                currentTime = Temp.end;
                proccesses[i].finishTime = currentTime;
    
                runtimes.Add(Temp);
                //adding context switching
                currentTime += contextSwitching;
            }
            for (int i = 0; i < proccesses.Count; i++)
            {
                proccesses[i].responseTime = proccesses[i].finishTime - proccesses[i].arrivalTime - proccesses[i].burstTime;
                proccesses[i].turnaroundTime = proccesses[i].finishTime - proccesses[i].arrivalTime;
                proccesses[i].weightedTurnaroundTime = proccesses[i].turnaroundTime / proccesses[i].burstTime;
                averageTurnaround += proccesses[i].turnaroundTime;
                averageWeightedTurnaround += proccesses[i].weightedTurnaroundTime;

            }
            for (int i = 0; i < runtimes.Count; i++)
            {
                this.chart1.Series["Series1"].Points.AddXY(runtimes[i].processId, runtimes[i].start, runtimes[i].end);

            }
            averageTurnaround = averageTurnaround / proccesses.Count;
            averageWeightedTurnaround = averageWeightedTurnaround / proccesses.Count;
            this.label3.Text = "average turnaround time:" + averageTurnaround;
            this.label4.Text = "average weighted turnaround time:" + averageWeightedTurnaround;
        }
        //Round Robin
        private void button2_Click(object sender, EventArgs e)
        {
            this.chart1.Series["Series1"].Points.Clear();
            List<Process> scheduled = new List<Process>();
            double contextSwitching = Convert.ToDouble(textBox2.Text);

            proccesses.Sort(delegate (Process p1, Process p2)
            {
                return p1.arrivalTime.CompareTo(p2.arrivalTime);
            });
            currentTime = proccesses[0].arrivalTime;
            //adding context switching
            currentTime += contextSwitching;
            double quantumSlice = Convert.ToDouble(textBox1.Text);
            runtimes = new List<RunTime>();

            bool finished = false;
            bool hasNotFinished = false;
            int currentIndex = 0;
            int index = 0;
            while (scheduled.Count!=0||index<proccesses.Count)
            {   
                for(int i = index; i < proccesses.Count; i++)
                {
                    if (proccesses[i].arrivalTime <= currentTime)
                    {
                        index++;
                        scheduled.Add(proccesses[i]);
                    }
                    else { break; }
                }
                //hasNotFinished = false;
                //finished = true;
                //for (int i = 0; i < scheduled.Count; i++)
                //{
                    if (scheduled[0].remainingTime == 0)
                    {
                    int procIndex =proccesses.FindIndex(r => r.processId == scheduled[0].processId);
                    proccesses[procIndex] = scheduled[0];
                        scheduled.RemoveAt(0);
                    }
                    
                    else if (scheduled[0].remainingTime < quantumSlice )
                    {
                        finished = false;
                        RunTime temp = new RunTime();
                        temp.processId = scheduled[0].processId;
                        temp.runTime = scheduled[0].remainingTime;
                        temp.start = currentTime;
                        currentTime += scheduled[0].remainingTime;
                        scheduled[0].remainingTime -= scheduled[0].remainingTime;
                        temp.end = currentTime;
                        scheduled[0].finishTime = currentTime;

                    runtimes.Add(temp);
                        Process reverse = scheduled[0];
                        scheduled.RemoveAt(0);
                    for (int i = index; i < proccesses.Count; i++)
                    {
                        if (proccesses[i].arrivalTime <= currentTime)
                        {
                            index++;
                            scheduled.Add(proccesses[i]);
                        }
                        else { break; }
                    }
                    scheduled.Add(reverse);
                    //adding context switching
                    currentTime += contextSwitching;
                }
                    else
                    {
                        
                            finished = false;
                            //currentIndex = Math.Max(currentIndex, i);
                            RunTime temp = new RunTime();
                            temp.processId = scheduled[0].processId;
                            temp.runTime = quantumSlice;
                            temp.start = currentTime;
                            if (scheduled[0].remainingTime == scheduled[0].burstTime) scheduled[0].startTime=currentTime ;
                            scheduled[0].remainingTime -= quantumSlice;
                            currentTime += quantumSlice;
                            temp.end = currentTime;

                            runtimes.Add(temp);
                            Process reverse = scheduled[0];
                            scheduled.RemoveAt(0);
                    for (int i = index; i < proccesses.Count; i++)
                    {
                        if (proccesses[i].arrivalTime <= currentTime)
                        {
                            index++;
                            scheduled.Add(proccesses[i]);
                        }
                        else { break; }
                    }
                    scheduled.Add(reverse);
                    //adding context switching
                    currentTime += contextSwitching;


                }
                //}
                if (scheduled.Count==0&index<proccesses.Count)
                {
                    currentTime = proccesses[index].arrivalTime;
                    //adding context switching
                    currentTime += contextSwitching;
                    finished = false;
                }
            }
            for (int i = 0; i < proccesses.Count; i++)
            {
                proccesses[i].responseTime = proccesses[i].finishTime - proccesses[i].arrivalTime - proccesses[i].burstTime;
                proccesses[i].turnaroundTime = proccesses[i].finishTime - proccesses[i].arrivalTime;
                proccesses[i].weightedTurnaroundTime = proccesses[i].turnaroundTime / proccesses[i].burstTime;
                averageTurnaround += proccesses[i].turnaroundTime;
                averageWeightedTurnaround += proccesses[i].weightedTurnaroundTime;
                
            }
            for(int i = 0; i < runtimes.Count; i++)
            {
                this.chart1.Series["Series1"].Points.AddXY(runtimes[i].processId, runtimes[i].start, runtimes[i].end);
            }
            averageTurnaround = averageTurnaround / proccesses.Count;
            averageWeightedTurnaround = averageWeightedTurnaround / proccesses.Count;
            this.label3.Text = "average turnaround time:" + averageTurnaround;
            this.label4.Text = "average weighted turnaround time:" + averageWeightedTurnaround;

        }
        //Highest Priority First
        private void button3_Click(object sender, EventArgs e)
        {
            this.chart1.Series["Series1"].Points.Clear();
            double contextSwitching = Convert.ToDouble(textBox2.Text);
            proccesses.Sort(delegate (Process p1, Process p2)
            {
                return p1.arrivalTime.CompareTo(p2.arrivalTime);
            });
            currentTime = proccesses[0].arrivalTime;
            currentTime += contextSwitching;
            runtimes = new List<RunTime>();
            List<Process> scheduled = new List<Process>();
            int index = 0;
            while (runtimes.Count < proccesses.Count)
            {
                //add new arriving processes to the scheduled list
                for(int i = index; i < proccesses.Count; i++)
                {
                    if (proccesses[i].arrivalTime <= currentTime)
                    {
                        index++;
                        scheduled.Add(proccesses[i]);
                    }
                    else
                    {
                        break;
                    }
                }
                if (scheduled.Count != 0) { 
                    scheduled.Sort(delegate (Process p1, Process p2)
                    {
                        if (p1.priority != p2.priority) return p2.priority.CompareTo(p1.priority);
                        else return p1.processId.CompareTo(p2.processId);
                    });
                    RunTime temprun = new RunTime();
                    int processId = scheduled[0].processId;
                    int processIdx = proccesses.FindIndex(r=>r.processId==processId);
                    scheduled.RemoveAt(0);
                    proccesses[processIdx].startTime = currentTime;
                    proccesses[processIdx].finishTime = currentTime + proccesses[processIdx].burstTime;
                    temprun.processId = processId;
                    temprun.runTime = proccesses[processIdx].burstTime;
                    temprun.start = proccesses[processIdx].startTime;
                    temprun.end = proccesses[processIdx].finishTime;
                    currentTime = temprun.end;
                    runtimes.Add(temprun);
                    //adding context switching
                    currentTime += contextSwitching;

                }
                else if(scheduled.Count==0&&index!=proccesses.Count)
                {

                    currentTime = proccesses[index].arrivalTime;
                    //adding context switching
                    currentTime += contextSwitching;
                }
            }
            for (int i = 0; i < proccesses.Count; i++)
            {
                proccesses[i].responseTime = proccesses[i].finishTime - proccesses[i].arrivalTime - proccesses[i].burstTime;
                proccesses[i].turnaroundTime = proccesses[i].finishTime - proccesses[i].arrivalTime;
                proccesses[i].weightedTurnaroundTime = proccesses[i].turnaroundTime / proccesses[i].burstTime;
                averageTurnaround += proccesses[i].turnaroundTime;
                averageWeightedTurnaround += proccesses[i].weightedTurnaroundTime;

            }
            for (int i = 0; i < runtimes.Count; i++)
            {
                this.chart1.Series["Series1"].Points.AddXY(runtimes[i].processId, runtimes[i].start, runtimes[i].end);

            }
            averageTurnaround = averageTurnaround / proccesses.Count;
            averageWeightedTurnaround = averageWeightedTurnaround / proccesses.Count;
            this.label3.Text = "average turnaround time:" + averageTurnaround;
            this.label4.Text = "average weighted turnaround time:" + averageWeightedTurnaround;
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
        //shortest remaining time first
        private void button4_Click(object sender, EventArgs e)
        {
            this.chart1.Series["Series1"].Points.Clear();
            double contextSwitching = Convert.ToDouble(textBox2.Text);
            proccesses.Sort(delegate (Process p1, Process p2)
            {
                return p1.arrivalTime.CompareTo(p2.arrivalTime);
            });
            currentTime = proccesses[0].arrivalTime;
            currentTime += contextSwitching;
            double nextTime = proccesses[1].arrivalTime;
            runtimes = new List<RunTime>();
            List<Process> scheduled = new List<Process>();
            int index = 0;
            bool finished = false;
            bool hasNotFinished = true;
            while (!finished)
            {
                hasNotFinished = true;
                finished = true;
                for(int i = index; i < proccesses.Count; i++)
                {
                    if (proccesses[i].arrivalTime <= currentTime)
                    {
                        scheduled.Add(proccesses[i]);
                        index++;
                    }
                    else
                    {
                        break;
                    }
                }
                if (index < proccesses.Count&&scheduled.Count!=0) {
                    finished = false;
                    scheduled.Sort(delegate (Process p1, Process p2)
                    {
                        return p1.remainingTime.CompareTo(p2.remainingTime);
                    });
                    RunTime temp = new RunTime();
                    int processId = scheduled[0].processId;
                    int processIdx = proccesses.FindIndex(r => r.processId == processId);
                    if (proccesses[processIdx].remainingTime == proccesses[processIdx].remainingTime){ proccesses[processIdx].startTime = currentTime;}
                    temp.start = currentTime;
                    temp.processId = processId;
                    double timeDiff = proccesses[index].arrivalTime - currentTime;
                    double workingTime = Math.Min(timeDiff, proccesses[processIdx].remainingTime);
                    temp.runTime = workingTime;
                    proccesses[processIdx].remainingTime -= workingTime;
                    currentTime += workingTime;
                    temp.end = currentTime;
                    runtimes.Add(temp);
                    if (proccesses[processIdx].remainingTime == 0)
                    {
                        proccesses[processIdx].finishTime = currentTime;
                        scheduled.RemoveAt(0);
                    }
                    //adding context switching
                    currentTime += contextSwitching;

                }
                else if (scheduled.Count != 0)
                {
                    finished = false;
                    scheduled.Sort(delegate (Process p1, Process p2)
                    {
                        return p1.remainingTime.CompareTo(p2.remainingTime);
                    });
                    RunTime temp = new RunTime();
                    int processId = scheduled[0].processId;
                    int processIdx = proccesses.FindIndex(r => r.processId == processId);
                    if (proccesses[processIdx].remainingTime == proccesses[processIdx].remainingTime) { proccesses[processIdx].startTime = currentTime; }
                    temp.start = currentTime;
                    temp.processId = processId;
                    double workingTime =  proccesses[processIdx].remainingTime;
                    temp.runTime = workingTime;
                    proccesses[processIdx].remainingTime -= workingTime;
                    currentTime += workingTime;
                    temp.end = currentTime;
                    runtimes.Add(temp);
                    proccesses[processIdx].finishTime = currentTime;
                    scheduled.RemoveAt(0);
                    //adding context switching
                    currentTime += contextSwitching;
                 
                }
                if (finished && index<proccesses.Count&&scheduled.Count==0)
                {
                    currentTime = proccesses[index].arrivalTime;
                    //adding context switching
                    currentTime += contextSwitching;
                    finished = false;
                }

            }
            for(int i = 0; i < proccesses.Count; i++)
            {
                proccesses[i].responseTime = proccesses[i].finishTime - proccesses[i].arrivalTime - proccesses[i].burstTime;
                proccesses[i].turnaroundTime = proccesses[i].finishTime - proccesses[i].arrivalTime;
                proccesses[i].weightedTurnaroundTime = proccesses[i].turnaroundTime / proccesses[i].burstTime;
                averageTurnaround += proccesses[i].turnaroundTime;
                averageWeightedTurnaround += proccesses[i].weightedTurnaroundTime;

            }
            
            for (int i = 0; i < runtimes.Count; i++)
            {
                this.chart1.Series["Series1"].Points.AddXY(runtimes[i].processId, runtimes[i].start, runtimes[i].end);
                
            }
            averageTurnaround = averageTurnaround / proccesses.Count;
            averageWeightedTurnaround = averageWeightedTurnaround / proccesses.Count;
            this.label3.Text = "average turnaround time:" + averageTurnaround;
            this.label4.Text = "average weighted turnaround time:" + averageWeightedTurnaround;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
        





