﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConsoleApp;
using ConsoleApp.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Shape.Triangle.Test
{
    [TestClass]
    public class TriangleTest
    {
        [TestMethod]
        public void File_WithValidJSONFormat()
        {
            //arrange
            AllShapes<int> allShapes = new AllShapes<int>();
            var file = new FileInfo(CreateTempFile(new string[] { "[{'id': 1, 'b': 34, 'h': 34.5}]" }));

            try
            {
                //act
                allShapes.Triangles.LoadShapes(file);
                List<IShape<int>> igc = allShapes.GetAllShapes<int>();

                //assert
                Assert.IsTrue(igc.Any(i => i.Id == 1 && igc.Cast<IJson>().Any(j => Math.Round(j.CalculateArea()) == 1173)));
            }
            finally
            {
                File.Delete(file.FullName);
            }
        }

        [TestMethod]
        public void File_WithInvalidJSONFormat()
        {
            //arrange
            AllShapes<int> allShapes = new AllShapes<int>();
            var file = new FileInfo(CreateTempFile(new string[] { "[ 'id': 1, 'b': 34, 'h': 34.5 ]" }));

            try
            {
                //assert
                Assert.ThrowsException<JsonSerializationException>(() => allShapes.Triangles.LoadShapes(file));
            }
            finally
            {
                File.Delete(file.FullName);
            }
        }

        [TestMethod]
        public void File_WithInvalidJSONPropertyNames()
        {
            //arrange
            AllShapes<int> allShapes = new AllShapes<int>();
            var file = new FileInfo(CreateTempFile(new string[] { "[{'idd': 1, 'bbb': 34, 'hhh': 34.5}]" }));

            try
            {
                //assert
                Assert.ThrowsException<ArgumentException>(() => allShapes.Triangles.LoadShapes(file));
            }
            finally
            {
                File.Delete(file.FullName);
            }
        }

        [TestMethod]
        public void File_WithJSONSortedProperly()
        {
            //arrange
            AllShapes<int> allShapes = new AllShapes<int>();
            var file = new FileInfo(CreateTempFile(new string[] { "[{'id': 1, 'b': 34, 'h': 34.5},{'id': 5, 'b': 22, 'h': 8},{'id': 1, 'b': 20, 'h': 21}]" }));

            string[,] expectedResults = new string[,]
            {
                {"Triangle", "1", "1173" },
                {"Triangle", "1", "420" },
                {"Triangle", "5", "176" },
            };

            try
            {
                //act
                allShapes.Triangles.LoadShapes(file);
                List<IShape<int>> testResults = allShapes.GetAllShapes<int>();

                //assert - validate that the sorted test results match the sorted expected results
                int counter = 0;
                foreach (var tr in testResults)
                {
                    Assert.AreEqual(tr.Name, expectedResults[counter, 0]);
                    Assert.AreEqual(tr.Id.ToString(), expectedResults[counter, 1]);
                    Assert.AreEqual(((IJson)tr).CalculateArea().ToString(), expectedResults[counter, 2]);

                    counter++;
                }
            }
            finally
            {
                File.Delete(file.FullName);
            }
        }

        //creates temporary testing file
        string CreateTempFile(string[] fileData)
        {
            string file = Path.GetTempFileName();
            File.WriteAllLines(file, fileData);

            return file;
        }
    }
}
