using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.UITest;

namespace Uno.Toolkit.UITest.Extensions
{
	public partial record struct AppRect(float X, float Y, float Width, float Height) : IAppRect
	{
		public float CenterX => Width / 2f + X;
		public float CenterY => Height / 2f + Y;
		public float Right => X + Width;
		public float Bottom => Y + Height;
	}
}
