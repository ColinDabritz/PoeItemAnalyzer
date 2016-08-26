using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ItemModel;

namespace ItemModelUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string itemCopyText =
@"Rarity: Currency
Scroll of Wisdom
--------
Stack Size: 1/40
--------
Identifies an item
--------
Right click this item then left click an unidentified item to apply it.
";

            var item = new LootItem(itemCopyText);

            Assert.AreEqual(expected: 4, actual: item.RawSections.Length, message: "Unexpected number of sections");
        }
    }
}
