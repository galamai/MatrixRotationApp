using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MatrixRotationApp.Services;
using MatrixRotationApp.Controllers;
using System.Threading.Tasks;
using System.Web.Mvc;
using MatrixRotationApp.Models;
using System.Web;
using System.IO;

namespace MatrixRotationApp.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTests
    {
        [TestMethod]
        public async Task Test_HomeController_Index_ViewResultNotNull()
        {
            var matrixService = Mock.Of<IMatrixService>();
            var controller = new HomeController(matrixService);
            var actionResult = await controller.Index();

            Assert.IsNotNull(actionResult);
        }

        [TestMethod]
        public async Task Test_HomeController_Index_ViewEqualDefaultCshtml()
        {
            var matrixService = Mock.Of<IMatrixService>();
            var controller = new HomeController(matrixService);
            var viewResult = (await controller.Index()) as ViewResult;

            Assert.AreEqual("", viewResult.ViewName);
        }

        [TestMethod]
        public async Task Test_HomeController_Index_MatrixInViewModel()
        {
            var matrix = new int[1][];
            var matrixServiceMock = new Mock<IMatrixService>();
            matrixServiceMock.Setup(x => x.ReadMatrixAsync()).ReturnsAsync(matrix);
            var controller = new HomeController(matrixServiceMock.Object);
            var viewResult = (await controller.Index()) as ViewResult;
            var model = viewResult.Model as HomeViewModel;

            Assert.AreEqual(matrix, model.Matrix);
        }

        [TestMethod]
        public async Task Test_HomeController_Index_ErrorInViewModel()
        {
            var error = "Error";
            var matrixService = Mock.Of<IMatrixService>();
            var controller = new HomeController(matrixService);
            var viewResult = (await controller.Index(error)) as ViewResult;
            var model = viewResult.Model as HomeViewModel;

            Assert.AreEqual(error, model.Error);
        }

        [TestMethod]
        public async Task Test_HomeController_Actions_RedirectToIndex()
        {
            var matrixService = Mock.Of<IMatrixService>();
            var controller = new HomeController(matrixService);
            var redirectResult = (await controller.Actions(null)) as RedirectToRouteResult;
            var routes = redirectResult.RouteValues;

            Assert.AreEqual("Home", routes["controller"]);
            Assert.AreEqual("Index", routes["action"]);
        }

        [TestMethod]
        public async Task Test_HomeController_Actions_RotateMatrixСlockwise()
        {
            var matrix = new int[1][];
            var matrixServiceMock = new Mock<IMatrixService>();
            matrixServiceMock.Setup(x => x.ReadMatrixAsync()).ReturnsAsync(matrix);
            var controller = new HomeController(matrixServiceMock.Object);
            await controller.Actions("RotateСlockwise");

            matrixServiceMock.Verify(x => x.ReadMatrixAsync(), Times.Once);
            matrixServiceMock.Verify(x => x.RotateMatrixСlockwise(matrix), Times.Once);
            matrixServiceMock.Verify(x => x.WriteMatrixAsync(matrix), Times.Once);
        }

        [TestMethod]
        public async Task Test_HomeController_UploadMatrix_RedirectToIndex()
        {
            var matrixService = Mock.Of<IMatrixService>();
            var file = Mock.Of<HttpPostedFileBase>();
            var controller = new HomeController(matrixService);
            var redirectResult = (await controller.UploadMatrix(file)) as RedirectToRouteResult;
            var routes = redirectResult.RouteValues;

            Assert.AreEqual("Home", routes["controller"]);
            Assert.AreEqual("Index", routes["action"]);
        }

        [TestMethod]
        public async Task Test_HomeController_UploadMatrix_RedirectToIndexIsFileNull()
        {
            var matrixService = Mock.Of<IMatrixService>();
            var controller = new HomeController(matrixService);
            var redirectResult = (await controller.UploadMatrix(null)) as RedirectToRouteResult;
            var routes = redirectResult.RouteValues;

            Assert.AreEqual("Home", routes["controller"]);
            Assert.AreEqual("Index", routes["action"]);
        }

        [TestMethod]
        public async Task Test_HomeController_Upload_WriteMatrix()
        {
            var matrix = new int[1][];
            var stream = Mock.Of<Stream>();
            var fileMock = new Mock<HttpPostedFileBase>();
            fileMock.Setup(x => x.InputStream).Returns(stream);
            var matrixServiceMock = new Mock<IMatrixService>();
            matrixServiceMock.Setup(x => x.ToMatrixAsync(stream)).ReturnsAsync(matrix);
            var controller = new HomeController(matrixServiceMock.Object);
            await controller.UploadMatrix(fileMock.Object);

            matrixServiceMock.Verify(x => x.WriteMatrixAsync(matrix), Times.Once);
        }
    }
}
