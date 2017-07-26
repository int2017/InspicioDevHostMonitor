using System;
using System.Collections.Generic;

namespace InspicioDevHostMonitor
{
	internal class ApacheControl
	{
		private string virtualHostFile;

		public ApacheControl(string virtualHostFile)
		{
			this.virtualHostFile = virtualHostFile;
		}

		internal void UpdateVirtualHosts(IEnumerable<Tuple<string, int>> urlAndPorts)
		{
			// read the template vhosts file
			
			// insert settings into template file

			// write the file
		}
	}
}