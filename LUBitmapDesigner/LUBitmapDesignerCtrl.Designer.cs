namespace LUBitmapDesigner
{
    partial class LUBitmapDesignerCtrl
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            helplineRulerCtrl1 = new HelplineRulerControl.HelplineRulerCtrl();
            SuspendLayout();
            // 
            // helplineRulerCtrl1
            // 
            helplineRulerCtrl1.Bmp = null;
            helplineRulerCtrl1.Dock = DockStyle.Fill;
            helplineRulerCtrl1.DontDoLayout = false;
            helplineRulerCtrl1.DontHandleDoubleClick = false;
            helplineRulerCtrl1.DontPaintBaseImg = false;
            helplineRulerCtrl1.DontProcDoubleClick = false;
            helplineRulerCtrl1.DrawModeClipped = false;
            helplineRulerCtrl1.IgnoreZoom = false;
            helplineRulerCtrl1.Location = new Point(0, 0);
            helplineRulerCtrl1.Margin = new Padding(5, 3, 5, 3);
            helplineRulerCtrl1.MoveHelpLinesOnResize = false;
            helplineRulerCtrl1.Name = "helplineRulerCtrl1";
            helplineRulerCtrl1.SetZoomOnlyByMethodCall = false;
            helplineRulerCtrl1.Size = new Size(875, 865);
            helplineRulerCtrl1.TabIndex = 0;
            helplineRulerCtrl1.Zoom = 1F;
            helplineRulerCtrl1.ZoomSetManually = false;
            helplineRulerCtrl1.DBPanelDblClicked += helplineRulerCtrl1_DBPanelDblClicked;
            helplineRulerCtrl1.Load += helplineRulerCtrl1_Load;
            // 
            // LUBitmapDesignerCtrl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(helplineRulerCtrl1);
            Margin = new Padding(4, 3, 4, 3);
            Name = "LUBitmapDesignerCtrl";
            Size = new Size(875, 865);
            ResumeLayout(false);
        }

        #endregion

        public HelplineRulerControl.HelplineRulerCtrl helplineRulerCtrl1;
    }
}
