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

		public ValueWithRange(float value, float min, float max)
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
	}
}
