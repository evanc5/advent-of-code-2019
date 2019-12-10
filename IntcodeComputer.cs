using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventUtils
{
    public class IntcodeComputer
    {
        private readonly IDictionary<long, long> Memory;
        private long RelativeBase;
        private long CurrentPos;

        public IntcodeComputer()
        {
            RelativeBase = 0;
            CurrentPos = 0;
        }

        public IntcodeComputer(IEnumerable<long> intcode) : this()
        {
            long idx = 0;
            Memory = intcode.ToDictionary(key => idx++, value => value);
        }

        public IntcodeComputer(IDictionary<long, long> intcode) : this()
        {
            Memory = intcode;
        }

        public long Run()
        {
            long output;
            return Run(out output);
        }

        public long Run(out long output, params long[] inputParams)
        {
            RelativeBase = 0;
            CurrentPos = 0;
            output = -1;
            var inputList = inputParams.ToList();
            var sw = System.Diagnostics.Stopwatch.StartNew();

            while (true)
            {
                string fullOpcode = Memory[CurrentPos].ToString().PadLeft(5, '0');
                long opcode = long.Parse(fullOpcode.Substring(3, 2));
                long param1Mode = long.Parse(fullOpcode.Substring(2, 1));
                long param2Mode = long.Parse(fullOpcode.Substring(1, 1));
                long param3Mode = long.Parse(fullOpcode.Substring(0, 1));

                long param1 = 0;
                long param2 = 0;
                long param3 = 0;

                long param1Key = 0;
                long param2Key = 0;
                long param3Key = 0;

                if (new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }.Contains(opcode))
                {
                    if (!Memory.ContainsKey(CurrentPos + 1)) Memory.Add(CurrentPos + 1, 0);
                    switch (param1Mode)
                    {
                        case 0:
                            param1Key = Memory[CurrentPos + 1];
                            if (!Memory.ContainsKey(param1Key)) Memory.Add(param1Key, 0);
                            param1 = Memory[param1Key];
                            break;
                        case 1:
                            param1 = Memory[CurrentPos + 1];
                            break;
                        case 2:
                            param1Key = RelativeBase + Memory[CurrentPos + 1];
                            if (!Memory.ContainsKey(param1Key)) Memory.Add(param1Key, 0);
                            param1 = Memory[param1Key];
                            break;
                    }
                }

                if (new long[] { 1, 2, 5, 6, 7, 8 }.Contains(opcode))
                {
                    if (!Memory.ContainsKey(CurrentPos + 2)) Memory.Add(CurrentPos + 2, 0);
                    switch (param2Mode)
                    {
                        case 0:
                            param2Key = Memory[CurrentPos + 2];
                            if (!Memory.ContainsKey(param2Key)) Memory.Add(param2Key, 0);
                            param2 = Memory[param2Key];
                            break;
                        case 1:
                            param2 = Memory[CurrentPos + 2];
                            break;
                        case 2:
                            param2Key = RelativeBase + Memory[CurrentPos + 2];
                            if (!Memory.ContainsKey(param2Key)) Memory.Add(param2Key, 0);
                            param2 = Memory[param2Key];
                            break;
                    }
                }

                if (new long[] { 1, 2, 7, 8 }.Contains(opcode))
                {
                    if (!Memory.ContainsKey(CurrentPos + 3)) Memory.Add(CurrentPos + 3, 0);
                    switch (param3Mode)
                    {
                        case 0:
                            param3Key = Memory[CurrentPos + 3];
                            if (!Memory.ContainsKey(param3Key)) Memory.Add(param3Key, 0);
                            param3 = Memory[param3Key];
                            break;
                        case 1:
                            param3 = Memory[CurrentPos + 3];
                            break;
                        case 2:
                            param3Key = RelativeBase + Memory[CurrentPos + 3];
                            if (!Memory.ContainsKey(param3Key)) Memory.Add(param3Key, 0);
                            param3 = Memory[param3Key];
                            break;
                    }
                }

                switch (opcode)
                {
                    case 1:
                        Memory[param3Key] = param1 + param2;
                        CurrentPos += 4;
                        break;
                    case 2:
                        Memory[param3Key] = param1 * param2;
                        CurrentPos += 4;
                        break;
                    case 3:
                        Console.Write("Input: ");
                        string input;
                        if (inputList.Any())
                        {
                            input = inputList[0].ToString();
                            Console.WriteLine(input);
                            inputList.RemoveAt(0);
                        }
                        else
                        {
                            sw.Stop();
                            input = Console.ReadLine();
                            sw.Start();
                        }
                        Memory[param1Key] = int.Parse(input.Trim());
                        CurrentPos += 2;
                        break;
                    case 4:
                        Console.WriteLine($"Output: {param1}");
                        output = param1;
                        CurrentPos += 2;
                        break;
                    case 5:
                        if (param1 != 0) CurrentPos = param2;
                        else CurrentPos += 3;
                        break;
                    case 6:
                        if (param1 == 0) CurrentPos = param2;
                        else CurrentPos += 3;
                        break;
                    case 7:
                        Memory[param3Key] = param1 < param2 ? 1 : 0;
                        CurrentPos += 4;
                        break;
                    case 8:
                        Memory[param3Key] = param1 == param2 ? 1 : 0;
                        CurrentPos += 4;
                        break;
                    case 9:
                        RelativeBase += param1;
                        CurrentPos += 2;
                        break;
                    case 99:
                        sw.Stop();
                        System.Diagnostics.Debug.WriteLine(sw.Elapsed);
                        return Memory[0];
                    default:
                        throw new Exception($"Unknown opcode: {opcode} at {CurrentPos}");
                }
            }
        }
    }
}