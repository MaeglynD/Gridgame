using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace Gridgame._1_Web_API1.Controllers
{
    [Route("[controller]")]
    public class GridgameController : Controller
    {
        // Dimension of matrix e.g 5 = 5x5
        public static int multiplier = 5;

        // Matrix
        public static bool[,] matrix = init();

        // Re-define size of matrix, populate with 'false'
        public static bool[,] init()
        {
            bool[,] matrix = new bool[multiplier, multiplier];

            int row = matrix.GetLength(0);
            int col = matrix.GetLength(1);

            for (int i = 0; i < row * col; i++)
            {
                matrix[i / col, i % col] = false;
            }


            // Assign, at random, a true value
            Random r = new Random();
            matrix[r.Next(row), r.Next(col)] = true;

            return matrix;
        }

        // Alter values around a given entry
        void alterMatrix(int a, int b)
        {
            int[][] tests = {
                new int[] { a, b },     // center
				new int[] { a - 1, b }, // north
				new int[] { a + 1, b }, // south
				new int[] { a, b + 1 }, // east
				new int[] { a, b - 1 }  // west
			};

            bool lambda(int x) => x < multiplier && x > -1;

            foreach (var x in tests)
            {
                if (lambda(x[0]) && lambda(x[1]))
                {
                    matrix[x[0], x[1]] ^= true;
                }
            }
        }

        // Reset matrix, post back result
        [HttpPost("PostReset")]
        public string postReset([FromBody] MultiplierModel vm)
        {
            multiplier = vm.multiplier;
            matrix = init();
            return getMatrix();
        }

        // Get current matrix
        [HttpGet("GetMatrix")]
        public string getMatrix()
        {
            return JsonConvert.SerializeObject(matrix);
        }

        // Alter an entry
        [HttpPost("PostAlterEntry")]
        public string postAlterEntry([FromBody] EntryModel vm)
        {
            alterMatrix(vm.x, vm.y);
            return getMatrix();
        }
    }

    public class MultiplierModel
    {
        public int multiplier { get; set; }
    }

    public class EntryModel
    {
        public int x { get; set; }
        public int y { get; set; }
    }
}