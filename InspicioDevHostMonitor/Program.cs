using System;
using System.Reactive;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.CommandLineUtils;
using System.Reactive.Linq;

namespace InspicioDevHostMonitor
{

	class Program
    {
		static int Main(string[] args)
		{
			var watchFolder = ParseCommandLine(args);

			if (string.IsNullOrWhiteSpace(watchFolder))
			{
				return 1;
			}

			var fileSystemWatcher = new FileSystemWatcher(watchFolder)
			{
				IncludeSubdirectories = true,
				EnableRaisingEvents = true,
			};


			var webInstance = new WebInstance(Path.Combine(watchFolder, "development"));
			webInstance.Restart();
			//var apacheControl = new ApacheControl(input.Value.virtualHostFile);

			System.Threading.Tasks.Task.Run(() =>
			{
				Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h => fileSystemWatcher.Changed += h, h => fileSystemWatcher.Changed -= h)
					.Throttle(TimeSpan.FromSeconds(30))
					.Subscribe(_ =>
					{
						Console.WriteLine("Restarting the web server");

						webInstance.Restart();

						// Stop apache
						//apacheControl.Stop();

						// Loop through all folders in root folder & start new Kestrel with settings based on folder name


						// Write Apache vhosts config with correct URLs & ports for forward stuff
						//apacheControl.UpdateVirtualHosts(webInstances.Select(x => Tuple.Create));
						//apacheControl.Start();

						Console.WriteLine("Hosting updated & restarted");
					});
			});
			
			// Wait forever
			new System.Threading.AutoResetEvent(false).WaitOne();
			return 0;
		}

		

		private static string ParseCommandLine(string[] args)
		{
			var commandLineApplication = new CommandLineApplication();

		/*	var virtualHostFile = commandLineApplication.Option("--virtualHostFile <path>",
			  "The path to the Apache vhost config file that will be replaced with a new config after each branch build", CommandOptionType.SingleValue);*/

			var rootFolder = commandLineApplication.Option("--root <path>",
			  "The root folder that will contain the various branch folders", CommandOptionType.SingleValue);

			commandLineApplication.HelpOption("-? | -h | --help");
			commandLineApplication.OnExecute(() =>
			{
				var error = false;


			/*	if (!virtualHostFile.HasValue())
				{
					Console.WriteLine($"{virtualHostFile.LongName} is required");
					error = true;
				}
				*/
				if (!rootFolder.HasValue())
				{
					Console.WriteLine($"{rootFolder.LongName} is required");
					error = true;
				}
				return error ? 1 : 0;
			});

			if (commandLineApplication.Execute(args) == 0)
			{
				return rootFolder.Value();
			}
			else
			{
				return null;
			}
		}
	}
}