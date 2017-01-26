using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Off_EE.Blocks
{
	public static class Animation
	{
		private static Bitmap animationPanel = new Bitmap(@"data\anim.png");
		private static int AnimationTick = 1;
		public static Cache anim = new Cache(-1);

		public static Dictionary<int, Point[]> Animations = new Dictionary<int, Point[]>()
			{
				{ 100, new Point[]
					{
						new Point(0, 0 * 16),
						new Point(1, 0 * 16),
						new Point(2 * 16, 0 * 16),
						new Point(3 * 16, 0 * 16),
						new Point(4 * 16, 0 * 16),
						new Point(5 * 16, 0 * 16),
						new Point(6 * 16, 0 * 16),
						new Point(7 * 16, 0 * 16),
						new Point(8 * 16, 0 * 16),
						new Point(9 * 16, 0 * 16),
						new Point(10 * 16, 0 * 16),
						new Point(11 * 16, 0 * 16),
						new Point(12 * 16, 0 * 16)
					}
				},
				{ 101, new Point[]
					{
						new Point(0, 1 * 16),
						new Point(1, 1 * 16),
						new Point(2 * 16, 1 * 16),
						new Point(3 * 16, 1 * 16),
						new Point(4 * 16, 1 * 16),
						new Point(5 * 16, 1 * 16),
						new Point(6 * 16, 1 * 16),
						new Point(7 * 16, 1 * 16),
						new Point(8 * 16, 1 * 16),
						new Point(9 * 16, 1 * 16),
						new Point(10 * 16, 1 * 16),
						new Point(11 * 16, 1 * 16),
						new Point(12 * 16, 1 * 16)
					}
				}
			};
		private static Dictionary<int, int> AnimationCount = new Dictionary<int, int>();

		static Animation()
		{
			foreach (int i in Animations.Keys)
				AnimationCount.Add(i, 0);
		}

		public static void AdvanceAnimationTicks()
		{
			var a = AnimationCount.Keys;
			List<int> stuff = new List<int>();
			foreach (int i in a)
			{
				stuff.Add(i);
			}

			foreach (var i in stuff)
				AnimationNext(i);
		}

		public static Texture2D GetCurrentAnimation(int blockid)
		{
			return GetAnimation(AnimationCount[blockid] / AnimationTick, blockid);
		}

		public static bool IsAnimationBlock(int id)
		{
			return Animations.ContainsKey(id);
		}

		public static Texture2D GetAnimation(int blockid)
		{
			return GetAnimation(AnimationNext(blockid), blockid);
		}

		public static int AnimationNext(int BlockId)
		{
			if (!(++AnimationCount[BlockId] < (Animations[BlockId].Length * AnimationTick)))
			{
				AnimationCount[BlockId] = 0;
			}
			return AnimationCount[BlockId] / AnimationTick;
		}

		public static Texture2D GetAnimation(int frame, int blockid)
		{
			string objkey = string.Concat(blockid, "::", frame);

			if (!anim.ContainsKey(objkey))
			{
				anim.Store(objkey, new ResourceImg(
					(System.Drawing.Bitmap)animationPanel.Clone(new System.Drawing.Rectangle(Animations[blockid][frame], new Size(16, 16)
						), animationPanel.PixelFormat)));
			}

			return ((ResourceImg)(anim.Get(objkey))).GetImage.MonoPic;
		}
	}
}
