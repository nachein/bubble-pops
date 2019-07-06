using BubblePops.ScriptableObjects;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace BubblePops.Board
{
	public class BubblePopView : MonoBehaviour
	{
		[SerializeField] SpriteRenderer _renderer;
    	[SerializeField] TextMeshPro _text;
		[SerializeField] ParticleSystem _particleSystem;

		public void Pop(BubbleConfigItem config, Vector3 towards) 
		{
			_renderer.color = config.color;
			_text.SetText(config.display);

			transform.DOMove(towards, 0.25f).OnComplete(() => PlayPopParticles(config.color));
		}

		public void InstantPop(BubbleConfigItem config) 
		{
			PlayPopParticles(config.color);
		}

		private void PlayPopParticles(Color32 color)
		{
			_renderer.enabled = false;
			_text.enabled = false;

			ParticleSystem.MainModule main = _particleSystem.main;
			main.startColor = new ParticleSystem.MinMaxGradient(color);
			_particleSystem.Play();

			Invoke("Destroy", 1f);
		}

		private void Destroy()
		{
			Destroy(gameObject);
		}
	}
}