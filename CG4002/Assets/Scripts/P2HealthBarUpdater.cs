using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//////////////////////////////////////////////////////////
// -- P2HealthBarUpdater -- 
// Updates health bar information of Player 2.
//////////////////////////////////////////////////////////
public class P2HealthBarUpdater : MonoBehaviour 
{
	public Slider healthBar;
	public TextMeshProUGUI healthNumber;
	public Image healthBarImage;
	private PlayerHealth playerHealth;
	private Color32 barColor = new Color32(0, 63, 190, 255);
	private PlayerHealth p1Info;
	public TextMeshProUGUI p2Score;
	public TextMeshProUGUI p2EndGameScore;

	void Start() 
	{
		playerHealth = GameObject.FindGameObjectWithTag("Player2").GetComponent<PlayerHealth>();
		p1Info = GameObject.FindGameObjectWithTag("Player1").GetComponent<PlayerHealth>();
	}

	void Update() 
	{
		healthBar.value = playerHealth.health;

		healthNumber.text = playerHealth.health.ToString();

		p2Score.text = p1Info.deathCount.ToString(); // Player1's death count is Player2's score.
		p2EndGameScore.text = p1Info.deathCount.ToString();

		if ((int)playerHealth.health <= 30) {
            healthBarImage.color = Color.Lerp(barColor, Color.white, Mathf.PingPong(Time.time, 0.5f));
        }
		else
		{
			healthBarImage.color = barColor;
		}
	}

}
