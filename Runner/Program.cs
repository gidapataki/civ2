using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Runner
{
	class Program
	{

		static string[] players = new string[] 
		{
			//"Alice", 
			//"Ellen",
			//"Arya",
			"Cortana",
			"Rikku",
		};

		static void Main(string[] args)
		{
			var baseDir = Path.GetFullPath(@"..\..\..");
			var civDir = Path.Combine(baseDir, "CivSharp 2");

			Func<string, string> playerDll = (string s) => Path.Combine(baseDir, @"Player\bin\debug", s + ".dll");
			Func<string, string> targetDll = (string s) => Path.Combine(civDir, "Players", s + ".dll");

			foreach (var i in players)
			{
				Console.WriteLine(i);
				File.Copy(playerDll(i), targetDll(i), true);
			}

			var procInfo = new ProcessStartInfo(Path.Combine(civDir, "CivSharpGame.exe"));
			procInfo.WorkingDirectory = Path.GetFullPath(civDir);
			var proc = Process.Start(procInfo);
			proc.WaitForExit();
			//Console.WriteLine("exited");
			//Console.ReadLine();

			foreach (var i in players)
			{
				File.Delete(targetDll(i));
			}
		}
	}
}
