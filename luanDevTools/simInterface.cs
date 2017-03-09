using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

namespace luanDevTools
{
    public class simInterface
    {
        /// <summary>
        /// Progress Bar Minimum value
        /// </summary>
        public long progressBarCurrent
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

        /// <summary>
        /// Progress Bar maximum value
        /// </summary>
        public long progressBarMaximum
        {
            get
            {
                lock (lockpad) { return _totalValue; }
            }
            set
            {
                lock (lockpad) { _totalValue = Convert.ToInt32((value == 0)? 1 : value); }
            }
        }
        public bool isDrawing { get; set; }
        private bool isRunning { get; set; }
        public int FPS { get; set; }

        private volatile int _totalValue;
        private volatile int _currentValue;
        private string header;
        private ConcurrentDictionary<string, string> listValues = new ConcurrentDictionary<string, string>();


        private string spinner = "▲►▼◄";
        private int indexSpinner = 0;
        private object lockpad = new object();

        private Stopwatch sleepTimer = new Stopwatch();

        /// <summary>
        /// Start the interface drawing, remember to call a Stop method to stop it's rendering
        /// </summary>
        /// <param name="Header">Interface title (it cannot be changed)</param>
        /// <param name="fps">Redrawing frequency</param>
        public simInterface(string Header, int fps = 4)
        {
            progressBarMaximum = progressBarCurrent = 1;
            isDrawing = isRunning = true;
            header = Header;
            FPS = fps;

            Task.Factory.StartNew(redraw, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Properties to be listed in the interface
        /// </summary>
        /// <param name="key">Description</param>
        /// <param name="value">Value</param>
        public void addProperty(string key, string value)
        {
            listValues.AddOrUpdate(key, value, (k, o) => value);
        }


        /// <summary>
        /// Disables interface
        /// </summary>
        public void stop()
        {
            isDrawing = isRunning = false;
        }



        private void redraw()
        {
            while (isRunning)
                if (isDrawing)
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


                        if(progressBarMaximum > 1 || progressBarCurrent > 1)
                        {
                            Console.WriteLine("    Total Items = " + progressBarMaximum.ToString());
                            Console.WriteLine("    Done Items  = " + progressBarCurrent.ToString());

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
                            
                        }

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
