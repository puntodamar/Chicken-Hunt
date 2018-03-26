using UnityEngine;

namespace Player.Skills
{
	public class HideSkill : MonoBehaviour, ISkill
	{
		public bool CanHide = true;
		public static System.Action OnPlayerEnterBushes;
		public static System.Action OnPlayerExitBushes;

		private void OnTriggerEnter(Collider other)
		{
			if (!other.gameObject.CompareTag("Bushes")) return;
			if (OnPlayerEnterBushes != null  && CanHide)
				OnPlayerEnterBushes();
		}

		private void OnTriggerExit(Collider other)
		{
			if (!other.gameObject.CompareTag("Bushes")) return;

			if (OnPlayerExitBushes != null)
				OnPlayerExitBushes();
		}

		public void Deactivate()
		{
			CanHide = false;
		}

		public void ResetToRespawn(){}
	}
}
