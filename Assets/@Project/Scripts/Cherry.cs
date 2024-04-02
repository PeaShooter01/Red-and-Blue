using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cherry : MonoBehaviour
{
	// 获取的特效预制体
	public GameObject m_collectedPrefab;

	// 获取水果时播放的音效
	public AudioClip m_collectedClip;

    public GameObject player1, player2;

	void Update(){

	}

	// 当与其他对象发生碰撞时调用的函数
	private void OnTriggerEnter2D(Collider2D other)
	{
		// 如果与名称包含"Player"的对象发生碰撞
		if (other.name.Contains("Player"))
		{
			// 创建获取特效对象
			var collected = Instantiate(
				m_collectedPrefab,
				transform.position,
				Quaternion.identity
			);

			// 获取获取特效对象的动画组件
			var animator = collected.GetComponent<Animator>();

			// 获取当前播放的动画状态信息
			var info = animator.GetCurrentAnimatorStateInfo(0);

			// 获取当前播放动画的时长（秒）
			var time = info.length;

			// 当动画播放完成时，注册销毁获取特效对象的函数
			Destroy(collected, time);

			// 销毁自身对象
			// Destroy(gameObject);
			if (gameObject.activeSelf)
				gameObject.SetActive(false);

			// 播放获取水果的音效
			var audioSource = FindObjectOfType<AudioSource>();
			audioSource.PlayOneShot(m_collectedClip);

            if(player1.name == other.name){
                player2.GetComponent<Player>().addHealth();
            }
            else{
                player1.GetComponent<Player>().addHealth();
            }
		}
	}

	public void Reset()
	{
		float distance1 = Vector2.Distance(player1.transform.position, transform.position);
		float distance2 = Vector2.Distance(player2.transform.position, transform.position);
		if (!gameObject.activeSelf && distance1 > 20.0f && distance2 > 20.0f){
			gameObject.SetActive(true);
		}
	}
}