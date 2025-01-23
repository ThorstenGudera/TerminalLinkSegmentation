namespace QuickExtract2
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing.Drawing2D;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.VisualBasic;

    public class FPathEventArgs : EventArgs, IDisposable
    {
        private bool disposedValue;
        public GraphicsPath? Path { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        public FPathEventArgs(GraphicsPath fPath, float dx, float dy)
        {
            this.Path = fPath;
            this.X = dx;
            this.Y = dy;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (this.Path != null)
                    {
                        this.Path.Dispose();
                        this.Path = null;
                    }
                }

                // TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
                // TODO: Große Felder auf NULL setzen
                disposedValue = true;
            }
        }

        // ' TODO: Finalizer nur überschreiben, wenn "Dispose(disposing As Boolean)" Code für die Freigabe nicht verwalteter Ressourcen enthält
        // Protected Overrides Sub Finalize()
        // ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
        // Dispose(disposing:=False)
        // MyBase.Finalize()
        // End Sub

        public void Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

}