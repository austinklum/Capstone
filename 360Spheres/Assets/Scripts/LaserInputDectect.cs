using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Valve.VR;
using Valve.VR.Extras;
using Valve.VR.InteractionSystem;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class LaserInputDectect : MonoBehaviour
{
    private string getQuestionsURL = "http://localhost/QuestionAnswer/GetQuestions.php";
    private List<Question> questions = new List<Question>();
    private Question currentQuestion;
    public static GameObject currentObject;
    int currentID;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(GetQuestions());
        SteamVR_Actions.default_GrabPinch.AddOnStateDownListener(TriggerPressed, SteamVR_Input_Sources.Any);
        currentObject = null;
        currentID = 0;

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void TriggerPressed(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.forward, 100.0F);

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            int id = hit.collider.gameObject.GetInstanceID();

            if (currentID != id)
            {
                currentID = id;
                currentObject = hit.collider.gameObject;

                Button btnPressed = currentObject.GetComponent<Button>();
                string btnPressedText = btnPressed.GetComponentInChildren<Text>().text;
                if (btnPressedText.Contains("1"))
                {
                    UnityEngine.Debug.Log("Correct Answer");
                    Button btnAnswer = currentObject.GetComponent<Button>();
                    btnAnswer.GetComponentInChildren<Text>().text = "Correct!!";
                    getNextQuestion();
                }
            }
        }
    }

    IEnumerator GetQuestions()
    {
        updateTxtQuestion("Loading Q/A's");

        UnityWebRequest questionsRequest = UnityWebRequest.Get(getQuestionsURL);
        yield return questionsRequest.SendWebRequest();

        string response = System.Text.Encoding.UTF8.GetString(questionsRequest.downloadHandler.data);
        if (questionsRequest.error != null)
        {
            UnityEngine.Debug.Log("There was an error getting the question: " + questionsRequest.error);
        }
        else
        {
            questions = JsonConvert.DeserializeObject<List<Question>>(response);
            getNextQuestion();
        }
    }

    private void getNextQuestion()
    {
        if (questions.Count > 0)
        {
            currentQuestion = questions.First();
            updateTxtQuestion(currentQuestion.Content);
            updateBtnAnswers(currentQuestion.Answers);
            questions.Remove(currentQuestion);
        }
        else
        {
            updateTxtQuestion("That's all folks!");
        }
    }

    private void updateTxtQuestion(string updatedText)
    {
        GameObject questionCanvas = GameObject.FindGameObjectWithTag("questionCanvasTag");
        Transform txtQuestion = questionCanvas.transform.Find("txtQuestion");
        txtQuestion.GetComponentInChildren<Text>().text = updatedText;
    }

    private void updateBtnAnswers(List<Answer> answers)
    {
        GameObject pnlAnswers = GameObject.FindGameObjectWithTag("pnlAnswers");
        Transform btnAnswer1 = pnlAnswers.transform.Find("btnAnswer1");
        Transform btnAnswer2 = pnlAnswers.transform.Find("btnAnswer2");
        Transform btnAnswer3 = pnlAnswers.transform.Find("btnAnswer3");
        Transform btnAnswer4 = pnlAnswers.transform.Find("btnAnswer4");

        btnAnswer1.GetComponentInChildren<Text>().text = answers.First().Content;
        answers.Remove(answers.First());

        btnAnswer2.GetComponentInChildren<Text>().text = answers.First().Content;
        answers.Remove(answers.First());

        btnAnswer3.GetComponentInChildren<Text>().text = answers.First().Content;
        answers.Remove(answers.First());

        btnAnswer4.GetComponentInChildren<Text>().text = answers.First().Content;
        answers.Remove(answers.First());

    }
}

public class Question
{
    public int QuestionId;
    public string Content;
    public int LocationId;
    public List<Answer> Answers;
}
public class Answer
{
    public int AnswerId;
    public int QuestionId;
    public string Content;
    public bool IsCorrect;
}
