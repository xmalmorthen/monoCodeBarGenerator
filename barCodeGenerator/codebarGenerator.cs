using System;
using Gtk;
using System.IO;
using System.Drawing.Imaging;
using System.Reflection;

namespace barCodeGenerator
{
	public partial class codebarGenerator : Gtk.Window
	{
		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Application.Quit ();
			a.RetVal = true;
		}

		public codebarGenerator () :
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();

			hpan.Position = 278;

			ListStore storeBarCodeType = new ListStore (typeof(string));

			foreach (var item in Enum.GetValues (typeof(BarcodeLib.TYPE))) {
				storeBarCodeType.AppendValues (item.ToString ());
			}

			cmbBarCodeType.Model = storeBarCodeType; 
			cmbBarCodeType.Active = 1;

			ListStore storeTextPosition = new ListStore (typeof(string));
			foreach (var item in Enum.GetValues (typeof(BarcodeLib.LabelPositions))) {
				storeTextPosition.AppendValues (item.ToString ());
			}

			cmbTextPosition.Model = storeTextPosition;
			cmbTextPosition.Active = 4;

			ListStore storeColorText = new ListStore (typeof(string));
			ListStore storeColorBackground = new ListStore (typeof(string));


			Type colorType = typeof(System.Drawing.Color);
			PropertyInfo[] propInfos = colorType.GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public);
			int idx = 0, idxColorText, idxColorBackground;
			idx = idxColorText = idxColorBackground = 0;
			foreach (PropertyInfo propInfo in propInfos) 
			{
				if (propInfo.PropertyType == typeof(System.Drawing.Color)) {
					storeColorText.AppendValues (propInfo.Name);
					storeColorBackground.AppendValues (propInfo.Name);

					if (propInfo.Name.Equals("black", StringComparison.CurrentCultureIgnoreCase)){
						idxColorText = idx;
					}

					if (propInfo.Name.Equals("white", StringComparison.CurrentCultureIgnoreCase)){
						idxColorBackground = idx;
					}
					idx++;
				}
			}
			cmbColorText.Model = storeColorText;
			cmbColorText.Active = idxColorText;
			cmbColorBackground.Model = storeColorBackground;
			cmbColorBackground.Active = idxColorBackground;

		}

		protected void OnBtnGenerarClicked (object sender, EventArgs e)
		{
			try {
				BarcodeLib.Barcode codeBar = new BarcodeLib.Barcode ();
				codeBar.IncludeLabel = chkIncludeLabel.Active;

				BarcodeLib.LabelPositions lblPos = (BarcodeLib.LabelPositions)Enum.Parse (typeof(BarcodeLib.LabelPositions), cmbTextPosition.ActiveText.ToString ());
				codeBar.LabelPosition =   lblPos;

				BarcodeLib.TYPE bCodeType = (BarcodeLib.TYPE)Enum.Parse (typeof(BarcodeLib.TYPE), cmbBarCodeType.ActiveText.ToString ());

				int width,height;
				if (int.TryParse(txtWidth.Text.Trim(), out width)){
					if (int.TryParse(txtHeight.Text.Trim(), out height)){

						if (!string.IsNullOrEmpty(txtData.Text.Trim())){
							System.Drawing.Image imgTmpCodeBar = codeBar.Encode (bCodeType, txtData.Text.Trim (), System.Drawing.Color.FromName(cmbColorText.ActiveText) , System.Drawing.Color.FromName(cmbColorBackground.ActiveText), int.Parse(txtWidth.Text.Trim()),int.Parse(txtHeight.Text.Trim()));

							MemoryStream memoryStream = new MemoryStream();
							imgTmpCodeBar.Save(memoryStream, ImageFormat.Png);
							Gdk.Pixbuf pb = new Gdk.Pixbuf (memoryStream.ToArray());

							imgCodeBar.Pixbuf = pb;
						} else {
							txtData.GrabFocus();
							throw new Exception ("Falta indicar los datos a generar");
						}
					} else {
						txtHeight.GrabFocus();
						throw new Exception ("Altura incorrecta");
					}	
				} else {
					txtWidth.GrabFocus();
					throw new Exception ("Anchura incorrecta");
				}

			} catch (Exception err) {
				MessageDialog dlg = new MessageDialog (this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, string.Format ("Ocurrió un error \n {0}", err.Message));
				dlg.Run ();
				dlg.Destroy ();
				dlg.Dispose ();
				dlg = null;
			}
		}

		protected void OnChkIncludeLabelToggled (object sender, EventArgs e)
		{
			cmbTextPosition.Sensitive = chkIncludeLabel.Active;
		}
	}
}

