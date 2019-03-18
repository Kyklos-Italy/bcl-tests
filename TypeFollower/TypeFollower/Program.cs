using System;
using System.Linq;
using System.Reflection;

namespace TypeFollower
{
    public class Program
	{
        static void Main(string[] args)
		{
			try
			{
				if (args.Count() < 2)
					Console.WriteLine("Wrong number of arguments!");
				else
				{					
					ComparationResultType resultType = ComparationResultType.JSON;
					string filenameTypeNameMap = args.Count() > 2 ? args[2] : string.Empty;
					TypeFollow tf = new TypeFollow(args[0], args[1], filenameTypeNameMap);

					string resultFileName = args.Count() > 3 ? args[3] : $"CompareResult.html";
					tf.GenerateComparationResult(resultType, resultFileName);
					if (resultType == ComparationResultType.Console)
						Console.ReadKey();
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
				if (ex is ReflectionTypeLoadException)
				{
					var typeLoadException = ex as ReflectionTypeLoadException;
					var loaderExceptions = typeLoadException.LoaderExceptions;
					foreach (var lex in loaderExceptions)
					{
						Console.WriteLine(lex.Message);
					}
				}

				Console.ReadKey();
			}
		}
	}
}
