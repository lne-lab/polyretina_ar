using UnityEngine;

namespace LNE.PupilTest
{

	public class OscillatingColour : MonoBehaviour
	{
		public Color firstColour;
		public Color secondColour;

		public float holdTime;
		public float fadeTime;

		void FixedUpdate()
		{
			float lerpAmount;

			var period = Time.time % (holdTime + fadeTime) * 2;
			if (period < holdTime)
			{
				lerpAmount = 0;
			}
			else if (period < holdTime + fadeTime)
			{
				lerpAmount = (period - holdTime) / fadeTime;
			}
			else if (period < holdTime + fadeTime + holdTime)
			{
				lerpAmount = 1;
			}
			else
			{
				lerpAmount = 1 - ((period - (holdTime + fadeTime + holdTime)) / fadeTime);
			}

			SetColour(
				Color.Lerp(firstColour, secondColour, lerpAmount)
			);
		}

		public Color GetColour()
		{
			return GetComponent<Renderer>().material.color;
		}

		private void SetColour(Color colour)
		{
			GetComponent<Renderer>().material.color = colour;
		}
	}
}
