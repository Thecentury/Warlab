
namespace ScientificStudio.Charting.Isoline {
	/// <summary>
	/// Isoline's grid cell
	/// </summary>
	internal interface ICell {
		Vector2D LeftTop { get; }
		Vector2D LeftBottom { get; }
		Vector2D RightTop { get; }
		Vector2D RightBottom { get; }
	}

	internal class IrregularCell : ICell {
		public IrregularCell(Vector2D lt, Vector2D rt, Vector2D rb, Vector2D lb) {
			leftTop = lt;
			leftBottom = lb;
			rightTop = rt;
			rightBottom = rb;
		}

		#region ICell Members

		private readonly Vector2D leftTop;
		public Vector2D LeftTop {
			get { return leftTop; }
		}

		private readonly Vector2D leftBottom;
		public Vector2D LeftBottom {
			get { return leftBottom; }
		}

		private readonly Vector2D rightTop;
		public Vector2D RightTop {
			get { return rightTop; }
		}

		private readonly Vector2D rightBottom;
		public Vector2D RightBottom {
			get { return rightBottom; }
		}

		#endregion

		#region Sides
		public Vector2D LeftSide {
			get { return (leftBottom + leftTop) / 2; }
		}

		public Vector2D RightSide {
			get { return (rightBottom + rightTop) / 2; }
		}
		public Vector2D TopSide {
			get { return (leftTop + rightTop) / 2; }
		}
		public Vector2D BottomSide {
			get { return (leftBottom + rightBottom) / 2; }
		}
		#endregion

		public Vector2D Center {
			get { return (LeftSide + RightSide) / 2; }
		}

		public IrregularCell GetSubRect(SubCell sub) {
			switch (sub) {
				case SubCell.LeftBottom:
					return new IrregularCell(LeftSide, Center, BottomSide, LeftBottom);
				case SubCell.LeftTop:
					return new IrregularCell(LeftTop, TopSide, Center, LeftSide);
				case SubCell.RightBottom:
					return new IrregularCell(Center, RightSide, RightBottom, BottomSide);
				case SubCell.RightTop:
				default:
					return new IrregularCell(TopSide, RightTop, RightSide, Center);
			}
		}
	}

	/// <summary>
	/// Rectangular grid cell
	/// </summary>
	internal class RectangularCell : ICell {
		public RectangularCell(double left, double right, double top, double bottom) {
			ChartDebug.AssertDoubleNNaN(left);
			ChartDebug.AssertDoubleNNaN(right);
			ChartDebug.AssertDoubleNNaN(top);
			ChartDebug.AssertDoubleNNaN(bottom);

			this.left = left;
			this.right = right;
			this.top = top;
			this.bottom = bottom;

			leftTop = new Vector2D(left, top);
			rightTop = new Vector2D(right, top);
			leftBottom = new Vector2D(left, bottom);
			rightBottom = new Vector2D(right, bottom);
		}

		private readonly double left;
		private readonly double right;
		private readonly double top;
		private readonly double bottom;

		#region ICell Members

		private Vector2D leftTop;
		public Vector2D LeftTop {
			get { return leftTop; }
		}

		private Vector2D leftBottom;
		public Vector2D LeftBottom {
			get { return leftBottom; }
		}

		private Vector2D rightTop;
		public Vector2D RightTop {
			get { return rightTop; }
		}

		private Vector2D rightBottom;
		public Vector2D RightBottom {
			get { return rightBottom; }
		}

		#endregion
	}

	internal enum SubCell {
		LeftBottom = 0,
		LeftTop = 1,
		RightBottom = 2,
		RightTop = 3
	}

	internal class ValuesInCell {
		public ValuesInCell(double lt, double rt, double rb, double lb) {
			ChartDebug.AssertDoubleNNaN(lt);
			ChartDebug.AssertDoubleNNaN(rt);
			ChartDebug.AssertDoubleNNaN(lb);
			ChartDebug.AssertDoubleNNaN(rb);

			leftTop = lt;
			leftBottom = lb;
			rightTop = rt;
			rightBottom = rb;

			left = (lt + lb) / 2;
			right = (rt + rb) / 2;
			top = (lt + rt) / 2;
			bottom = (lb + rb) / 2;
		}

		#region Edges
		private readonly double leftTop;
		public double LeftTop { get { return leftTop; } }

		private readonly double leftBottom;
		public double LeftBottom { get { return leftBottom; } }

		private readonly double rightTop;
		public double RightTop {
			get { return rightTop; }
		}

		private readonly double rightBottom;
		public double RightBottom {
			get { return rightBottom; }
		}
		#endregion

		#region Sides & center
		private readonly double left;
		public double Left {
			get { return left; }
		}

		private readonly double right;
		public double Right {
			get { return right; }
		}

		private readonly double top;
		public double Top {
			get { return top; }
		}

		private readonly double bottom;
		public double Bottom {
			get { return bottom; }
		}

		public double Center {
			get { return (Left + Right) * 0.5; }
		}
		#endregion

		#region SubCells
		public ValuesInCell LeftTopCell {
			get { return new ValuesInCell(LeftTop, Top, Center, Left); }
		}

		public ValuesInCell RightTopCell {
			get { return new ValuesInCell(Top, RightTop, Right, Center); }
		}

		public ValuesInCell RightBottomCell {
			get { return new ValuesInCell(Center, Right, RightBottom, Bottom); }
		}

		public ValuesInCell LeftBottomCell {
			get { return new ValuesInCell(Left, Center, Bottom, LeftBottom); }
		}

		public ValuesInCell GetSubCell(SubCell subCell) {
			switch (subCell) {
				case SubCell.LeftBottom:
					return LeftBottomCell;
				case SubCell.LeftTop:
					return LeftTopCell;
				case SubCell.RightBottom:
					return RightBottomCell;
				case SubCell.RightTop:
				default:
					return RightTopCell;
			}
		}

		#endregion
	}
}
