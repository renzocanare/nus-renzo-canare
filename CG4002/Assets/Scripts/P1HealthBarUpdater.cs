using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//////////////////////////////////////////////////////////
// -- P1HealthBarUpdater -- 
// Updates health bar information of Player 1.
//////////////////////////////////////////////////////////
public class P1HealthBarUpdater : MonoBehaviour 
{
	public Slider healthBar;
	public TextMeshProUGUI healthNumber;
	public Image healthBarImage;
	private PlayerHealth playerHealth;
	private Color32 barColor = new Color32(183, 0, 0, 255);
	private PlayerHealth p2Info;
	public TextMeshProUGUI p1Score;
	public TextMeshProUGUI p1EndGameScore;

	void Start() 
	{
		playerHealth = GameObject.FindGameObjectWithTag("Player1").GetComponent<PlayerHealth>();
		p2Info = GameObject.FindGameObjectWithTag("Player2").GetComponent<PlayerHealth>();
	}

	void Update() 
	{
		healthBar.value = playerHealth.health;

		healthNumber.text = playerHealth.health.ToString();

		p1Score.text = p2Info.deathCount.ToString(); // Player2's death count is Player1's score.
		p1EndGameScore.text = p2Info.deathCount.ToString();
		
        if ((int)playerHealth.health <= 30) {
            healthBarImage.color = Color.Lerp(barColor, Color.white, Mathf.PingPong(Time.time, 0.5f));
        }
		else
		{
			healthBarImage.color = barColor;
		}
	}
}
