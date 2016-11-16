using System;
using System.Collections.Generic;

namespace LeapModels{
	
	public class Hand
	{
		//public List<List<double>> armBasis { get; set; }
		//public double armWidth { get; set; }
		//public double confidence { get; set; }
		public List<float> direction { get; set; }
		//public List<double> elbow { get; set; }
		//public double grabAngle { get; set; }
		//public double grabStrength { get; set; }
		public int id { get; set; }
		public List<float> palmNormal { get; set; }
		public List<float> palmPosition { get; set; }
		public List<float> palmVelocity { get; set; }
		//public double palmWidth { get; set; }
		//public double pinchDistance { get; set; }
		//public double pinchStrength { get; set; }
		//public List<List<double>> r { get; set; }
		//public double s { get; set; }
		//public List<double> sphereCenter { get; set; }
		//public double sphereRadius { get; set; }
		//public List<double> stabilizedPalmPosition { get; set; }
		//public List<double> t { get; set; }
		//public double timeVisible { get; set; }
		public string type { get; set; }
		//public List<double> wrist { get; set; }
		public List<Pointable> fingers { get; set; }
		public bool isLeft { get; set; }
		public bool isRight { get; set; }
	}

	public class InteractionBox
	{
		public List<double> center { get; set; }
		public List<double> size { get; set; }
	}

	public class Pointable
	{
		//public List<List<List<double>>> bases { get; set; }
		//public List<double> btipPosition { get; set; }
		//public List<double> carpPosition { get; set; }
		//public List<double> dipPosition { get; set; }
		public List<float> direction { get; set; }
		public bool extended { get; set; }
		public int handId { get; set; }
		public int id { get; set; }
		//public double length { get; set; }
		//public List<double> mcpPosition { get; set; }
		//public List<double> pipPosition { get; set; }
		//public List<double> stabilizedTipPosition { get; set; }
		//public double timeVisible { get; set; }
		public List<float> tipPosition { get; set; }
		public List<float> tipVelocity { get; set; }
		//public bool tool { get; set; }
		//public double touchDistance { get; set; }
		//public string touchZone { get; set; }
		public int type { get; set; }
		//public double width { get; set; }
	}

	public class RootObject
	{
		public double currentFrameRate { get; set; }
		public List<object> devices { get; set; }
		public List<object> gestures { get; set; }
		public List<Hand> hands { get; set; }
		public int id { get; set; }
		public InteractionBox interactionBox { get; set; }
		public List<Pointable> pointables { get; set; }
		public List<List<double>> r { get; set; }
		public double s { get; set; }
		public List<double> t { get; set; }
		public long timestamp { get; set; }
	}


	/*
	public class LeapVector{
		public float x { get; set; }
		public float y { get; set; }
		public float z { get; set; }
	}

	public class Hand{
		
	}
	*/
	
}

