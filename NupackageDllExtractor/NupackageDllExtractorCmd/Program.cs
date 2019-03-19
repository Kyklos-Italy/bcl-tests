using Common.Logging;
using NupackageDllExtractorLib;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NupackageDllExtractorCmd
{
	class Program
	{
		static void Main(string[] args)
		{
			log4net.Config.XmlConfigurator.Configure();
			ILog Logger = LogManager.GetLogger("NupackageDllExtractorCmd");

			string sourceFolder;
			string destinationFolder;
			bool bcleanDestination = false;
			string customFolderFilter = "debug";
			string versionFilter = string.Empty;

			try
			{
				int paramCount = args.Count();

				if (paramCount < 2)
				{
					string msg = $"Wrong number of arguments ({paramCount})!";
					Console.WriteLine(msg);
					Logger.Error(msg);

					StringBuilder sb = new StringBuilder($"Usage: {Assembly.GetExecutingAssembly().GetName().Name} <sourceFolder> <destinationFolder> [/c] [/f <subfolderName>] [/v <version>]");
					Console.WriteLine(sb.ToString());
				}
				else
				{
					sourceFolder = args[0];
					if (string.IsNullOrEmpty(sourceFolder) || sourceFolder.StartsWith("/"))
					{
						string msg = $"Source folder was not specified!";
						Console.WriteLine(msg);
						Logger.Error(msg);
					}
					else
					{
						if (!Directory.Exists(sourceFolder))
						{
							string msg = $"Source folder {sourceFolder} was not found!";
							Console.WriteLine(msg);
							Logger.Error(msg);
						}
						else
						{
							destinationFolder = args[1];
							if (string.IsNullOrEmpty(destinationFolder) || destinationFolder.StartsWith("/"))
							{
								string msg = $"Destination folder was not specified!";
								Console.WriteLine(msg);
								Logger.Error(msg);
							}
							else
							{
								bool bparamError = false;
								for (int i = 2; i < paramCount; i++)
								{
									string cmdparam = args[i];
									if (cmdparam.ToLower() == "/c")
									{
										bcleanDestination = true;
									}
									if (cmdparam.ToLower() == "/f")
									{
										if (paramCount < i + 1)
										{
											string msg = "Custom folder filter not specified for switch /f";
											Console.WriteLine(msg);
											Logger.Error(msg);
											bparamError = true;
											break;
										}
										i = i + 1;
										customFolderFilter = args[i];
									}
									if (cmdparam.ToLower() == "/v")
									{
										if (paramCount < i + 1)
										{
											string msg = "Version filter not specified for switch /v";
											Console.WriteLine(msg);
											Logger.Error(msg);
											bparamError = true;
											break;
										}
										i = i + 1;
										versionFilter = args[i];
									}
								}
								if (!bparamError)
								{
									DllExtractor extractor = new DllExtractor(sourceFolder, destinationFolder, bcleanDestination, customFolderFilter, versionFilter);
									string msg = extractor.Extract();
									Console.WriteLine(msg);
									Logger.Debug(msg);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				string msg = "Unexpected error during extraction";
				Console.WriteLine(msg);
				Logger.Error(msg, ex);
			}
		}
	}
}
