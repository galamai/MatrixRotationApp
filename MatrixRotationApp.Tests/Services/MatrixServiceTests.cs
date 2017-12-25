using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using MatrixRotationApp.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MatrixRotationApp.Tests.Services
{
    [TestClass]
    public class MatrixServiceTests
    {
        [TestMethod]
        public void Test_MatrixService_RotateMatrixСlockwise()
        {
            var matrix = new int[3][]
            {
                new int[3] { 1, 2, 3 },
                new int[3] { 4, 5, 6 },
                new int[3] { 7, 8, 9 }
            };
            var clockwiseMatrix = new int[3][]
            {
                new int[3] { 7, 4, 1 },
                new int[3] { 8, 5, 2 },
                new int[3] { 9, 6, 3 }
            };

            var service = new MatrixService();
            service.RotateMatrixСlockwise(matrix);

            Assert.IsTrue(IsMatrixEqual(clockwiseMatrix, matrix));
        }

        [TestMethod]
        public async Task Test_MatrixService_ToMatrixAsync()
        {
            var str = "1;2;3\r\n4;5;6\r\n7;8;9";
            var originMatrix = new int[3][]
            {
                new int[3] { 1, 2, 3 },
                new int[3] { 4, 5, 6 },
                new int[3] { 7, 8, 9 }
            };
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(str));
            var service = new MatrixService();
            var matrix = await service.ToMatrixAsync(stream);

            Assert.IsTrue(IsMatrixEqual(originMatrix, matrix));
        }

        [TestMethod]
        public async Task Test_MatrixService_ToMatrixAsync_WithNullStream()
        {
            var service = new MatrixService();
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => service.ToMatrixAsync(null));
        }

        [TestMethod]
        public async Task Test_MatrixService_ReadMatrixAsync()
        {
            var directoryPath = "/App_Data";
            Directory.CreateDirectory(directoryPath);
            var str = "1;2;3\r\n4;5;6\r\n7;8;9";
            var originMatrix = new int[3][]
            {
                new int[3] { 1, 2, 3 },
                new int[3] { 4, 5, 6 },
                new int[3] { 7, 8, 9 }
            };

            File.WriteAllText("/App_Data/matrix.csv", str);

            var service = new MatrixService();
            var matrix = await service.ReadMatrixAsync();

            Assert.IsTrue(IsMatrixEqual(originMatrix, matrix));

            Directory.Delete(directoryPath, true);
        }

        [TestMethod]
        public async Task Test_MatrixService_ReadMatrixAsync_FromNonExistentFile()
        {
            var service = new MatrixService();
            var matrix = await service.ReadMatrixAsync();

            Assert.IsNull(matrix);
        }

        [TestMethod]
        public async Task Test_MatrixService_WriteMatrixAsync()
        {
            var directoryPath = "/App_Data";
            Directory.CreateDirectory(directoryPath);

            var filePayload = "1;2;3\r\n4;5;6\r\n7;8;9\r\n";
            var matrix = new int[3][]
            {
                new int[3] { 1, 2, 3 },
                new int[3] { 4, 5, 6 },
                new int[3] { 7, 8, 9 }
            };

            var service = new MatrixService();
            await service.WriteMatrixAsync(matrix);

            Assert.IsTrue(File.Exists("/App_Data/matrix.csv"));
            Assert.AreEqual(filePayload, File.ReadAllText("/App_Data/matrix.csv"));

            Directory.Delete(directoryPath, true);
        }

        [TestMethod]
        public async Task Test_MatrixService_WriteMatrixAsync_WithNullMatrix()
        {
            var service = new MatrixService();
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => service.WriteMatrixAsync(null));
        }

        [TestMethod]
        public async Task Test_MatrixService_WriteMatrixAsync_WithNotSquareMatrix()
        {
            var matrix = new int[2][]
            {
                new int[3] { 1, 2, 3 },
                new int[3] { 4, 5, 6 }
            };

            var service = new MatrixService();
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => service.WriteMatrixAsync(matrix));
        }

        private bool IsMatrixEqual(int[][] clockwiseMatrix, int[][] matrix)
        {
            if (clockwiseMatrix.Length != matrix.Length)
                return false;

            for (int i = 0; i < clockwiseMatrix.Length; i++)
            {
                if (!Enumerable.SequenceEqual(clockwiseMatrix[i], matrix[i]))
                    return false;
            }

            return true;
        }
    }
}
