using System;
using Gtk;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

public partial class MainWindow: Gtk.Window
{
	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();

		ListStore store = new ListStore (typeof(string));

		foreach (var item in Enum.GetValues (typeof(BarcodeLib.TYPE))) {
			store.AppendValues (item.ToString ());
		}
		cmbBarCodeType.Model = store;
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void OnBtnGenerateClicked (object sender, EventArgs e)
	{
		try {
			BarcodeLib.Barcode codeBar = new BarcodeLib.Barcode ();
			codeBar.Alignment = BarcodeLib.AlignmentPositions.CENTER;
			codeBar.IncludeLabel = true;
			codeBar.LabelPosition = BarcodeLib.LabelPositions.BOTTOMCENTER;

			BarcodeLib.TYPE bCodeType = (BarcodeLib.TYPE)Enum.Parse (typeof(BarcodeLib.TYPE), cmbBarCodeType.ActiveText.ToString ());
			System.Drawing.Image imgTmpCodeBar = codeBar.Encode (bCodeType, txtData.Text.Trim (), System.Drawing.Color.Black, System.Drawing.Color.White, 300, 300);

			MemoryStream memoryStream = new MemoryStream();
			imgTmpCodeBar.Save(memoryStream, ImageFormat.Png);
			Gdk.Pixbuf pb = new Gdk.Pixbuf (memoryStream.ToArray());

			imgCodeBar.Pixbuf = pb;

		} catch (Exception err) {
			MessageDialog dlg = new MessageDialog (this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, string.Format ("Ocurrió un error \n {0}", err.Message));
			dlg.Run ();
			dlg.Destroy ();
			dlg.Dispose ();
			dlg = null;
		}
		
	}
}
