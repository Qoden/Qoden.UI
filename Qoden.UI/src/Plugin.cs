using System;
using System.Reflection;
using DeviceInfo = Plugin.DeviceInfo.CrossDeviceInfo;
#pragma warning disable CS1701 // Assuming assembly reference matches identity
namespace Qoden.UI
{
	public class Plugin
	{
		internal static T Load<T>(string name)
		{
			var deviceInfo = DeviceInfo.Current;
			var assembly = Assembly.Load(new AssemblyName($"Qoden.UI.Platform.{deviceInfo.Platform}"));
			var t = assembly.GetType($"Qoden.UI.Platform.{deviceInfo.Platform}.{name}", true);
			return (T)Activator.CreateInstance(t);
		}
	}
}
