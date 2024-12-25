using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogManager : MonoBehaviour
{
	public static LogManager Instance;

	public RectTransform logContent;
	public Text logText;

	private void Awake()
	{
		Instance = this;
	}

	public static void Log(object message)
	{
		if (Instance != null)
		{
			var logText = Instantiate(Instance.logText, Instance.logContent, false);
			logText.text = message.ToString();
		}
		else
		{
			print(message);
		}
	}
}

