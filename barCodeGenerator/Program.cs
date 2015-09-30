using System;
using Gtk;

namespace barCodeGenerator
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			//MainWindow win = new MainWindow ();
			codebarGenerator win = new codebarGenerator ();
			win.Show ();

			Application.Run ();
		}
	}
}
