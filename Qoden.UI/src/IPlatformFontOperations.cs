using System;
namespace Qoden.UI
{
	public interface IPlatformFontOperations
	{
		bool IsFont(object font);
		PlatformFont? FontWithSize(string name, float size);
	}
}
