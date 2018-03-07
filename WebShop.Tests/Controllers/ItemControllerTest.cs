using System;
using WebShop;
using Moq;
using WebShop.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.Mvc;
using System.Web.Helpers;
using WebShop.Models;
using WebShop.Models.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
namespace WebShop.Tests.Controllers
{
    [TestClass]
    public class ItemControllerTest
    {
        [TestMethod]
        public void IndexViewResultNotNull()
        {
            ItemController controller = new ItemController();

            ViewResult result = controller.Index() as ViewResult;

            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void IndexViewEqualIndexCshtml()
        {
            ItemController controller = new ItemController();

            ViewResult result = controller.Index() as ViewResult;

            Assert.AreEqual("Index", result.ViewName);
        }
        [TestMethod]
        public void GetWithoutOrderViewResultNotNull()
        {
            ItemController controller = new ItemController();

            ViewResult result = controller.GetWithoutOrderView() as ViewResult;

            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void GetWithoutOrderViewEqualCshtml()
        {
            ItemController controller = new ItemController();

            ViewResult result = controller.GetWithoutOrderView() as ViewResult;

            Assert.AreEqual("GetWithoutOrderView", result.ViewName);
        }
        [TestMethod]
        public void UpdateModelSaveException()
        {
            // Arrange
            Exception ex = new Exception("Test Message");

            var mockRepository = new Mock<IGenericRepository<Item>>();
            Item OldItem = new Item()
            {
                Category = "test",
                Code = "11-1111-QQ66",
                Name = "TestName",
                Price = 9,
                Id = Guid.NewGuid()

            };
            Item NewItem = new Item()
            {
                Category = "testNew",
                Code = "22-2222-QQ66",
                Name = "TestNameNew",
                Price = 2,
                Id = OldItem.Id

            };
            mockRepository.Setup(a => a.GetByID(OldItem.Id)).Returns(OldItem);
            var mock = new Mock<IUnitOfWork>();
            mock.Setup(a => a.GetItems).Returns(mockRepository.Object);
            mock.Setup(a => a.Save()).Throws(ex);
            ItemController controller = new ItemController(mock.Object);

            string expected = "{\"Msg\":\"" + ex.Message + "\",\"success\":false}";
            // Act
            JsonResult result = controller.Update(NewItem);
            var serializer = new JavaScriptSerializer();


            string serialize = serializer.Serialize(result.Data);
            string actual = new JavaScriptSerializer().Serialize(result.Data);


            // Assert
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void UpdateModelTrueJsonResult()
        {
            // Arrange
            var mockRepository = new Mock<IGenericRepository<Item>>();
            Item OldItem = new Item()
            {
                Category = "test",
                Code = "11-1111-QQ66",
                Name = "TestName",
                Price = 9,
                Id = Guid.NewGuid()

            };
            Item NewItem = new Item()
            {
                Category = "testNew",
                Code = "22-2222-QQ66",
                Name = "TestNameNew",
                Price = 2,
                Id = OldItem.Id

            };
            mockRepository.Setup(a => a.GetByID(OldItem.Id)).Returns(OldItem);
            var mock = new Mock<IUnitOfWork>();
            mock.Setup(a => a.GetItems).Returns(mockRepository.Object);
            mock.Setup(a => a.Save());
            ItemController controller = new ItemController(mock.Object);

            string expected = Json.Encode(new 
                    {
                        Data = NewItem,
                        success = true
                    }
                );
            // Act
            JsonResult result = controller.Update(NewItem);
            var serializer = new JavaScriptSerializer();


            string serialize = serializer.Serialize(result.Data);
            string actual = new JavaScriptSerializer().Serialize(result.Data);


            // Assert
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void CreateModelSaveException()
        {
            // Arrange
            Exception ex = new Exception("Test Message");
            var mockRepository = new Mock<IGenericRepository<Item>>();
            Item item = new Item()
            {
                Category = "test",
                Code = "11-1111-QQ66",
                Name = "TestName",
                Price = 9,
                Id = Guid.NewGuid()

            };
            mockRepository.Setup(a => a.Insert(item));
            var mock = new Mock<IUnitOfWork>();
            mock.Setup(a => a.GetItems).Returns(mockRepository.Object);
            mock.Setup(a => a.Save()).Throws(ex);
            ItemController controller = new ItemController(mock.Object);

            string expected = "{\"Msg\":\"" + ex.Message + "\",\"success\":false}";
            // Act
            JsonResult result = controller.Create(item.Code, item.Name, item.Price, item.Category);
            var serializer = new JavaScriptSerializer();


            string serialize = serializer.Serialize(result.Data);
            string actual = new JavaScriptSerializer().Serialize(result.Data);


            // Assert
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void CreateModelTrueJsonResult()
        {
            // Arrange
            var mockRepository = new Mock<IGenericRepository<Item>>();
            Regex regex = new Regex("\"Id\":\"[A-Za-z0-9]{8}(-[A-Za-z0-9]{4}){3}-[A-Za-z0-9]{12}\",");
            Item item = new Item()
            {
                    Category = "test",
                    Code = "11-1111-QQ66",
                    Name = "TestName",
                    Price = 9,
                    Id = Guid.NewGuid()
                
            };
            mockRepository.Setup(a => a.Insert(item));
            //\"Id\"\:\"[A-Za-z0-9]{8}(-[A-Za-z0-9]{4}){3}-[A-Za-z0-9]{12}\"
            var mock = new Mock<IUnitOfWork>();
            mock.Setup(a => a.GetItems).Returns(mockRepository.Object);
            mock.Setup(a => a.Save());
            ItemController controller = new ItemController(mock.Object);
            string  JsobSerialize = Json.Encode(new {
                Data = item,
                success = true
            });
            string expected = regex.Replace(JsobSerialize,String.Empty);
            // Act
            JsonResult result = controller.Create(item.Code, item.Name,item.Price,item.Category);
            var serializer = new JavaScriptSerializer();
           
            
            string serialize = serializer.Serialize(result.Data);
            string actual = regex.Replace(serialize,String.Empty);
           

            // Assert
            Assert.AreEqual(expected, actual);
        }
       
        [TestMethod]
        public void DeleteModelTrueJsonResult()
        {
            // Arrange
            var mockRepository = new Mock<IGenericRepository<Item>>();
            Guid Id = Guid.NewGuid();
            mockRepository.Setup(a => a.DeleteById(Id));
           
            var mock = new Mock<IUnitOfWork>();
            mock.Setup(a => a.GetItems).Returns(mockRepository.Object);
            mock.Setup(a => a.Save());
            ItemController controller = new ItemController(mock.Object);
            string expected = "{\"Data\":\"" + Id.ToString() + "\",\"success\":true}";
            // Act
            JsonResult result =  controller.Delete(Id.ToString());
            string actual = new JavaScriptSerializer().Serialize(result.Data);
            // Assert
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]

        public void DeleteModelDeleteByIdException()
        {
            // Arrange
            var mockRepository = new Mock<IGenericRepository<Item>>();
            Guid Id = Guid.NewGuid();
            Exception ex = new Exception("Test Message");
            mockRepository.Setup(a => a.DeleteById(Id)).Throws(ex); ;

            var mock = new Mock<IUnitOfWork>();
           
            mock.Setup(a => a.GetItems).Returns(mockRepository.Object);
            mock.Setup(a => a.Save());
            ItemController controller = new ItemController(mock.Object);
            string expected = "{\"Msg\":\"" + ex.Message + "\",\"success\":false}";
            // Act
            JsonResult result = controller.Delete(Id.ToString());
            string actual = new JavaScriptSerializer().Serialize(result.Data);
            // Assert
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void DeleteModelSaveException()
        {
            // Arrange
            var mockRepository = new Mock<IGenericRepository<Item>>();
            Guid Id = Guid.NewGuid();
            mockRepository.Setup(a => a.DeleteById(Id));

            var mock = new Mock<IUnitOfWork>();
            Exception ex = new Exception("Test Message");
            mock.Setup(a => a.GetItems).Returns(mockRepository.Object);
            mock.Setup(a => a.Save()).Throws(ex);
            ItemController controller = new ItemController(mock.Object);
            string expected = "{\"Msg\":\"" + ex.Message + "\",\"success\":false}";
            // Act
            JsonResult result = controller.Delete(Id.ToString());
            string actual = new JavaScriptSerializer().Serialize(result.Data);
            // Assert
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void GetModelIsNull()
        {
            // Arrange
            var mockRepository = new Mock<IGenericRepository<Item>>();
            mockRepository.Setup(a=>a.Get()).Returns(new List<Item>());
            var mock = new Mock<IUnitOfWork>();
            mock.Setup(a => a.GetItems).Returns(mockRepository.Object);
            ItemController controller = new ItemController(mock.Object);
            // Act
            JsonResult result = controller.Get();
            
            // Assert
            Assert.IsNotNull(result.Data);
           
        }
        [TestMethod]
        public void GetModelJsonResult()
        {
            // Arrange
            var mockRepository = new Mock<IGenericRepository<Item>>();
            Guid Id = Guid.NewGuid();
            mockRepository.Setup(a => a.Get()).Returns(new List<Item>() { new Item(){
                Code="11-1111-AA11",
                Name ="Test",
                Price = 6,
                Category = "Test",
                Id = Id
                }
            });
            var mock = new Mock<IUnitOfWork>();
            mock.Setup(a => a.GetItems).Returns(mockRepository.Object);
            ItemController controller = new ItemController(mock.Object);
            var expected = Json.Encode(new
            {
                Data =new object []{ new
                {
                    Category = "Test",
                    Code = "11-1111-AA11",
                    Name = "Test",
                    Price = 6,
                    Id = Id
                }
                }
            });
            // Act
            JsonResult result = controller.Get();
            string actual = new JavaScriptSerializer().Serialize(result.Data); ;
            /*Console.WriteLine("{0}={1}",actual,expected);
            Console.ReadLine();*/
             // Assert
           
            Assert.AreEqual(expected, actual);
            

        }
        
        
    }
}
