using System.Collections;
using UnityEngine;

namespace PxlSpace.Fox
{
	public static class Extensions
	{
		public static bool IsInViewFrustrum(this Camera cam, Renderer rend)
		{
			return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(cam), rend.bounds);
		}
	}
}