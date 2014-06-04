using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Masa.BridgeSim
{
	public class ValueWithRange
	{
		float val;
		public readonly float Min, Max;

		public float Value
		{
			get { return val; }
			set
			{
				val = value;
				Debug.Assert(Min <= Value && Value <= Max);
			}
		}

		public ValueWithRange(float value)
		{
			Min = float.NegativeInfinity;
			Max = float.PositiveInfinity;
			Value = value;
		}

		public ValueWithRange(float min, float max)
			: this(min, min, max)
		{
		}

		private ValueWithRange(float value, float min, float max)
		{
			Min = min;
			Max = max;
			Value = value;
		}

		public ValueWithRange Mirror()
		{
			return new ValueWithRange(-Value, -Max, -Min);
		}

		public XElement ToXml(string name)
		{
			var elm = new XElement(name);
			elm.Value = Value.ToString();
			elm.SetAttributeValue("max", Max);
			elm.SetAttributeValue("min", Min);
			return elm;
		}

		public override string ToString()
		{
			return String.Format("{0} < {1} < {2}", Min, Value, Max);
		}
	}
}
