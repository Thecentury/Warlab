using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using ScientificStudio.Charting.GraphicalObjects;
using ScientificStudio.Charting;
using System.Windows;

namespace WarLab.SampleUI.Charts {
	public class StaticObjectGraph : WarGraph {
		public StaticObject StaticObject { get; set; }
		public ImageSource SpriteImage { get; set; }

		private bool smallSprite = true;
		public bool SmallSprite {
			get { return smallSprite; }
			set { smallSprite = value; }
		}

		protected Size SpriteSize {
			get {
				Size size = new Size(SpriteImage.Width, SpriteImage.Height);
				if (SpriteImage.Width < 20) {
					// для правильного отображения EnemyPlane.png - он почему-то считает, что его размер 16x14
					size = new Size(100, 90);
				}

				if (smallSprite) {
					size = new Size(size.Width / 3, size.Height / 3);
				}
				return size;
			}
		}

		protected Point GetSpriteCenter(RenderState state) {
			Vector2D pos2D = StaticObject.Position.Projection2D;
			Point transformedPos = CoordinateUtils.Transform(pos2D, state.Visible, state.OutputWithMargin);
			return transformedPos;
		}

		protected override void OnRenderCore(DrawingContext dc, RenderState state) {
			if (StaticObject == null) return;
			if (SpriteImage == null) return;


			Size spriteSize = SpriteSize;
			Point transformedPos = GetSpriteCenter(state);

			dc.DrawImage(SpriteImage, MathHelper.CreateRectFromCenterSize(transformedPos, spriteSize));
		}
	}
}
