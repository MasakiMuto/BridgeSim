using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Masa.BridgeSim
{
	public class KeyFrameAnime
	{
		struct PartId
		{
			public readonly Part Part;
			public readonly Position Position;

			public PartId(Part part, Position position)
			{
				Part = part;
				Position = position;
			}
		}

		struct State
		{
			public readonly float Yaw, Pitch, Roll;

			public State(float yaw, float pitch, float roll)
			{
				Yaw = yaw;
				Pitch = pitch;
				Roll = roll;
			}
		}

		public class Frame
		{
			public readonly float Time;//時刻
			public Dictionary<PartId, State> Values { get; private set; }

			public Frame(float time, Dictionary<PartId, State> values)
			{
				Time = time;
				Values = values;
			}

			public void Mirror()
			{
				var mirror = new Dictionary<PartId, State>();
				foreach (var item in Values.Where(x=>x.Key.Position == Position.Left))
				{
					mirror.Add(new PartId(item.Key.Part, Position.Right), new State(-item.Value.Yaw, item.Value.Pitch, -item.Value.Roll));
				}
				foreach (var item in mirror)
				{
					Values.Add(item.Key, item.Value);
				}
			}

			public Frame Clone()
			{
				return new Frame(Time, Values.ToDictionary(x=>x.Key, x=>x.Value));
			}
		}

		List<Frame> frames;

		public KeyFrameAnime()
		{
			frames = new List<Frame>();
			frames.Add(CreateInitial());
			frames.Add(new Frame(3, new Dictionary<PartId, State>() {{ new PartId(Part.Hiji, Position.Left), new State(-MathHelper.Pi, 0, 0)} }));
		}

		Frame CreateInitial()
		{
			var frame = new Frame(0, new Dictionary<PartId, State>() 
			{
				{new PartId(Part.Root, Position.Center), new State(0, 0, 0)},
				{new PartId(Part.Head, Position.Center), new State(0, MathHelper.PiOver2, 0)},
				{new PartId(Part.Kata, Position.Left), new State(0, 0, 0)},
				{new PartId(Part.Hiji, Position.Left), new State(MathHelper.PiOver2, MathHelper.PiOver2 * .8f, 0)},
				{new PartId(Part.Tekubi, Position.Left), new State(0, 0, 0)},
				{new PartId(Part.Tesaki, Position.Left), new State(0, 0, 0)},
				{new PartId(Part.Mata, Position.Left), new State(0, 0, 0)},
				{new PartId(Part.Hiza, Position.Left), new State(0, MathHelper.PiOver2, 0)},
				{new PartId(Part.Ashikubi, Position.Left), new State(0, 0, 0)},
				{new PartId(Part.Tsumasaki, Position.Left), new State(0, -MathHelper.PiOver2, 0)},
			});
			frame.Mirror();
			return frame;
		}

		public void Setup()
		{

		}

		public Frame CurrentFrame(float second)
		{
			var last = frames.Last(x => x.Time <= second);
			var next = frames.First(x => x.Time > second);
			var dict = new Dictionary<PartId, State>();
			foreach (var item in frames[0].Values.Keys)
			{
				
			}
		}


	}
}
