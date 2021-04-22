using System.Text.Json;
public static class JsonElementExtension
{
	public static JsonElement? GetPropertyExt(this JsonElement jsonElement, string propertyName)
	{
		if (jsonElement.TryGetProperty(propertyName, out JsonElement returnElement))
		{
			return returnElement;
		}
		else 
		{
			return null;
		}
	}

}
