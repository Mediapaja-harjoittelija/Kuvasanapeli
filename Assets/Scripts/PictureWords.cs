using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using System.IO;

public class PictureWords : MonoBehaviour {

	// Arrays for all categories
	public string[] folderList; 
	public string[][] categoryWordLists;
	public Object[][] categoryPictureObjectLists;
	public Texture2D[][] categoryPictureTextureLists;

	public Button answerButtonA;
	public Button answerButtonB;
	public Button answerButtonC;
	public Button answerButtonD;
	public Button answerButtonE;
	public Button answerButtonF;
	public Button answerButtonG;
	public Button answerButtonH;
	public Button questionButtonPictures;
	public Button questionButtonWords;
	public Text questionButtonTextWordsPanel;

	public Text answerTextE;
	public Text answerTextF;
	public Text answerTextG;
	public Text answerTextH;

	public Text checkAnswer;
	public Text question;
	public Text scoreTextPicturePanel;
	public Text scoreTextWordPanel;
	public GameObject answerA;
	public GameObject answerB;
	public GameObject answerC;
	public GameObject answerD;
	public int listSize;
	public int rightAnswer;
	public int score;
	public int thisQuestion;
	public int correctAnswer;

	public List <int> randomList;
	public List <int> newRandomList;
	public List <int> questionList;

	public string gameMode;
	public GameObject gamePanelPictureGame;
	public GameObject gamePanelWordGame;
	public GameObject selectGamePanel;
	public GameObject categoryPanel;

	public GameObject dropDownCategories;
	public Text dropDownLabelText;
	public Text categoryTextlabel;

	public int categoryValue;

	// Set this to true if you create a WebGL build
	public bool thisIsWebGLBuild;
	public string[] dirPaths;
	public GameObject gamePanelExitButton;
	public GameObject categoryPanelExitButton;

	public GameObject endPanel;
	public Text endScoreText;
	public Text endResultText;

	public bool gameIsStarted = false;
	public GameObject closeButton;

	public AudioClip wrongSound;
	public AudioClip buttonSound;
	public AudioClip correctSound;

	public AudioSource audioSource;

	public GameObject slideEffectSelectGame;
	public GameObject slideEffectSelectCategory;
	public GameObject slideEffectEndPanel;

	// Use this for initialization
	void Start () 
	{

		// Get the current time in seconds. Source: http://answers.unity3d.com/questions/417939/how-can-i-get-the-time-since-the-epoch-date-in-uni.html
		System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
		int curTime = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
		Random.InitState(curTime);

		audioSource = GetComponent<AudioSource> ();
		closeButton.SetActive(gameIsStarted);
		// Set dirPaths manually if this is a WebGL or Android build !!
		//if (thisIsWebGLBuild)
			dirPaths = new string[] {"Eläimet", "Esineet", "Maantiede", "Matkailu", "Media", "Metallityö",
			"Puutyö", "Ruoka", "Siivous", "Tietotekniikka", "Urheilu", "Vapaa-aika", "Yleinen"};
		//if (!thisIsWebGLBuild)
		//	dirPaths = Directory.GetDirectories (Application.dataPath + "/Resources/Textures", "*.*", SearchOption.AllDirectories);

		selectGamePanel.SetActive (true);
		categoryPanel.SetActive (true);
		if (thisIsWebGLBuild)
			DisableExitButtons ();

		GetFolders ();
		GenerateCategoryArrays();
		gameMode = "PictureGame";
		SetSettings ();
	}

	public void SlideSelectGamePanelOn()
	{
		StartCoroutine (SlideEffectOn (slideEffectSelectGame));
	}

	public void SlideSelectGamePanelOff()
	{
		StartCoroutine (SlideEffectOff (slideEffectSelectGame));
	}

	public void SlideSelectCategoryPanelOn()
	{
		StartCoroutine (SlideEffectOn (slideEffectSelectCategory));
	}

	public void SlideSelectCategoryPanelOff()
	{
		StartCoroutine (SlideEffectOff (slideEffectSelectCategory));
	}

	public void SlideEndPanelOn()
	{
		StartCoroutine (SlideEffectOn (slideEffectEndPanel));
	}

	public void SlideEndPanelOff()
	{
		StartCoroutine (SlideEffectOff (slideEffectEndPanel));
	}

	// Example source code from: http://answers.unity3d.com/questions/1105969/how-to-change-a-value-over-time-in-a-coroutine.html

	IEnumerator SlideEffectOff(GameObject slidePanel)
	{
		RectTransform panelRectTransform = slidePanel.GetComponent<RectTransform> ();
		float startValue = 0f;
		float endValue = 60f;
		float increment = 1f;
		float timeToWait = 0.01f;

		while (startValue < endValue) {
			startValue = startValue + increment;
			panelRectTransform.offsetMin = new Vector2 (-startValue * 10f, 0f);
			panelRectTransform.offsetMax = new Vector2 (-startValue * 10f, 0f);
			yield return new WaitForSeconds (timeToWait);
			//Debug.Log ("Value: " + startValue.ToString ());
		}
	}

	IEnumerator SlideEffectOn(GameObject slidePanel)
	{
		RectTransform panelRectTransform = slidePanel.GetComponent<RectTransform> ();
		float startValue = 60f;
		float endValue = 0f;
		float increment = 1f;
		float timeToWait = 0.01f;

		while (startValue > endValue) {
			startValue = startValue - increment;
			panelRectTransform.offsetMin = new Vector2 (-startValue * 10f, 0f);
			panelRectTransform.offsetMax = new Vector2 (-startValue * 10f, 0f);
			yield return new WaitForSeconds (timeToWait);
			//Debug.Log ("Value: " + startValue.ToString ());
		}
	}
		
	public void PlayButtonSound()
	{
		audioSource.PlayOneShot(buttonSound);
	}

	public void PlayCorrectSound()
	{
		audioSource.PlayOneShot(correctSound);
	}

	public void PlayWrongSound()
	{
		audioSource.PlayOneShot(wrongSound);
	}

	void DisableExitButtons()
	{
		gamePanelExitButton.SetActive (false);
		categoryPanelExitButton.SetActive (false);
	}
		
	void GetFolders()
	{

		int tmp = 0;
		folderList = new string[dirPaths.Length];
		dropDownLabelText.text = "Valitse kategoria";
		foreach (string file in dirPaths) {
			//folderList[tmp] = Path.GetFileName(file);
			folderList[tmp] = file;
			dropDownCategories.GetComponent<Dropdown> ().options.Add (new Dropdown.OptionData() { text = folderList[tmp]});
			tmp++;
		}
		dropDownCategories.GetComponent<Dropdown> ().RefreshShownValue ();
		DropdownValueChanged ();
	}

	void GenerateCategoryArrays()
	{
		categoryWordLists = new string[folderList.Length][];
		categoryPictureObjectLists = new Object[folderList.Length][];
		categoryPictureTextureLists = new Texture2D[folderList.Length][];
		int tmpA = 0;
		foreach (string category in folderList) {
			categoryPictureObjectLists[tmpA] = Resources.LoadAll("Textures/" + folderList[tmpA].ToString(), typeof(Texture));
			categoryWordLists[tmpA] = new string[categoryPictureObjectLists[tmpA].Length];
			categoryPictureTextureLists[tmpA] = new Texture2D[categoryPictureObjectLists[tmpA].Length];
			int tmpB = 0;
			foreach (Texture textureObject in categoryPictureObjectLists[tmpA]) {
				categoryWordLists [tmpA] [tmpB] = textureObject.name.ToString ();
				categoryPictureTextureLists [tmpA] [tmpB] = (Texture2D)textureObject;
				// Debug.Log(textureObject.name.ToString() + " : " + categoryWordLists [tmpA] [tmpB].ToString() + " : " + categoryPictureTextureLists [tmpA] [tmpB].ToString());
				tmpB++;
			}
			tmpA++;
		}
	}
		
	public void NewQuessWordGame()
	{
		gameIsStarted = true;
		closeButton.SetActive (gameIsStarted);
		gameMode = "WordGame";
		SetSettings ();
	}

	public void NewQuessPictureGame()
	{
		gameIsStarted = true;
		closeButton.SetActive (gameIsStarted);
		gameMode = "PictureGame";
		SetSettings ();
	}

	public void SetSettings()
	{
		if (!gameIsStarted)
			selectGamePanel.SetActive (false);
		thisQuestion = 0;
		score = 0;

		if (gameMode == "WordGame") {
			gamePanelPictureGame.SetActive (false);
			gamePanelWordGame.SetActive (true);
			SetNewScore ();
		}

		if (gameMode == "PictureGame") {
			gamePanelPictureGame.SetActive (true);
			gamePanelWordGame.SetActive (false);
			questionButtonPictures.interactable = false;
			SetNewScore ();
		}
		GenerateQuestionList ();
		GenerateNewQuestion ();
	}
		
	void SetCheckAnswerText(bool value)
	{
		if (value) {
			checkAnswer.text = "OIKEIN";
			checkAnswer.color = new Color32(0, 255, 97, 255);
		} 
		if (!value) {
			checkAnswer.text = "VÄÄRIN";
			checkAnswer.color = new Color32(255, 45, 45, 255);
		}
			
	}

	public void DropdownValueChanged()
	{
		categoryValue = dropDownCategories.GetComponent<Dropdown> ().value;
		categoryTextlabel.text = folderList [categoryValue].ToString ();
	}

	public void ButtonActivityForAD(Button pressedButton, GameObject answer, bool correctAnswer)
	{
		SetButtonInteractable (false);
		if (correctAnswer) {
			score++;
			SetCheckAnswerText (true);
			pressedButton.GetComponent<Image>().color = new Color32(0, 255, 97, 255);
			answer.GetComponent<RawImage> ().color = new Color32(0, 255, 97, 255);
			PlayCorrectSound ();
		}
		else {
			SetCheckAnswerText (false);
			pressedButton.GetComponent<Image>().color = new Color32(255, 45, 45, 255);
			answer.GetComponent<RawImage> ().color = new Color32(255, 45, 45, 255);
			PlayWrongSound ();
		}
		StartCoroutine(WaitSomeTime ());
		SetNewScore ();
	}

	public void ButtonActivityA()
	{
		if (rightAnswer == 0)
			ButtonActivityForAD (answerButtonA, answerA, true);
		else
			ButtonActivityForAD (answerButtonA, answerA, false);
	}

	public void ButtonActivityB()
	{
		if (rightAnswer == 1)
			ButtonActivityForAD (answerButtonB, answerB, true);
		else
			ButtonActivityForAD (answerButtonB, answerB, false);
	}

	public void ButtonActivityC()
	{
		if (rightAnswer == 2)
			ButtonActivityForAD (answerButtonC, answerC, true);
		else
			ButtonActivityForAD (answerButtonC, answerC, false);
	}

	public void ButtonActivityD()
	{
		if (rightAnswer == 3)
			ButtonActivityForAD (answerButtonD, answerD, true);
		else
			ButtonActivityForAD (answerButtonD, answerD, false);
	}

	public void ButtonActivityForEH(Button pressedButton, bool correctAnswer)
	{
		SetButtonInteractable (false);
		if (correctAnswer) {
			score++;
			pressedButton.GetComponent<Image>().color = new Color32(0, 255, 97, 255);
			PlayCorrectSound ();
		}
		else {
			pressedButton.GetComponent<Image>().color = new Color32(255, 45, 45, 255);
			PlayWrongSound ();
		}
		StartCoroutine(WaitSomeTime ());
		SetNewScore ();
	}

	public void ButtonActivityE()
	{
		if (rightAnswer == 0)
			ButtonActivityForEH (answerButtonE, true);
		else
			ButtonActivityForEH (answerButtonE, false);
	}

	public void ButtonActivityF()
	{
		if (rightAnswer == 1)
			ButtonActivityForEH (answerButtonF, true);
		else
			ButtonActivityForEH (answerButtonF, false);
	}

	public void ButtonActivityG()
	{
		if (rightAnswer == 2)
			ButtonActivityForEH (answerButtonG, true);
		else
			ButtonActivityForEH (answerButtonG, false);
	}

	public void ButtonActivityH()
	{
		if (rightAnswer == 3)
			ButtonActivityForEH (answerButtonH, true);
		else
			ButtonActivityForEH (answerButtonH, false);
	}

	void ResetButtonColors()
	{
		// Reset button image colors
		answerA.GetComponent<RawImage> ().color = new Color32(255, 255, 255, 255);
		answerB.GetComponent<RawImage> ().color = new Color32(255, 255, 255, 255);
		answerC.GetComponent<RawImage> ().color = new Color32(255, 255, 255, 255);
		answerD.GetComponent<RawImage> ().color = new Color32(255, 255, 255, 255);
		// Reset button colors
		answerButtonA.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
		answerButtonB.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
		answerButtonC.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
		answerButtonD.GetComponent<Image>().color = new Color32(255, 255, 255, 255);

		answerButtonE.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
		answerButtonF.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
		answerButtonG.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
		answerButtonH.GetComponent<Image>().color = new Color32(255, 255, 255, 255);

		// Reset answer check text
		checkAnswer.text = "";
	}

	void SetButtonInteractable(bool setValue)
	{
		answerButtonA.interactable = setValue;
		answerButtonB.interactable = setValue;
		answerButtonC.interactable = setValue;
		answerButtonD.interactable = setValue;
		answerButtonE.interactable = setValue;
		answerButtonF.interactable = setValue;
		answerButtonG.interactable = setValue;
		answerButtonH.interactable = setValue;
	}

	void SetNewScore()
	{
		if (gameMode == "PictureGame") {
			scoreTextPicturePanel.text = "Pisteet: " + score.ToString () + " / " + categoryPictureObjectLists [categoryValue].Length.ToString ();
		}
		if (gameMode == "WordGame") {
			scoreTextWordPanel.text = "Pisteet: " + score.ToString () + " / " + categoryPictureObjectLists [categoryValue].Length.ToString ();
		}
	}

	IEnumerator WaitSomeTime()
	{
		yield return new WaitForSeconds (1);
		ResetButtonColors ();
		if (thisQuestion < listSize) {
			GenerateNewQuestion ();
			SetButtonInteractable (true);
		} else {
			endPanel.SetActive (true);
			SlideEndPanelOn ();
			endScoreText.text = "Pisteet: " + score.ToString () + " / " + categoryPictureObjectLists [categoryValue].Length.ToString ();
			int resultPercentage = EndScorePercentage (score, categoryPictureObjectLists [categoryValue].Length);
			SetGameEndText (resultPercentage);
			SetButtonInteractable (true);
			gameIsStarted = false;
			closeButton.SetActive (gameIsStarted);
		}

	}

	void SetGameEndText(int percentage)
	{
		if (percentage <= 25)
			endResultText.text = "Sait " + percentage.ToString() + " prosenttia oikein tämän pelin kysymyksistä. Heikko tulos. Sinun pitäisi harjoitella lisää mahdollisimman paljon.";
		if (percentage > 25 && percentage <= 50)
			endResultText.text = "Sait " + percentage.ToString() + " prosenttia oikein tämän pelin kysymyksistä. Tulos oli melko vaatimaton. Harjoittele lisää.";
		if (percentage > 50 && percentage <= 75)
			endResultText.text = "Sait " + percentage.ToString() + " prosenttia oikein tämän pelin kysymyksistä. Melko hyvä tulos, mutta kannattaa harjoitella vielä lisää.";
		if (percentage > 75 && percentage < 100)
			endResultText.text = "Sait " + percentage.ToString() + " prosenttia oikein tämän pelin kysymyksistä. Hyvä tulos, mutta kannattaa harjoitella vielä lisää.";
		if (percentage == 100)
			endResultText.text = "Onneksi olkoon! Vastasit kaikkiin kysymyksiin täysin oikein.";
	}

	int EndScorePercentage(int scorePoints, int questionsCount)
	{
		return Mathf.RoundToInt(100 * ((float)scorePoints / questionsCount));
	}

	public void GenerateNewQuestion()
	{
		if (gameMode == "PictureGame") {
			GenerateRandomList ();
			ShuffleList ();
			SelectRightAnswer ();
			answerA.GetComponent<RawImage> ().texture = categoryPictureTextureLists [categoryValue][randomList [0]];
			answerB.GetComponent<RawImage> ().texture = categoryPictureTextureLists [categoryValue][randomList [1]];
			answerC.GetComponent<RawImage> ().texture = categoryPictureTextureLists [categoryValue][randomList [2]];
			answerD.GetComponent<RawImage> ().texture = categoryPictureTextureLists [categoryValue][randomList [3]];
			thisQuestion++;
		}
		if (gameMode == "WordGame") {
			GenerateRandomList ();
			ShuffleList ();
			SelectRightAnswer ();
			answerTextE.text = categoryWordLists [categoryValue][randomList [0]];
			answerTextF.text = categoryWordLists [categoryValue][randomList [1]];
			answerTextG.text = categoryWordLists [categoryValue][randomList [2]];
			answerTextH.text = categoryWordLists [categoryValue][randomList [3]];
			thisQuestion++;
		}
	}

	public void SelectRightAnswer()
	{
			for (int b = 0; b < 4; b++) {
				if (randomList [b] == questionList [thisQuestion]) {
					rightAnswer = b;
				}
			}
			correctAnswer = questionList [thisQuestion];
		if (gameMode == "PictureGame") {
			question.text = categoryWordLists [categoryValue][randomList [rightAnswer]];
		}
		if (gameMode == "WordGame") {
			questionButtonWords.GetComponentInChildren<RawImage> ().texture = categoryPictureTextureLists [categoryValue][randomList [rightAnswer]];
		}
	}

	// Generate unique random number list. Source: http://answers.unity3d.com/questions/715799/how-to-generate-unique-random-number.html
	public void GenerateRandomList()
	{
		listSize = categoryPictureTextureLists[categoryValue].Length;
		randomList = new List<int> ();
		randomList.Add (questionList [thisQuestion]);
		for (int i = 0; i < 3; i++) {
			int numToAdd = Random.Range (0, listSize);
			while (randomList.Contains (numToAdd)) {
				numToAdd = Random.Range (0, listSize);
			}
			randomList.Add (numToAdd);
		}
	}

	public void GenerateQuestionList()
	{
		listSize = categoryPictureTextureLists[categoryValue].Length;
		questionList = new List<int> ();
		for (int i = 0; i < listSize; i++) {
			int addNumber = Random.Range (0, listSize);
			while (questionList.Contains (addNumber)) {
				addNumber = Random.Range (0, listSize);
			}
			questionList.Add (addNumber);
		}
	}

	public void ShuffleList()
	{
		newRandomList = new List<int> ();
		for (int i = 0; i < 4; i++) {
			int numToAdd = randomList[Random.Range (0, 4)];
			while (newRandomList.Contains (numToAdd)) {
				numToAdd = randomList[Random.Range (0, 4)];
			}
			newRandomList.Add (numToAdd);
		}
		randomList = newRandomList;
	}

	public void ExitGame()
	{
		Application.Quit ();
	}
}