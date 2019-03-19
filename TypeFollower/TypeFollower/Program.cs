﻿using System;
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
				if (args.Count() < 4)
					Console.WriteLine("Wrong number of arguments!");
				else
				{					
					ComparationResultType resultType = ComparationResultType.JSON;
					string filenameTypeNameMap = args.Count() > 4 ? args[4] : string.Empty;
					TypeFollow tf = new TypeFollow(args[0], args[1], args[2], args[3], filenameTypeNameMap);

					string resultFileName = args.Count() > 5 ? args[5] : $"CompareResult.txt";
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
