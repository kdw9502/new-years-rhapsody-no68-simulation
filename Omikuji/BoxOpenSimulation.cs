using System;
using System.Linq;
using System.Threading.Tasks;

const int simulationCount = 100000;

void BoxOpenSimulation()
{
    const int boxCountPerReset = 300;
    const int winBoxCount = 4;
    int SimulateBox(int boxPerTry)
    {
        Span<bool> boxes = stackalloc bool[boxCountPerReset];
        var random = new Random();
        for (int i = 0; i < winBoxCount;)
        {
            int index = random.Next(boxCountPerReset);
            if (boxes[index])
                continue;
            
            boxes[index] = true;
            i++;
        }

        int boxIndex = 0;
        for (int winCount = 0; winCount < winBoxCount && boxIndex < boxCountPerReset;)
        {
            for (int indexOfThisTry = 0; indexOfThisTry < boxPerTry; indexOfThisTry++, boxIndex++)
            {
                if (boxes[boxIndex])
                    winCount++;   
            }
        }

        int totalBoxToAllPrize = boxIndex + 1;
        return totalBoxToAllPrize;
    }

    void RunSimulate(int boxPerTry)
    {
        int[] results = new int[simulationCount];
        Parallel.For(0, simulationCount, i =>
        {
            results[i] = SimulateBox(boxPerTry);
        });
        Console.WriteLine($"한번에 {boxPerTry}개 오픈시 평균 : {results.Average():F1} ");
    }

    for (int i = 0; i < 5; i++)
    {
        RunSimulate(1);
        RunSimulate(10);
        RunSimulate(30);
        RunSimulate(50);    
    }
}

void OmikujiOpenSimulation()
{

    (int tryCount, int pieceCount) SimulateOmikuji()
    {
        int tryCount = 0;
        int totalPieceCount = 0;
        var random = new Random();

        Span<double> probabilities = stackalloc double[(int)OmikujiResult.대길];
        probabilities[(int)OmikujiResult.흉] = 1.0;
        probabilities[(int)OmikujiResult.말길] = 20.0;
        probabilities[(int)OmikujiResult.소길] = 29.0;
        probabilities[(int)OmikujiResult.중길] = 25.0;
        probabilities[(int)OmikujiResult.길] = 20.0;
        Span<double> bonusProbabilities = stackalloc double[(int)OmikujiResult.대길];
        bonusProbabilities[(int)OmikujiResult.흉] = -0.1;
        bonusProbabilities[(int)OmikujiResult.말길] = -2;
        bonusProbabilities[(int)OmikujiResult.소길] = -2.9;
        bonusProbabilities[(int)OmikujiResult.중길] = -2.5;
        bonusProbabilities[(int)OmikujiResult.길] = -2;
        Span<int> pieceCount = stackalloc int[(int) OmikujiResult.대길 + 1];
        pieceCount[(int)OmikujiResult.흉] = 5;
        pieceCount[(int)OmikujiResult.말길] = 1;
        pieceCount[(int)OmikujiResult.소길] = 1;
        pieceCount[(int)OmikujiResult.중길] = 2;
        pieceCount[(int)OmikujiResult.길] = 3;
        pieceCount[(int)OmikujiResult.대길] = 5;

        while (true)
        {
            tryCount++;
            var val = random.NextDouble() * 100;
            OmikujiResult result;
            for (result = OmikujiResult.흉; result < OmikujiResult.대길; result++)
            {
                val -= probabilities[(int) result];
                if (val < 0)
                {
                    totalPieceCount += pieceCount[(int)result];
                    break;
                }
            }

            if (result == OmikujiResult.대길)
            {
                totalPieceCount += pieceCount[(int) OmikujiResult.대길];
                return (tryCount, totalPieceCount);
            }
            else
            {
                if (tryCount < 5)
                    continue;
                for (result = OmikujiResult.흉; result < OmikujiResult.대길; result++)
                {
                    probabilities[(int) result] += bonusProbabilities[(int) result];
                }
            }
        }
    }

    void RunSimulate()
    {
        var results = new (int tryCount, int pieceCount)[simulationCount];
        Parallel.For(0, simulationCount, i =>
        {
            results[i] = SimulateOmikuji();
        });
        int totalTryCount = results.Sum(x => x.tryCount);
        int totalPieceCount = results.Sum(x => x.pieceCount);
        
        Console.WriteLine($"오미쿠지 총 횟수: {totalTryCount} 총 엘레프 갯수: {totalPieceCount} 대길까지 평균 횟수 : {totalTryCount / (float)simulationCount:F3} 엘레프 평균 : {totalPieceCount/(double)totalTryCount:F3} ");
    }

    for (int i = 0; i < 5; i++)
    {
        RunSimulate();
    }
}

// BoxOpenSimulation();
OmikujiOpenSimulation();


enum OmikujiResult
{
    흉 = 0,
    말길,
    소길,
    중길,
    길,
    대길,
}