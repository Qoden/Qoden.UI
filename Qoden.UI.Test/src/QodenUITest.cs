using System;
namespace Qoden.UI.Test
{
    public class QodenUITest
    {
        public QodenUITest()
        {
            Qoden.Util.Plugin.AddPlugin("Qoden.UI.PlatformViewOperations", new FakePlatformViewOperations());
        }
    }
}
