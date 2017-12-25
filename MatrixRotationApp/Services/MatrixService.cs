using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace MatrixRotationApp.Services
{
    public class MatrixService : IMatrixService
    {
        private const char Separator = ';';

        private readonly string _matrixPath = "/App_Data/matrix.csv";

        public async Task<int[][]> ReadMatrixAsync()
        {
            try
            {
                using (var fileStream = File.OpenRead(HostingEnvironment.MapPath("~" + _matrixPath) ?? _matrixPath))
                {
                    return await ToMatrixAsync(fileStream);
                }
            }
            catch(DirectoryNotFoundException)
            {
                // Nothing;
            }
            catch (FileNotFoundException)
            {
                // Nothing;
            }

            return null;
        }

        public void RotateMatrixСlockwise(int[][] matrix)
        {
            if (matrix == null || matrix.Length == 0)
                return;

            if (!IsMatrixSquare(matrix))
                throw new ArgumentOutOfRangeException();

            var maxIndex = matrix.Length - 1;
            int tmp;

            for (int col = 0; col <= maxIndex / 2; col++)
            {
                for (int row = col; row < maxIndex - col; row++)
                {
                    tmp = matrix[col][row];
                    matrix[col][row] = matrix[maxIndex - row][col];
                    matrix[maxIndex - row][col] = matrix[maxIndex - col][maxIndex - row];
                    matrix[maxIndex - col][maxIndex - row] = matrix[row][maxIndex - col];
                    matrix[row][maxIndex - col] = tmp;
                }
            }
        }

        public async Task WriteMatrixAsync(int[][] matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException(nameof(matrix));

            if (!IsMatrixSquare(matrix))
                throw new ArgumentOutOfRangeException();

            using (var writer = new StreamWriter(HostingEnvironment.MapPath("~" + _matrixPath) ?? _matrixPath))
            {
                for (int i = 0; i < matrix.Length; i++)
                {
                    var line = String.Join(Separator.ToString(), matrix[i].Select(x => x.ToString()).ToArray());
                    await writer.WriteLineAsync(line);
                }
            }
        }

        public async Task<int[][]> ToMatrixAsync(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            List<int[]> lines = new List<int[]>();

            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lines.Add(line.Split(Separator).Select(x => int.Parse(x)).ToArray());
                }
            }

            return lines.ToArray();
        }

        private bool IsMatrixSquare(int[][] matrix)
        {
            return !matrix.Any(x => x.Length != matrix.Length);
        }
    }
}