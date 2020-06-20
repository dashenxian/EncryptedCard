using EncryptedCard;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreConsoleApp
{
    public class TaskHostedService : IHostedService
    {
        private readonly IConfiguration config;
        private readonly ILogger logger;

        public TaskHostedService(IConfiguration config, ILogger<TaskHostedService> logger)
        {
            this.config = config;
            this.logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine(nameof(TaskHostedService) + "已开始执行...");

            //TODO 业务逻辑代码，如下模拟
            Stopwatch stopwatch = Stopwatch.StartNew();

            var card = new Card(6,10).GenerateData();
            card.PrintCard();
            Console.WriteLine($"{JsonConvert.SerializeObject(card, Formatting.Indented)}");
            Console.WriteLine();

            WriteMessage("Load Data:", ConsoleColor.Yellow);
            var str = card.CellData;
            var card2 = new Card(6, 10).LoadCellData(str);
            card2.PrintCard();
            Console.WriteLine();

            WriteMessage("Matrix Card Validation:", ConsoleColor.Yellow);
            var cellsToValidate = card.PickRandomCells(5).ToList();
            var sb = new StringBuilder();
            foreach (var t in cellsToValidate)
            {
                sb.Append($"[{t.ColumnName}{t.RowIndex}] ");
            }
            Console.WriteLine($"Please input number(s) at {sb} (use ',' to seperate values):");
            var userInput = Console.ReadLine();
            if (userInput != null)
            {
                var inputArr = userInput.Split(",");
                if (inputArr.Length != cellsToValidate.Count)
                {
                    WriteMessage($"Invalid input, numbers doesn't match, must input {cellsToValidate.Count} numbers.", ConsoleColor.Red);
                }
                else
                {
                    var isValid = card.Validate(new List<CardCell>
                    {
                        new CardCell(cellsToValidate[0].RowIndex, cellsToValidate[0].ColIndex, int.Parse(inputArr[0])),
                        new CardCell(cellsToValidate[1].RowIndex, cellsToValidate[1].ColIndex, int.Parse(inputArr[1])),
                        new CardCell(cellsToValidate[2].RowIndex, cellsToValidate[2].ColIndex, int.Parse(inputArr[2]))
                    });
                    WriteMessage(isValid.ToString(), isValid ? ConsoleColor.Green : ConsoleColor.Red);
                }
            }

            stopwatch.Stop();

            logger.LogInformation("Logging - Execute Elapsed Times:{}ms", stopwatch.ElapsedMilliseconds);

            return Task.FromResult(0);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine(nameof(TaskHostedService) + "已被停止");
            return Task.FromResult(0);
        }
        static void WriteMessage(string message, ConsoleColor color = ConsoleColor.White, bool resetColor = true)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            if (resetColor)
            {
                Console.ResetColor();
            }
        }
    }

    public static class CardExtension
    {
        public static void PrintCard(this Card card)
        {
            var columns = Enum.GetNames(typeof(CardCell.ColumnCode));
            Console.Write(" |");
            for (int i = 0; i < card.Columns; i++)
            {
                Console.Write($"\t{columns[i]}");
            }
            Console.WriteLine();

            Console.Write("--");
            for (int i = 0; i < card.Columns; i++)
            {
                Console.Write("--------");
            }
            Console.WriteLine("--");


            for (int i = 0; i < card.Rows; i++)
            {
                Console.Write($"{i}|\t");
                for (int j = 0; j < card.Columns; j++)
                {
                    Console.Write($"{card[i, j]}\t");
                }
                Console.WriteLine();
            }
        }
    }

}
