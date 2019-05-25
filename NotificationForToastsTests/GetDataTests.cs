using Microsoft.VisualStudio.TestTools.UnitTesting;
using NotificationForToasts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationForToasts.Tests
{
    [TestClass()]
    public class GetDataTests
    {
        private GetData _getData = new GetData();
        [TestMethod()]
        public void getNewsTest()
        {
            List<NewsModel> gd = _getData.UpdateNews();
            Assert.IsTrue(gd != null && gd.Count > 0);
        }

        [TestMethod()]
        public void GetTop1NewsTest()
        {
            NewsModel item = _getData.GetTop1News();
            Assert.IsTrue(item != null);
        }
    }
}