using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

namespace simInterface
{
    public class simInterface
    {
        private ConcurrentDictionary<string, string> listValues = new ConcurrentDictionary<string, string>();
        public long currentValue
        {
            get
            {
                lock (lockpad) { return _currentValue; }
            }
            set
            {
                lock (lockpad) { _currentValue = Convert.ToInt32(value); }
            }
        }


        public long totalValue
        {
            get
            {
                lock (lockpad) { return _totalValue; }
            }
            set
            {
                lock (lockpad) { _totalValue = Convert.ToInt32(value); }
            }
        }
        public bool drawing { get; set; }
        public bool exit { get; set; }
        public int FPS { get; set; }

        private volatile int _totalValue;
        private volatile int _currentValue;
        private string header;


        private string spinner = "▲►▼◄";
        private int indexSpinner = 0;
        private object lockpad = new object();

        private Stopwatch sleepTimer = new Stopwatch();


        public simInterface(string h, int fps = 4)
        {
            totalValue = currentValue = 1;
            drawing = exit = true;
            header = h;
            FPS = fps;

            Task.Factory.StartNew(redraw, TaskCreationOptions.LongRunning).Wait();
        }

        public void addProp(string key, string value)
        {
            listValues.AddOrUpdate(key, value, (k, o) => value);
        }

        private void redraw()
        {
            while (exit)
                if (drawing)
                {
                    lock (lockpad)
                    {
                        Console.Clear();
                        sleepTimer.Start();

                        int pereight = Convert.ToInt32((_currentValue * 76) / _totalValue);
                        float percent = (_currentValue * 100) / _totalValue;
                        Console.WriteLine();

                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.WriteLine();
                        Console.WriteLine("    " + header);
                        Console.WriteLine();

                        Console.ResetColor();

                        Console.WriteLine();
                        Console.WriteLine();

                        foreach (var v in listValues)
                        {
                            Console.WriteLine(string.Format("    {0} = {1}", v.Key, v.Value));
                        }

                        Console.WriteLine();

                        Console.WriteLine("    Total Items = " + totalValue.ToString());
                        Console.WriteLine("    Done Items  = " + currentValue.ToString());

                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine("    " + percent.ToString() + "%");
                        Console.WriteLine();

                        string output = string.Empty;
                        for (var x = 0; x < 76; x++)
                        {
                            output += (x < 4) ? " " : (x > pereight) ? "░" : "█";
                        }

                        Console.WriteLine(output);
                        Console.WriteLine();
                        Console.WriteLine();

                        if (indexSpinner > spinner.Length - 1)
                            indexSpinner = 1;
                        else
                            indexSpinner++;

                        Console.WriteLine("    " + spinner.Substring(indexSpinner - 1, 1));



                        Console.WriteLine();
                        Console.WriteLine();

                        while (sleepTimer.ElapsedMilliseconds < (1000 / FPS))
                        {

                        }
                        sleepTimer.Restart();
                    }

                }


        }
    }
}
