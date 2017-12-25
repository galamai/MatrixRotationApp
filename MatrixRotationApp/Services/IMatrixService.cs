using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixRotationApp.Services
{
    public interface IMatrixService
    {
        void RotateMatrixСlockwise(int[][] matrix);
        Task<int[][]> ReadMatrixAsync();
        Task WriteMatrixAsync(int[][] matrix);
        Task<int[][]> ToMatrixAsync(Stream stream);
    }
}
