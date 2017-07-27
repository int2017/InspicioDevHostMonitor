using System;
using System.IO;
using System.Diagnostics;

namespace InspicioDevHostMonitor
{
	public class WebInstance
	{
		private string folder;
		private Process process;
		private string branchName = "development";

		public string Name { get; }
		public int Port { get; }

		public WebInstance(string folder)
		{
			this.folder = folder;
		}

		internal void Restart()
		{
			if (process != null)
			{
				process.Kill();
				Console.WriteLine("... waiting for existing process to exit");
				process.WaitForExit();
				Console.WriteLine("... existing process gone");
			}

			UpdateToNextBuild();

			process = new Process();
			process.StartInfo = new ProcessStartInfo("dotnet")
			{
				Arguments = "Inspicio.dll",
				WorkingDirectory = folder
			};

			process.StartInfo.Environment.Add("ASPNETCORE_ENVIRONMENT", "Development");
			process.StartInfo.Environment.Add("InspicioBasePath", $"/{branchName}");
			process.StartInfo.Environment.Add($"ConnectionStrings__DefaultConnection", $"Server=localhost\\SQLEXPRESS;Database=InspicioDB_{branchName};Trusted_Connection=True;");

			process.Start();
		}

		private void UpdateToNextBuild()
		{
			var pendingFolder = folder + ".next";
			if (Directory.Exists(pendingFolder))
			{
				if (!Directory.Exists(folder))
				{
					Console.WriteLine("...starting new branch");
				}
				else
				{
					Console.WriteLine("...Updating to next build");
					Directory.Delete(folder, true);
				}
				Directory.Move(pendingFolder, folder);
			}
		}
	}
}