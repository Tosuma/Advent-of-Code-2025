using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode;

internal static class Day03
{
    record BatteryBank(Battery[] Batteries);
    record Battery(int Joltage, int Index, int BankSize);
    public static void Run(TextReader reader)
    {
        var batteryBanks = reader.ReadBatteryBanks().ToList();

        var sumPart1 = batteryBanks.Sum(bank => bank.SelectBatteries(2).Join());
        var sumPart2 = batteryBanks.Sum(bank => bank.SelectBatteries(12).Join());

        Console.WriteLine($"Sum of joltage  (2 batteries) : {sumPart1} --> Is correct ? {sumPart1 == 17311}");
        Console.WriteLine($"Sum of joltage (12 Batteries) : {sumPart2} --> Is correct ? {sumPart2 == 171419245422055}");
    }

    private static long Join(this IEnumerable<Battery> batteries) =>
        batteries.OrderBy(b => b.Index).Aggregate(0L, (acc, b) => acc * 10 + b.Joltage);

    private static IEnumerable<Battery> SelectBatteries(this BatteryBank bank, int remainingSlots)
    {
        var firstAvailableBattery = 0;

        while (remainingSlots > 0)
        {
            var candidates = bank.Batteries
                .Where(b => b.Index >= firstAvailableBattery)
                .Where(b => b.Index <= bank.Batteries.Length - remainingSlots);
            var joltage = candidates.Max(b => b.Joltage);
            var selectedBattery = candidates.Where(b => b.Joltage == joltage).MinBy(b => b.Index)!;

            yield return selectedBattery;

            firstAvailableBattery = selectedBattery.Index + 1;
            remainingSlots--;
        }
    }

    private static IEnumerable<BatteryBank> ReadBatteryBanks(this TextReader reader) =>
        reader.ReadLines().Select(ToBatteryBank);

    private static BatteryBank ToBatteryBank(this string line) =>
        new BatteryBank(line.Select((c, i) => c.ToBattery(i, line.Length)).ToArray());

    private static Battery ToBattery(this char c, int index, int bankSize) =>
        new Battery(int.Parse(c.ToString()), index, bankSize);
}
