using UnityEngine;

namespace Extensions
{
	public static class TransformExtensionMethods
	{
		public static Vector3 DirectionFromAngle(this Transform position, float angle, bool angleIsGlobal)
		{
			if (!angleIsGlobal)
				angle += position.eulerAngles.y;
			return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
		}
	
		public static Transform FindDeepChild(this Transform aParent, string aName)
		{
			foreach(Transform child in aParent)
			{
				if(child.name == aName )
					return child;
				var result = child.FindDeepChild(aName);
				if (result != null)
					return result;
			}
			return null;
		}
	}
}
