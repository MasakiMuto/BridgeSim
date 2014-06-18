using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Masa.BridgeSim
{
	public class KeyFrameAnime
	{
		public class Frame
		{
			public readonly double Time;//時刻
			public readonly PartId Part;
			public readonly RotationState State;

			public Frame(double time, PartId part, RotationState state)
			{
				Time = time;
				Part = part;
				State = state;
			}

			public Frame Mirror()
			{
				if (Part.Position == Position.Center)
				{
					throw new ArgumentException();
				}
				var p = Part.Position == Position.Left ? Position.Right : Position.Left;
				return new Frame(this.Time, new PartId(Part.Part, p), State.Mirror());
				
			}

			public static Frame Lerp(Frame last, Frame next, double time)
			{
				Debug.Assert(last.Part == next.Part);
				Debug.Assert(last.Time <= time && time <= next.Time);
				var ratio = (time - last.Time) / (next.Time - last.Time);
				if (double.IsInfinity(next.Time))
				{
					ratio = 0;
				}
				return new Frame(time, last.Part, last.State.Lerp(next.State, (float)ratio));
			}

			public Frame CreateFinalFrame()
			{
				return new Frame(double.PositiveInfinity, Part, State);
			}

		}

		struct JointState
		{
			public readonly Frame Last, Next;
			public JointState(Frame last, Frame next)
			{
				Last = last;
				Next = next;
			}
		}

		List<Frame> frames;
		Dictionary<PartId, JointState> states;
		Frame[] initials;

		public KeyFrameAnime()
		{
			frames = new List<Frame>();
			initials = CreateInitial().ToArray();
			Debug.Assert(initials.Select(x => x.Part.Part).Distinct().Count() == Enum.GetValues(typeof(Part)).Length);//全部位のキーフレームがあるか
			frames.AddRange(initials);
		}

		public void AddFrame(Frame f)
		{
			frames.Add(f);
		}

		public void AddFrameWithMirror(Frame f)
		{
			frames.Add(f);
			frames.Add(f.Mirror());
		}

		IEnumerable<Frame> CreateInitial()
		{
			return new Dictionary<PartId, RotationState>
			{
				{new PartId(Part.Root, Position.Center), new RotationState(0, 0, 0)},
				{new PartId(Part.Spine1, Position.Center), new RotationState(0, 0, 0)},
				{new PartId(Part.Spine2, Position.Center), new RotationState(0, 0, 0)},
				//{new PartId(Part.Kubi, Position.Center), new RotationState(0, 0, 0)},
				{new PartId(Part.Head, Position.Center), new RotationState(0, MathHelper.Pi, 0)},
				{new PartId(Part.Kata, Position.Left), new RotationState(0, 0, 0)},
				{new PartId(Part.Hiji, Position.Left), new RotationState(0, 0, 0)},
				{new PartId(Part.Tekubi, Position.Left), new RotationState(0, 0, 0)},
				{new PartId(Part.Tesaki, Position.Left), new RotationState(0, 0, 0)},
				{new PartId(Part.Mata, Position.Left), new RotationState(0, 0, 0)},
				{new PartId(Part.Hiza, Position.Left), new RotationState(0, 0, 0)},
				{new PartId(Part.Ashikubi, Position.Left), new RotationState(0, 0, 0)},
				{new PartId(Part.Tsumasaki, Position.Left), new RotationState(0, 0, 0)},
			}
			.Select(x=>new Frame(0, x.Key, x.Value))
			.SelectMany(x=>x.Part.Position == Position.Left ? new[]{x, x.Mirror()} : new[]{x});
			
		}

		public void Setup()
		{
			Comparison<Frame> comp = (f1, f2)=>
			{
				var d = f1.Time - f2.Time;
				if(d == 0) return 0;
				else if(d > 0) return 1;
				else return -1;
			};
			frames.Sort(comp);
			states = new Dictionary<PartId, JointState>();
			foreach (var item in initials.Select(x=>x.Part))
			{
				var last = frames.Last(x => x.Part == item);
				frames.Add(last.CreateFinalFrame());
			}

			Reset();
		}



		public void Reset()
		{
			lastTime = 0;
			foreach (var item in initials.Select(x => x.Part))//初期姿勢設定
			{
				var last = initials.Single(x => x.Part == item);
				SetNextFrame(0, item, last);
			}
		}

		void SetNextFrame(double lastTime, PartId part, Frame last)
		{
			var next = frames.First(x => x.Part == part && x.Time > lastTime);
			states[part] = new JointState(last, next);
		}

		double lastTime;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="time">現在時刻 > 直前のupdate時刻</param>
		/// <returns></returns>
		public IEnumerable<Frame> Update(double delta)
		{
			lastTime += delta;
			foreach (var item in states.ToArray())
			{
				if (item.Value.Next.Time <= lastTime)
				{
					SetNextFrame(lastTime, item.Key, item.Value.Next);
				}
			}
			return states.Select(x => Frame.Lerp(x.Value.Last, x.Value.Next, lastTime));
		}


	}
}
