using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Awesomium.Core.Data;

namespace AwesomiumSample
{
	class SampleDataSource : DataSource
	{
		protected override void OnRequest(DataSourceRequest request)
		{
			Console.WriteLine("Request for: " + request.Path);

			var response = new DataSourceResponse();
			var data = File.ReadAllBytes(Environment.CurrentDirectory + @"\..\..\..\html\" + request.Path);
			//var data = File.ReadAllBytes(@"E:/Cell/Visual .NET Projects/Asteroid Outpost/WorkingDirectory/UI/" + request.Path);

			IntPtr unmanagedPointer = Marshal.AllocHGlobal(data.Length);
			Marshal.Copy(data, 0, unmanagedPointer, data.Length);

			response.Buffer = unmanagedPointer;
			response.MimeType = "text/html";
			response.Size = (uint)data.Length;
			SendResponse(request, response);

			Marshal.FreeHGlobal(unmanagedPointer);
		}
	}
}
