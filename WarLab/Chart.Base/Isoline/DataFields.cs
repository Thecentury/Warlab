using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScientificStudio.Charting.Isoline {
	public class DataField<TGrid, TArray> {
		protected TGrid grid;
		protected TArray data;

		public DataField(TGrid grid, TArray data) {
			this.grid = grid;
			this.data = data;
		}

		public TGrid Grid {
			get { return grid; }
		}

		public TArray Data {
			get { return data; }
		}
	}

	public class DataField2d<TGrid, TArray> : DataField<TGrid, TArray>
		where TArray : IArray2d
		where TGrid : IGrid2d {
		public DataField2d(TGrid grid, TArray data)
			: base(grid, data) {
			if (grid.Width != data.Width || grid.Height != data.Height)
				throw new ArgumentException("DataField2d: data dimensions do not match");
		}
	}

	public class GeneralScalarField2d : DataField2d<IWarpedGrid2d, IScalarArray2d> {
		public GeneralScalarField2d(IWarpedGrid2d grid, IScalarArray2d data)
			: base(grid, data) {
		}
	}
}
