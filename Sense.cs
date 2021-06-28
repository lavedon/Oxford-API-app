using System.Collections.Generic;
using System;

namespace OxfordV2
{
	public class Sense : IDisposable
	{
		public string Definition { get; set; }
		public int Start { get; set; } 
		public bool IsObsolete { get; set; }
		public bool IsMainUsage { get; set; }
		public string SenseID { get; set; }
		public string OedReference { get; set; }
		
		bool disposed;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					GC.ReRegisterForFinalize(this);
				}
			}
			//dispose unmanaged resources
			disposed = true;
		}
		public Sense() {
			this.IsObsolete = false;
		}
		public void Dispose()
		{
			Dispose(true);
			// GC.SuppressFinalize(this);
		}

	}
}
