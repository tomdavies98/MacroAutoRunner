using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MacroAutoRunner.LowLevelInputSimulator;

namespace MacroAutoRunner
{
    public class AutoRunner
    {
        private bool _isRunning { get; set; }
        private ScanCodeShort AutoRunKey {get;set;}
        private List<ScanCodeShort> KeysToPress { get; set; }
        public AutoRunner()
        {
            Console.InputEncoding = Encoding.GetEncoding(1252);
            Console.OutputEncoding = Encoding.GetEncoding(1252);
            KeysToPress = new List<ScanCodeShort>();
        }

        public void ToggleRunning()
        {
            if(_isRunning)
            {
                StopRun();
            }
            else
            {
                Run();
            }
        }

        private void Run()
        {
            foreach(var key in KeysToPress)
            {
                HoldKey(key);
            }
            _isRunning = true;
        }

        private void StopRun()
        {
            //Release
            foreach (var key in KeysToPress)
            {
                ReleaseKey(key);
            }
            
            _isRunning = false;
        }
      
        public void StartAutoRunner()
        {
            Console.WriteLine("=========================\n Made by Tom D 06/06/21 \n=========================\n");


            Console.WriteLine("What key do you want to toggle the auto run?");
            var lookingForToggleKey = true;
            
            while(lookingForToggleKey)
            {
                var autoRunKeyToggle = Console.ReadLine().Trim();
                var res = StringToScanCodeShort(autoRunKeyToggle);

                if (res.Item1)
                {
                    Console.WriteLine($"Key set to toggle auto run: {res.Item2}");
                    lookingForToggleKey = false;
                    AutoRunKey = res.Item2;
                }
                else
                {
                    Console.WriteLine($"Key inputted is not supported: {res.Item2}");
                }
            }
                       
            Console.WriteLine("How many keys do you want to press for auto run? (These keys will be pressed at the same time)");
            var lookingForNumKeys = true;
            int numKeysToPress = 0;
            while(lookingForNumKeys)
            {
                var res = Console.ReadLine();
                if(int.TryParse(res, out numKeysToPress))
                {
                    lookingForNumKeys = false;
                }
                else
                {
                    Console.WriteLine("You need to enter in the NUMBER (integer) of keys you wish to press");
                }
            }


            Console.WriteLine($"**Type 'help' for supported special keys**");
            for (int i = 0; i<numKeysToPress; i++)
            {
                Console.WriteLine($"Type your key, number: {i+1}");
                var lookingForAccceptedKey = true;

                while (lookingForAccceptedKey)
                {
                    var enteredKey = Console.ReadLine().Trim();

                    if (enteredKey == "help")
                    {
                        var keyScanCodeDict = GetKeyToScanCodeShort();
                        var keys = keyScanCodeDict.Keys.ToList();
                        Console.WriteLine(String.Join(",\n", keys));
                    }
                    else
                    {
                        var result = StringToScanCodeShort(enteredKey);
                        if (result.Item1)
                        {
                            lookingForAccceptedKey = false;
                            //AutoRunKey = result.Item2;
                            KeysToPress.Add(result.Item2);
                            Console.WriteLine($"Success: Added key {enteredKey} to auto run list");
                        }
                        else
                        {
                            Console.WriteLine($"Error: key {enteredKey} is not supported, please enter another key\n");
                        }
                    }
                }

            }
            var border = "==============================================================";
            Console.WriteLine($"{border}\n Auto run has started, press {AutoRunKey} to toggle...\n{border}\n");
            _isRunning = false;
            KeyboardListener keyListen = new KeyboardListener();
            keyListen.KeyDown += ProcessKeyDown;
            System.Windows.Forms.Application.Run();
        }


        private void ProcessKeyDown(object sender, RawKeyEventArgs args)
        {
            var key = args.ToString();

            if (TryConvertKeyStringToScanCodeShort(key, out var convertedKey))
            {
                if (convertedKey == AutoRunKey)
                {
                    ToggleRunning();
                }
            }
        }

        private bool TryConvertKeyStringToScanCodeShort(string key, out ScanCodeShort convertedKey)
        {
            var result = StringToScanCodeShort(key);
            convertedKey = result.Item2;
            return result.Item1;
        }

        public static void HoldKey(ScanCodeShort key)
        {
            var Inputs = new INPUT[1];
            var Input = new INPUT();
            Input.type = 1;
            Input.U.ki.wScan = key;
            Input.U.ki.dwFlags = KEYEVENTF.SCANCODE;
            Inputs[0] = Input;
            SendInput(1, Inputs, INPUT.Size);
        }

        public static void ReleaseKey(ScanCodeShort key)
        {
            var Inputs = new INPUT[1];
            var Input = new INPUT();
            Input.type = 1;
            Input.U.ki.wScan = key;
            Input.U.ki.dwFlags = KEYEVENTF.KEYUP | KEYEVENTF.SCANCODE;
            Inputs[0] = Input;
            SendInput(1, Inputs, INPUT.Size);
        }

        public static void HoldLShift()
        {
            var Inputs = new INPUT[1];
            var Input = new INPUT();
            Input.type = 1;
            Input.U.ki.wScan = ScanCodeShort.LSHIFT;
            Input.U.ki.dwFlags = KEYEVENTF.SCANCODE;
            Inputs[0] = Input;
            SendInput(1, Inputs, INPUT.Size);
        }
        public static void ReleaseLShift()
        {
            var Inputs = new INPUT[1];
            var Input = new INPUT();
            Input.type = 1;
            Input.U.ki.wScan = ScanCodeShort.LSHIFT;
            Input.U.ki.dwFlags = KEYEVENTF.KEYUP | KEYEVENTF.SCANCODE;
            Inputs[0] = Input;
            SendInput(1, Inputs, INPUT.Size);
        }
    }
}
