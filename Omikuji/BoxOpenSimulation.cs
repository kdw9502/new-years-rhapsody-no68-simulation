using System;
using System.Linq;
using System.Threading.Tasks;


const int boxCountPerReset = 300;
const int winBoxCount = 4;
const int simulationCount = 100000;
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
