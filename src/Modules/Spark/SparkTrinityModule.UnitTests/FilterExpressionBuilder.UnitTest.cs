using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.Modules.Spark;
using Trinity.Storage;
using Xunit;

namespace SparkTrinityModule.UnitTests
{
    public class FilterExpressionBuilderUnitTests : IDisposable
    {
        public FilterExpressionBuilderUnitTests()
        {
            TrinityConfig.LoggingLevel = Trinity.Diagnostics.LogLevel.Off;
            Global.LocalStorage.ResetStorage();
            Global.LocalStorage.SavePeople(1, "A", 10, 1000.0);
            Global.LocalStorage.SavePeople(2, "B", 12, 2000.0);
            Global.LocalStorage.SavePeople(3, "C", 13, 3000.0);
        }

        public void Dispose()
        {
            Global.LocalStorage.ResetStorage();
        }

        [Fact]
        public void Test()
        {
            var filtersJson = JsonConvert.SerializeObject(new object[]
            {
                new { @operator = "StringStartsWith", attr = "Name", value = "A" },
                new { @operator = "GreaterThan", attr = "Age", value = 3 },
                new { @operator = "LessThan", attr = "Salary", value = 5000.0 }
            });

            var filters = (JsonConvert.DeserializeObject(filtersJson) as JArray).Select(_ => _ as JObject);

            var cellDesc = Global.StorageSchema.CellDescriptors.FirstOrDefault(_ => _.TypeName == "People");
            var cells = Global.LocalStorage.GenericCellAccessor_Selector().AsQueryable();
            var filtersExpr = FilterExpressionBuilder.BuildExpression(cells, cellDesc, filters);
            var cellsFiltered = cells.Provider.CreateQuery<ICell>(filtersExpr);

            Assert.Equal(1, cellsFiltered.Count());
            foreach (var c in cellsFiltered)
            {
                Console.WriteLine(c.ToString());
            }
        }
    }
}
