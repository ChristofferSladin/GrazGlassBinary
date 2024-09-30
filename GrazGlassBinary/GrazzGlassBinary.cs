namespace GrazGlassBinary;

public static class GrazzGlassBinary
{
    const double GLASS_CAPACITY = 1.0;
    const double FLOW_RATE = 1.0 / 10.0;
    const double EPSILON = 1e-6;
    const double MAX_TIME = 1e9;

    public static void Run()
    {
        Console.Write("Row ? ");
        if (!int.TryParse(Console.ReadLine(), out int targetRow) || targetRow < 2 || targetRow > 50)
        {
            Console.WriteLine("Invalid row number. Please enter an integer between 2 and 50.");
            return;
        }

        Console.Write("Glass ? ");
        if (!int.TryParse(Console.ReadLine(), out int targetGlass) || targetGlass < 1 || targetGlass > targetRow)
        {
            Console.WriteLine($"Invalid glass number. Please enter an integer between 1 and {targetRow}.");
            return;
        }

        double overflowTime = BinarySearchOverflowTime(targetRow, targetGlass);

        Console.WriteLine($"Det tar {overflowTime:F3} sekunder.");
    }

    private static double BinarySearchOverflowTime(int targetRow, int targetGlass)
    {
        double low = 0.0;
        double high = MAX_TIME;
        double result = high;

        while (high - low > EPSILON)
        {
            double mid = (low + high) / 2.0;
            double inflow = ComputeWater(mid, targetRow, targetGlass);

            if (inflow >= GLASS_CAPACITY)
            {
                result = mid;
                high = mid;
            }
            else
            {
                low = mid;
            }
        }

        return result;
    }

    private static double ComputeWater(double time, int targetRow, int targetGlass)
    {
        double[,] flow = new double[targetRow + 2, targetRow + 2];

        flow[1, 1] = time * FLOW_RATE;

        // Iterate through each glass to distribute overflow
        for (int row = 1; row <= targetRow; row++)
        {
            for (int glass = 1; glass <= row; glass++)
            {
                if (flow[row, glass] > GLASS_CAPACITY)
                {
                    double overflow = flow[row, glass] - GLASS_CAPACITY;
                    flow[row, glass] = GLASS_CAPACITY;

                    // Distribute overflow based on special rules for row 3
                    if (row == 2 && row + 1 == 3)
                    {
                        if (glass == 1)
                        {
                            // From row 2, glass 1 to row 3, glass 1 and 2
                            flow[3, 1] += overflow * (1.0 / 3.0);
                            flow[3, 2] += overflow * (2.0 / 3.0);
                        }
                        else if (glass == 2)
                        {
                            // From row 2, glass 2 to row 3, glass 2 and 3
                            flow[3, 2] += overflow * (2.0 / 3.0);
                            flow[3, 3] += overflow * (1.0 / 3.0);
                        }
                    }
                    else
                    {
                        // Standard distribution: equally to the two glasses below
                        flow[row + 1, glass] += overflow * 0.5;
                        flow[row + 1, glass + 1] += overflow * 0.5;
                    }
                }
            }
        }

        return flow[targetRow, targetGlass];
    }
}
